using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class ConsumableItemService(
        IItemRepository itemRepository,
        IAzureStorageBlob azureStorageBlob) : IConsumableItemService
    {
        private readonly IItemRepository _itemRepository = itemRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<List<ConsumableItemDto>>> GetListAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            var items = await _itemRepository.GetListForUserAsync(userId, skip, take, ct);
            return Result<List<ConsumableItemDto>>.Ok(
                items.Select(item => item.ToDto()).ToList());
        }

        public async Task<Result<ConsumableItemDto>> GetByIdAsync(
            int userId,
            int id,
            CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdForUserAsync(userId, id, ct);
            return item is null
                ? NotFound()
                : Result<ConsumableItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<ConsumableItemDto>> CreateAsync(
            int userId,
            string name,
            string description,
            IFormFile photo,
            int hpEffect,
            int mpEffect,
            CancellationToken ct)
        {
            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);
            var now = DateTime.UtcNow;
            var item = new ConsumableItem
            {
                UserId = userId,
                Name = name.Trim(),
                Description = description.Trim(),
                PhotoUrl = photoUrl,
                FileName = fileName,
                HpEffect = hpEffect,
                MpEffect = mpEffect,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _itemRepository.AddAsync(item, ct);
            await _itemRepository.SaveChangesAsync(ct);
            return Result<ConsumableItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<ConsumableItemDto>> UpdateAsync(
            int userId,
            int id,
            string name,
            string description,
            IFormFile? photo,
            int hpEffect,
            int mpEffect,
            CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdForUserAsync(userId, id, ct);
            if (item is null)
                return NotFound();

            var oldPhotoUrl = item.PhotoUrl;
            if (photo is not null)
            {
                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);
                item.PhotoUrl = photoUrl;
                item.FileName = fileName;
            }

            item.Name = name.Trim();
            item.Description = description.Trim();
            item.HpEffect = hpEffect;
            item.MpEffect = mpEffect;
            item.UpdatedAt = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync(ct);

            if (photo is not null && !string.IsNullOrWhiteSpace(oldPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(oldPhotoUrl);

            return Result<ConsumableItemDto>.Ok(item.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdForUserAsync(userId, id, ct);
            if (item is null)
                return Result.Fail(new Error(
                    "ConsumableItem.NotFound", "The consumable item was not found."));

            if (await _itemRepository.IsAssignedAsync(userId, id, ct))
                return Result.Fail(new Error(
                    "ConsumableItem.InUse",
                    "The consumable item cannot be deleted while it is assigned " +
                    "to a playthrough character or scene chest."));

            _itemRepository.Remove(item);
            await _itemRepository.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(item.PhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(item.PhotoUrl);

            return Result.Ok();
        }

        private static Result<ConsumableItemDto> NotFound() =>
            Result<ConsumableItemDto>.Fail(new Error(
                "ConsumableItem.NotFound", "The consumable item was not found."));
    }
}
