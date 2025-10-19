using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class JourneyCharacterService : IJourneyCharacterService
    {
        private readonly IJourneyCharacterRepository _journeyCharacterRepository;
        private readonly IRepository<Character> _characterRepository;
        private readonly IRepository<Journey> _journeyRepository;

        public JourneyCharacterService(IJourneyCharacterRepository journeyCharacterRepository, IRepository<Journey> journeyRepository, IRepository<Character> characterRepository)
        {
            _journeyCharacterRepository = journeyCharacterRepository;
            _journeyRepository = journeyRepository;
            _characterRepository = characterRepository;
        }

        public async Task<Result> AdjustCharacterHpMpAsync(int journeyCharacterId, int newHp, int newMp, CancellationToken ct)
        {
            var journeyCharacter = await _journeyCharacterRepository.GetByIdAsync(journeyCharacterId, ct);

            if (journeyCharacter == null)
                return Result.Fail(new Error("JourneyCharacter.NotFound", "Journey Character was not found"));

            journeyCharacter.CurrentHp = newHp;
            journeyCharacter.CurrentMp = newMp;            

            await _journeyCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int journeyCharacterId, CancellationToken ct)
        {
            var journeyCharacter = await _journeyCharacterRepository.GetByIdAsync(journeyCharacterId, ct);

            if (journeyCharacter is null)
                return Result.Fail(new Error("JourneyCharacter.NotFound", "Journey Character does not exist."));

            _journeyCharacterRepository.Remove(journeyCharacter);
            await _journeyCharacterRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> ReplaceJourneyCharacters(int journeyId, List<int> characterIds, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(journeyId, ct);

            if (journey is null)
                return Result.Fail(new Error("Journey.NotFound", "Journey was not found."));

            var missingCharacterIds = new List<int>();
            var selectedCharacters = new List<Character>();

            foreach (var c in characterIds)
            {
                var character = await _characterRepository.GetByIdAsync(c, ct);

                if (character is null)
                    missingCharacterIds.Add(c);
                else
                    selectedCharacters.Add(character);
            }

            if (missingCharacterIds.Count > 0)
                return Result.Fail(new Error("Character.NotFound", $"The following characters were not found: {string.Join(", ", missingCharacterIds)}"));

            var journeyCharacters = await _journeyCharacterRepository.GetJourneyCharacters(journeyId, ct);

            foreach (var jc in journeyCharacters)
                _journeyCharacterRepository.Remove(jc);

            foreach (var c in selectedCharacters)
            {
                await _journeyCharacterRepository.AddAsync(new JourneyCharacter
                {
                    CurrentHp = c.MaxHp,
                    CurrentMp = c.MaxMp,
                    IsDown = false,
                    IsAlternateForm = false,
                    JourneyId = journeyId,
                    CharacterId = c.Id,
                }, ct);
            }

            await _journeyCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
