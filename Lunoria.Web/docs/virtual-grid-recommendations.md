# Virtual Grid Recommendations

## Summary

A built-in virtual grid is viable and could become a meaningful Lunoria differentiator. The initial goal should be a grid designed specifically for Lunoria encounters, not a general-purpose replacement for products such as Foundry or Roll20.

The largest risk is uncontrolled scope. Real-time multiplayer, lighting, fog of war, drawing tools, pathfinding, and similar VTT capabilities could become more complex than the rest of the application. The feature should begin as a small proof of concept and expand only after the core play interaction proves valuable.

## Recommended Product Model

Journey creation can eventually offer two broad play styles:

- Narrative or in-person play
- Virtual play with optional built-in grids

A virtual journey should not require a grid for every scene. Dialog, exploration, and narrative scenes may not need one. Each scene should independently choose one of these options:

- No grid
- External grid URL
- Built-in Lunoria grid

This preserves the existing `Scene.GridUrl` workflow while allowing gradual adoption of the built-in experience.

## Proof of Concept

Before changing the journey-creation workflow or introducing a large domain model, build a narrow proof of concept that can:

1. Render a fixed square grid.
2. Display an optional scene background image.
3. Render journey and scene characters as tokens.
4. Drag tokens between grid cells.
5. Save and restore token positions.
6. Support basic zooming and panning.

The proof of concept should use one scene and one playthrough. Its purpose is to validate whether the board interaction is enjoyable and useful before committing to a full editor.

## Recommended MVP

The first production version should include:

- Square grids only
- Configurable rows and columns
- Configurable cell size
- Optional uploaded background image
- Initial token placement during scene authoring
- Journey-character and scene-character tokens
- Dragging tokens between cells during play
- Persisted playthrough-specific token positions
- Basic zoom and pan
- A host-controlled play surface
- Support for scenes without grids
- Continued support for external grid URLs

Hex grids can be added later if actual user demand justifies the additional coordinate and rendering complexity.

## Definition Versus Playthrough State

The authored grid definition must remain separate from the active playthrough state.

### Scene grid definition

The scene editor owns reusable configuration such as:

- Grid type
- Rows and columns
- Cell dimensions
- Background image
- Initial token placements
- Initial token visibility

### Playthrough grid state

Each playthrough owns mutable state such as:

- Current token positions
- Current active or inactive state
- Current token visibility
- Other encounter-specific changes

Starting a playthrough should snapshot the authored scene-grid definition. Moving a token during play must never modify the original scene configuration.

## Possible Domain Model

The exact schema should be designed alongside the Play Hub snapshot architecture, but a useful starting point is:

```text
SceneGridDefinition
├── Id
├── SceneId
├── GridType
├── RowCount
├── ColumnCount
├── CellSize
├── BackgroundImageUrl
└── InitialTokenPlacements

SceneGridTokenPlacement
├── Id
├── SceneGridDefinitionId
├── JourneyCharacterId or SceneCharacterId
├── Row
├── Column
└── IsVisible

ScenePlaythroughGridState
├── Id
├── ScenePlaythroughId
└── TokenStates

ScenePlaythroughTokenState
├── Id
├── ScenePlaythroughGridStateId
├── PlaythroughCharacter reference
├── Row
├── Column
└── IsVisible
```

Token state should reference playthrough snapshots rather than mutable authoring entities whenever possible.

## Features to Defer

The following capabilities should not be part of the first release:

- Real-time multiplayer editing
- Dynamic lighting
- Automated line of sight
- Fog of war
- Freehand drawing
- Complex terrain rules
- Automated pathfinding
- Movement-cost calculations
- Spell templates and area measurement
- Advanced map-building tools
- Asset marketplaces
- General-purpose VTT scripting

These can be reconsidered individually after the basic grid is in use. They should not be treated as an inseparable package.

## Suggested Delivery Phases

### Phase 1: Interaction prototype

- Fixed square grid
- Background image
- Character tokens
- Drag and save token positions

### Phase 2: Scene authoring

- Grid configuration in the scene editor
- Initial token placement
- Optional built-in, external, or absent grid

### Phase 3: Play Hub integration

- Snapshot authored grid state when play begins
- Persist token movement per playthrough
- Host controls and active-scene integration

### Phase 4: Usability improvements

- Better zoom and pan
- Token labels and status indicators
- Mobile and tablet interaction
- Grid resizing and background alignment

### Phase 5: Evidence-based expansion

Add capabilities such as hex grids, fog, measurements, or multiplayer synchronization only in response to validated play needs.

## Decision Criteria

Continue investing after the prototype if:

- Moving tokens materially improves the Lunoria play experience.
- Scene setup remains understandable for nontechnical users.
- Grid authoring does not become mandatory busywork.
- Playthrough state remains isolated from authored journey data.
- Performance is acceptable on typical laptops and tablets.

If the prototype does not meet those criteria, Lunoria can continue supporting external grid URLs without compromising the rest of the product.

## Recommendation

Build a small grid proof of concept after the Play Hub snapshot boundaries are understood. Preserve external-grid support and make built-in grids optional per scene. Keep the initial feature focused on Lunoria encounters and resist expanding it into a full general-purpose VTT until user behavior demonstrates that the additional complexity is worthwhile.
