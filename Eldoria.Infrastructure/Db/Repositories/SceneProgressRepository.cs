using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneProgressRepository(ApplicationDbContext dbContext)
        : Repository<SceneProgress>(dbContext), ISceneProgressRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<SceneProgress?> GetAsync(
            int userId,
            int sceneProgressId,
            CancellationToken ct)
        {
            return ProgressQuery().SingleOrDefaultAsync(
                progress =>
                    progress.Id == sceneProgressId &&
                    progress.JourneyPlaythrough.Journey.UserId == userId,
                ct);
        }

        public Task<SceneProgress?> GetForSceneAsync(
            int userId,
            int playthroughId,
            int sceneId,
            CancellationToken ct)
        {
            return ProgressQuery().SingleOrDefaultAsync(
                progress =>
                    progress.JourneyPlaythroughId == playthroughId &&
                    progress.SceneId == sceneId &&
                    progress.JourneyPlaythrough.Journey.UserId == userId,
                ct);
        }

        public Task<List<SceneProgress>> ListForPlaythroughAsync(
            int userId,
            int playthroughId,
            CancellationToken ct)
        {
            return ProgressQuery()
                .AsNoTracking()
                .Where(progress =>
                    progress.JourneyPlaythroughId == playthroughId &&
                    progress.JourneyPlaythrough.Journey.UserId == userId)
                .OrderBy(progress => progress.Scene.SortOrder)
                .ToListAsync(ct);
        }

        public Task AddParticipantAsync(
            SceneParticipant participant,
            CancellationToken ct) =>
            _dbContext.SceneParticipants.AddAsync(participant, ct).AsTask();

        public void RemoveParticipant(SceneParticipant participant) =>
            _dbContext.SceneParticipants.Remove(participant);

        public Task AddTurnAsync(SceneParticipantTurn turn, CancellationToken ct) =>
            _dbContext.SceneParticipantTurns.AddAsync(turn, ct).AsTask();

        public void RemoveTurn(SceneParticipantTurn turn) =>
            _dbContext.SceneParticipantTurns.Remove(turn);

        public async Task ReorderTurnsAsync(
            IReadOnlyDictionary<int, int> positions,
            CancellationToken ct)
        {
            if (positions.Count == 0)
                return;

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            var turns = await _dbContext.SceneParticipantTurns
                .Where(turn => positions.Keys.Contains(turn.Id))
                .ToListAsync(ct);

            var temporaryPosition = -1;
            foreach (var turn in turns)
                turn.TurnPosition = temporaryPosition--;

            await _dbContext.SaveChangesAsync(ct);

            foreach (var turn in turns)
                turn.TurnPosition = positions[turn.Id];

            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        private IQueryable<SceneProgress> ProgressQuery()
        {
            return _dbContext.SceneProgresses
                .AsSplitQuery()
                .Include(progress => progress.JourneyPlaythrough)
                .Include(progress => progress.Scene)
                .Include(progress => progress.Participants)
                    .ThenInclude(participant => participant.Turns)
                .Include(progress => progress.ParticipantTurns);
        }
    }
}
