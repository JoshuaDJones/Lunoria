using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Eldoria.Core.Exceptions;
using Eldoria.Core.Enums;
using Eldoria.Core.Snapshots;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Eldoria.Core.Entities.Playthrough;
using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Application.Services
{
    public class JourneyPlaythroughService(
        IJourneyPlaythroughRepository playthroughRepository,
        IJourneySnapshotBuilder snapshotBuilder) : IJourneyPlaythroughService
    {
        private readonly IJourneyPlaythroughRepository _playthroughRepository =
            playthroughRepository;
        private readonly IJourneySnapshotBuilder _snapshotBuilder = snapshotBuilder;

        public async Task<Result<JourneyPlaythroughDto>> StartAsync(
            int userId,
            int journeyId,
            CancellationToken ct)
        {
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

            JourneySnapshotV1? snapshot;
            try
            {
                snapshot = await _snapshotBuilder.BuildAsync(userId, journeyId, ct);
            }
            catch (InvalidOperationException ex)
            {
                return Result<JourneyPlaythroughDto>.Fail(new Error(
                    "JourneyPlaythrough.InvalidSnapshot",
                    ex.Message));
            }

            if (snapshot is null)
                return NotFound<JourneyPlaythroughDto>("Journey.NotFound", "Journey was not found.");

            var snapshotJson = JsonSerializer.Serialize(snapshot, SnapshotJsonOptions);
            var revision = new JourneyRevision
            {
                SourceJourneyId = journeyId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                SchemaVersion = JourneySnapshotV1.Version,
                ContentHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(snapshotJson))),
                SnapshotJson = snapshotJson
            };
            var playthrough = CreateRuntimeState(journeyId, snapshot, revision);

            try
            {
                var started = await _playthroughRepository.StartAsync(
                    userId, journeyId, revision, playthrough, ct);
                return Result<JourneyPlaythroughDto>.Ok(started.ToDto());
            }
            catch (ActivePlaythroughExistsException)
            {
                return Result<JourneyPlaythroughDto>.Fail(new Error(
                    "JourneyPlaythrough.ActiveExists",
                    "The journey already has an active playthrough."));
            }
        }

        public async Task<Result<JourneyPlaythroughDto>> GetActiveAsync(
            int userId,
            int journeyId,
            CancellationToken ct)
        {
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

        public async Task<Result<JourneyPlaythroughDto>> ResumeAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            var playthrough = await _playthroughRepository.GetForUserAsync(
                userId, journeyId, playthroughId, ct);

            if (playthrough is null)
                return NotFound<JourneyPlaythroughDto>("JourneyPlaythrough.NotFound", "Journey playthrough was not found.");

            if (playthrough.CompletedAt is not null)
                return Result<JourneyPlaythroughDto>.Fail(new Error("JourneyPlaythrough.Completed", "A completed playthrough cannot be resumed."));

            if (playthrough.IsActive)
                return Result<JourneyPlaythroughDto>.Ok(playthrough.ToDto());

            var active = await _playthroughRepository.GetActiveForJourneyAsync(userId, journeyId, ct);
            if (active is not null)
                return Result<JourneyPlaythroughDto>.Fail(new Error("JourneyPlaythrough.ActiveExists", "The journey already has an active playthrough."));

            playthrough.IsActive = true;
            await _playthroughRepository.SaveChangesAsync(ct);
            return Result<JourneyPlaythroughDto>.Ok(playthrough.ToDto());
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

        private static Playthrough CreateRuntimeState(
            int journeyId,
            JourneySnapshotV1 snapshot,
            JourneyRevision revision)
        {
            var playthrough = new Playthrough
            {
                JourneyId = journeyId,
                SourceJourneyId = journeyId,
                JourneyRevision = revision,
                StartedAt = DateTime.UtcNow,
                IsActive = true
            };

            var characterDefinitions = snapshot.Characters.ToDictionary(character => character.Key);
            var journeyRuntimeCharacters = snapshot.Journey.Characters.ToDictionary(
                definition => definition.Key,
                definition => new PlaythroughCharacter
                {
                    JourneyCharacterId = definition.SourceJourneyCharacterId,
                    SnapshotAssignmentKey = definition.Key,
                    SnapshotCharacterKey = definition.CharacterKey,
                    MeleeAttackDamage = definition.MeleeAttackDamage,
                    BowAttackDamage = definition.BowAttackDamage,
                    Movement = definition.Movement,
                    MaxConsumableInventory = definition.MaxConsumableInventory,
                    MaxEquippableInventory = definition.MaxEquippableInventory,
                    CurrentHp = definition.MaxHp,
                    CurrentMp = definition.MaxMp,
                    MaxHp = definition.MaxHp,
                    MaxMp = definition.MaxMp,
                    IsDown = false,
                    CharacterSpells = definition.Spells.Select(spell => new JourneyPlaythroughCharacterSpell
                    {
                        JourneyCharacterSpellId = spell.SourceAssignmentId,
                        SnapshotSpellKey = spell.SpellKey
                    }).ToList()
                });

            foreach (var definition in snapshot.Journey.Characters)
            {
                if (definition.AlternateFormCharacterKey is null)
                    continue;
                var alternateDefinition = characterDefinitions[definition.AlternateFormCharacterKey];
                var alternateAssignmentKey = $"{definition.Key}:alternate";
                var alternate = new PlaythroughCharacter
                {
                    SnapshotAssignmentKey = alternateAssignmentKey,
                    SnapshotCharacterKey = alternateDefinition.Key,
                    MeleeAttackDamage = alternateDefinition.BaseMeleeAttackDamage,
                    BowAttackDamage = alternateDefinition.BaseBowAttackDamage,
                    Movement = alternateDefinition.BaseMovement,
                    MaxConsumableInventory = alternateDefinition.BaseMaxConsumableInventory,
                    MaxEquippableInventory = alternateDefinition.BaseMaxEquippableInventory,
                    CurrentHp = alternateDefinition.BaseMaxHp,
                    CurrentMp = alternateDefinition.BaseMaxMp,
                    MaxHp = alternateDefinition.BaseMaxHp,
                    MaxMp = alternateDefinition.BaseMaxMp,
                    IsDown = false,
                    CharacterSpells = alternateDefinition.SpellKeys.Select(spellKey =>
                        new JourneyPlaythroughCharacterSpell { SnapshotSpellKey = spellKey }).ToList()
                };
                journeyRuntimeCharacters[alternateAssignmentKey] = alternate;
                journeyRuntimeCharacters[definition.Key].AlternateForm = alternate;
            }

            foreach (var runtimeCharacter in journeyRuntimeCharacters.Values)
                playthrough.JourneyCharacters.Add(runtimeCharacter);

            foreach (var scene in snapshot.Scenes)
            {
                var scenePlaythrough = new ScenePlaythrough
                {
                    SceneId = scene.SourceSceneId,
                    SourceSceneId = scene.SourceSceneId,
                    SnapshotSceneKey = scene.Key,
                    SnapshotSortOrder = scene.SortOrder,
                    Status = ScenePlaythroughStatus.NotStarted,
                    RoundNumber = 0
                };

                var sceneRuntimeCharacters = scene.Characters.ToDictionary(
                    definition => definition.Key,
                    definition => new ScenePlaythroughCharacter
                    {
                        SceneCharacterId = definition.SourceSceneCharacterId,
                    SnapshotAssignmentKey = definition.Key,
                    SnapshotCharacterKey = definition.CharacterKey,
                        MeleeAttackDamage = definition.MeleeAttackDamage,
                        BowAttackDamage = definition.BowAttackDamage,
                        Movement = definition.Movement,
                        MaxConsumableInventory = definition.MaxConsumableInventory,
                        MaxEquippableInventory = definition.MaxEquippableInventory,
                        CurrentHp = definition.MaxHp,
                        CurrentMp = definition.MaxMp,
                        MaxHp = definition.MaxHp,
                        MaxMp = definition.MaxMp,
                        IsDead = false,
                        CharacterSpells = definition.Spells.Select(spell => new ScenePlaythroughCharacterSpell
                        {
                            SceneCharacterSpellId = spell.SourceAssignmentId,
                            SnapshotSpellKey = spell.SpellKey
                        }).ToList()
                    });

                foreach (var definition in scene.Characters)
                {
                    if (definition.AlternateFormCharacterKey is null)
                        continue;
                    var alternateDefinition = characterDefinitions[definition.AlternateFormCharacterKey];
                    var alternateAssignmentKey = $"{definition.Key}:alternate";
                    var alternate = new ScenePlaythroughCharacter
                    {
                        SnapshotAssignmentKey = alternateAssignmentKey,
                        SnapshotCharacterKey = alternateDefinition.Key,
                        MeleeAttackDamage = alternateDefinition.BaseMeleeAttackDamage,
                        BowAttackDamage = alternateDefinition.BaseBowAttackDamage,
                        Movement = alternateDefinition.BaseMovement,
                        MaxConsumableInventory = alternateDefinition.BaseMaxConsumableInventory,
                        MaxEquippableInventory = alternateDefinition.BaseMaxEquippableInventory,
                        CurrentHp = alternateDefinition.BaseMaxHp,
                        CurrentMp = alternateDefinition.BaseMaxMp,
                        MaxHp = alternateDefinition.BaseMaxHp,
                        MaxMp = alternateDefinition.BaseMaxMp,
                        IsDead = false,
                        CharacterSpells = alternateDefinition.SpellKeys.Select(spellKey =>
                            new ScenePlaythroughCharacterSpell { SnapshotSpellKey = spellKey }).ToList()
                    };
                    sceneRuntimeCharacters[alternateAssignmentKey] = alternate;
                    sceneRuntimeCharacters[definition.Key].AlternateForm = alternate;
                }

                foreach (var runtimeCharacter in sceneRuntimeCharacters.Values)
                    scenePlaythrough.SceneCharacters.Add(runtimeCharacter);

                foreach (var sceneEvent in scene.Events)
                {
                    scenePlaythrough.PlaythroughEvents.Add(new ScenePlaythroughEvent
                    {
                        SceneEventId = sceneEvent.SourceEventId,
                        SnapshotEventKey = sceneEvent.Key,
                        ExecutionStatus = SceneEventExecutionStatus.NotStarted
                    });
                }

                foreach (var chest in scene.Chests)
                {
                    scenePlaythrough.PlaythroughChests.Add(new ScenePlaythroughChest
                    {
                        SceneChestId = chest.SourceChestId,
                        SnapshotChestKey = chest.Key,
                        Status = ChestStatus.Unopened
                    });
                }

                var participantSortOrders = new Dictionary<ParticipantType, int>();
                foreach (var definition in snapshot.Journey.Characters.Where(character => character.IsInitiallyActive))
                {
                    var type = ToParticipantType(characterDefinitions[definition.CharacterKey].CharacterType);
                    scenePlaythrough.Participants.Add(new ScenePlaythroughParticipant
                    {
                        JourneyPlaythroughCharacter = journeyRuntimeCharacters[definition.Key],
                        IsActive = true,
                        ParticipantType = type,
                        SortOrderWithinType = NextSortOrder(participantSortOrders, type)
                    });
                }

                foreach (var definition in scene.Characters.Where(character => character.IsInitiallyActive))
                {
                    var type = ToParticipantType(characterDefinitions[definition.CharacterKey].CharacterType);
                    scenePlaythrough.Participants.Add(new ScenePlaythroughParticipant
                    {
                        ScenePlaythroughCharacter = sceneRuntimeCharacters[definition.Key],
                        IsActive = true,
                        ParticipantType = type,
                        SortOrderWithinType = NextSortOrder(participantSortOrders, type)
                    });
                }

                playthrough.ScenePlaythroughs.Add(scenePlaythrough);
            }

            return playthrough;
        }

        private static int NextSortOrder(IDictionary<ParticipantType, int> values, ParticipantType type)
        {
            values.TryGetValue(type, out var current);
            values[type] = current + 1;
            return current;
        }

        private static ParticipantType ToParticipantType(CharacterType type) => type switch
        {
            CharacterType.Player => ParticipantType.Player,
            CharacterType.NPC => ParticipantType.NPC,
            CharacterType.Enemy => ParticipantType.Enemy,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
