using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class ItemService(
        IItemRepository itemRepository,
        IAzureStorageBlob azureStorageBlob) : IItemService
    {
        private readonly IItemRepository _itemRepository = itemRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<List<ItemDto>>> GetListAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            var items = await _itemRepository.GetListForUserAsync(userId, skip, take, ct);
            return Result<List<ItemDto>>.Ok(items.Select(item => item.ToDto()).ToList());
        }

        public async Task<Result<ItemDto>> GetByIdAsync(int userId, int id, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdForUserAsync(userId, id, ct);
            return item is null
                ? Result<ItemDto>.Fail(new Error("Item.NotFound", "The item was not found."))
                : Result<ItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<ItemDto>> CreateAsync(
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
            var item = new Item
            {
                UserId = userId,
                Name = name.Trim(),
                Description = description.Trim(),
                PhotoUrl = photoUrl,
                FileName = fileName,
                HpEffect = hpEffect,
                MpEffect = mpEffect,
                CreateDate = now,
                UpdateDate = now,
            };

            await _itemRepository.AddAsync(item, ct);
            await _itemRepository.SaveChangesAsync(ct);
            return Result<ItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<ItemDto>> UpdateAsync(
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
                return Result<ItemDto>.Fail(new Error("Item.NotFound", "The item was not found."));

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
            item.UpdateDate = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync(ct);

            if (photo is not null && !string.IsNullOrWhiteSpace(oldPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(oldPhotoUrl);

            return Result<ItemDto>.Ok(item.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdForUserAsync(userId, id, ct);
            if (item is null)
                return Result.Fail(new Error("Item.NotFound", "The item was not found."));

            _itemRepository.Remove(item);
            await _itemRepository.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(item.PhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(item.PhotoUrl);

            return Result.Ok();
        }
    }
}
