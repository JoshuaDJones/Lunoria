using System.ComponentModel.DataAnnotations;
using Eldoria.Application.Dtos;

namespace Eldoria.Api.Requests;

public sealed record SyncCharacterFromBaseRequest(
    [property: Required, StringLength(64, MinimumLength = 64)] string ExpectedReviewToken,
    bool Stats = false,
    bool SpellAssignments = false,
    bool AlternateForm = false,
    bool SharedSpellUpdates = false)
{
    public CharacterSyncSelectionDto ToSelection() 
        => new(ExpectedReviewToken, Stats, SpellAssignments, AlternateForm, SharedSpellUpdates);
}
