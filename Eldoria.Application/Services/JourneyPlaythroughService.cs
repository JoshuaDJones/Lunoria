using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class JourneyPlaythroughService(
        IJourneyPlaythroughRepository playthroughRepository,
        IOwnershipRepository ownershipRepository) : IJourneyPlaythroughService
    {
        private readonly IJourneyPlaythroughRepository _playthroughRepository =
            playthroughRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;

        public async Task<Result<JourneyPlaythroughDto>> StartAsync(
            int userId,
            int journeyId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetJourneyAsync(userId, journeyId, ct) is null)
                return NotFound<JourneyPlaythroughDto>("Journey.NotFound", "Journey was not found.");

            var active = await _playthroughRepository.GetActiveForJourneyAsync(
                userId,
                journeyId,
                ct);

            if (active is not null)
            {
                return Result<JourneyPlaythroughDto>.Fail(new Error(
                    "JourneyPlaythrough.ActiveExists",
                    "The journey already has an active playthrough."));
            }

            var playthrough = new JourneyPlaythrough
            {
                JourneyId = journeyId,
                StartedAt = DateTime.UtcNow,
                IsActive = true,
            };

            await _playthroughRepository.AddAsync(playthrough, ct);
            await _playthroughRepository.SaveChangesAsync(ct);
            return Result<JourneyPlaythroughDto>.Ok(playthrough.ToDto());
        }

        public async Task<Result<JourneyPlaythroughDto>> GetActiveAsync(
            int userId,
            int journeyId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetJourneyAsync(userId, journeyId, ct) is null)
                return NotFound<JourneyPlaythroughDto>("Journey.NotFound", "Journey was not found.");

            var playthrough = await _playthroughRepository.GetActiveForJourneyAsync(
                userId,
                journeyId,
                ct);

            return playthrough is null
                ? NotFound<JourneyPlaythroughDto>(
                    "JourneyPlaythrough.ActiveNotFound",
                    "The journey does not have an active playthrough.")
                : Result<JourneyPlaythroughDto>.Ok(playthrough.ToDto());
        }

        public async Task<Result<List<JourneyPlaythroughDto>>> ListAsync(
            int userId,
            int journeyId,
            int skip,
            int take,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetJourneyAsync(userId, journeyId, ct) is null)
            {
                return NotFound<List<JourneyPlaythroughDto>>(
                    "Journey.NotFound",
                    "Journey was not found.");
            }

            var playthroughs = await _playthroughRepository.ListForJourneyAsync(
                userId,
                journeyId,
                skip,
                take,
                ct);

            return Result<List<JourneyPlaythroughDto>>.Ok(
                playthroughs.Select(playthrough => playthrough.ToDto()).ToList());
        }

        public Task<Result<JourneyPlaythroughDto>> CompleteAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            return EndAsync(userId, journeyId, playthroughId, true, ct);
        }

        public Task<Result<JourneyPlaythroughDto>> DeactivateAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            return EndAsync(userId, journeyId, playthroughId, false, ct);
        }

        private async Task<Result<JourneyPlaythroughDto>> EndAsync(
            int userId,
            int journeyId,
            int playthroughId,
            bool complete,
            CancellationToken ct)
        {
            var playthrough = await _playthroughRepository.GetForUserAsync(
                userId,
                journeyId,
                playthroughId,
                ct);

            if (playthrough is null)
            {
                return NotFound<JourneyPlaythroughDto>(
                    "JourneyPlaythrough.NotFound",
                    "Journey playthrough was not found.");
            }

            if (!playthrough.IsActive)
            {
                return Result<JourneyPlaythroughDto>.Fail(new Error(
                    "JourneyPlaythrough.NotActive",
                    "The journey playthrough is not active."));
            }

            playthrough.IsActive = false;
            if (complete)
                playthrough.CompletedAt = DateTime.UtcNow;

            await _playthroughRepository.SaveChangesAsync(ct);
            return Result<JourneyPlaythroughDto>.Ok(playthrough.ToDto());
        }

        private static Result<T> NotFound<T>(string code, string message) =>
            Result<T>.Fail(new Error(code, message));
    }
}
