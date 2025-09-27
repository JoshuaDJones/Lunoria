using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace Eldoria.Application.Services
{
    public interface ICharacterService
    {
        Task<Result<List<CharacterDto>>> GetListAsync(int skip, int take, CancellationToken ct);
        Task<Result<CharacterDto>> GetByIdAsync(int id, CancellationToken ct);
        Task<Result> DeleteAsync(int id, CancellationToken ct);

        Task<Result<CharacterDto>> CreateAsync(string name,
            string description,
            IFormFile photo,
            int maxHp,
            int maxMp,
            int? meleeAttackDamage,
            int? bowAttackDamage,
            int movement,
            int maxInventory,
            bool isPlayer,
            bool isNPC,
            bool isEnemy,
            CancellationToken ct);

        Task<Result<CharacterDto>> UpdateAsync(int id,
            string name,
            string description,
            IFormFile? photo,
            int maxHp,
            int maxMp,
            int? meleeAttackDamage,
            int? bowAttackDamage,
            int movement,
            int maxInventory,
            bool isPlayer,
            bool isNPC,
            bool isEnemy,
            CancellationToken ct);
    }
}