using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IAzureStorageBlob _azureStorageBlob;

        public ItemService(IRepository<Item> itemRepository, IAzureStorageBlob azureStorageBlob)
        {
            _itemRepository = itemRepository;
            _azureStorageBlob = azureStorageBlob;
        }

        public async Task<Result<List<ItemDto>>> GetListAsync(int skip, int take, CancellationToken ct)
        {
            var items = await _itemRepository.ListAsync(skip, take, ct);
            var dtos = items.Select(i => i.ToDto()).ToList();

            return Result<List<ItemDto>>.Ok(dtos);
        }

        public async Task<Result<ItemDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdAsync(id, ct);

            if (item is null)
                return Result<ItemDto>.Fail(new Error("Item.NotFound", "The items does not exist."));

            return Result<ItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<ItemDto>> CreateAsync(string name, string description, IFormFile photo, int hpEffect, int mpEffect, CancellationToken ct)
        {
            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

            var item = new Item
            {
                Name = name,
                Description = description,
                PhotoUrl = photoUrl,
                FileName = fileName,
                HpEffect = hpEffect,
                MpEffect = mpEffect,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            await _itemRepository.AddAsync(item, ct);
            await _itemRepository.SaveChangesAsync(ct);

            return Result<ItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<ItemDto>> UpdateAsync(int id, string name, string description, IFormFile? photo, int hpEffect, int mpEffect, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdAsync(id, ct);

            if (item is null)
                return Result<ItemDto>.Fail(new Error("Item.NotFound", "The item does not exist."));

            item.Name = name;
            item.Description = description;

            if(photo is not null)
            {
                await _azureStorageBlob.DeletePhotoFromUrl(item.PhotoUrl);

                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

                item.PhotoUrl = photoUrl;
                item.FileName = fileName;
            }

            item.HpEffect = hpEffect;
            item.MpEffect = mpEffect;
            item.UpdateDate = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync(ct);

            return Result<ItemDto>.Ok(item.ToDto());
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct)
        {
            var item = await _itemRepository.GetByIdAsync(id, ct);

            if (item is null)
                return Result.Fail(new Error("Item.NotFound", "The item does not exist."));

            _itemRepository.Remove(item);
            await _itemRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }        
    }
}
