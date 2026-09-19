using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SceneEventActionMappings
{
    public static SceneEventActionDto ToDto(this SceneEventAction action) => new()
    {
        Id = action.Id,
        Name = action.Name,
        SortOrder = action.SortOrder,
        ActionTargetType = action.ActionTargetType,
        EventActionType = action.EventActionType,
        SceneEventId = action.SceneEventId,
        CharacterStatAdjustmentAction = action.CharacterStatAdjustmentAction?.ToDto(),
        CharacterAddSpellAction = action.CharacterAddSpellAction is { } spell
            ? new CharacterAddSpellActionDto
            {
                CharacterId = spell.CharacterId,
                CharacterName = spell.Character?.Name,
                SpellId = spell.SpellId,
                SpellName = spell.Spell.Name
            } : null,
        CharacterGiveItemAction = action.CharacterGiveItemAction is { } item
            ? new CharacterGiveItemActionDto
            {
                CharacterId = item.CharacterId,
                CharacterName = item.Character?.Name,
                ConsumableItemId = item.ConsumableItemId,
                EquippableItemId = item.EquippableItemId,
                ItemName = item.ConsumableItem?.Name ?? item.EquippableItem?.Name ?? "Unavailable item",
                Quantity = item.Quantity
            } : null,
        CharacterChangeAlternateFormAction = action.CharacterChangeAlternateFormAction is { } change
            ? new CharacterChangeAlternateFormActionDto
            {
                CharacterId = change.CharacterId,
                CharacterName = change.Character?.Name,
                AlternateFormId = change.AlternateFormId,
                AlternateFormName = change.AlternateForm?.Name
            }
            : null,
        CharacterInAlternateFormAction = action.CharacterInAlternateFormAction is { } inForm
            ? new CharacterInAlternateFormActionDto
            {
                CharacterId = inForm.CharacterId,
                CharacterName = inForm.Character?.Name,
            }
            : null,
    };
}
