using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using Eldoria.Core.Snapshots;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Snapshots;

public sealed class JourneySnapshotBuilder(ApplicationDbContext dbContext) : IJourneySnapshotBuilder
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<JourneySnapshotV1?> BuildAsync(int userId, int journeyId, CancellationToken ct)
    {
        var journey = await _dbContext.Journeys
            .AsNoTracking()
            .IgnoreQueryFilters()
            .AsSplitQuery()
            .Include("IntroPages")
            .Include("JourneyCharacters.Character.CharacterDialogSettings")
            .Include("JourneyCharacters.Character.CharacterSpells.Spell.SpellType")
            .Include("JourneyCharacters.Character.BaseAlternateForm.CharacterDialogSettings")
            .Include("JourneyCharacters.Character.BaseAlternateForm.CharacterSpells.Spell.SpellType")
            .Include("JourneyCharacters.AlternateForm.CharacterDialogSettings")
            .Include("JourneyCharacters.AlternateForm.CharacterSpells.Spell.SpellType")
            .Include("JourneyCharacters.JourneyCharacterSpells.Spell.SpellType")
            .Include("Scenes.Grid")
            .Include("Scenes.SceneIntroPages")
            .Include("Scenes.SceneCharacters.Character.CharacterDialogSettings")
            .Include("Scenes.SceneCharacters.Character.CharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneCharacters.Character.BaseAlternateForm.CharacterDialogSettings")
            .Include("Scenes.SceneCharacters.Character.BaseAlternateForm.CharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneCharacters.AlternateForm.CharacterDialogSettings")
            .Include("Scenes.SceneCharacters.AlternateForm.CharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneCharacters.SceneCharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneDialogs.DialogPages.DialogPageSections.Character.CharacterDialogSettings")
            .Include("Scenes.SceneDialogs.DialogPages.DialogPageSections.Character.CharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneDialogs.DialogPages.DialogPageSections.Character.BaseAlternateForm.CharacterDialogSettings")
            .Include("Scenes.SceneDialogs.DialogPages.DialogPageSections.Character.BaseAlternateForm.CharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneEvents.SceneEventActions.CharacterStatAdjustmentAction.Character.CharacterDialogSettings")
            .Include("Scenes.SceneEvents.SceneEventActions.CharacterStatAdjustmentAction.Character.CharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneEvents.SceneEventActions.CharacterStatAdjustmentAction.Character.BaseAlternateForm.CharacterDialogSettings")
            .Include("Scenes.SceneEvents.SceneEventActions.CharacterStatAdjustmentAction.Character.BaseAlternateForm.CharacterSpells.Spell.SpellType")
            .Include("Scenes.SceneChests.LootEntries.ConsumableItem")
            .Include("Scenes.SceneChests.LootEntries.EquippableItem.AffectedSpellType")
            .Include("Scenes.SceneChests.LootEntries.EquippableItem.AddedSpells.SpellType")
            .SingleOrDefaultAsync(j => j.Id == journeyId && j.UserId == userId, ct);

        if (journey is null)
            return null;

        Validate(journey, userId);

        var characters = CollectCharacters(journey);
        var equipment = journey.Scenes
            .SelectMany(scene => scene.SceneChests)
            .SelectMany(chest => chest.LootEntries)
            .Where(entry => entry.EquippableItem is not null)
            .Select(entry => entry.EquippableItem!)
            .DistinctBy(item => item.Id)
            .OrderBy(item => item.Id)
            .ToList();
        var consumables = journey.Scenes
            .SelectMany(scene => scene.SceneChests)
            .SelectMany(chest => chest.LootEntries)
            .Where(entry => entry.ConsumableItem is not null)
            .Select(entry => entry.ConsumableItem!)
            .DistinctBy(item => item.Id)
            .OrderBy(item => item.Id)
            .ToList();

        var spells = characters.SelectMany(character => character.CharacterSpells.Select(cs => cs.Spell))
            .Concat(journey.JourneyCharacters.SelectMany(character => character.JourneyCharacterSpells.Select(cs => cs.Spell)))
            .Concat(journey.Scenes.SelectMany(scene => scene.SceneCharacters)
                .SelectMany(character => character.SceneCharacterSpells.Select(cs => cs.Spell)))
            .Concat(equipment.SelectMany(item => item.AddedSpells))
            .DistinctBy(spell => spell.Id)
            .OrderBy(spell => spell.Id)
            .ToList();

        var spellTypes = spells.Select(spell => spell.SpellType)
            .Concat(equipment.Where(item => item.AffectedSpellType is not null).Select(item => item.AffectedSpellType!))
            .DistinctBy(type => type.Id)
            .OrderBy(type => type.Id)
            .ToList();

        return new JourneySnapshotV1
        {
            Journey = new JourneyDefinitionSnapshot
            {
                SourceJourneyId = journey.Id,
                Name = journey.Name,
                Description = journey.Description,
                PhotoUrl = journey.PhotoUrl,
                FileName = journey.FileName,
                SortOrder = journey.SortOrder,
                IntroPages = journey.IntroPages.OrderBy(page => page.SortOrder).ThenBy(page => page.Id)
                    .Select(page => MapIntroPage(page, "journey-intro")).ToList(),
                Characters = journey.JourneyCharacters.OrderBy(character => character.Id)
                    .Select(MapJourneyCharacter).ToList(),
                SceneKeys = journey.Scenes.OrderBy(scene => scene.SortOrder).ThenBy(scene => scene.Id)
                    .Select(scene => SceneKey(scene.Id)).ToList()
            },
            Characters = characters.OrderBy(character => character.Id).Select(MapCharacter).ToList(),
            SpellTypes = spellTypes.Select(MapSpellType).ToList(),
            Spells = spells.Select(MapSpell).ToList(),
            Consumables = consumables.Select(MapConsumable).ToList(),
            Equipment = equipment.Select(MapEquipment).ToList(),
            Scenes = journey.Scenes.OrderBy(scene => scene.SortOrder).ThenBy(scene => scene.Id)
                .Select(MapScene).ToList()
        };
    }

    private static List<Character> CollectCharacters(Journey journey)
    {
        var characters = journey.JourneyCharacters.Select(character => character.Character)
            .Concat(journey.JourneyCharacters.Where(character => character.AlternateForm is not null)
                .Select(character => character.AlternateForm!))
            .Concat(journey.Scenes.SelectMany(scene => scene.SceneCharacters).Select(character => character.Character))
            .Concat(journey.Scenes.SelectMany(scene => scene.SceneCharacters)
                .Where(character => character.AlternateForm is not null).Select(character => character.AlternateForm!))
            .Concat(journey.Scenes.SelectMany(scene => scene.SceneDialogs)
                .SelectMany(dialog => dialog.DialogPages)
                .SelectMany(page => page.DialogPageSections)
                .Where(section => section.Character is not null).Select(section => section.Character!))
            .Concat(journey.Scenes.SelectMany(scene => scene.SceneEvents)
                .SelectMany(sceneEvent => sceneEvent.SceneEventActions)
                .Where(action => action.CharacterStatAdjustmentAction?.Character is not null)
                .Select(action => action.CharacterStatAdjustmentAction!.Character!))
            .ToList();

        characters.AddRange(characters.Where(character => character.BaseAlternateForm is not null)
            .Select(character => character.BaseAlternateForm!).ToList());

        return characters.DistinctBy(character => character.Id).ToList();
    }

    private static void Validate(Journey journey, int userId)
    {
        var errors = new List<string>();
        if (journey.Scenes.GroupBy(scene => scene.SortOrder).Any(group => group.Count() > 1))
            errors.Add("Scene sort order must be unique.");

        foreach (var chest in journey.Scenes.SelectMany(scene => scene.SceneChests))
        {
            var entries = chest.LootEntries.OrderBy(entry => entry.RollMinimum).ToList();
            if (chest.DieSides < 1)
                errors.Add($"Chest {chest.Id} must have at least one die side.");
            if (entries.Any(entry => entry.RollMinimum < 1 || entry.RollMaximum > chest.DieSides))
                errors.Add($"Chest {chest.Id} has a loot range outside its die.");
            if (entries.Zip(entries.Skip(1)).Any(pair => pair.First.RollMaximum >= pair.Second.RollMinimum))
                errors.Add($"Chest {chest.Id} has overlapping loot ranges.");
            if (entries.Any(entry => (entry.ConsumableItem is null) == (entry.EquippableItem is null)))
                errors.Add($"Chest {chest.Id} has a loot entry without exactly one item.");
        }

        foreach (var action in journey.Scenes.SelectMany(scene => scene.SceneEvents)
                     .SelectMany(sceneEvent => sceneEvent.SceneEventActions))
        {
            if (action.EventActionType == EventActionType.CharacterStatAdjustment &&
                action.CharacterStatAdjustmentAction is null)
                errors.Add($"Event action {action.Id} is missing its stat adjustment definition.");
            if (action.ActionTargetType == ActionTargetType.SingleJourneyCharacter &&
                action.CharacterStatAdjustmentAction?.Character is null)
                errors.Add($"Event action {action.Id} is missing its target character.");
        }

        var referencedCharacters = CollectCharacters(journey);
        var referencedCharacterIds = referencedCharacters.Select(character => character.Id).ToHashSet();
        if (referencedCharacters.Any(character => character.BaseAlternateFormId is int baseAlternateId &&
                                                  !referencedCharacterIds.Contains(baseAlternateId)) ||
            journey.JourneyCharacters.Any(character => character.AlternateFormId is int journeyAlternateId &&
                                                         !referencedCharacterIds.Contains(journeyAlternateId)) ||
            journey.Scenes.SelectMany(scene => scene.SceneCharacters)
                .Any(character => character.AlternateFormId is int sceneAlternateId &&
                                  !referencedCharacterIds.Contains(sceneAlternateId)))
            errors.Add("Every alternate form must resolve to a character inside the snapshot.");

        var lootEntries = journey.Scenes.SelectMany(scene => scene.SceneChests)
            .SelectMany(chest => chest.LootEntries).ToList();
        var referencedEquipment = lootEntries.Where(entry => entry.EquippableItem is not null)
            .Select(entry => entry.EquippableItem!).DistinctBy(item => item.Id).ToList();
        var referencedSpells = referencedCharacters.SelectMany(character => character.CharacterSpells.Select(spell => spell.Spell))
            .Concat(journey.JourneyCharacters.SelectMany(character => character.JourneyCharacterSpells.Select(spell => spell.Spell)))
            .Concat(journey.Scenes.SelectMany(scene => scene.SceneCharacters)
                .SelectMany(character => character.SceneCharacterSpells.Select(spell => spell.Spell)))
            .Concat(referencedEquipment.SelectMany(item => item.AddedSpells))
            .DistinctBy(spell => spell.Id).ToList();
        var foreignCatalog = referencedCharacters.Any(character => character.UserId != userId)
            || referencedSpells.Any(spell => spell.UserId != userId || spell.SpellType.UserId != userId)
            || lootEntries.Any(entry => entry.ConsumableItem is not null && entry.ConsumableItem.UserId != userId ||
                                        entry.EquippableItem is not null && entry.EquippableItem.UserId != userId)
            || referencedEquipment.Any(item => item.AffectedSpellType is not null && item.AffectedSpellType.UserId != userId);
        if (foreignCatalog)
            errors.Add("All referenced catalog records must belong to the journey owner.");

        if (errors.Count > 0)
            throw new InvalidOperationException(string.Join(" ", errors));
    }

    private static CharacterDefinitionSnapshot MapCharacter(Character character) => new()
    {
        Key = CharacterKey(character.Id),
        SourceCharacterId = character.Id,
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
        CharacterType = character.CharacterType,
        BaseAlternateFormCharacterKey = character.BaseAlternateFormId is int alternateId ? CharacterKey(alternateId) : null,
        DialogSettings = character.CharacterDialogSettings is null ? null : new CharacterDialogDefinitionSnapshot
        {
            ActiveColor = character.CharacterDialogSettings.DialogActiveColor,
            InactiveColor = character.CharacterDialogSettings.DialogInActiveColor
        },
        SpellKeys = character.CharacterSpells.OrderBy(spell => spell.SpellId)
            .Select(spell => SpellKey(spell.SpellId)).ToList()
    };

    private static JourneyCharacterDefinitionSnapshot MapJourneyCharacter(JourneyCharacter character) => new()
    {
        Key = $"journey-character:{character.Id}",
        SourceJourneyCharacterId = character.Id,
        CharacterKey = CharacterKey(character.CharacterId),
        MeleeAttackDamage = character.MeleeAttackDamage,
        BowAttackDamage = character.BowAttackDamage,
        Movement = character.Movement,
        MaxConsumableInventory = character.MaxConsumableInventory,
        MaxEquippableInventory = character.MaxEquippableInventory,
        MaxHp = character.MaxHp,
        MaxMp = character.MaxMp,
        IsInitiallyActive = character.IsInitiallyActive,
        AlternateFormCharacterKey = character.AlternateFormId is int alternateId ? CharacterKey(alternateId) : null,
        Spells = character.JourneyCharacterSpells.OrderBy(spell => spell.Id).Select(spell => new AssignedSpellDefinitionSnapshot
        {
            Key = $"journey-character-spell:{spell.Id}",
            SourceAssignmentId = spell.Id,
            SpellKey = SpellKey(spell.SpellId)
        }).ToList()
    };

    private static SceneDefinitionSnapshot MapScene(Scene scene) => new()
    {
        Key = SceneKey(scene.Id),
        SourceSceneId = scene.Id,
        Name = scene.Name,
        Description = scene.Description,
        PhotoUrl = scene.PhotoUrl,
        FileName = scene.FileName,
        GridUrl = scene.GridUrl,
        SortOrder = scene.SortOrder,
        Grid = scene.Grid is null ? null : new SceneGridDefinitionSnapshot
        {
            Rows = scene.Grid.Rows,
            Columns = scene.Grid.Columns,
            GridColor = scene.Grid.GridColor,
            BackgroundImageUrl = scene.Grid.BackgroundImageUrl,
            BackgroundFileName = scene.Grid.BackgroundFileName
        },
        IntroPages = scene.SceneIntroPages.OrderBy(page => page.SortOrder).ThenBy(page => page.Id)
            .Select(page => MapIntroPage(page, "scene-intro")).ToList(),
        Characters = scene.SceneCharacters.OrderBy(character => character.Id).Select(MapSceneCharacter).ToList(),
        Dialogs = scene.SceneDialogs.OrderBy(dialog => dialog.Id).Select(MapDialog).ToList(),
        Events = scene.SceneEvents.OrderBy(sceneEvent => sceneEvent.SortOrder).ThenBy(sceneEvent => sceneEvent.Id)
            .Select(MapEvent).ToList(),
        Chests = scene.SceneChests.OrderBy(chest => chest.Id).Select(MapChest).ToList()
    };

    private static SceneCharacterDefinitionSnapshot MapSceneCharacter(SceneCharacter character) => new()
    {
        Key = $"scene-character:{character.Id}",
        SourceSceneCharacterId = character.Id,
        CharacterKey = CharacterKey(character.CharacterId),
        MeleeAttackDamage = character.MeleeAttackDamage,
        BowAttackDamage = character.BowAttackDamage,
        Movement = character.Movement,
        MaxConsumableInventory = character.MaxConsumableInventory,
        MaxEquippableInventory = character.MaxEquippableInventory,
        MaxHp = character.MaxHp,
        MaxMp = character.MaxMp,
        IsInitiallyActive = character.IsInitiallyActive,
        AlternateFormCharacterKey = character.AlternateFormId is int alternateId ? CharacterKey(alternateId) : null,
        Spells = character.SceneCharacterSpells.OrderBy(spell => spell.Id).Select(spell => new AssignedSpellDefinitionSnapshot
        {
            Key = $"scene-character-spell:{spell.Id}",
            SourceAssignmentId = spell.Id,
            SpellKey = SpellKey(spell.SpellId)
        }).ToList()
    };

    private static IntroPageDefinitionSnapshot MapIntroPage(JourneyIntroPage page, string prefix) => new()
    {
        Key = $"{prefix}:{page.Id}", SourceId = page.Id, SortOrder = page.SortOrder,
        Type = page.Type, Config = page.Config, PreviewPhotoUrl = page.PreviewPhotoUrl
    };

    private static IntroPageDefinitionSnapshot MapIntroPage(SceneIntroPage page, string prefix) => new()
    {
        Key = $"{prefix}:{page.Id}", SourceId = page.Id, SortOrder = page.SortOrder,
        Type = page.Type, Config = page.Config, PreviewPhotoUrl = page.PreviewPhotoUrl
    };

    private static DialogDefinitionSnapshot MapDialog(SceneDialog dialog) => new()
    {
        Key = $"dialog:{dialog.Id}", SourceDialogId = dialog.Id, Title = dialog.Title,
        Pages = dialog.DialogPages.OrderBy(page => page.OrderNum).ThenBy(page => page.Id).Select(page => new DialogPageDefinitionSnapshot
        {
            Key = $"dialog-page:{page.Id}", SourcePageId = page.Id, OrderNumber = page.OrderNum,
            PhotoUrl = page.PhotoUrl, FileName = page.FileName,
            Sections = page.DialogPageSections.OrderBy(section => section.OrderNum).ThenBy(section => section.Id)
                .Select(section => new DialogSectionDefinitionSnapshot
                {
                    Key = $"dialog-section:{section.Id}", SourceSectionId = section.Id,
                    OrderNumber = section.OrderNum, ReadingText = section.ReadingText,
                    IsNarrator = section.IsNarrator,
                    CharacterKey = section.CharacterId is int characterId ? CharacterKey(characterId) : null
                }).ToList()
        }).ToList()
    };

    private static EventDefinitionSnapshot MapEvent(SceneEvent sceneEvent) => new()
    {
        Key = $"event:{sceneEvent.Id}", SourceEventId = sceneEvent.Id, Name = sceneEvent.Name,
        Description = sceneEvent.Description, SortOrder = sceneEvent.SortOrder,
        Actions = sceneEvent.SceneEventActions.OrderBy(action => action.SortOrder).ThenBy(action => action.Id)
            .Select(action => new EventActionDefinitionSnapshot
            {
                Key = $"event-action:{action.Id}", SourceActionId = action.Id, Name = action.Name,
                SortOrder = action.SortOrder, TargetType = action.ActionTargetType, ActionType = action.EventActionType,
                CharacterStatAdjustment = action.CharacterStatAdjustmentAction is null ? null : new CharacterStatAdjustmentDefinitionSnapshot
                {
                    StatType = action.CharacterStatAdjustmentAction.CharacterStatType,
                    Operation = action.CharacterStatAdjustmentAction.AdjustmentOperation,
                    Value = action.CharacterStatAdjustmentAction.Value,
                    CharacterKey = action.CharacterStatAdjustmentAction.CharacterId is int characterId ? CharacterKey(characterId) : null
                }
            }).ToList()
    };

    private static ChestDefinitionSnapshot MapChest(SceneChest chest) => new()
    {
        Key = $"chest:{chest.Id}", SourceChestId = chest.Id, Name = chest.Name, DieSides = chest.DieSides,
        LootEntries = chest.LootEntries.OrderBy(entry => entry.RollMinimum).ThenBy(entry => entry.Id)
            .Select(entry => new LootEntryDefinitionSnapshot
            {
                Key = $"loot:{entry.Id}", SourceLootEntryId = entry.Id,
                RollMinimum = entry.RollMinimum, RollMaximum = entry.RollMaximum, Quantity = entry.Quantity,
                EquipmentKey = entry.EquippableItemId is int equipmentId ? EquipmentKey(equipmentId) : null,
                ConsumableKey = entry.ConsumableItemId is int consumableId ? ConsumableKey(consumableId) : null
            }).ToList()
    };

    private static SpellTypeDefinitionSnapshot MapSpellType(SpellType type) => new()
    {
        Key = SpellTypeKey(type.Id), SourceSpellTypeId = type.Id, Name = type.TypeName,
        Description = type.Description, PhotoUrl = type.PhotoUrl, FileName = type.FileName
    };

    private static SpellDefinitionSnapshot MapSpell(Spell spell) => new()
    {
        Key = SpellKey(spell.Id), SourceSpellId = spell.Id, Name = spell.Name, Description = spell.Description,
        PhotoUrl = spell.PhotoUrl, FileName = spell.FileName, Range = spell.Range, IsRadius = spell.IsRadius,
        MpCost = spell.MpCost, DamageEffect = spell.DamageEffect, HealthEffect = spell.HealthEffect,
        MagicEffect = spell.MagicEffect, SpellTypeKey = SpellTypeKey(spell.SpellTypeId)
    };

    private static ConsumableDefinitionSnapshot MapConsumable(ConsumableItem item) => new()
    {
        Key = ConsumableKey(item.Id), SourceConsumableId = item.Id, Name = item.Name,
        Description = item.Description, PhotoUrl = item.PhotoUrl, FileName = item.FileName,
        HpEffect = item.HpEffect, MpEffect = item.MpEffect
    };

    private static EquipmentDefinitionSnapshot MapEquipment(EquippableItem item) => new()
    {
        Key = EquipmentKey(item.Id), SourceEquipmentId = item.Id, Name = item.Name, Description = item.Description,
        PhotoUrl = item.PhotoUrl, FileName = item.FileName,
        MeleeAttackDamageModifier = item.MeleeAttackDamageModifier,
        BowAttackDamageModifier = item.BowAttackDamageModifier, MovementModifier = item.MovementModifier,
        MaxHpModifier = item.MaxHpModifier, MaxMpModifier = item.MaxMpModifier,
        MaxConsumableInventoryModifier = item.MaxConsumableInventoryModifier,
        MaxEquippableInventoryModifier = item.MaxEquippableInventoryModifier,
        MeleeDamageReduction = item.MeleeDamageReduction, BowDamageReduction = item.BowDamageReduction,
        SpellDamageReduction = item.SpellDamageReduction,
        AffectedSpellTypeKey = item.AffectedSpellTypeId is int typeId ? SpellTypeKey(typeId) : null,
        SpellDamageModifier = item.SpellDamageModifier,
        AddedSpellKeys = item.AddedSpells.OrderBy(spell => spell.Id).Select(spell => SpellKey(spell.Id)).ToList()
    };

    private static string CharacterKey(int id) => $"character:{id}";
    private static string SceneKey(int id) => $"scene:{id}";
    private static string SpellKey(int id) => $"spell:{id}";
    private static string SpellTypeKey(int id) => $"spell-type:{id}";
    private static string ConsumableKey(int id) => $"consumable:{id}";
    private static string EquipmentKey(int id) => $"equipment:{id}";
}
