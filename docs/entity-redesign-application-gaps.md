# Entity Redesign Application Gaps

## Purpose

This document records the API, application-layer, and React changes still
required after the June 2026 entity and Entity Framework redesign.

The database model is currently ahead of both the API and React application.

Checklist status:

- `[x]` Done
- `[ ]` Not done or only partially implemented

## Immediate Runtime Blockers

### Spell Creation

`SpellTypeId` is now required by the database, but the current spell creation
and update workflows do not provide it.

Required changes:

- [x] Add `SpellTypeId` to spell create and update requests.
- [x] Add spell type information to `SpellDto`.
- [x] Update spell mappings and service method signatures.
- [x] Set and validate `SpellTypeId` when creating or updating a spell.
- [ ] Add a spell type selector to the React spell form.

Relevant files:

- `Eldoria.Api/Requests/CreateSpellRequest.cs`
- `Eldoria.Api/Requests/UpdateSpellRequest.cs`
- `Eldoria.Application/Dtos/SpellDto.cs`
- `Eldoria.Application/Common/SpellMappings.cs`
- `Eldoria.Application/Services/SpellService.cs`
- `Eldoria.Web/src/types/spell.ts`
- `Eldoria.Web/src/components/modals/AddEditSpellModal.tsx`

### Catalog Ownership

Characters, consumables, equipment, spells, and spell types now have optional
`UserId` ownership, but ownership is not enforced in the application.

Required changes:

- [x] Retrieve the authenticated user ID in all catalog controllers.
- [x] Pass the user ID into catalog service methods.
- [x] Assign `UserId` when creating all catalog records.
- [x] Filter all catalog list and lookup operations by owner.
- [x] Require ownership for all catalog updates and deletes.
- [x] Enforce ownership when selecting alternate forms, assigning spells, or
  referencing catalog records from another entity.
- [x] Backfill existing catalog rows with the correct owners.
- [x] Make ownership foreign keys non-nullable after the backfill is complete.

Authentication should also be required globally or through `[Authorize]` on
protected controllers. The React application sends bearer tokens, but most API
controllers are not currently marked as requiring authentication.

Status:

- [x] A global fallback authorization policy requires authenticated users.
- [x] Authentication endpoints remain available through `[AllowAnonymous]`.

Relevant files include:

- `Eldoria.Api/Controllers/CharacterController.cs`
- `Eldoria.Api/Controllers/ItemController.cs`
- `Eldoria.Api/Controllers/SpellController.cs`
- `Eldoria.Application/Services/CharacterService.cs`
- `Eldoria.Application/Services/ItemService.cs`
- `Eldoria.Application/Services/SpellService.cs`
- `Eldoria.Infrastructure/Db/Repositories/CharacterRepository.cs`
- `Eldoria.Infrastructure/Db/Repositories/SpellRepository.cs`
- `Eldoria.Api/Program.cs`

## API Contract Changes

### Separate Character Inventory Capacities

The API still exposes one `MaxInventory` value. The character service copies
that value into both `BaseMaxConsumableInventory` and
`BaseMaxEquippableInventory`.

Replace the old field with two independent fields:

- [x] `BaseMaxConsumableInventory`
- [x] `BaseMaxEquippableInventory`

Update:

- [x] Character DTOs
- [x] Character create and update requests
- [x] Character mappings
- [x] Character service signatures and assignments
- [ ] React character types
- [ ] Character create and edit forms
- [ ] Character detail and list displays

Relevant files:

- `Eldoria.Application/Dtos/CharacterDto.cs`
- `Eldoria.Application/Common/CharacterMappings.cs`
- `Eldoria.Application/Services/CharacterService.cs`
- `Eldoria.Api/Requests/CreateCharacterRequest.cs`
- `Eldoria.Api/Requests/UpdateCharacterRequest.cs`
- `Eldoria.Web/src/types/character.ts`
- `Eldoria.Web/src/components/modals/AddEditCharacterModal.tsx`
- `Eldoria.Web/src/components/lists/CharacterList.tsx`
- `Eldoria.Web/src/components/lists/JourneyCharacterList.tsx`

### Expand the Journey Character Contract

`JourneyCharacterDto` currently omits most mutable journey state. It should
expose:

- [x] Current HP and MP
- [x] Maximum HP and MP
- [x] Melee and bow attack damage
- [x] Movement
- [x] Consumable inventory capacity
- [x] Equipment inventory capacity
- [x] Alternate-form ID and current alternate-form state
- [x] Consumable inventory
- [x] Equipment inventory and equipped state
- [x] Journey-character spells
- [x] Effective statistics calculated from stored journey statistics and equipped
  modifiers

- [ ] Update the React dashboard to display journey-character effective
  statistics instead of character-template statistics.

Relevant files:

- `Eldoria.Application/Dtos/JourneyCharacterDto.cs`
- `Eldoria.Application/Common/JourneyCharacterMappings.cs`
- `Eldoria.Infrastructure/Db/Repositories/JourneyCharacterRepository.cs`
- `Eldoria.Web/src/types/journey.ts`
- `Eldoria.Web/src/components/cards/DashboardPlayerCard.tsx`
- `Eldoria.Web/src/components/lists/JourneyCharacterList.tsx`

### Existing Mapping Defects

Two mapping defects should be corrected:

1. [x] `JourneyCharacterDto.JourneyId` is assigned from `journeyCharacter.Id`
   instead of `journeyCharacter.JourneyId`.
2. [x] `CharacterDto.AlternateFormId` is declared but is not populated by
   `CharacterMappings.ToDto`.

Relevant files:

- `Eldoria.Application/Common/JourneyCharacterMappings.cs`
- `Eldoria.Application/Common/CharacterMappings.cs`

## Missing Backend Features

### Spell Types

Add complete spell type application support:

- [x] DTOs
- [x] Create and update requests
- [x] Controller and service
- [x] User-scoped list, lookup, create, update, and delete operations
- [x] Unique-name conflict handling
- [x] Validation when deleting a type used by spells or equipment
- [ ] React type definitions
- [ ] Spell type management UI or inline creation
- [ ] Spell type selection in spell and equipment forms

### Equipment

There is no API, service, DTO, or React workflow for equipment.

Required features:

- [x] Equipment catalog CRUD
- [x] Ownership enforcement for equipment catalog operations and references
- [x] All integer modifier fields
- [x] Optional affected spell type
- [x] Spells granted by equipment
- [x] Assigning and removing equipment from journey characters
- [x] Equip and unequip operations
- [x] Equipment inventory capacity enforcement
- [x] Effective-stat calculation
- [x] Effective spell calculation
- [x] Current HP clamping when effective maximum HP decreases
- [ ] React catalog and journey-character equipment interfaces

### Journey Character Spells

The database includes `JourneyCharacterSpell`, but the application still uses
template-level `CharacterSpell` records for journey characters.

Required changes:

- [x] Decide when template spells are copied to a journey character.
- [x] Seed journey-character spells when a character is added to a journey.
- [x] Provide endpoints for granting and removing journey-character spells.
- [x] Load journey-character spells in repository queries and DTO mappings.
- [x] Combine journey-character spells with spells granted by equipped items.
- [x] Prevent duplicate spell rows.
- [ ] Render effective journey-character spells in React instead of
  `character.characterSpells`.

### Preserve Journey Character State

`JourneyCharacterService.ReplaceJourneyCharacters` currently deletes every
journey character and recreates the selected set. This destroys:

- Current HP and MP
- Journey statistics
- Consumable inventory
- Equipment inventory
- Journey-character spells
- Scene participant relationships

Replace this operation with a diff:

- [ ] Preserve rows for characters that remain selected.
- [ ] Add rows only for newly selected characters.
- [ ] Explicitly handle removal of characters referenced by scene progress.
- [ ] Avoid resetting state when starting a new playthrough.

### Inventory Capacity Enforcement

Consumables can currently be added without checking capacity.

Required changes:

- [x] Count the inventory entries that consume capacity.
- [x] Keep used consumables in inventory history and exclude them from active
  capacity usage.
- [x] Reject additions above effective consumable capacity.
- [x] Reject equipment additions above effective equipment capacity.
- [x] Account for capacity modifiers from equipped items.
- [x] Reject unequip or removal operations that lower capacity below current
  inventory usage.

## Effective Statistics and Equipment Rules

Effective statistics should be calculated from:

1. Stored journey-character statistics.
2. Modifiers from currently equipped items.

Equip and unequip operations should not mutate the stored journey statistics.

Calculations are needed for:

- [x] Maximum HP and MP
- [x] Melee and bow attack damage
- [x] Movement
- [x] Consumable capacity
- [x] Equipment capacity
- [x] Melee, bow, and spell damage reduction
- [x] General or spell-type-specific spell damage modifiers
- [x] Spells granted by equipment

- [x] Clamp current HP to the effective maximum when effective maximum HP
  decreases below current HP.

## Playthrough and Scene Progression

The new playthrough and scene-progress entities are not connected to the API or
React application.

### Journey Playthroughs

Add workflows to:

- [ ] Start a playthrough.
- [ ] Retrieve the active playthrough.
- [ ] Complete or deactivate a playthrough.
- [ ] List previous playthroughs.
- [ ] Enforce one active playthrough per journey.
- [ ] Preserve journey-character state when transitioning between playthroughs.

### Scene Progress

Add workflows to:

- [ ] Create or retrieve progress for a scene in the active playthrough.
- [ ] Transition progress through `NotStarted`, `InProgress`, and `Completed`.
- [ ] List scene progress for a playthrough.
- [ ] Keep replayed scene progress independent between playthroughs.

### Participants and Turns

Add workflows to:

- [ ] Add journey characters or scene characters as scene participants.
- [ ] Enforce exactly one participant character type.
- [ ] Remove participants safely.
- [ ] Create, reorder, and delete participant turns.
- [ ] Enforce unique turn positions.
- [ ] Prevent turns from referencing participants from another progress record.

- [ ] Update the scene dashboard to load a specific `SceneProgress` and its
  participants rather than directly combining a scene with every journey
  character.

Relevant files:

- `Eldoria.Application/Services/SceneService.cs`
- `Eldoria.Application/Dtos/SceneDashboardDto.cs`
- `Eldoria.Web/src/types/scene.ts`
- `Eldoria.Web/src/pages/authenticated/SceneDashboard.tsx`

## Authorization Gaps

Several existing operations accept entity IDs without verifying ownership
through their parent entities.

These include:

- [x] Journey-character replace, update, and delete
- [x] Scene-character add, update, and delete
- [x] Journey consumable assignment and use
- [x] Scene consumable assignment and use
- [x] Character-spell replacement
- [x] Scene list and individual scene retrieval

- [x] Enforce authorization through the complete parent chain rather than
  treating a client-supplied `journeyId`, `sceneId`, or character ID as proof
  of access.

## React Changes

Add or update React support for:

- [ ] Separate consumable and equipment capacities
- [ ] Spell type management and selection
- [ ] Equipment catalog management
- [ ] Equipment assignment and equip/unequip controls
- [ ] Effective journey-character statistics
- [ ] Journey-character spell availability
- [ ] Active playthrough controls
- [ ] Scene progress status
- [ ] Scene participant selection
- [ ] Turn-order editing
- [ ] Capacity validation and API error display

- [ ] Make TypeScript contracts correspond directly to revised API DTOs rather
  than exposing template fields as mutable journey state.

## React Build Status

The React production build currently fails before Vite runs.

Current failures are unrelated to the entity redesign and primarily include:

- [ ] Unused imports, props, variables, and callback parameters
- [ ] Missing Node type definitions for `vite.config.ts`
- [ ] Missing definitions for `path` and `__dirname`

- [ ] Resolve the existing React errors before using the production build to
  validate the redesigned frontend.

## Recommended Implementation Order

1. [x] Require authentication and enforce catalog ownership.
2. [x] Implement spell type CRUD and repair spell creation and updates.
3. [x] Update character and journey-character API contracts.
4. [x] Correct the existing mapping defects.
5. [x] Implement equipment and effective-stat calculations.
6. [x] Implement inventory capacity enforcement.
7. [ ] Implement journey-character spell availability.
8. [ ] Replace destructive journey-character replacement.
9. [ ] Implement playthrough lifecycle APIs.
10. [ ] Implement scene progress, participants, and turns.
11. [ ] Update React screens and state management for the new APIs.
12. [ ] Resolve the existing React production build errors.
13. [ ] Add automated tests for ownership, capacity, equipment calculations,
    journey-state persistence, and playthrough lifecycle behavior.
