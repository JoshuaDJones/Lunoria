using Eldoria.Infrastructure.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Api.GridPrototype;

[ApiController]
[AllowAnonymous]
[Route("api/v1/grid-prototype")]
public sealed class GridPrototypeController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("characters")]
    public Task<List<GridPrototypeCharacterDto>> ListCharacters(CancellationToken ct) =>
        dbContext.Characters.AsNoTracking()
            .OrderBy(character => character.Name)
            .Take(1000)
            .Select(character => new GridPrototypeCharacterDto(
                character.Id,
                character.Name,
                !string.IsNullOrEmpty(character.PortraitUrl) ? character.PortraitUrl : character.PhotoUrl,
                (int)character.CharacterType))
            .ToListAsync(ct);
}
