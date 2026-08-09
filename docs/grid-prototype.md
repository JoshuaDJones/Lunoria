# Live Grid Prototype

## Purpose

The live grid is an isolated experiment for a shared tabletop board. It does not create or modify journeys, scenes, or playthroughs.

Open the public client route:

```text
/grid-prototype
```

## Behavior

- A host creates a temporary 20-row × 36-column board.
- The server returns an eight-character session code and a private host token.
- The host token is stored in that tab's `sessionStorage`; it is not an application login.
- Anyone with the session code can join anonymously and move pieces.
- Only the host can add duplicate character tokens, remove a selected token, upload/clear a background, change the grid color, or close the session.
- Pieces resize with the board and snap to the center of the nearest grid cell on drop.
- A short server-side lock prevents two connections from dragging the same piece simultaneously.
- Every accepted board mutation broadcasts a complete board snapshot to the SignalR group.
- Automatic reconnection rejoins the session and reloads the current snapshot.

## Server implementation

The prototype lives in `Eldoria.Api/GridPrototype`:

- `GridPrototypeHub` exposes create, join, move, and host-control hub methods.
- `GridPrototypeSessionStore` is a singleton, thread-safe in-memory store.
- `GridPrototypeController` exposes `GET /api/v1/grid-prototype/characters` anonymously for the character drawer.
- `Program.cs` maps `/hubs/grid-prototype`, permits anonymous access to that endpoint, and allows SignalR messages up to 4 MB for compressed background data URLs.

Sessions expire after eight hours of inactivity. They are lost whenever the API process restarts and do not work across multiple API instances.

## Client implementation

The active client implementation lives under:

```text
Lunoria.Web/src/features/gridPrototype
Lunoria.Web/src/pages/public/GridPrototypePage.tsx
```

The client uses the official `@microsoft/signalr` package. `VITE_API_BASE_URL` supplies the API origin; the hub itself is mapped at `/hubs/grid-prototype`, outside the `/api/v1` controller prefix.

Background files are compressed in the browser before being sent as data URLs. This is acceptable for an in-memory prototype but should be replaced with durable object storage before production use.

## Local development

Run the API and active client:

```powershell
dotnet run --project Eldoria.Api/Eldoria.Api.csproj --launch-profile http

cd Lunoria.Web
npm run dev
```

Then visit:

```text
http://localhost:5173/grid-prototype
```

Open the same URL in another browser or tab and enter the generated session code.

## Prototype limitations

- No persistence or replay history.
- No user authentication or participant identity.
- Possession of the code permits movement of every token.
- Host authority depends on a secret token kept in browser session storage.
- The anonymous character endpoint returns characters across all users and must not be treated as a production authorization design.
- Full snapshots and data-URL backgrounds are inefficient for large boards or frequent continuous movement.
- A single in-memory API instance is required; there is no Redis backplane or Azure SignalR Service.
- The code is not connected to immutable journey/playthrough revisions.

Before production use, add authenticated ownership, durable/shared state, asset storage, rate limiting, participant permissions, and a deliberate connection to the playthrough snapshot model.
