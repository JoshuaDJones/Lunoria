# Journey Playthrough Isolation Review — Context Handoff

## Purpose

This document is context for reviewing Eldoria's journey playthrough implementation.

The user wants to review the design carefully, one piece at a time. The assistant's role during this review is **scribe and advisor, not programmer**.

Do not edit source code, entities, migrations, or tests unless the user later gives explicit permission to implement a specific agreed change. Capture decisions, identify consequences, ask focused questions when necessary, and help the user reason through each part of the model.

## Product goal

When the user presses **Start** on a journey, the application must capture all information needed to play that journey:

- Journey metadata and intro pages
- Every scene and its configuration
- Journey and scene characters, including alternate forms
- Character stats and assigned spells
- Spell types and spell definitions
- Consumable and equippable item definitions
- Dialogs, pages, sections, and character references
- Scene events, actions, targets, and effects
- Chests, loot tables, and item references
- Grid configuration and other gameplay-relevant values
- Any other definition that can affect how the playthrough looks or behaves

The resulting playthrough must be isolated from editable authoring/catalog data. After play begins, changes made outside the playthrough must not change its rules, content, ordering, text, stats, relationships, or behavior.

The central invariant is:

> Once a playthrough starts, it must remain playable and internally consistent even if the source journey or any linked source records are edited or deleted.

"Capture" or "move into playthrough state" means creating an independent frozen representation. It does not mean deleting the original authoring records.

## Current implementation

The latest implementation was introduced in commit:

`920888f feat: journey playthrough`

It added an immutable snapshot architecture centered on `JourneyRevision`:

1. `JourneySnapshotBuilder` reads the editable journey graph.
2. `JourneyPlaythroughService.StartAsync` serializes that graph as `JourneySnapshotV1` JSON.
3. A SHA-256 content hash is calculated.
4. A `JourneyRevision` stores the JSON, schema version, content hash, revision number, source journey, creator, and creation time.
5. A playthrough references the revision through `JourneyRevisionId`.
6. Identical snapshot content can reuse an existing revision.
7. Relational playthrough tables store mutable runtime state such as HP, MP, scene progress, participants, event status, chest status, inventory state, and spell assignments.
8. Snapshot-local string keys connect runtime rows to frozen definitions in the JSON.

The related EF migration is:

`Eldoria.Infrastructure/Migrations/20260810000000_AddImmutablePlaythroughSnapshots.cs`

This migration has been committed and pushed to `main`. It may still be replaced if it has only been used locally. If another developer or deployed environment has applied it, changes should be made through a corrective migration instead.

## Current design is provisional

The user is not satisfied with the AI-generated implementation and does not want to assume it is correct simply because it is extensive or already committed.

In particular, the following decision remains open:

- Keep `JourneyRevision` as a shared immutable definition referenced by playthroughs.
- Store one frozen snapshot directly on each `JourneyPlaythrough`.
- Copy all definitions into normalized playthrough-specific relational tables.
- Use another design agreed during the review.

Do not prematurely optimize for deduplication, revision history, or architectural elegance. First establish the actual product requirements and the simplest design that satisfies the isolation invariant.

## How to conduct the review

Review one bounded area at a time. For each entity or feature:

1. Identify all editable source records and fields used by gameplay or presentation.
2. Identify what is copied or snapshotted when Start is pressed.
3. Identify the mutable runtime state created for the playthrough.
4. Trace every read and mutation performed while playing.
5. Check whether any playthrough behavior still reads editable source data.
6. Check what happens if the source record is edited, soft-deleted, or hard-deleted.
7. Record gaps and possible solutions.
8. Let the user choose the design before moving to implementation.

Suggested review order:

1. Journey and playthrough lifecycle
2. Scenes and scene ordering
3. Journey characters and alternate forms
4. Scene characters and participants
5. Spells and spell types
6. Consumable items
7. Equippable items and modifiers
8. Dialogs and intro pages
9. Events and event actions
10. Chests and loot selection
11. Grid and visual configuration
12. Assets and deletion behavior
13. DTOs, mappings, and API read paths
14. Ownership and authorization
15. Concurrency and duplicate active playthrough prevention
16. EF relationships, migration, and tests

## Questions to apply to every area

- Is this value a frozen definition, mutable runtime state, or both?
- Is every gameplay-relevant field preserved?
- Does gameplay read the preserved value or accidentally load the current source value?
- Are source foreign keys optional historical references, or are they still required for gameplay?
- Would editing or deleting the source break an active or completed playthrough?
- Are stable snapshot-local identifiers necessary here?
- Is the same information duplicated in JSON and relational rows? If so, which copy is authoritative?
- Can a saved playthrough be loaded without querying editable journey/catalog tables?
- Do media URLs remain usable if an author deletes or replaces the source asset?
- Is the added complexity justified by a concrete requirement?

## Known observations

- Existing playthrough tables primarily contain runtime state and only some copied definition values.
- They do not relationally preserve every definition needed for dialogs, event actions, loot rules, item effects, spell metadata, scene presentation, and similar content.
- The current snapshot attempts to preserve those definitions in `JourneyRevision.SnapshotJson`.
- Nullable source relationships and snapshot keys were introduced so runtime rows can survive source deletion.
- The current revision table provides snapshot reuse and version metadata, but those capabilities may be unnecessary for the product.
- A snapshot stored directly on `JourneyPlaythrough` could provide isolation with less lifecycle complexity, at the cost of duplicating JSON for repeated runs.
- Copying everything into relational playthrough tables could eliminate snapshot JSON, but would require a much larger parallel definition schema and more copying/mapping code.
- External assets require separate consideration. Freezing a URL is not sufficient if the referenced blob can later be deleted or overwritten.

These are observations, not final decisions.

## Assistant working agreement

During this review, the assistant should:

- Explain the current behavior using concrete references to the repository.
- Point out inconsistencies, hidden dependencies, unnecessary complexity, and missing data.
- Present tradeoffs neutrally and make a recommendation when useful.
- Maintain a written decision log as the user makes choices.
- Keep the discussion focused on the current entity or feature.
- Distinguish verified repository behavior from inference.
- Avoid writing code or changing migrations until explicitly asked.

The assistant should not:

- Treat the current implementation or architecture documents as authoritative requirements.
- Make broad redesign decisions without the user.
- Silently expand the scope of a decision.
- Begin refactoring while the design review is still underway.
- Remove migrations or drop/recreate the database without explicit authorization at that time.

## Definition of success

The review is complete when there is an agreed, documented design showing:

- Exactly what is frozen when a journey starts
- Exactly what remains mutable during play
- Where each frozen and mutable value is stored
- Which copy is authoritative when data exists in more than one place
- How playthrough records reference frozen definitions
- How active and completed playthroughs survive source edits and deletion
- How asset lifetime is protected
- How the API loads and mutates a playthrough without depending on current authoring data
- What entity, configuration, migration, mapping, and test changes are required

Only after that agreement should implementation begin.
