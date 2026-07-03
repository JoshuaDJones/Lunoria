using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class JourneyCharacterSpellService(
        IJourneyCharacterSpellRepository journeyCharacterSpellRepository,
        ISpellRepository spellRepository) : IJourneyCharacterSpellService
    {
        private readonly IJourneyCharacterSpellRepository _journeyCharacterSpellRepository =
            journeyCharacterSpellRepository;
        private readonly ISpellRepository _spellRepository = spellRepository;

        public async Task<Result<JourneyCharacterSpellDto>> GrantAsync(
            int userId,
            int journeyCharacterId,
            int spellId,
            CancellationToken ct)
        {
            var character = await _journeyCharacterSpellRepository.GetCharacterForUserAsync(
                userId,
                journeyCharacterId,
                ct);

            if (character is null)
                return Fail("JourneyCharacter.NotFound", "Journey character was not found.");

            var spell = await _spellRepository.GetByIdForUserAsync(userId, spellId, ct);
            if (spell is null)
                return Fail("Spell.NotFound", "Spell was not found.");

            if (character.JourneyCharacterSpells.Any(
                assignment => assignment.SpellId == spellId))
            {
                return Fail(
                    "JourneyCharacterSpell.AlreadyGranted",
                    "The journey character already has this spell.");
            }

            var assignment = new JourneyCharacterSpell
            {
                JourneyCharacterId = journeyCharacterId,
                SpellId = spellId,
                Spell = spell,
            };

            await _journeyCharacterSpellRepository.AddAsync(assignment, ct);
            await _journeyCharacterSpellRepository.SaveChangesAsync(ct);

            return Result<JourneyCharacterSpellDto>.Ok(assignment.ToDto());
        }

        public async Task<Result> RemoveAsync(
            int userId,
            int journeyCharacterId,
            int spellId,
            CancellationToken ct)
        {
            var assignment = await _journeyCharacterSpellRepository.GetAssignmentForUserAsync(
                userId,
                journeyCharacterId,
                spellId,
                ct);

            if (assignment is null)
            {
                return Result.Fail(new Error(
                    "JourneyCharacterSpell.NotFound",
                    "The journey character spell was not found."));
            }

            _journeyCharacterSpellRepository.Remove(assignment);
            await _journeyCharacterSpellRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        private static Result<JourneyCharacterSpellDto> Fail(string code, string message) =>
            Result<JourneyCharacterSpellDto>.Fail(new Error(code, message));
    }
}
