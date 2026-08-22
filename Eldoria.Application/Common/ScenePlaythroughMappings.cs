using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughMappings
{
    public static ScenePlaythroughDetailsDto ToDetailsDto(this ScenePT scene)
    {
        return new ScenePlaythroughDetailsDto
        {
            Id = scene.Id,
            PlaythroughId = scene.PlaythroughId,
            Name = scene.Name,
            Description = scene.Description,
            PhotoUrl = scene.PhotoUrl,
            Status = scene.Status,
            RoundNumber = scene.RoundNumber,
            StartedAt = scene.StartedAt,
            EndedAt = scene.EndedAt,
            CurrentParticipantId = scene.CurrentParticipantId,
            Participants = scene.SceneParticipants
                .OrderBy(participant => participant.ParticipantType)
                .ThenBy(participant => participant.SortOrderWithinType)
                .ThenBy(participant => participant.Id)
                .Select(participant => participant.ToDto(scene.CurrentParticipantId))
                .ToList(),
            Chests = scene.SceneChests
                .OrderBy(chest => chest.SourceSceneChestId)
                .ThenBy(chest => chest.Id)
                .Select(chest => new ScenePlaythroughChestDto
                {
                    Id = chest.Id,
                    Name = chest.Name,
                    DieSides = chest.DieSides,
                    Status = chest.Status,
                    RolledValue = chest.RolledValue,
                    OpenedAt = chest.OpenedAt,
                    SelectedLootEntryId = chest.SelectedLootEntryId,
                    LootEntries = chest.ChestLootEntries
                        .OrderBy(entry => entry.RollMinimum)
                        .ThenBy(entry => entry.Id)
                        .Select(entry => new ScenePlaythroughChestLootEntryDto
                        {
                            Id = entry.Id,
                            RollMinimum = entry.RollMinimum,
                            RollMaximum = entry.RollMaximum,
                            Quantity = entry.Quantity,
                            EquippableItem = entry.PlaythroughEquippableItem is null
                                ? null
                                : new ScenePlaythroughLootItemDto
                                {
                                    Id = entry.PlaythroughEquippableItem.Id,
                                    Name = entry.PlaythroughEquippableItem.Name,
                                    Description = entry.PlaythroughEquippableItem.Description,
                                    PhotoUrl = entry.PlaythroughEquippableItem.PhotoUrl
                                },
                            ConsumableItem = entry.PlaythroughConsumableItem is null
                                ? null
                                : new ScenePlaythroughLootItemDto
                                {
                                    Id = entry.PlaythroughConsumableItem.Id,
                                    Name = entry.PlaythroughConsumableItem.Name,
                                    Description = entry.PlaythroughConsumableItem.Description,
                                    PhotoUrl = entry.PlaythroughConsumableItem.PhotoUrl
                                }
                        })
                        .ToList()
                })
                .ToList(),
            Dialogs = scene.SceneDialogs
                .OrderBy(dialog => dialog.SourceSceneDialogId)
                .ThenBy(dialog => dialog.Id)
                .Select(dialog => new ScenePlaythroughDialogDto
                {
                    Id = dialog.Id,
                    Title = dialog.Title,
                    DialogPages = dialog.DialogPages
                        .OrderBy(page => page.OrderNum)
                        .ThenBy(page => page.Id)
                        .Select(page => new ScenePlaythroughDialogPageDto
                        {
                            Id = page.Id,
                            OrderNum = page.OrderNum,
                            PhotoUrl = page.PhotoUrl,
                            DialogPageSections = page.DialogPageSections
                                .OrderBy(section => section.OrderNum)
                                .ThenBy(section => section.Id)
                                .Select(section => new ScenePlaythroughDialogSectionDto
                                {
                                    Id = section.Id,
                                    OrderNum = section.OrderNum,
                                    ReadingText = section.ReadingText,
                                    IsNarrator = section.IsNarrator,
                                    Character = section.Character is null
                                        ? null
                                        : new ScenePlaythroughDialogCharacterDto
                                        {
                                            Id = section.Character.Id,
                                            Name = section.Character.Name,
                                            PhotoUrl = section.Character.PhotoUrl,
                                            PortraitUrl = section.Character.PortraitUrl,
                                            DialogActiveColor =
                                                section.Character.DialogActiveColor,
                                            DialogInActiveColor =
                                                section.Character.DialogInActiveColor
                                        }
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList(),
            EventLogs = scene.Playthrough.EventLogs
                .OrderBy(eventLog => eventLog.EventTime)
                .ThenBy(eventLog => eventLog.Id)
                .Select(eventLog => new PlaythroughEventLogDto
                {
                    Id = eventLog.Id,
                    Message = eventLog.Message,
                    EventTime = eventLog.EventTime
                })
                .ToList()
        };
    }

    private static ScenePlaythroughParticipantDto ToDto(
        this ScenePTParticipant participant,
        int? currentParticipantId)
    {
        var journeyCharacter = participant.JourneyPlaythroughCharacter;
        var sceneCharacter = participant.ScenePlaythroughCharacter;
        var baseCharacter = journeyCharacter?.PlaythroughCharacter
            ?? sceneCharacter?.PlaythroughCharacter
            ?? throw new InvalidOperationException(
                "A scene participant must reference a playthrough character.");
        var alternateForm = journeyCharacter?.AlternateForm
            ?? sceneCharacter?.AlternateForm;
        var isInAlternateForm = journeyCharacter?.IsInAlternateForm
            ?? sceneCharacter?.IsInAlternateForm
            ?? false;
        PlaythroughCharacter displayedCharacter =
            isInAlternateForm && alternateForm is not null
                ? alternateForm
                : baseCharacter;

        return new ScenePlaythroughParticipantDto
        {
            Id = participant.Id,
            ParticipantType = participant.ParticipantType,
            SortOrderWithinType = participant.SortOrderWithinType,
            IsActive = participant.IsActive,
            IsCurrentParticipant = participant.Id == currentParticipantId,
            JourneyPlaythroughCharacterId = participant.JourneyPlaythroughCharacterId,
            ScenePlaythroughCharacterId = participant.ScenePlaythroughCharacterId,
            PlaythroughCharacterId = baseCharacter.Id,
            DisplayedPlaythroughCharacterId = displayedCharacter.Id,
            Name = displayedCharacter.Name,
            PhotoUrl = displayedCharacter.PhotoUrl,
            PortraitUrl = displayedCharacter.PortraitUrl,
            CurrentHp = journeyCharacter?.CurrentHp ?? sceneCharacter!.CurrentHp,
            MaxHp = journeyCharacter?.MaxHp ?? sceneCharacter!.MaxHp,
            CurrentMp = journeyCharacter?.CurrentMp ?? sceneCharacter!.CurrentMp,
            MaxMp = journeyCharacter?.MaxMp ?? sceneCharacter!.MaxMp,
            IsDown = journeyCharacter?.IsDown ?? false,
            IsDead = sceneCharacter?.IsDead ?? false,
            IsInAlternateForm = isInAlternateForm
        };
    }
}
