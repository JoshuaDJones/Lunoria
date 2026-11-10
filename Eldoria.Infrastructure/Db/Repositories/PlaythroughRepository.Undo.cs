using System.Text.Json;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed partial class PlaythroughRepository
{
    private ScenePT? checkpointScene;

    // Only mutable gameplay rows are captured, never user/account or library data.
    private static IEnumerable<object> TurnRows(ScenePT scene)
    {
        foreach (var character in scene.Playthrough.JourneyCharacters)
        {
            yield return character;
            foreach (var item in character.Spells) yield return item;
            foreach (var item in character.ConsumableItems) yield return item;
            foreach (var item in character.EquippableItems) yield return item;
        }
        foreach (var character in scene.SceneCharacters)
        {
            yield return character;
            foreach (var item in character.Spells) yield return item;
            foreach (var item in character.ConsumableItems) yield return item;
            foreach (var item in character.EquippableItems) yield return item;
        }
        foreach (var participant in scene.SceneParticipants) yield return participant;
        foreach (var chest in scene.SceneChests)
        {
            yield return chest;
            foreach (var entry in chest.ChestLootEntries) yield return entry;
        }
    }

    public void CaptureTurnCheckpoint(ScenePT scene)
    {
        checkpointScene = scene;
        if (scene.TurnStarted) return;
        var checkpoint = new SceneTurnCheckpoint
        {
            SceneId = scene.Id,
            RoundNumber = scene.RoundNumber,
            ParticipantId = scene.CurrentParticipantId,
            Rows = TurnRows(scene).Select(entity =>
            {
                var entry = dbContext.Entry(entity);
                return new SceneTurnCheckpointRow
                {
                    Type = entity.GetType().Name,
                    Id = (int)entry.Property("Id").CurrentValue!,
                    Values = entry.Properties.ToDictionary(p => p.Metadata.Name,
                        p => JsonSerializer.SerializeToElement(p.CurrentValue, p.Metadata.ClrType))
                };
            }).ToList()
        };
        scene.TurnCheckpointJson = JsonSerializer.Serialize(checkpoint);
        scene.TurnStarted = true;
        scene.MovementRecorded = false;
    }

    public async Task RestoreTurnCheckpointAsync(ScenePT scene, CancellationToken ct)
    {
        var checkpoint = JsonSerializer.Deserialize<SceneTurnCheckpoint>(scene.TurnCheckpointJson!)
            ?? throw new InvalidOperationException("The turn checkpoint is missing.");
        if (checkpoint.Version != 1 || checkpoint.SceneId != scene.Id)
            throw new InvalidOperationException("The turn checkpoint is incompatible.");

        var current = TurnRows(scene).ToDictionary(entity =>
            (entity.GetType().Name, (int)dbContext.Entry(entity).Property("Id").CurrentValue!));
        var originalKeys = checkpoint.Rows.Select(row => (row.Type, row.Id)).ToHashSet();

        // Break the scene's pointer before removing participants added during the turn.
        scene.CurrentParticipant = null;
        scene.CurrentParticipantId = null;
        scene.CounterattackerId = null;
        scene.CounterattackTargetId = null;
        scene.CounterattackToken = null;
        foreach (var chest in scene.SceneChests)
        {
            chest.SelectedLootEntry = null;
            chest.SelectedLootEntryId = null;
        }
        await dbContext.SaveChangesAsync(ct);

        // Move original inventory links back before deleting newly created owners.
        foreach (var row in checkpoint.Rows)
        {
            if (!current.TryGetValue((row.Type, row.Id), out var entity)) continue;
            var entry = dbContext.Entry(entity);
            foreach (var (name, value) in row.Values)
            {
                if (name == "Id") continue;
                var property = entry.Property(name);
                property.CurrentValue = value.Deserialize(property.Metadata.ClrType);
            }
        }
        await dbContext.SaveChangesAsync(ct);

        foreach (var (key, entity) in current)
            if (!originalKeys.Contains(key)) dbContext.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        // Only rows that gameplay can delete are recreated. New identity values are
        // intentional: old requests referencing removed inventory/participants stay stale.
        var recreatable = new[]
        {
            typeof(ScenePTParticipant), typeof(JourneyPTCharacterSpell),
            typeof(JourneyPTCharacterConsumableItem), typeof(JourneyPTCharacterEquippableItem),
            typeof(ScenePTCharacterSpell), typeof(ScenePTCharacterConsumableItem),
            typeof(ScenePTCharacterEquippableItem)
        }.ToDictionary(type => type.Name);
        var restoredParticipants = new Dictionary<int, ScenePTParticipant>();
        var restoredRows = new List<(SceneTurnCheckpointRow Row, object Entity)>();
        foreach (var row in checkpoint.Rows)
        {
            var exists = current.TryGetValue((row.Type, row.Id), out var entity);
            if (!exists)
            {
                if (!recreatable.TryGetValue(row.Type, out var type))
                    throw new InvalidOperationException("A required checkpoint entity no longer exists.");
                entity = Activator.CreateInstance(type)!;
                dbContext.Add(entity);
            }
            var entry = dbContext.Entry(entity!);
            foreach (var (name, value) in row.Values)
            {
                if (name == "Id") continue;
                var property = entry.Property(name);
                property.CurrentValue = value.Deserialize(property.Metadata.ClrType);
            }
            if (entity is ScenePTParticipant participant)
                restoredParticipants[row.Id] = participant;
            restoredRows.Add((row, entity!));
        }
        await dbContext.SaveChangesAsync(ct);

        // EF may substitute database defaults for CLR sentinel values on inserts
        // (for example zero attacks). Apply the exact checkpoint values once inserted.
        foreach (var (row, entity) in restoredRows)
        {
            var entry = dbContext.Entry(entity);
            foreach (var (name, value) in row.Values)
            {
                if (name == "Id") continue;
                var property = entry.Property(name);
                property.CurrentValue = value.Deserialize(property.Metadata.ClrType);
            }
        }
        foreach (var participant in restoredParticipants.Values)
            if (participant.LockedAttackTargetId is int oldId && restoredParticipants.TryGetValue(oldId, out var target))
                participant.LockedAttackTargetId = target.Id;
        scene.CurrentParticipant = checkpoint.ParticipantId is int id ? restoredParticipants[id] : null;
        scene.CurrentParticipantId = scene.CurrentParticipant?.Id;
        scene.RoundNumber = checkpoint.RoundNumber;
        scene.TurnStarted = false;
        scene.MovementRecorded = false;
        scene.TurnCheckpointJson = null;
        scene.UndoToken = null;
        checkpointScene = null;
    }
}
