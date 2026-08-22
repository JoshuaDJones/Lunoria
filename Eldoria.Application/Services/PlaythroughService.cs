using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services;

public sealed class PlaythroughService(
    IPlaythroughRepository playthroughRepository,
    IJourneyRepository journeyRepository
    ) : IPlaythroughService
{
    public async Task<Result<PlaythroughSummaryDto>> StartAsync(
        int userId,
        int journeyId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginStartTransactionAsync(ct);

        var journey = await journeyRepository.GetByIdAsync(journeyId, ct);

        if (journey is null)
        {
            return Result<PlaythroughSummaryDto>.Fail(
                new Error("Journey.NotFound", "Journey was not found."));
        }

        if (journey.UserId != userId)
        {
            return Result<PlaythroughSummaryDto>.Fail(
                new Error(
                    "Auth.Forbidden",
                    "You do not have permission to start a playthrough for this journey."));
        }

        var unfinishedPlaythroughs =
            await playthroughRepository.ListUnfinishedForJourneyAsync(
                userId,
                journeyId,
                ct);

        var startedAt = DateTime.UtcNow;

        foreach (var playthrough in unfinishedPlaythroughs)
            playthrough.CompletedAt = startedAt;

        var sourceJourney =
            await journeyRepository.GetPlaythroughSourceAsync(userId, journeyId, ct);

        if (sourceJourney is null)
        {
            return Result<PlaythroughSummaryDto>.Fail(
                new Error("Journey.NotFound", "Journey was not found."));
        }

        var referencedCharacterIds = GetReferencedCharacterIds(sourceJourney);
        var assets = await playthroughRepository.GetStartAssetsAsync(
            userId,
            referencedCharacterIds,
            ct);

        var sourceError = ValidateStartSource(sourceJourney, assets);

        if (sourceError is not null)
            return Result<PlaythroughSummaryDto>.Fail(sourceError);

        var newPlaythrough = new Playthrough
        {
            SourceJourneyId = sourceJourney.Id,
            UserId = sourceJourney.UserId,
            Name = sourceJourney.Name,
            Description = sourceJourney.Description,
            PhotoUrl = sourceJourney.PhotoUrl,
            FileName = sourceJourney.FileName,
            StartedAt = startedAt
        };

        // Add scenes from journey to playthrough

        var scenePTsBySourceId = sourceJourney.Scenes
            .Select(scene => new ScenePT
            {
                SourceSceneId = scene.Id,
                Name = scene.Name,
                Description = scene.Description,
                PhotoUrl = scene.PhotoUrl,
                FileName = scene.FileName,
                GridUrl = scene.GridUrl,
                SortOrder = scene.SortOrder,
                Status = ScenePlaythroughStatus.NotStarted,
                RoundNumber = 0
            })
            .ToDictionary(
                scene => scene.SourceSceneId,
                scene => scene);

        newPlaythrough.Scenes = [.. scenePTsBySourceId.Values];

        // Add intro pages from journey to playthrough

        var introPagePTList = sourceJourney.IntroPages
            .Select(introPage => new PlaythroughIntroPage
            {
                SourceIntroPageId = introPage.Id,
                SortOrder = introPage.SortOrder,
                Type = introPage.Type,
                Config = introPage.Config,
                PreviewPhotoUrl = introPage.PreviewPhotoUrl
            })
            .ToList();

        newPlaythrough.IntroPages = introPagePTList;

        // Add spell types to playthrough

        var spellTypes = assets.SpellTypes;

        var playthroughSpellTypesBySourceId = spellTypes
            .Select(spellType => new PlaythroughSpellType
            {
                SourceSpellTypeId = spellType.Id,
                TypeName = spellType.TypeName,
                Description = spellType.Description,
                PhotoUrl = spellType.PhotoUrl,
                FileName = spellType.FileName
            })
            .ToDictionary(
                spellType => spellType.SourceSpellTypeId,
                spellType => spellType);

        newPlaythrough.SpellTypes = [.. playthroughSpellTypesBySourceId.Values];

        // Add Spells to playthrough

        var spells = assets.Spells;

        var playthroughSpellsBySourceId = spells
            .Select(spell => new PlaythroughSpell
            {
                SourceSpellId = spell.Id,
                Name = spell.Name,
                Description = spell.Description,
                PhotoUrl = spell.PhotoUrl,
                FileName = spell.FileName,
                Range = spell.Range,
                IsRadius = spell.IsRadius,
                MpCost = spell.MpCost,
                DamageEffect = spell.DamageEffect,
                HealthEffect = spell.HealthEffect,
                MagicEffect = spell.MagicEffect,
                PlaythroughSpellType =
                    playthroughSpellTypesBySourceId[spell.SpellTypeId]
            })
            .ToDictionary(
                spell => spell.SourceSpellId,
                spell => spell);

        newPlaythrough.Spells = [.. playthroughSpellsBySourceId.Values];

        // Add characters to playthrough

        var characters = assets.Characters;

        var playthroughCharactersBySourceId = characters
              .Select(character => new PlaythroughCharacter
              {
                  SourceCharacterId = character.Id,
                  CharacterType = character.CharacterType,
                  Name = character.Name,
                  Description = character.Description,
                  PhotoUrl = character.PhotoUrl,
                  FileName = character.FileName,
                  PortraitUrl = character.PortraitUrl,
                  PortraitFileName = character.PortraitFileName,

                  BaseMaxHp = character.BaseMaxHp,
                  BaseMaxMp = character.BaseMaxMp,
                  BaseMeleeAttackDamage = character.BaseMeleeAttackDamage,
                  BaseBowAttackDamage = character.BaseBowAttackDamage,
                  BaseMovement = character.BaseMovement,
                  BaseMaxConsumableInventory = character.BaseMaxConsumableInventory,
                  BaseMaxEquippableInventory = character.BaseMaxEquippableInventory,

                  DialogActiveColor =
                      character.CharacterDialogSettings.DialogActiveColor,
                  DialogInActiveColor =
                      character.CharacterDialogSettings.DialogInActiveColor
              })
              .ToDictionary(
                  character => character.SourceCharacterId,
                  character => character);

        foreach (var character in characters)
        {
            var playthroughCharacter = playthroughCharactersBySourceId[character.Id];

            if (character.BaseAlternateFormId is int alternateFormSourceId)
            {
                playthroughCharacter.BaseAlternateForm =
                    playthroughCharactersBySourceId[alternateFormSourceId];
            }

            playthroughCharacter.Spells = [.. character.CharacterSpells
                .Select(characterSpell => new PlaythroughCharacterSpell
                {
                    SourceCharacterSpellId = characterSpell.Id,
                    PlaythroughSpell =
                        playthroughSpellsBySourceId[characterSpell.SpellId]
                })];
        }

        newPlaythrough.Characters = [.. playthroughCharactersBySourceId.Values];

        // Add consumables to playthrough

        var consumables = assets.Consumables;

        var playthroughConsumablesBySourceId = consumables
            .Select(consumable => new PlaythroughConsumableItem
            {
                SourceConsumableItemId = consumable.Id,
                Name = consumable.Name,
                Description = consumable.Description,
                PhotoUrl = consumable.PhotoUrl,
                FileName = consumable.FileName,
                HpEffect = consumable.HpEffect,
                MpEffect = consumable.MpEffect
            })
            .ToDictionary(
                consumable => consumable.SourceConsumableItemId,
                consumable => consumable);

        newPlaythrough.ConsumableItems =
            [.. playthroughConsumablesBySourceId.Values];

        // Add equippables to playthrough

        var equippables = assets.Equippables;

        var playthroughEquippablesBySourceId = equippables
            .Select(equippable => new PlaythroughEquippableItem
            {
                SourceEquippableItemId = equippable.Id,
                Name = equippable.Name,
                Description = equippable.Description,
                PhotoUrl = equippable.PhotoUrl,
                FileName = equippable.FileName,
                MeleeAttackDamageModifier = equippable.MeleeAttackDamageModifier,
                BowAttackDamageModifier = equippable.BowAttackDamageModifier,
                MovementModifier = equippable.MovementModifier,
                MaxHpModifier = equippable.MaxHpModifier,
                MaxMpModifier = equippable.MaxMpModifier,
                MaxConsumableInventoryModifier =
                    equippable.MaxConsumableInventoryModifier,
                MaxEquippableInventoryModifier =
                    equippable.MaxEquippableInventoryModifier,
                MeleeDamageReduction = equippable.MeleeDamageReduction,
                BowDamageReduction = equippable.BowDamageReduction,
                SpellDamageReduction = equippable.SpellDamageReduction,
                SpellDamageModifier = equippable.SpellDamageModifier,
                AffectedSpellType = equippable.AffectedSpellTypeId is int spellTypeId
                    ? playthroughSpellTypesBySourceId[spellTypeId]
                    : null,
                AddedSpells = [.. equippable.AddedSpells
                    .Select(spell => playthroughSpellsBySourceId[spell.Id])]
            })
            .ToDictionary(
                equippable => equippable.SourceEquippableItemId,
                equippable => equippable);

        newPlaythrough.EquippableItems =
            [.. playthroughEquippablesBySourceId.Values];

        // Add JourneyCharacter to playthrough

        var journeyCharacters = sourceJourney.JourneyCharacters;

        var journeyPTCharactersBySourceId = journeyCharacters
            .Select(journeyCharacter => new JourneyPTCharacter
            {
                SourceJourneyCharacterId = journeyCharacter.Id,

                InitialMeleeAttackDamage = journeyCharacter.MeleeAttackDamage,
                InitialBowAttackDamage = journeyCharacter.BowAttackDamage,
                InitialMovement = journeyCharacter.Movement,
                InitialMaxConsumableInventory =
                    journeyCharacter.MaxConsumableInventory,
                InitialMaxEquippableInventory =
                    journeyCharacter.MaxEquippableInventory,
                InitialMaxHp = journeyCharacter.MaxHp,
                InitialMaxMp = journeyCharacter.MaxMp,
                IsInitiallyActive = journeyCharacter.IsInitiallyActive,

                MeleeAttackDamage = journeyCharacter.MeleeAttackDamage,
                BowAttackDamage = journeyCharacter.BowAttackDamage,
                Movement = journeyCharacter.Movement,
                MaxConsumableInventory = journeyCharacter.MaxConsumableInventory,
                MaxEquippableInventory = journeyCharacter.MaxEquippableInventory,
                CurrentHp = journeyCharacter.MaxHp,
                CurrentMp = journeyCharacter.MaxMp,
                MaxHp = journeyCharacter.MaxHp,
                MaxMp = journeyCharacter.MaxMp,
                IsActive = journeyCharacter.IsInitiallyActive,
                IsDown = false,
                IsInAlternateForm = false,

                PlaythroughCharacter =
                    playthroughCharactersBySourceId[journeyCharacter.CharacterId],
                AlternateForm = journeyCharacter.AlternateFormId is int alternateFormSourceId
                    ? playthroughCharactersBySourceId[alternateFormSourceId]
                    : null,
                Spells = [.. journeyCharacter.JourneyCharacterSpells
                    .Select(journeyCharacterSpell => new JourneyPTCharacterSpell
                    {
                        SourceJourneyCharacterSpellId = journeyCharacterSpell.Id,
                        PlaythroughSpell =
                            playthroughSpellsBySourceId[journeyCharacterSpell.SpellId]
                    })]
            })
            .ToDictionary(
                journeyCharacter => journeyCharacter.SourceJourneyCharacterId,
                journeyCharacter => journeyCharacter);

        newPlaythrough.JourneyCharacters =
            [.. journeyPTCharactersBySourceId.Values];

        // Add the remaining scene graph to playthrough

        foreach (var sourceScene in sourceJourney.Scenes)
        {
            var scenePT = scenePTsBySourceId[sourceScene.Id];

            if (sourceScene.Grid is not null)
            {
                scenePT.ScenePTGrid = new ScenePTGrid
                {
                    SourceSceneGridId = sourceScene.Grid.Id,
                    Rows = sourceScene.Grid.Rows,
                    Columns = sourceScene.Grid.Columns,
                    GridColor = sourceScene.Grid.GridColor,
                    BackgroundImageUrl = sourceScene.Grid.BackgroundImageUrl,
                    BackgroundFileName = sourceScene.Grid.BackgroundFileName
                };
            }

            scenePT.IntroPages = [.. sourceScene.SceneIntroPages
                .Select(introPage => new ScenePTIntroPage
                {
                    SourceIntroPageId = introPage.Id,
                    SortOrder = introPage.SortOrder,
                    Type = introPage.Type,
                    Config = introPage.Config,
                    PreviewPhotoUrl = introPage.PreviewPhotoUrl
                })];

            scenePT.SceneCharacters = [.. sourceScene.SceneCharacters
                .Select(sceneCharacter => new ScenePTCharacter
                {
                    SourceSceneCharacterId = sceneCharacter.Id,

                    InitialMeleeAttackDamage = sceneCharacter.MeleeAttackDamage,
                    InitialBowAttackDamage = sceneCharacter.BowAttackDamage,
                    InitialMovement = sceneCharacter.Movement,
                    InitialMaxConsumableInventory =
                        sceneCharacter.MaxConsumableInventory,
                    InitialMaxEquippableInventory =
                        sceneCharacter.MaxEquippableInventory,
                    InitialMaxHp = sceneCharacter.MaxHp,
                    InitialMaxMp = sceneCharacter.MaxMp,
                    IsInitiallyActive = sceneCharacter.IsInitiallyActive,

                    MeleeAttackDamage = sceneCharacter.MeleeAttackDamage,
                    BowAttackDamage = sceneCharacter.BowAttackDamage,
                    Movement = sceneCharacter.Movement,
                    MaxConsumableInventory = sceneCharacter.MaxConsumableInventory,
                    MaxEquippableInventory = sceneCharacter.MaxEquippableInventory,
                    CurrentHp = sceneCharacter.MaxHp,
                    CurrentMp = sceneCharacter.MaxMp,
                    MaxHp = sceneCharacter.MaxHp,
                    MaxMp = sceneCharacter.MaxMp,
                    IsActive = sceneCharacter.IsInitiallyActive,
                    IsDead = false,
                    IsInAlternateForm = false,

                    PlaythroughCharacter =
                        playthroughCharactersBySourceId[sceneCharacter.CharacterId],
                    AlternateForm = sceneCharacter.AlternateFormId is int alternateFormSourceId
                        ? playthroughCharactersBySourceId[alternateFormSourceId]
                        : null,
                    Spells = [.. sceneCharacter.SceneCharacterSpells
                        .Select(sceneCharacterSpell => new ScenePTCharacterSpell
                        {
                            SourceSceneCharacterSpellId = sceneCharacterSpell.Id,
                            PlaythroughSpell =
                                playthroughSpellsBySourceId[sceneCharacterSpell.SpellId]
                        })]
                })];

            scenePT.SceneChests = [.. sourceScene.SceneChests
                .Select(chest => new ScenePTChest
                {
                    SourceSceneChestId = chest.Id,
                    Name = chest.Name,
                    DieSides = chest.DieSides,
                    Status = ChestStatus.Unopened,
                    RolledValue = null,
                    OpenedAt = null,
                    SelectedLootEntry = null,
                    ChestLootEntries = [.. chest.LootEntries
                        .Select(lootEntry => new ScenePTChestLootEntry
                        {
                            SourceSceneChestLootEntryId = lootEntry.Id,
                            RollMinimum = lootEntry.RollMinimum,
                            RollMaximum = lootEntry.RollMaximum,
                            Quantity = lootEntry.Quantity,
                            PlaythroughEquippableItem =
                                lootEntry.EquippableItemId is int equippableItemId
                                    ? playthroughEquippablesBySourceId[equippableItemId]
                                    : null,
                            PlaythroughConsumableItem =
                                lootEntry.ConsumableItemId is int consumableItemId
                                    ? playthroughConsumablesBySourceId[consumableItemId]
                                    : null
                        })]
                })];

            scenePT.SceneDialogs = [.. sourceScene.SceneDialogs
                .Select(dialog => new ScenePTDialog
                {
                    SourceSceneDialogId = dialog.Id,
                    Title = dialog.Title,
                    DialogPages = [.. dialog.DialogPages
                        .Select(page => new ScenePTDialogPage
                        {
                            SourceDialogPageId = page.Id,
                            OrderNum = page.OrderNum,
                            PhotoUrl = page.PhotoUrl,
                            FileName = page.FileName,
                            DialogPageSections = [.. page.DialogPageSections
                                .Select(section => new ScenePTDialogSection
                                {
                                    SourceDialogSectionId = section.Id,
                                    OrderNum = section.OrderNum,
                                    ReadingText = section.ReadingText,
                                    IsNarrator = section.IsNarrator,
                                    CreatedAt = section.CreatedAt,
                                    UpdatedAt = section.UpdatedAt,
                                    Character = section.CharacterId is int characterId
                                        ? playthroughCharactersBySourceId[characterId]
                                        : null
                                })]
                        })]
                })];

            scenePT.SceneEvents = [.. sourceScene.SceneEvents
                .Select(sceneEvent => new ScenePTEvent
                {
                    SourceSceneEventId = sceneEvent.Id,
                    Name = sceneEvent.Name,
                    Description = sceneEvent.Description,
                    SortOrder = sceneEvent.SortOrder,
                    ExecutionStatus = SceneEventExecutionStatus.NotStarted,
                    ErrorMessage = null,
                    StartedAt = null,
                    CompletedAt = null,
                    ScenePTActionEvents = [.. sceneEvent.SceneEventActions
                        .Select(action => new ScenePTActionEvent
                        {
                            SourceSceneEventActionId = action.Id,
                            Name = action.Name,
                            SortOrder = action.SortOrder,
                            ActionTargetType = action.ActionTargetType,
                            EventActionType = action.EventActionType,
                            CharacterStatAdjustmentAction =
                                action.CharacterStatAdjustmentAction is null
                                    ? null
                                    : new PTCharacterStatAdjustmentAction
                                    {
                                        SourceCharacterStatAdjustmentActionId =
                                            action.CharacterStatAdjustmentAction.Id,
                                        CharacterStatType =
                                            action.CharacterStatAdjustmentAction.CharacterStatType,
                                        AdjustmentOperation =
                                            action.CharacterStatAdjustmentAction.AdjustmentOperation,
                                        Value = action.CharacterStatAdjustmentAction.Value,
                                        Character =
                                            action.CharacterStatAdjustmentAction.CharacterId
                                                is int adjustmentCharacterId
                                                    ? playthroughCharactersBySourceId[
                                                        adjustmentCharacterId]
                                                    : null
                                    },
                            CharacterAddSpellAction =
                                action.CharacterAddSpellAction is null
                                    ? null
                                    : new PTCharacterAddSpellAction
                                    {
                                        SourceCharacterAddSpellActionId =
                                            action.CharacterAddSpellAction.Id,
                                        PlaythroughCharacter =
                                            action.CharacterAddSpellAction.CharacterId
                                                is int addSpellCharacterId
                                                    ? playthroughCharactersBySourceId[
                                                        addSpellCharacterId]
                                                    : null,
                                        PlaythroughSpell =
                                            playthroughSpellsBySourceId[
                                                action.CharacterAddSpellAction.SpellId]
                                    }
                        })]
                })];
        }




        await playthroughRepository.AddAsync(newPlaythrough, ct);
        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result<PlaythroughSummaryDto>.Ok(newPlaythrough.ToSummaryDto());
    }

    private static HashSet<int> GetReferencedCharacterIds(Journey journey)
    {
        var characterIds = new HashSet<int>();

        static void AddIfPresent(HashSet<int> ids, int? id)
        {
            if (id.HasValue)
                ids.Add(id.Value);
        }

        foreach (var journeyCharacter in journey.JourneyCharacters)
        {
            characterIds.Add(journeyCharacter.CharacterId);
            AddIfPresent(characterIds, journeyCharacter.AlternateFormId);
        }

        foreach (var scene in journey.Scenes)
        {
            foreach (var sceneCharacter in scene.SceneCharacters)
            {
                characterIds.Add(sceneCharacter.CharacterId);
                AddIfPresent(characterIds, sceneCharacter.AlternateFormId);
            }

            foreach (var section in scene.SceneDialogs
                .SelectMany(dialog => dialog.DialogPages)
                .SelectMany(page => page.DialogPageSections))
            {
                AddIfPresent(characterIds, section.CharacterId);
            }

            foreach (var action in scene.SceneEvents
                .SelectMany(sceneEvent => sceneEvent.SceneEventActions))
            {
                AddIfPresent(
                    characterIds,
                    action.CharacterStatAdjustmentAction?.CharacterId);
                AddIfPresent(
                    characterIds,
                    action.CharacterAddSpellAction?.CharacterId);
            }
        }

        return characterIds;
    }

    private static Error? ValidateStartSource(
        Journey journey,
        PlaythroughStartAssets assets)
    {
        var characterIds = assets.Characters.Select(character => character.Id).ToHashSet();
        var consumableIds = assets.Consumables.Select(item => item.Id).ToHashSet();
        var equippableIds = assets.Equippables.Select(item => item.Id).ToHashSet();
        var spellIds = assets.Spells.Select(spell => spell.Id).ToHashSet();
        var spellTypeIds = assets.SpellTypes.Select(type => type.Id).ToHashSet();

        static Error Missing(string relationship, int sourceId)
        {
            return new Error(
                "Playthrough.InvalidSourceGraph",
                $"The playthrough source has an invalid {relationship} reference ({sourceId}).");
        }

        foreach (var spell in assets.Spells)
        {
            if (!spellTypeIds.Contains(spell.SpellTypeId))
                return Missing("spell type", spell.SpellTypeId);
        }

        foreach (var character in assets.Characters)
        {
            if (character.CharacterDialogSettings is null)
            {
                return new Error(
                    "Playthrough.InvalidSourceGraph",
                    $"Character {character.Id} does not have dialog settings.");
            }

            if (character.BaseAlternateFormId is int alternateFormId &&
                !characterIds.Contains(alternateFormId))
            {
                return Missing("base alternate form", alternateFormId);
            }

            foreach (var characterSpell in character.CharacterSpells)
            {
                if (!spellIds.Contains(characterSpell.SpellId))
                    return Missing("base character spell", characterSpell.SpellId);
            }
        }

        foreach (var equippable in assets.Equippables)
        {
            if (equippable.AffectedSpellTypeId is int affectedSpellTypeId &&
                !spellTypeIds.Contains(affectedSpellTypeId))
            {
                return Missing("affected spell type", affectedSpellTypeId);
            }

            foreach (var addedSpell in equippable.AddedSpells)
            {
                if (!spellIds.Contains(addedSpell.Id))
                    return Missing("equippable added spell", addedSpell.Id);
            }
        }

        foreach (var journeyCharacter in journey.JourneyCharacters)
        {
            if (!characterIds.Contains(journeyCharacter.CharacterId))
                return Missing("journey character", journeyCharacter.CharacterId);

            if (journeyCharacter.AlternateFormId is int alternateFormId &&
                !characterIds.Contains(alternateFormId))
            {
                return Missing("journey alternate form", alternateFormId);
            }

            foreach (var characterSpell in journeyCharacter.JourneyCharacterSpells)
            {
                if (!spellIds.Contains(characterSpell.SpellId))
                    return Missing("journey character spell", characterSpell.SpellId);
            }
        }

        foreach (var scene in journey.Scenes)
        {
            foreach (var sceneCharacter in scene.SceneCharacters)
            {
                if (!characterIds.Contains(sceneCharacter.CharacterId))
                    return Missing("scene character", sceneCharacter.CharacterId);

                if (sceneCharacter.AlternateFormId is int alternateFormId &&
                    !characterIds.Contains(alternateFormId))
                {
                    return Missing("scene alternate form", alternateFormId);
                }

                foreach (var characterSpell in sceneCharacter.SceneCharacterSpells)
                {
                    if (!spellIds.Contains(characterSpell.SpellId))
                        return Missing("scene character spell", characterSpell.SpellId);
                }
            }

            foreach (var lootEntry in scene.SceneChests
                .SelectMany(chest => chest.LootEntries))
            {
                var hasConsumable = lootEntry.ConsumableItemId.HasValue;
                var hasEquippable = lootEntry.EquippableItemId.HasValue;

                if (hasConsumable == hasEquippable)
                {
                    return new Error(
                        "Playthrough.InvalidSourceGraph",
                        $"Chest loot entry {lootEntry.Id} must reference exactly one item.");
                }

                if (lootEntry.ConsumableItemId is int consumableId &&
                    !consumableIds.Contains(consumableId))
                {
                    return Missing("chest consumable", consumableId);
                }

                if (lootEntry.EquippableItemId is int equippableId &&
                    !equippableIds.Contains(equippableId))
                {
                    return Missing("chest equippable", equippableId);
                }
            }

            foreach (var section in scene.SceneDialogs
                .SelectMany(dialog => dialog.DialogPages)
                .SelectMany(page => page.DialogPageSections))
            {
                if (section.CharacterId is int characterId &&
                    !characterIds.Contains(characterId))
                {
                    return Missing("dialog character", characterId);
                }
            }

            foreach (var action in scene.SceneEvents
                .SelectMany(sceneEvent => sceneEvent.SceneEventActions))
            {
                var adjustment = action.CharacterStatAdjustmentAction;

                if (adjustment?.CharacterId is int adjustmentCharacterId &&
                    !characterIds.Contains(adjustmentCharacterId))
                {
                    return Missing("event adjustment character", adjustmentCharacterId);
                }

                var addSpell = action.CharacterAddSpellAction;

                if (addSpell?.CharacterId is int addSpellCharacterId &&
                    !characterIds.Contains(addSpellCharacterId))
                {
                    return Missing("event add-spell character", addSpellCharacterId);
                }

                if (addSpell is not null && !spellIds.Contains(addSpell.SpellId))
                    return Missing("event added spell", addSpell.SpellId);
            }
        }

        return null;
    }

    public async Task<Result<List<PlaythroughSummaryDto>>> GetForJourneyAsync(
        int userId,
        int journeyId,
        CancellationToken ct)
    {
        var journey = await journeyRepository.GetByIdAsync(journeyId, ct);

        if (journey is null)
        {
            return Result<List<PlaythroughSummaryDto>>.Fail(
                new Error("Journey.NotFound", "Journey was not found."));
        }

        if (journey.UserId != userId)
        {
            return Result<List<PlaythroughSummaryDto>>.Fail(
                new Error("Auth.Forbidden", "You do not have permission to access this journey."));
        }

        var playthroughs = await playthroughRepository.ListForJourneyAsync(
            userId,
            journeyId,
            ct);

        return Result<List<PlaythroughSummaryDto>>.Ok(
            playthroughs.Select(playthrough => playthrough.ToSummaryDto()).ToList());
    }
}
