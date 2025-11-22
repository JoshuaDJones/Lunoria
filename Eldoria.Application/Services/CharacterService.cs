using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using static System.Formats.Asn1.AsnWriter;

namespace Eldoria.Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICharacterSpellRepository _characterSpellRepository;
        private readonly IRepository<Spell> _spellRepository;
        private readonly IAzureStorageBlob _azureStorageBlob;

        public CharacterService(ICharacterRepository characterRepository, ICharacterSpellRepository characterSpellRepository, IRepository<Spell> spellRepository, IAzureStorageBlob azureStorageBlob)
        {
            _characterRepository = characterRepository;
            _characterSpellRepository = characterSpellRepository;
            _spellRepository = spellRepository;
            _azureStorageBlob = azureStorageBlob;
        }

        public async Task<Result<CharacterDto>> CreateAsync(string name, string description, IFormFile photo, int maxHp, int maxMp, int? meleeAttackDamage, int? bowAttackDamage, int movement, int maxInventory, bool isPlayer, bool isNPC, bool isEnemy, int? alternateFormId, CancellationToken ct)
        {
            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

            var character = new Character
            {
                Name = name,
                Description = description,
                PhotoUrl = photoUrl,
                FileName = fileName,
                MaxHp = maxHp,
                MaxMp = maxMp,
                MeleeAttackDamage = meleeAttackDamage,
                BowAttackDamage = bowAttackDamage,
                Movement = movement,
                MaxInventory = maxInventory,
                IsPlayer = isPlayer,
                IsNPC = isNPC,
                IsEnemy = isEnemy,
                AlternateFormId = alternateFormId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            await _characterRepository.AddAsync(character, ct);
            await _characterRepository.SaveChangesAsync(ct);

            return Result<CharacterDto>.Ok(character.ToDto());
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdAsync(id, ct);

            if (character is null)
                return Result.Fail(new Error("Character.NotFound", "Character was not found."));

            var associatedCharacterSpells = await _characterSpellRepository.GetCharacterSpells(id, ct);

            foreach(var characterSpell in associatedCharacterSpells)
            {
                _characterSpellRepository.Remove(characterSpell);
            }

            _characterRepository.Remove(character);
            await _characterRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result<CharacterDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdAsync(id, ct);            

            if (character is null)
                return Result<CharacterDto>.Fail(new Error("Character.NotFound", "Character was not found"));

            if (character.AlternateFormId is int altId)
                character.AlternateForm = await _characterRepository.GetByIdAsync(altId, ct);

            var associatedCharacterSpells = await _characterSpellRepository.GetCharacterSpells(id, ct);
            var characterSpellDtos = associatedCharacterSpells.Select(s => s.ToDto()).ToList();            

            var characterDto = character.ToDto();
            characterDto.CharacterSpells = characterSpellDtos;

            return Result<CharacterDto>.Ok(characterDto);
        }

        public async Task<Result<List<CharacterDto>>> GetListAsync(int skip, int take, CharacterType characterType, CancellationToken ct)
        {
            var characters = await _characterRepository.GetCharacters(skip, take, characterType, ct);

            List<CharacterDto> characterDtos = [];

            foreach(var character in characters)
            {
                var spells = character.CharacterSpells.Select(s => s.ToDto()).ToList();
                var dto = character.ToDto();
                dto.CharacterSpells = spells;

                characterDtos.Add(dto);
            }

            return Result<List<CharacterDto>>.Ok(characterDtos);
        }

        public async Task<Result<CharacterDto>> UpdateAsync(int id, string name, string description, IFormFile? photo, int maxHp, int maxMp, int? meleeAttackDamage, int? bowAttackDamage, int movement, int maxInventory, bool isPlayer, bool isNPC, bool isEnemy, int? alternateFormId, CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdAsync(id, ct);

            if (character is null)
                return Result<CharacterDto>.Fail(new Error("Character.NotFound", "Character was not found"));

            character.Name = name;
            character.Description = description;

            if (photo is not null)
            {
                if (!string.IsNullOrEmpty(character.FileName))
                    await _azureStorageBlob.DeletePhotoFromUrl(character.PhotoUrl);

                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

                character.PhotoUrl = photoUrl;
                character.FileName = fileName;
            }

            character.MaxHp = maxHp;
            character.MaxMp = maxMp;
            character.MeleeAttackDamage = meleeAttackDamage;
            character.BowAttackDamage = bowAttackDamage;
            character.Movement = movement;
            character.MaxInventory = maxInventory;
            character.IsPlayer = isPlayer;
            character.IsNPC = isNPC;
            character.IsEnemy = isEnemy;
            character.AlternateFormId = alternateFormId;

            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync(ct);

            return Result<CharacterDto>.Ok(character.ToDto());
        }
    }
}
