# Entity Redesign Summary

## Purpose

This document records the entity and Entity Framework redesign discussed and
implemented in June 2026.

## Character Model

- `Character` is the reusable character template.
- Template statistics use the `Base*` prefix:
  - `BaseMaxHp`
  - `BaseMaxMp`
  - `BaseMeleeAttackDamage`
  - `BaseBowAttackDamage`
  - `BaseMovement`
  - `BaseMaxConsumableInventory`
  - `BaseMaxEquippableInventory`
- `BaseAlternateForm` defines the template's optional alternate form.
- `JourneyCharacter` stores mutable journey state, including current and
  maximum HP/MP, combat statistics, inventory capacities, and current
  alternate-form state.
- Journey character state persists when an old playthrough becomes inactive
  and a new playthrough begins.

## Consumables and Equipment

- `Item` represents consumable, one-time-use inventory such as potions.
- `EquippableItem` is a separate catalog entity and is not derived from `Item`.
- `JourneyCharacterItem` records consumables owned by a journey character.
- `JourneyCharacterEquippableItem` records equipment owned by a journey
  character and contains `IsEquipped`.
- Consumable and equipment inventory capacities are tracked independently.
- Equipment effects use explicit integer modifiers that default to zero:
  - Attack damage modifiers
  - Movement, maximum HP, and maximum MP modifiers
  - Consumable and equipment inventory modifiers
  - Melee, bow, and spell damage reduction
  - Spell damage modifier, optionally scoped to a spell type
- Effective statistics should be calculated from journey statistics plus
  currently equipped item modifiers. Equip/unequip operations should not
  repeatedly mutate the stored base journey statistics.
- If removing equipment lowers maximum HP, current HP must be clamped to the
  new effective maximum.

## Spells

- `SpellType` is an entity rather than an enum so users can create types
  dynamically.
- Every spell has a required `SpellTypeId`.
- Existing spells are assigned to an `Uncategorized` type by the migration.
- `JourneyCharacterSpell` records spells available to a journey character.
- Duplicate `(JourneyCharacterId, SpellId)` rows are prevented by a unique
  index.
- Equipment can grant spells through the `EquippableItemSpells` relationship.
- Spell type names are unique per owner.

## Scenes, Playthroughs, and Turns

- `JourneyPlaythrough` represents one run of a journey.
- A journey can have multiple playthroughs, but a filtered unique index permits
  only one active playthrough at a time.
- `SceneProgress` belongs to a scene and a playthrough.
- `(JourneyPlaythroughId, SceneId)` is unique, allowing independent progress
  records for replayed scenes.
- `SceneParticipant` is the common scene-level identity for either a
  `JourneyCharacter` or `SceneCharacter`.
- A database check constraint requires each participant to reference exactly
  one character kind.
- `SceneParticipantTurn` references a participant rather than using mixed
  character foreign keys.
- Turn positions are unique within a scene progress record.
- Composite foreign keys prevent turns from referencing participants belonging
  to another progress record.

## Ownership

- Characters, consumables, equipment, spells, and spell types now have optional
  user ownership relationships.
- Ownership is nullable as a staged migration strategy because existing global
  catalog rows have no reliably inferable owner.
- A later application/API pass should:
  1. Assign existing rows to their correct owners.
  2. Pass authenticated user IDs when creating catalog records.
  3. Filter all catalog reads and mutations by owner.
  4. Make ownership foreign keys non-nullable in a follow-up migration.

## EF Migration

Migration:

`20260628155932_ExpandGameplayDomain`

The initially generated migration was unsafe because EF incorrectly inferred
some renames, including treating `Character.Movement` as `UserId`. It was
manually corrected.

The corrected migration:

- Uses explicit renames for existing character columns.
- Preserves existing character statistics and alternate-form relationships.
- Backfills journey character statistics from character templates.
- Initializes both new inventory capacities from the former single capacity.
- Creates an `Uncategorized` spell type and assigns existing spells to it.
- Introduces ownership columns as nullable.
- Includes corrected rollback operations.
- Updates the EF model snapshot and migration designer.

The migration has been applied successfully to the local development database.

SQL script generation was blocked by the local Windows Application Control
policy when EF attempted to load the compiled `Eldoria.Core.dll`. This was an
environment restriction rather than a C# compilation failure.

## Validation Status

- The full .NET solution builds successfully.
- Existing nullable-reference and migration naming warnings remain.
- The React production build had pre-existing TypeScript errors unrelated to
  this entity redesign.
- No automated test projects currently cover the migration or domain behavior.

## Remaining Work

- Implement CRUD endpoints and services for spell types, equipment, and
  playthroughs.
- Update DTOs and frontend contracts to expose separate consumable and
  equipment capacities.
- Enforce authenticated ownership throughout catalog services and repositories.
- Implement effective-stat calculation and equip/unequip rules.
- Enforce inventory capacities in application logic.
- Decide whether active equipment uses only a count limit or named equipment
  slots.
- Review and test the generated migration SQL in a disposable database before
  production deployment.
- Rotate exposed development credentials and move secrets from tracked
  `appsettings.json` into user secrets or environment variables.
