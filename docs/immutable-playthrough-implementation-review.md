# Immutable Playthrough Implementation Review Guide

## Purpose

This document explains the immutable playthrough implementation added on 2026-08-10. It is intended as a practical review guide for checking the change without reading every modified file in an arbitrary order.

The central guarantee for newly started playthroughs is:

> After a playthrough starts, edits or deletions to journey-authoring and catalog records cannot change the playable definition captured by that playthrough.

This includes journeys, scenes, grids, journey characters, scene characters, base characters, alternate forms, spells and spell types, consumables, equipment, intro pages, dialogs, events, chests, loot entries, and referenced images.

Runtime state remains mutable. HP, MP, down/dead state, alternate-form state, inventory state, equipped state, participants, turn ordering, scene status, event execution, chest results, and logs can still change during play.

## High-level architecture

The implementation separates editable source data, immutable definition data, and mutable runtime state:

```text
Editable authoring and catalog records
Journey / Scene / Character / Spell / Item / Dialog / Event / Chest
                         |
                         | Start playthrough
                         v
JourneyRevision
- Versioned immutable JSON snapshot
- Deterministic SHA-256 content hash
- Source IDs retained only for traceability
- Reused when the same journey content starts again
                         |
                         v
JourneyPlaythrough and ScenePlaythrough runtime rows
- Current HP/MP
- Status and timestamps
- Participants and turn state
- Inventory and equipment state
- Event execution state
- Chest results
- Snapshot keys identify immutable definitions
```

The JSON snapshot is the definition source for a playthrough. Runtime DTOs and repository queries no longer load mutable authoring navigation properties to determine names, stats, spell behavior, item effects, scene order, events, or chest contents.

## Recommended review order

Review these files in this order:

1. `Eldoria.Core/Snapshots/JourneySnapshotV1.cs`
2. `Eldoria.Core/Entities/JourneyRevision.cs`
3. `Eldoria.Infrastructure/Db/Snapshots/JourneySnapshotBuilder.cs`
4. `Eldoria.Application/Services/JourneyPlaythroughService.cs`
5. `Eldoria.Infrastructure/Db/Repositories/JourneyPlaythroughRepository.cs`
6. The playthrough entity and configuration changes
7. The playthrough DTO and mapping changes
8. `Eldoria.Infrastructure/Migrations/20260810000000_AddImmutablePlaythroughSnapshots.cs`
9. The asset-retention changes
10. The frontend playthrough types and Play Hub revision display

Those files contain the important decisions. Most other modified files mechanically expose snapshot keys, make source IDs optional, or remove live navigation mapping.

## 1. Snapshot contract

### File

`Eldoria.Core/Snapshots/JourneySnapshotV1.cs`

`JourneySnapshotV1` is the typed contract serialized into `JourneyRevision.SnapshotJson`. It has `SchemaVersion = 1` and contains:

- Journey metadata, image information, intro pages, roster, and ordered scene keys.
- Every referenced character definition, including base stats, images, type, dialog colors, base alternate form, and spells.
- Journey-character overrides and assigned spells.
- Scene metadata, external grid URL, internal grid settings/background, and sort order.
- Scene-character overrides, activation state, alternate form, and spells.
- Complete dialog/page/section content with ordering and character attribution.
- Complete event/action/stat-adjustment definitions.
- Chest die definitions and ordered loot ranges.
- Referenced spell types, spells, consumables, and equipment definitions.
- Equipment modifiers, reductions, affected spell type, and granted spells.

Snapshot-local keys are deterministic strings such as:

```text
character:12
journey-character:30
scene:8
scene-character:42
spell:5
spell-type:2
equipment:9
consumable:6
event:14
chest:3
loot:18
```

Source database IDs are preserved in the snapshot for diagnostics only. Playthrough behavior must resolve through the snapshot key rather than load the source row.

## 2. Journey revision entity

### Files

- `Eldoria.Core/Entities/JourneyRevision.cs`
- `Eldoria.Infrastructure/Db/Configurations/JourneyRevisionConfig.cs`
- `Eldoria.Infrastructure/Db/ApplicationDbContext.cs`
- `Eldoria.Core/Entities/Journey.cs`
- `Eldoria.Core/Entities/User.cs`

`JourneyRevision` stores:

| Field | Purpose |
| --- | --- |
| `Id` | Revision database identity |
| `SourceJourneyId` | Optional FK to the current source journey for traceability |
| `RevisionNumber` | Per-user, per-journey revision number |
| `SchemaVersion` | Snapshot JSON schema version |
| `ContentHash` | SHA-256 hash used to identify identical content |
| `SnapshotJson` | Complete immutable playable definition |
| `CreatedAt` | Revision creation time |
| `CreatedByUserId` | Permanent ownership boundary for reads |

Important relationship rules:

- A playthrough requires a revision.
- A revision referenced by a playthrough uses restricted deletion.
- Deleting a source journey sets `JourneyRevision.SourceJourneyId` to null.
- Revision ownership does not depend on the continued existence of the journey.
- Snapshot-defining properties use init-only setters where practical, and no revision update service exists.
- Content-hash and revision-number indexes are unique while `SourceJourneyId` exists.

## 3. Building and validating the snapshot

### Files

- `Eldoria.Core/Interfaces/IJourneySnapshotBuilder.cs`
- `Eldoria.Infrastructure/Db/Snapshots/JourneySnapshotBuilder.cs`
- `Eldoria.Infrastructure/DependencyInjection.cs`

`JourneySnapshotBuilder.BuildAsync` loads the complete owned journey using `AsNoTracking`, `IgnoreQueryFilters`, `AsSplitQuery`, and explicit includes.

`IgnoreQueryFilters` is intentional. A soft-deleted character that is still referenced by the playable journey must not silently disappear while the snapshot is being assembled.

The builder loads:

- Journey intro pages and journey characters.
- Character definitions, dialog settings, alternate forms, and spells.
- Scenes, grids, intro pages, scene characters, dialogs, events, and chests.
- Spell types for all included spells.
- Loot consumables and equipment.
- Equipment-granted spells and affected spell types.

Before returning the snapshot, it validates:

- Scene sort orders are unique.
- Chest die sizes and roll ranges are valid.
- Chest ranges do not overlap.
- Each loot entry contains exactly one item type.
- Event actions have their required stat-adjustment definition.
- Single-character event actions have a target.
- Alternate forms resolve inside the snapshot.
- Referenced characters, spells, spell types, consumables, and equipment belong to the journey owner.

Invalid content prevents the playthrough from starting and returns `JourneyPlaythrough.InvalidSnapshot`.

## 4. Starting a playthrough

### File

`Eldoria.Application/Services/JourneyPlaythroughService.cs`

The previous implementation created only this root row:

```text
JourneyPlaythrough
- JourneyId
- StartedAt
- IsActive
```

The new `StartAsync` flow is:

1. Check for an existing active playthrough.
2. Build and validate `JourneySnapshotV1`.
3. Serialize the snapshot using stable camel-case JSON settings.
4. Calculate the SHA-256 content hash.
5. Create a candidate `JourneyRevision`.
6. Create the complete runtime graph from snapshot values.
7. Ask the repository to atomically persist or reuse the revision and persist the playthrough.
8. Return a snapshot-backed playthrough DTO.

### Runtime state seeded immediately

The service now creates:

- Every `JourneyPlaythroughCharacter` with copied starting stats and current HP/MP.
- Journey-character spell rows with snapshot spell keys.
- Separate runtime alternate-form rows derived from immutable alternate character definitions.
- Every `ScenePlaythrough` in snapshot sort order.
- Every `ScenePlaythroughCharacter` with copied starting stats.
- Scene-character spell rows.
- Initial active participants for journey and scene characters.
- Every scene event runtime row.
- Every scene chest runtime row.

All scenes are seeded immediately instead of being created lazily. A playthrough therefore cannot start successfully with only a partially created runtime graph.

### Assignment key versus character key

Runtime character rows contain two keys:

- `SnapshotAssignmentKey` identifies the particular journey-character or scene-character assignment. This remains unique even when the same catalog character is added to a scene more than once.
- `SnapshotCharacterKey` identifies the immutable character definition used for names, images, dialog settings, and base properties.

Alternate-form rows receive their own assignment key while pointing at the alternate character definition key.

## 5. Atomic creation and revision reuse

### Files

- `Eldoria.Core/Interfaces/IJourneyPlaythroughRepository.cs`
- `Eldoria.Infrastructure/Db/Repositories/JourneyPlaythroughRepository.cs`
- `Eldoria.Core/Exceptions/ActivePlaythroughExistsException.cs`

`JourneyPlaythroughRepository.StartAsync` uses a serializable database transaction.

Inside the transaction it:

1. Rechecks that no active playthrough exists.
2. Searches for a revision with the same user, source journey, and content hash.
3. Reuses the revision if the playable content is identical.
4. Otherwise allocates the next revision number and inserts a new revision.
5. Inserts the root playthrough and its complete runtime graph.
6. Saves and commits once.

The filtered unique index on active playthroughs remains the final concurrency guard. SQL unique-key failures are translated to `JourneyPlaythrough.ActiveExists`.

If any insert or validation fails, the transaction does not leave behind a partial revision or runtime graph.

## 6. Runtime entities no longer depend on mutable definitions

### Main entity files

- `Eldoria.Core/Entities/JourneyPlaythrough.cs`
- `Eldoria.Core/Entities/JourneyPlaythroughCharacter.cs`
- `Eldoria.Core/Entities/JourneyPlaythroughCharacterSpell.cs`
- `Eldoria.Core/Entities/JourneyPlaythroughCharacterConsumableItem.cs`
- `Eldoria.Core/Entities/JourneyPlaythroughCharacterEquippableItem.cs`
- `Eldoria.Core/Entities/ScenePlaythrough.cs`
- `Eldoria.Core/Entities/ScenePlaythroughCharacter.cs`
- `Eldoria.Core/Entities/ScenePlaythroughCharacterSpell.cs`
- `Eldoria.Core/Entities/ScenePlaythroughCharacterConsumableItem.cs`
- `Eldoria.Core/Entities/ScenePlaythroughCharacterEquippableItem.cs`
- `Eldoria.Core/Entities/ScenePlaythroughEvent.cs`
- `Eldoria.Core/Entities/ScenePlaythroughChest.cs`

The source foreign keys remain available as nullable audit links, but snapshot keys are now the runtime definition identity.

| Runtime row | Immutable definition key |
| --- | --- |
| Journey playthrough character | `SnapshotAssignmentKey` and `SnapshotCharacterKey` |
| Journey character spell | `SnapshotSpellKey` |
| Journey consumable | `SnapshotConsumableKey` |
| Journey equipment | `SnapshotEquipmentKey` |
| Scene playthrough | `SnapshotSceneKey` |
| Scene playthrough character | `SnapshotAssignmentKey` and `SnapshotCharacterKey` |
| Scene character spell | `SnapshotSpellKey` |
| Scene consumable | `SnapshotConsumableKey` |
| Scene equipment | `SnapshotEquipmentKey` |
| Scene event | `SnapshotEventKey` |
| Scene chest | `SnapshotChestKey` |
| Selected chest loot | `SelectedLootEntrySnapshotKey` |

Equipment runtime rows also gained `IsEquipped`, making equipped state explicitly mutable and independent from the immutable equipment definition.

### Preserved source identity after deletion

`JourneyPlaythrough.SourceJourneyId` and `ScenePlaythrough.SourceSceneId` are plain scalar values, not foreign keys. They preserve route and diagnostic identity after a source journey or scene is deleted.

The nullable `JourneyId` and `SceneId` properties remain only as optional current-source links.

## 7. Delete behavior

### Configuration files

The relevant files are under:

`Eldoria.Infrastructure/Db/Configurations/`

All runtime-to-source relationships changed to nullable `SetNull` relationships, including:

- Playthrough to journey.
- Scene playthrough to scene.
- Runtime character to journey/scene character.
- Runtime spell to journey/scene spell assignment.
- Runtime inventory to consumable/equipment source.
- Runtime event to scene event.
- Runtime chest to scene chest and selected source loot entry.

Deleting source content therefore clears audit links instead of deleting runtime history or preventing source deletion because of playthrough rows.

Playthrough runtime rows still cascade from their owning playthrough. Explicitly deleting a playthrough removes its mutable runtime graph, which is the intended ownership boundary.

## 8. Snapshot-backed DTOs and repositories

### DTO and mapping directories

- `Eldoria.Application/Dtos/`
- `Eldoria.Application/Common/`

The playthrough DTOs were changed so they no longer embed mutable authoring DTOs such as:

- `JourneyCharacterDto`
- `SceneCharacterDto`
- `SceneDto`
- `SceneEventDto`
- `SceneChestDto`
- `ConsumableItemDto`
- `EquippableItemDto`

They now expose runtime IDs, mutable runtime state, optional source IDs, and snapshot keys.

`JourneyPlaythroughDto` includes:

- `RevisionId`
- `RevisionNumber`
- `SnapshotSchemaVersion`
- The typed immutable `Snapshot`

### Repository files

- `Eldoria.Infrastructure/Db/Repositories/JourneyPlaythroughRepository.cs`
- `Eldoria.Infrastructure/Db/Repositories/JourneyPlaythroughCharacterRepository.cs`
- `Eldoria.Infrastructure/Db/Repositories/ScenePlaythroughRepository.cs`
- `Eldoria.Infrastructure/Db/Repositories/OwnershipRepository.cs`

Ownership checks now follow `JourneyRevision.CreatedByUserId`, not `JourneyPlaythrough.Journey.UserId`. This allows a playthrough to remain authorized and readable when the current journey row is gone.

Repositories no longer include live scene, character, spell, item, event, or chest navigation properties when loading runtime DTOs.

Scene playthrough ordering uses copied `SnapshotSortOrder`, not the current scene sort order.

## 9. Asset retention

### Files

- `Eldoria.Core/Interfaces/IPlaythroughAssetRepository.cs`
- `Eldoria.Infrastructure/Db/Repositories/PlaythroughAssetRepository.cs`
- `Eldoria.Application/Services/AzureStorageBlob.cs`

Uploaded blobs already use unique names, so replacing an image creates a new object. The missing behavior was preventing deletion of an old object referenced by a historical snapshot.

Before deleting a blob, `AzureStorageBlob` now asks whether any `JourneyRevision.SnapshotJson` contains the URL or filename. If a revision references it, deletion is skipped and the asset remains available to historical playthroughs.

This applies to journey, scene, grid, character, spell, item, dialog, and intro content because their URLs and filenames are stored inside the snapshot JSON.

There is currently no automated cleanup process for revision-retained assets. Cleanup should only be added alongside an explicit revision/playthrough retention policy.

## 10. Database migration

### Files

- `Eldoria.Infrastructure/Migrations/20260810000000_AddImmutablePlaythroughSnapshots.cs`
- `Eldoria.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs`

The migration:

- Creates `JourneyRevisions`.
- Adds the required revision FK to playthroughs.
- Adds snapshot and assignment keys to runtime tables.
- Adds preserved source journey/scene IDs.
- Adds explicit equipped state.
- Makes live source foreign keys nullable.
- Changes their delete behavior to `SetNull`.
- Replaces source-based unique indexes with snapshot-key indexes.
- Adds the active-playthrough concurrency index based on preserved `SourceJourneyId`.

### Existing playthrough rows

The migration creates a schema-version-0 placeholder revision for every pre-migration playthrough and connects the existing playthrough to it.

It also backfills runtime snapshot keys from current source IDs.

Important limitation:

> Existing playthroughs cannot be made historically accurate retroactively because the database does not contain the source values as they existed when those playthroughs started.

Only playthroughs started after this implementation receive a complete version-1 immutable snapshot. Legacy playthroughs remain identifiable and migratable without being dropped, but their original historical definition cannot be recovered.

## 11. Frontend changes

### Files

- `Lunoria.Web/src/features/playthroughs/types.ts`
- `Lunoria.Web/src/pages/authenticated/PlayHubPage.tsx`

The frontend playthrough contract now contains typed snapshot definitions matching the backend response.

The Play Hub displays the revision number next to each playthrough date. Existing start, resume, and log navigation behavior is otherwise unchanged.

## 12. Test changes

### File

`Eldoria.Application.Tests/JourneyWorkflowTests.cs`

A test was added that verifies starting a playthrough:

- Returns the captured journey definition.
- Serializes scene content into the revision.
- Copies journey-character HP into runtime state.
- Stores the correct assignment and character definition identities.
- Seeds a scene playthrough.
- Seeds the initially active participant.

Existing playthrough service tests were updated for the snapshot-builder dependency and revision-backed DTO mapping.

The repository already contains older tests that reference pre-redesign types and fields. Those pre-existing test-suite issues were not rewritten as part of this change.

## 13. Files that contain mostly mechanical changes

The large file count is primarily caused by applying the same source-decoupling rule consistently across runtime entities, EF configurations, DTOs, and mappings.

These groups can be reviewed together:

### Journey runtime character group

```text
JourneyPlaythroughCharacter*
- Core entity
- Infrastructure configuration
- Application DTO
- Application mapping
```

### Scene runtime character group

```text
ScenePlaythroughCharacter*
- Core entity
- Infrastructure configuration
- Application DTO
- Application mapping
```

### Scene event/chest group

```text
ScenePlaythroughEvent*
ScenePlaythroughChest*
- Core entity
- Infrastructure configuration
- Application DTO
- Application mapping
```

For each group, verify the same pattern:

1. Snapshot key is required.
2. Source ID is nullable.
3. Source FK uses `SetNull`.
4. DTO exposes the snapshot key and does not map the live source DTO.

## 14. Manual verification checklist

No build, tests, migration application, server, or browser verification was run while implementing this change, following the repository owner's verification preference.

Recommended verification sequence:

1. Review the generated migration for unexpected table or column drops.
2. Build the backend solution.
3. Run the application tests, keeping the documented stale-test caveat in mind.
4. Apply the migration to a disposable development database.
5. Create a fully populated journey with:
   - Intro pages.
   - Multiple ordered scenes.
   - Internal grid and background.
   - Journey and scene characters.
   - Alternate forms.
   - Character and assignment spells.
   - Dialog pages and sections.
   - Events and actions.
   - Chests containing consumable and equipment loot.
   - Equipment with modifiers and granted spells.
6. Start a playthrough and record its returned snapshot.
7. Change every source name, description, image, stat, spell value, item effect, equipment modifier, scene order, dialog section, event action, and loot range.
8. Reload the playthrough and confirm the snapshot and runtime starting values are unchanged.
9. Delete source scene content and confirm the playthrough remains readable.
10. Replace an image and confirm the old snapshot URL still resolves.
11. Deactivate and resume the playthrough and confirm it still uses the same revision.
12. Complete the playthrough and confirm it remains readable.
13. Start another playthrough without changing authoring content and confirm it reuses the same revision.
14. Change authoring content, start another playthrough, and confirm the revision number increments.
15. Send two simultaneous start requests and confirm only one active playthrough is created.

## 15. Known boundaries and future work

The implementation establishes the immutable playthrough definition boundary. These related product policies remain intentionally unresolved:

- How long completed playthroughs and retained assets should live.
- Whether catalog and authoring deletion should become archive-first throughout the UI.
- Whether active playthroughs will ever support an explicit audited patch workflow.
- How schema-version upgrades should transform future snapshot versions.
- Whether legacy schema-version-0 playthroughs should be hidden, labeled, or handled through a special legacy viewer.

Any future gameplay implementation should follow this rule:

> If a value affects an active or historical playthrough, resolve it from the playthrough revision or mutable runtime state, never from an editable authoring or catalog navigation property.
