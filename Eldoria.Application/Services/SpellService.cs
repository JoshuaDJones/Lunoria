using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using static System.Formats.Asn1.AsnWriter;

namespace Eldoria.Application.Services
{
    public class SpellService : ISpellService
    {
        private readonly IRepository<Spell> _spellRepository;
        private readonly IAzureStorageBlob _azureStorageBlob;

        public SpellService(IRepository<Spell> spellRepository, IAzureStorageBlob azureStorageBlob)
        {
            _spellRepository = spellRepository;
            _azureStorageBlob = azureStorageBlob;
        }

        public async Task<Result<List<SpellDto>>> GetListAsync(int skip, int take, CancellationToken ct)
        {
            var spells = await _spellRepository.ListAsync(skip, take, ct);
            var dtos = spells.Select(s => s.ToDto()).ToList();

            return Result<List<SpellDto>>.Ok(dtos);
        }

        public async Task<Result<SpellDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var spell = await _spellRepository.GetByIdAsync(id, ct);

            if (spell is null)
                return Result<SpellDto>.Fail(new Error("Spell.NotFound", "That spell does not exist."));

            return Result<SpellDto>.Ok(spell.ToDto());
        }

        public async Task<Result<SpellDto>> CreateAsync(string name, string description, IFormFile photo, int range, bool isRadius, int mpCost, int? damageEffect, int? healthEffect, int? magicEffect, CancellationToken ct)
        {
            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

            var spell = new Spell
            {
                Name = name,
                Description = description,
                PhotoUrl = photoUrl,
                FileName = fileName,
                Range = range,
                IsRadius = isRadius,
                MpCost = mpCost,
                DamageEffect = damageEffect,
                HealthEffect = healthEffect,
                MagicEffect = magicEffect,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            await _spellRepository.AddAsync(spell, ct);
            await _spellRepository.SaveChangesAsync(ct);

            return Result<SpellDto>.Ok(spell.ToDto());
        }

        public async Task<Result<SpellDto>> UpdateAsync(int id, string name, string description, IFormFile? photo, int range, bool isRadius, int mpCost, int? damageEffect, int? healthEffect, int? magicEffect, CancellationToken ct)
        {
            var spell = await _spellRepository.GetByIdAsync(id, ct);

            if (spell is null)
                return Result<SpellDto>.Fail(new Error("Spell.NotFound", "That spell does not exist."));

            if(photo is not null)
            {
                await _azureStorageBlob.DeletePhotoFromUrl(spell.PhotoUrl);
                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

                spell.PhotoUrl = photoUrl;
                spell.FileName = fileName;
            }         

            spell.Name = name;
            spell.Description = description;
            spell.Range = range;
            spell.IsRadius = isRadius;
            spell.MpCost = mpCost;
            spell.DamageEffect = damageEffect;
            spell.HealthEffect = healthEffect;
            spell.MagicEffect = magicEffect;
            spell.UpdateDate = DateTime.UtcNow;
            
            _spellRepository.Update(spell);
            await _spellRepository.SaveChangesAsync(ct);

            return Result<SpellDto>.Ok(spell.ToDto());
        }        

        public async Task<Result> DeleteAsync(int id, CancellationToken ct)
        {
            var spell = await _spellRepository.GetByIdAsync(id, ct);

            if (spell is null)
                return Result.Fail(new Error("Spell.NotFound", "This spell does not exists."));

            _spellRepository.Remove(spell);
            await _spellRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }        
    }
}
