using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface ICharacterService
    {
        Task<Result<List<CharacterDto>>> GetListAsync(int userId, int skip, int take, CharacterType characterType, CancellationToken ct);
        Task<Result<CharacterDto>> GetByIdAsync(int userId, int id, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int id, CancellationToken ct);

        Task<Result<CharacterDto>> CreateAsync(int userId, 
            string name,
            string description,
            IFormFile photo,
            int maxHp,
            int maxMp,
            int? meleeAttackDamage,
            int? bowAttackDamage,
            int movement,
            int baseMaxConsumableInventory,
            int baseMaxEquippableInventory,
            CharacterType characterType,
            int? alternateFormId,
            CancellationToken ct);

        Task<Result<CharacterDto>> UpdateAsync(int userId, 
            int id,
            string name,
            string description,
            IFormFile? photo,
            int maxHp,
            int maxMp,
            int? meleeAttackDamage,
            int? bowAttackDamage,
            int movement,
            int baseMaxConsumableInventory,
            int baseMaxEquippableInventory,
            CharacterType characterType,
            int? alternateFormId,
            CancellationToken ct);
    }
}
