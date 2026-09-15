using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Services;

public sealed partial class ScenePlaythroughService
{
    public async Task<Result<SceneStartResultDto>> ContinueStartAsync(
        int userId, int playthroughId, int sceneId,
        SceneInventoryResolutionInput? resolution, CancellationToken ct)
    {
        await using var transaction = await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForStartAsync(userId, playthroughId, sceneId, ct);
        var error = ValidateStartingScene(scene);
        if (error is not null)
            return Result<SceneStartResultDto>.Fail(error);

        if (resolution is not null)
        {
            error = ResolveStartInventory(scene!, resolution);
            if (error is not null)
                return Result<SceneStartResultDto>.Fail(error);
        }

        error = ProcessSceneStart(scene!);
        if (error is not null)
            return Result<SceneStartResultDto>.Fail(error);

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        // Inventory links added by earlier events now have their persisted IDs.
        return Result<SceneStartResultDto>.Ok(GetStartResult(scene!));
    }

    public async Task<Result<SceneStartResultDto>> GetStartInventoryAsync(
        int userId, int playthroughId, int sceneId, CancellationToken ct)
    {
        var scene = await playthroughRepository.GetSceneForStartAsync(userId, playthroughId, sceneId, ct);
        if (scene is null)
            return Result<SceneStartResultDto>.Fail(new Error("ScenePlaythrough.NotFound", "Scene playthrough was not found."));
        return Result<SceneStartResultDto>.Ok(GetStartResult(scene));
    }

    private static Error? ValidateStartingScene(ScenePT? scene)
    {
        if (scene is null)
            return new Error("ScenePlaythrough.NotFound", "Scene playthrough was not found.");
        if (scene.Playthrough.CompletedAt is not null)
            return new Error("Playthrough.Completed", "The playthrough is completed.");
        if (scene.Status != ScenePlaythroughStatus.NotStarted)
            return new Error("ScenePlaythrough.AlreadyStarted", "The scene has already been started.");
        if (scene.SceneParticipants.Count != 0)
            return new Error("ScenePlaythrough.InvalidState", "The unstarted scene already has participants.");
        if (scene.Playthrough.Scenes.Any(other => other.Id != scene.Id && other.PendingInventoryActionId is not null))
            return new Error("ScenePlaythrough.InventoryResolutionRequired", "Resolve the pending reward in the other scene before starting this scene.");
        return null;
    }

    private static Error? ProcessSceneStart(ScenePT scene)
    {
        scene.PendingInventoryActionId = null;
        scene.InventoryResolutionToken = null;
        foreach (var sceneEvent in scene.SceneEvents.OrderBy(item => item.SortOrder).ThenBy(item => item.Id))
        {
            if (sceneEvent.ExecutionStatus == SceneEventExecutionStatus.Completed)
                continue;
            sceneEvent.ExecutionStatus = SceneEventExecutionStatus.InProgress;
            sceneEvent.StartedAt ??= DateTime.UtcNow;
            sceneEvent.ErrorMessage = null;
            foreach (var action in sceneEvent.ScenePTActionEvents.OrderBy(item => item.SortOrder).ThenBy(item => item.Id))
            {
                if (action.IsCompleted) continue;
                var error = action.EventActionType == EventActionType.CharacterGiveItem
                    ? ProcessItemGrant(scene, action)
                    : ExecuteSceneEventAction(scene, action);
                if (error is not null) return error;
                if (scene.PendingInventoryActionId is not null) return null;
                action.IsCompleted = true;
            }
            sceneEvent.ExecutionStatus = SceneEventExecutionStatus.Completed;
            sceneEvent.CompletedAt = DateTime.UtcNow;
        }

        var journeyParticipants = scene.Playthrough.JourneyCharacters
            .Where(character => character.IsActive)
            .OrderBy(character => character.SortOrder)
            .ThenBy(character => character.SourceJourneyCharacterId)
            .Select(character => new ScenePTParticipant
            {
                IsActive = true,
                SortOrderWithinType = character.SortOrder,
                ParticipantType = ParticipantType.Player,
                JourneyPlaythroughCharacter = character
            })
            .ToList();

        var npcParticipants = CreateSceneCharacterParticipants(
            scene,
            CharacterType.NPC,
            ParticipantType.NPC);

        var enemyParticipants = CreateSceneCharacterParticipants(
            scene,
            CharacterType.Enemy,
            ParticipantType.Enemy);

        var participants = journeyParticipants
            .Concat(npcParticipants)
            .Concat(enemyParticipants)
            .ToList();

        foreach (var participant in participants)
            scene.SceneParticipants.Add(participant);

        var startedAt = DateTime.UtcNow;
        scene.Status = ScenePlaythroughStatus.InProgress;
        scene.StartedAt = startedAt;
        scene.RoundNumber = 1;
        scene.CurrentParticipant = journeyParticipants.FirstOrDefault();
        if (scene.CurrentParticipant is not null)
            ResetAttacksForTurn(scene.CurrentParticipant);
        scene.Playthrough.EventLogs.Add(new PlaythroughEventLog
        {
            Message = $"Scene Started: {scene.Name}",
            EventTime = startedAt
        });


        return null;
    }

    private static List<JourneyPTCharacter> GrantTargets(ScenePT scene, ScenePTActionEvent action) =>
        scene.Playthrough.JourneyCharacters
            .Where(character => action.ActionTargetType == ActionTargetType.AllJourneyCharacters ||
                character.PlaythroughCharacterId == action.CharacterGiveItemAction!.PlaythroughCharacterId)
            .OrderBy(character => character.Id).ToList();

    private static Error? ProcessItemGrant(ScenePT scene, ScenePTActionEvent action)
    {
        var grant = action.CharacterGiveItemAction;
        if (grant is null || grant.Quantity is < 1 or > 1000 ||
            grant.PlaythroughConsumableItemId.HasValue == grant.PlaythroughEquippableItemId.HasValue ||
            (grant.PlaythroughConsumableItemId.HasValue && grant.PlaythroughConsumableItem is null) ||
            (grant.PlaythroughEquippableItemId.HasValue && grant.PlaythroughEquippableItem is null))
            return EventExecutionError(action, "The item grant is missing or invalid.");
        if (action.ActionTargetType is not (ActionTargetType.AllJourneyCharacters or ActionTargetType.SingleJourneyCharacter))
            return EventExecutionError(action, "Select one or all journey characters.");
        var targets = GrantTargets(scene, action);
        if ((action.ActionTargetType == ActionTargetType.SingleJourneyCharacter && targets.Count != 1) ||
            (action.ActionTargetType == ActionTargetType.AllJourneyCharacters && grant.PlaythroughCharacterId is not null))
            return EventExecutionError(action, "The item recipient is missing or ambiguous.");

        for (; action.GrantRecipientIndex < targets.Count; action.GrantRecipientIndex++, action.GrantItemsHandled = 0)
        {
            var character = targets[action.GrantRecipientIndex];
            while (action.GrantItemsHandled < grant.Quantity)
            {
                if (!CanReceive(character, grant.PlaythroughEquippableItem))
                {
                    scene.PendingInventoryActionId = action.Id;
                    scene.InventoryResolutionToken = Guid.NewGuid();
                    return null;
                }
                AddReward(character, grant);
                action.GrantItemsHandled++;
                AddEvent(scene, $"{character.PlaythroughCharacter.Name} received {RewardName(grant)}");
            }
        }
        return null;
    }

    private static string RewardName(PTCharacterGiveItemAction grant) =>
        grant.PlaythroughEquippableItem?.Name ?? grant.PlaythroughConsumableItem!.Name;

    private static int InventoryCount(JourneyPTCharacter character, bool equipment) =>
        equipment ? character.EquippableItems.Count : character.ConsumableItems.Count(item => !item.IsUsed);

    private static int InventoryCapacity(JourneyPTCharacter character, bool equipment)
    {
        var effects = ScenePlaythroughEquipmentEffects.For(character);
        return ScenePlaythroughEquipmentEffects.Apply(
            equipment ? character.MaxEquippableInventory : character.MaxConsumableInventory,
            equipment ? effects.MaxEquippableInventoryModifier : effects.MaxConsumableInventoryModifier);
    }

    private static bool CanReceive(JourneyPTCharacter character, PlaythroughEquippableItem? equipment)
    {
        if (equipment is null)
            return (long)InventoryCount(character, false) + 1 <= InventoryCapacity(character, false);
        var effects = ScenePlaythroughEquipmentEffects.For(character);
        long equipmentCapacity = (long)character.MaxEquippableInventory + effects.MaxEquippableInventoryModifier;
        long consumableCapacity = (long)character.MaxConsumableInventory + effects.MaxConsumableInventoryModifier;
        // Equipment effects are applied once per item type throughout gameplay.
        if (!character.EquippableItems.Any(item => item.PlaythroughEquippableItemId == equipment.Id))
        {
            equipmentCapacity += equipment.MaxEquippableInventoryModifier;
            consumableCapacity += equipment.MaxConsumableInventoryModifier;
        }
        return (long)InventoryCount(character, true) + 1 <= Math.Clamp(equipmentCapacity, 0, int.MaxValue)
            && InventoryCount(character, false) <= Math.Clamp(consumableCapacity, 0, int.MaxValue);
    }

    private static void AddReward(JourneyPTCharacter character, PTCharacterGiveItemAction grant)
    {
        if (grant.PlaythroughEquippableItem is { } equipment)
            character.EquippableItems.Add(new JourneyPTCharacterEquippableItem
            {
                IsEquipped = true, PlaythroughEquippableItemId = equipment.Id,
                PlaythroughEquippableItem = equipment
            });
        else
            character.ConsumableItems.Add(new JourneyPTCharacterConsumableItem
            {
                IsUsed = false, PlaythroughConsumableItemId = grant.PlaythroughConsumableItem!.Id,
                PlaythroughConsumableItem = grant.PlaythroughConsumableItem
            });
    }

    private static SceneStartResultDto GetStartResult(ScenePT scene)
    {
        if (scene.PendingInventoryActionId is not int actionId)
            return new SceneStartResultDto { Started = scene.Status != ScenePlaythroughStatus.NotStarted };
        var action = scene.SceneEvents.SelectMany(item => item.ScenePTActionEvents).Single(item => item.Id == actionId);
        var grant = action.CharacterGiveItemAction!;
        var character = GrantTargets(scene, action)[action.GrantRecipientIndex];
        var equipment = grant.PlaythroughEquippableItem is not null;
        var recipients = scene.Playthrough.JourneyCharacters
            .Where(target => target.Id != character.Id && target.IsActive).OrderBy(target => target.Id).ToList();
        var pending = new SceneStartInventoryDto
        {
            ResolutionToken = scene.InventoryResolutionToken!.Value,
            CharacterName = character.PlaythroughCharacter.Name,
            EventName = action.Name,
            RewardName = RewardName(grant),
            IsEquippable = equipment,
            RemainingQuantity = grant.Quantity - action.GrantItemsHandled,
            InventoryCount = InventoryCount(character, equipment),
            InventoryCapacity = InventoryCapacity(character, equipment),
            RewardRecipientIds = recipients.Where(target => CanReceive(target, grant.PlaythroughEquippableItem)).Select(target => target.Id).ToList(),
            Recipients = recipients.Select(target => new SceneStartRecipientDto
            {
                Id = target.Id, Name = target.PlaythroughCharacter.Name,
                AvailableSlots = Math.Max(0, InventoryCapacity(target, equipment) - InventoryCount(target, equipment))
            }).ToList()
        };
        pending.Items = equipment
            ? character.EquippableItems.Select(item => new SceneStartInventoryItemDto
            {
                Id = item.Id, Name = item.PlaythroughEquippableItem.Name,
                RecipientIds = recipients.Where(target => CanReceive(target, item.PlaythroughEquippableItem)).Select(target => target.Id).ToList()
            }).ToList()
            : character.ConsumableItems.Where(item => !item.IsUsed).Select(item => new SceneStartInventoryItemDto
            {
                Id = item.Id, Name = item.PlaythroughConsumableItem.Name,
                RecipientIds = recipients.Where(target => CanReceive(target, null)).Select(target => target.Id).ToList()
            }).ToList();
        return new SceneStartResultDto { PendingInventory = pending };
    }

    private static Error? ResolveStartInventory(ScenePT scene, SceneInventoryResolutionInput input)
    {
        if (scene.PendingInventoryActionId is not int actionId || scene.InventoryResolutionToken != input.ResolutionToken)
            return new Error("ScenePlaythrough.InventoryResolutionStale", "This reward has changed. Reload its inventory to continue.");
        var action = scene.SceneEvents.SelectMany(item => item.ScenePTActionEvents).Single(item => item.Id == actionId);
        var grant = action.CharacterGiveItemAction!;
        var character = GrantTargets(scene, action)[action.GrantRecipientIndex];
        JourneyPTCharacter? target = null;
        if (input.TargetJourneyCharacterId is int targetId)
        {
            target = scene.Playthrough.JourneyCharacters.SingleOrDefault(candidate => candidate.Id == targetId);
            if (target is null || target.Id == character.Id || !target.IsActive)
                return new Error("ScenePlaythrough.InvalidInventoryRecipient", "Select another active player in this playthrough.");
        }
        string itemName;
        if (input.InventoryItemId is null)
        {
            if (target is not null && !CanReceive(target, grant.PlaythroughEquippableItem))
                return new Error("ScenePlaythrough.InventoryFull", "The receiving player's inventory is full.");
            itemName = RewardName(grant);
            if (target is not null) AddReward(target, grant);
            action.GrantItemsHandled++;
        }
        else if (grant.PlaythroughEquippableItem is not null)
        {
            var item = character.EquippableItems.SingleOrDefault(item => item.Id == input.InventoryItemId);
            if (item is null)
                return new Error("ScenePlaythrough.InventoryItemNotFound", "The selected inventory item was not found.");
            if (target is not null && !CanReceive(target, item.PlaythroughEquippableItem))
                return new Error("ScenePlaythrough.InventoryFull", "The receiving player's inventory is full.");
            if (character.EquippableItems.Count(link => link.PlaythroughEquippableItemId == item.PlaythroughEquippableItemId) == 1)
            {
                var effects = ScenePlaythroughEquipmentEffects.For(character);
                var equipmentCapacity = Math.Clamp((long)character.MaxEquippableInventory + effects.MaxEquippableInventoryModifier - item.PlaythroughEquippableItem.MaxEquippableInventoryModifier, 0, int.MaxValue);
                var consumableCapacity = Math.Clamp((long)character.MaxConsumableInventory + effects.MaxConsumableInventoryModifier - item.PlaythroughEquippableItem.MaxConsumableInventoryModifier, 0, int.MaxValue);
                if (InventoryCount(character, true) - 1 > equipmentCapacity || InventoryCount(character, false) > consumableCapacity)
                    return new Error("ScenePlaythrough.InventoryCapacityRequired", "Removing this equipment would leave too little inventory capacity. Choose another item or give away/discard the incoming reward.");
            }
            itemName = item.PlaythroughEquippableItem.Name;
            character.EquippableItems.Remove(item);
            if (target is not null)
            {
                item.JourneyPTCharacterId = target.Id;
                item.JourneyPTCharacter = target;
                target.EquippableItems.Add(item);
            }
        }
        else
        {
            var item = character.ConsumableItems.SingleOrDefault(item => item.Id == input.InventoryItemId && !item.IsUsed);
            if (item is null)
                return new Error("ScenePlaythrough.InventoryItemNotFound", "The selected inventory item was not found.");
            if (target is not null && !CanReceive(target, null))
                return new Error("ScenePlaythrough.InventoryFull", "The receiving player's inventory is full.");
            itemName = item.PlaythroughConsumableItem.Name;
            character.ConsumableItems.Remove(item);
            if (target is not null)
            {
                item.JourneyPTCharacterId = target.Id;
                item.JourneyPTCharacter = target;
                target.ConsumableItems.Add(item);
            }
        }
        AddEvent(scene, target is null
            ? $"{character.PlaythroughCharacter.Name} discarded {itemName} to resolve an event reward"
            : $"{character.PlaythroughCharacter.Name} gave {itemName} to {target.PlaythroughCharacter.Name} to resolve an event reward");
        return null;
    }
}
