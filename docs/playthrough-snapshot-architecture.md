# Immutable Playthrough Snapshot Architecture

## Status

Proposed design for protecting active and completed playthroughs from later edits or deletions to journey-authoring and catalog data.

## Problem

Lunoria currently separates some runtime state into `JourneyPlaythrough*` and `ScenePlaythrough*` entities, but those entities still reference editable source records. Starting a journey currently creates only the root `JourneyPlaythrough`; it does not create a complete, immutable copy of the playable journey.

Examples of live dependencies include:

- `JourneyPlaythrough` references the editable `Journey`.
- `ScenePlaythrough` references the editable `Scene`.
- `JourneyPlaythroughCharacter` and `ScenePlaythroughCharacter` retain foreign keys to journey/scene character definitions.
- Playthrough spell records point through live `JourneyCharacterSpell` or `SceneCharacterSpell` records.
- Playthrough consumable and equipment inventory rows point directly to editable catalog records.
- Scene playthrough events and chests point to editable scene event, chest, loot, and catalog records.
- Dialogs and intro content currently have no playthrough-specific representation.
- Playthrough reads map some of these live navigation properties directly into DTOs.

This creates two classes of failure:

1. **Modification leakage:** changing a name, image, stat, spell cost/effect, equipment modifier, consumable effect, event action, chest roll range, dialogue page, or scene order can change an already-running or historical playthrough.
2. **Deletion coupling:** foreign keys may block deletion, cascade-delete history, or leave runtime queries unable to load required data. A blocked delete is safer than silent corruption, but it is still poor behavior for a reusable authoring catalog.

Character soft deletion is not sufficient protection. Query filters can hide a deleted character even when a playthrough still needs its identity and presentation. Catalog item foreign keys configured with `NoAction` usually prevent deletion, but they do not prevent later edits from changing gameplay.

## Required invariants

The design should enforce these rules:

1. Starting a playthrough captures the complete playable definition at that moment.
2. A playthrough never reads gameplay-relevant values from mutable authoring or catalog records.
3. Editing or deleting a journey, scene, character, spell, spell type, consumable, equipment item, dialogue, event, chest, or loot entry cannot alter an existing playthrough.
4. Runtime changes such as HP, MP, inventory use, equipped state, participants, event execution, and chest results remain relational and mutable.
5. Completed playthroughs remain readable even after their source content is archived or deleted.
6. Snapshot content is immutable after creation.
7. Images and other assets referenced by a snapshot remain available for the lifetime of that snapshot.
8. Starting a playthrough is atomic: either the snapshot and initial runtime state are fully created, or nothing is created.

## Recommended model

Use an immutable, versioned journey revision for playable definitions and keep the existing playthrough entities for mutable runtime state.

```text
Editable authoring data
Journey + Scenes + Catalog records
              │
              │ publish/start
              ▼
JourneyRevision (immutable snapshot)
├── Journey metadata
├── Ordered scene definitions
├── Dialog and intro content
├── Event and chest definitions
└── Referenced catalog definitions
              │
              ▼
JourneyPlaythrough (mutable run)
├── JourneyPlaythroughCharacters
├── ScenePlaythroughs
├── Participants and ordering
├── Inventory instances and equipped state
├── Event execution state
└── Chest state and selected loot
```

Each `JourneyPlaythrough` should reference a `JourneyRevision`, not depend on the current `Journey` graph for gameplay. Multiple playthroughs may reference the same immutable revision when no authoring content has changed.

### Suggested revision entity

```text
JourneyRevision
- Id
- SourceJourneyId (nullable audit reference; never required to load the run)
- RevisionNumber
- SchemaVersion
- ContentHash
- SnapshotJson
- CreatedAt
- CreatedByUserId
```

Recommended relationship behavior:

- `JourneyPlaythrough.JourneyRevisionId` is required and uses `Restrict` deletion.
- `JourneyRevision.SourceJourneyId` is nullable and uses `SetNull` if hard deletion is supported.
- A source journey deletion must never cascade into revisions or playthroughs.
- Revisions are immutable. Application services must not expose update operations for snapshot content.

For the current application, a JSON snapshot is preferable to a complete parallel set of revision tables. The playable graph is naturally loaded as a unit, JSON avoids duplicating dozens of authoring tables, and `SchemaVersion` permits controlled evolution. Continue storing mutable runtime state relationally.

If future requirements demand SQL reporting over historical scene definitions, individual revision comparisons, or partial revision editing, the JSON model can later be replaced with normalized revision tables without changing the core immutability rule.

## Snapshot boundary

The snapshot must include every value that can influence gameplay, presentation, or historical understanding. Storing only foreign-key IDs is not a snapshot.

### Journey

- Source journey ID for traceability only
- Name and description
- Image asset reference
- Ordered intro pages and their complete configuration
- Ordered scene snapshot keys
- Initial journey-character roster
- Revision creation timestamp and schema version

### Scenes

- Source scene ID for traceability only
- Name, description, image, grid URL, and order
- Scene-specific character definitions
- Intro content
- Complete dialogue hierarchy: dialogs, ordered pages, ordered sections, narrator/character attribution, text, and images
- Ordered event definitions and all actions
- Chest definitions and ordered/ranged loot entries

### Characters

Create a character definition inside the revision for every character used by the journey or any scene. Include:

- Source character ID for traceability only
- Name, description, type, image, and portrait
- Base HP/MP, attack values, movement, and inventory capacities
- Alternate-form definition/reference within the snapshot
- Dialog presentation settings
- Initially assigned spells
- Any other value displayed or used in play

Runtime character entities should copy the starting values they need and refer to a snapshot character key, not to `Character`, `JourneyCharacter`, or `SceneCharacter` for required gameplay data.

### Spell types and spells

Snapshot every spell available from a character, journey assignment, scene assignment, equipment grant, event, or loot path.

Spell type definition:

- Snapshot key
- Source spell-type ID for traceability only
- Name, description, and image

Spell definition:

- Snapshot key
- Source spell ID for traceability only
- Name, description, and image
- Range and radius behavior
- MP cost
- Damage, health, and magic effects
- Snapshot spell-type key

Playthrough spell ownership should reference the snapshot spell key. It must not reference `JourneyCharacterSpell`, `SceneCharacterSpell`, or the live `Spell` row.

### Consumables

Snapshot every consumable initially owned, obtainable from a chest/event, or otherwise available to the playthrough:

- Snapshot key
- Source consumable ID for traceability only
- Name, description, and image
- HP and MP effects

A runtime inventory row represents an instance or stack and stores its snapshot consumable key, quantity, and used/remaining state. It must not depend on the live `ConsumableItem` table.

### Equipment

Snapshot every equipment item initially owned, obtainable from a chest/event, or otherwise available to the playthrough:

- Snapshot key
- Source equipment ID for traceability only
- Name, description, and image
- All attack, movement, HP/MP, and capacity modifiers
- All damage-reduction values
- General or spell-type-specific damage modifier
- Snapshot key of the affected spell type, when applicable
- Snapshot keys of granted spells

A runtime equipment row stores the snapshot equipment key, ownership, equipped state, and any future instance-specific state. Effective-stat calculations must read modifiers from the revision snapshot, never from the current `EquippableItem` record.

### Events

- Event snapshot key, name, description, and order
- Action snapshot key, name, order, target type, and action type
- Complete character-stat adjustment definition
- Snapshot character key for a single-character target, if applicable

Runtime event rows store execution status, error, and timestamps against an event snapshot key.

### Chests and loot

- Chest snapshot key, name, die sides, and scene ownership
- Loot-entry snapshot key, minimum/maximum roll, and quantity
- Exactly one snapshot consumable or equipment key per loot entry

Runtime chest rows store status, rolled value, opened time, and selected loot-entry snapshot key. They must not load a live `SceneChest` or `SceneChestLootEntry` to determine the result after play begins.

## Snapshot identity

Use snapshot-local stable keys, preferably GUIDs or deterministic strings, within `SnapshotJson`. Preserve source database IDs only as optional audit metadata.

Do not use source IDs as the runtime contract because:

- source records may be deleted;
- IDs describe authoring identity, not immutable content identity;
- one source record may have different values in different revisions.

Example:

```json
{
  "schemaVersion": 1,
  "journey": {
    "sourceJourneyId": 42,
    "name": "The Moonlit Pass",
    "sceneKeys": ["scene-forest", "scene-ruins"]
  },
  "spells": {
    "spell-firebolt": {
      "sourceSpellId": 8,
      "name": "Firebolt",
      "mpCost": 3,
      "damageEffect": 5,
      "spellTypeKey": "spell-type-fire"
    }
  }
}
```

The exact JSON contract should use typed C# snapshot records and normal serialization rather than anonymous objects or entity serialization. Never serialize EF navigation graphs directly.

## Runtime state versus definition data

Keep these values relational and mutable:

- active/completed playthrough status and timestamps;
- current HP/MP and down/dead state;
- current alternate form;
- inventory quantities and used state;
- equipment ownership and equipped state;
- participants, activation, ordering, current participant, and round;
- scene status and timestamps;
- event execution status and logs;
- chest roll, selected loot, and opened state.

Runtime rows should contain a snapshot key for the immutable definition they represent. Optional source IDs may be retained for diagnostics, but gameplay must continue if every source authoring row disappears.

## Starting a playthrough

`JourneyPlaythroughService.StartAsync` should become an orchestration operation executed in a database transaction:

1. Load the complete owned journey aggregate with `AsNoTracking`, including all nested playable content and referenced catalog definitions.
2. Validate that the journey can be played:
   - all required references exist;
   - scene order is valid;
   - loot ranges are valid and deterministic according to the chosen rules;
   - alternate forms and targeted characters belong to the snapshot;
   - all spells/items referenced by equipment and loot are included;
   - images/assets have durable snapshot references.
3. Build a typed `JourneySnapshotV1` in memory.
4. Serialize it using deterministic settings and calculate a content hash.
5. Reuse an existing immutable revision with the same journey/content hash, or insert a new revision.
6. Create the `JourneyPlaythrough` referencing that revision.
7. Seed journey-playthrough characters, spells, consumables, and equipment from snapshot values.
8. Either seed every `ScenePlaythrough` immediately or create it lazily from the revision. Immediate creation offers stronger validation; lazy creation reduces rows. In both cases the definition must come from the revision.
9. Commit once. Roll back the snapshot and all runtime rows on any failure.

Add a concurrency guard so two simultaneous start requests cannot both create an active playthrough. Keep the filtered unique database index as the final enforcement layer.

## Editing behavior after start

Ordinary authoring edits should affect only future revisions and playthroughs.

- Never silently apply a catalog or scene edit to an active playthrough.
- If live corrections are needed later, build an explicit **playthrough patch** workflow with validation, a preview of affected runtime state, confirmation, and an audit log.
- Do not mutate an existing revision to implement a correction.
- A “restart using latest content” action should create a new playthrough/revision rather than rewriting the current run.

## Deletion and archival policy

Snapshotting makes source deletion safe for existing playthroughs, but archival should remain the normal user-facing operation.

### Catalog records

Characters, spells, spell types, consumables, and equipment should support soft deletion/archive semantics:

- archived records disappear from new authoring selections;
- existing draft references can show an archived warning and require replacement before publishing;
- existing immutable revisions remain unchanged;
- existing playthroughs continue using snapshot values;
- permanent purge is an administrative/retention operation, not ordinary UI deletion.

This also produces consistent behavior; currently characters are soft deleted while most other catalog records are hard deleted or blocked by foreign keys.

### Journeys and scenes

- Archive journeys and scenes by default.
- Do not cascade source journey deletion into revisions or playthroughs.
- A hard-deleted source journey may set `JourneyRevision.SourceJourneyId` to null.
- Revisions referenced by playthroughs cannot be deleted.
- If retention rules allow deleting an entire playthrough, delete its runtime state first and delete its now-unreferenced revision only through an explicit cleanup process.

### Assets

Current image update/delete workflows can remove the old blob. That is incompatible with historical snapshots if the snapshot stores the same URL.

Use immutable, uniquely named asset objects:

- uploading a replacement creates a new object;
- authoring data points to the new object;
- old objects remain while any revision references them;
- cleanup deletes only objects with no draft, revision, or playthrough references.

Possible implementations include an `Asset` table with reference tracking, blob versioning with retained version IDs, or copying referenced assets into revision-owned paths. A URL alone is safe only if the target is immutable and retained.

## API and DTO direction

- Playthrough endpoints return snapshot-backed DTOs.
- Authoring endpoints continue returning current catalog/journey DTOs.
- Do not reuse authoring DTOs inside playthrough DTOs when doing so causes services or mappings to load live entities.
- Include `revisionId`, `revisionNumber`, and optionally the source IDs in playthrough responses for diagnostics.
- Expose the snapshot schema version internally; clients should receive a stable playthrough DTO rather than parse `SnapshotJson` directly.

## Migration plan

### Phase 1: establish the revision boundary

1. Add `JourneyRevision` and required `JourneyPlaythrough.JourneyRevisionId` support.
2. Define immutable typed snapshot records, starting with `JourneySnapshotV1`.
3. Implement a snapshot builder that loads and validates the complete journey graph.
4. Add deterministic serialization, schema versioning, and content hashing.
5. Write snapshot-builder tests before changing runtime reads.

### Phase 2: start playthroughs from snapshots

1. Refactor `StartAsync` into one transaction.
2. Seed journey playthrough characters and inventory using copied values and snapshot keys.
3. Seed or lazily create scene playthrough state from the revision.
4. Update playthrough DTO mappings so they never traverse authoring navigation properties.

### Phase 3: remove live catalog dependencies

1. Replace playthrough spell foreign keys to journey/scene spell assignments with snapshot spell keys.
2. Replace playthrough consumable/equipment foreign keys with snapshot definition keys.
3. Replace event/chest source foreign keys used for gameplay with snapshot keys.
4. Remove or make optional source navigation properties after all reads use snapshots.

### Phase 4: safe deletion and assets

1. Standardize archive/soft-delete behavior across all catalog resources.
2. Remove cascade paths from source journeys into revisions/playthroughs.
3. Implement immutable asset retention.
4. Add explicit cleanup policies for unreferenced revisions and assets.

### Phase 5: frontend behavior

1. Separate authoring types from playthrough types in `Lunoria.Web`.
2. Show the playthrough revision in operator diagnostics.
3. Make it clear that edits affect future games only.
4. Add an archived state to catalog management.
5. If required, add an explicit audited playthrough-patch workflow later.

## Required tests

At minimum, add tests proving that:

- starting a playthrough captures all required definitions;
- editing every snapshot-supported source entity does not change playthrough DTOs or effective calculations;
- archiving/deleting source characters, spells, consumables, equipment, scenes, or journeys does not break a playthrough;
- spell costs/effects remain fixed after source edits;
- equipment modifiers and granted spells remain fixed after source edits;
- consumable effects remain fixed after source edits;
- dialogue, event actions, scene ordering, and chest loot remain fixed after source edits;
- alternate forms resolve entirely inside the snapshot;
- asset replacement does not break historical images;
- snapshot creation rolls back completely on failure;
- simultaneous start requests still produce only one active playthrough;
- snapshot schema upgrades can read all previously stored versions;
- completed playthroughs remain readable without loading any mutable source navigation.

An especially valuable integration test is:

1. Create a fully populated journey.
2. Start a playthrough.
3. Record its returned playthrough representation and effective calculations.
4. Edit/archive/delete every source record allowed by policy.
5. Load the playthrough again.
6. Assert that the playable definition and calculations are unchanged.

## Decisions to make before implementation

1. Should revisions be created only when play starts, or also through an explicit Publish action?
2. Should identical content hashes reuse a revision?
3. Should all scene runtime rows be seeded immediately or lazily?
4. How long must completed playthroughs and their assets be retained?
5. Is permanent user-facing deletion required, or is archive plus administrative purge sufficient?
6. Will active playthrough patching be supported, or will corrections always require a new run?

## Recommendation

Implement an immutable `JourneyRevision` JSON snapshot and make it the sole definition source for playthrough behavior. Copy all gameplay-relevant character, spell, consumable, equipment, scene, dialogue, event, chest, and loot values into that revision. Keep mutable session state relational, reference snapshot-local keys from runtime rows, and remove required playthrough foreign keys to editable catalog records.

This approach prevents both modification leakage and deletion coupling without requiring a second normalized copy of every authoring table for every playthrough.
