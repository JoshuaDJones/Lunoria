# Real-Time Player Sessions

## Objective

Allow players at a physical game table to join a running Lunoria journey from their phones and see the current scene, participants, HP, MP, and turn order update in real time.

The game master remains the authority that changes gameplay state. Player access should initially be read-only and scoped to one active playthrough.

## Existing Architecture

### Server

- `Eldoria.Api` is an ASP.NET Core 8 API.
- Authentication uses JWT bearer tokens.
- A fallback authorization policy currently requires authentication for every endpoint unless explicitly overridden.
- Application services enforce resource ownership using the authenticated user's ID.
- Relevant domain entities already exist:
  - `JourneyPlaythrough` represents a running instance of a journey.
  - `SceneProgress` represents a scene within a playthrough.
  - `SceneParticipant` references a journey character or scene character.
  - `SceneParticipantTurn` represents turn ordering.
  - `JourneyCharacter` and `SceneCharacter` contain mutable gameplay stats.
- Relevant REST operations already exist for scene progress, participants, turn order, and HP/MP changes.
- There is currently no SignalR hub or other real-time transport.

### Client

- `Lunoria.Web` is a React 19 and TypeScript application built with Vite.
- REST access is centralized in `src/lib/apiClient.ts`.
- The normal authenticated client stores its JWT under `auth_token`.
- Scene API functions and types already cover scene progress, participants, and turns.
- The router currently has only the home and component showcase routes.
- `@microsoft/signalr` is not currently installed.

## Recommended Design

Use REST to load authoritative session snapshots and ASP.NET Core SignalR to notify connected devices that state has changed.

The high-level flow is:

1. The game master starts or selects an active `JourneyPlaythrough`.
2. The game master creates a player join session.
3. The API returns a short join code and URL suitable for a QR code.
4. A player opens `/join/{code}` on a phone.
5. The phone exchanges the code for a short-lived, playthrough-scoped player token.
6. The phone fetches an initial session snapshot over REST.
7. The phone connects to SignalR and joins `playthrough:{playthroughId}`.
8. Gameplay mutations commit to the database and then publish a state-change event.
9. Connected phones refetch the authoritative snapshot.
10. SignalR automatically reconnects after temporary network loss, after which the client refetches again.

SignalR should be a notification channel, not the sole store of state. This prevents missed events, reconnects, or multi-server timing from leaving a device with incorrect stats.

## Security Model

Do not share the game master's JWT with player devices.

Introduce a separate player-session credential with these properties:

- Scoped to one `JourneyPlaythrough`.
- Read-only initially.
- Short-lived, for example 8 to 12 hours.
- Revocable when the game master closes or rotates the session.
- Contains a random identifier or database-backed session ID.
- Does not contain the join code itself.
- Does not satisfy game-master ownership checks.

The join code must:

- Be generated using a cryptographically secure random source.
- Be difficult to guess; use at least 8 unambiguous characters.
- Be case-insensitive for manual entry.
- Be stored as a hash, not plaintext, if persisted.
- Have an expiration time.
- Support rotation and explicit closure.
- Be rate-limited at the join endpoint.

Player-facing snapshot queries must use dedicated authorization logic based on the playthrough claim. They must not reuse an owner-only service by impersonating the game master.

Only expose player-visible information. Avoid returning hidden NPC statistics, private dialog, encounter notes, unrevealed enemies, or game-master controls. A dedicated DTO is safer than returning existing administrative DTOs.

## Proposed Domain Model

Add a persistent entity similar to:

```csharp
public sealed class PlayerJoinSession
{
    public int Id { get; set; }
    public int JourneyPlaythroughId { get; set; }
    public JourneyPlaythrough JourneyPlaythrough { get; set; } = null!;

    public string JoinCodeHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}
```

Optional later additions:

- Player display name.
- A claimed `JourneyCharacterId`.
- Per-player visibility or permissions.
- Device/session records for selective revocation.

Create the corresponding EF Core configuration, repository access, and migration.

## Proposed HTTP API

Exact route naming may be adjusted to match existing conventions.

### Game-master endpoints

```text
POST   /api/v1/playthroughs/{playthroughId}/player-session
DELETE /api/v1/playthroughs/{playthroughId}/player-session
GET    /api/v1/playthroughs/{playthroughId}/player-session
```

The create response should include:

```json
{
  "joinCode": "DRAGON7X",
  "joinUrl": "https://lunoria.example/join/DRAGON7X",
  "expiresAt": "2026-07-05T03:00:00Z"
}
```

Only the playthrough owner can create, inspect, rotate, or close a player session.

### Player endpoints

```text
POST /api/v1/player-sessions/join
GET  /api/v1/player-sessions/current
```

Join request:

```json
{
  "code": "DRAGON7X",
  "displayName": "Jamie"
}
```

Join response:

```json
{
  "accessToken": "<short-lived-player-token>",
  "expiresAt": "2026-07-05T03:00:00Z",
  "playthroughId": 12
}
```

`GET /current` returns the player-visible snapshot and requires a valid player token.

## Player Snapshot Contract

Create dedicated DTOs rather than exposing persistence entities:

```ts
interface PlayerSessionSnapshot {
  playthroughId: number;
  journey: {
    id: number;
    name: string;
  };
  activeScene: {
    sceneProgressId: number;
    sceneId: number;
    name: string;
    status: string;
  } | null;
  participants: PlayerVisibleParticipant[];
  turnOrder: PlayerVisibleTurn[];
  revision: number;
  generatedAt: string;
}

interface PlayerVisibleParticipant {
  participantId: number;
  characterId: number;
  participantType: "player" | "npc";
  name: string;
  imageUrl?: string | null;
  hp: number;
  maxHp: number;
  mp: number;
  maxMp: number;
  isDefeated: boolean;
  isCurrentTurn: boolean;
}
```

Only include participants that should currently be visible. If hidden/revealed state is not represented in the domain, add it before exposing all scene characters.

The `revision` can initially be a timestamp or monotonically increasing state version. It helps clients ignore stale responses and can support conditional fetching later.

## SignalR Server

Add a hub such as:

```csharp
[Authorize(Policy = "PlayerOrGameMaster")]
public sealed class GameSessionHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var playthroughId = GetAuthorizedPlaythroughId(Context.User);
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            $"playthrough:{playthroughId}");

        await base.OnConnectedAsync();
    }
}
```

Register and map it in `Eldoria.Api/Program.cs`:

```csharp
builder.Services.AddSignalR();

// After authentication and authorization middleware:
app.MapHub<GameSessionHub>("/hubs/game-session");
```

Configure JWT bearer authentication to accept SignalR's `access_token` query parameter only for the hub route. The JavaScript SignalR client uses this mechanism for WebSockets and server-sent events.

The server must determine the playthrough group from validated claims. Never accept an arbitrary playthrough ID from the client and join that group without authorization.

### Event contract

Start with one coarse-grained event:

```ts
interface GameStateChanged {
  playthroughId: number;
  revision: number;
  reason:
    | "stats"
    | "participants"
    | "turnOrder"
    | "scene"
    | "sessionClosed";
}
```

SignalR method name:

```text
GameStateChanged
```

On receipt, the client refetches `GET /api/v1/player-sessions/current`. Debounce closely grouped events to avoid duplicate requests.

## Publishing Changes

Create an abstraction in the application layer so application services do not depend directly on SignalR:

```csharp
public interface IGameSessionNotifier
{
    Task StateChangedAsync(
        int playthroughId,
        GameStateChangeReason reason,
        CancellationToken cancellationToken);
}
```

Implement it in the API or infrastructure boundary using `IHubContext<GameSessionHub>`.

Publish only after the database operation succeeds. Relevant mutations include:

- Journey-character HP/MP changes.
- Scene-character HP/MP changes.
- Adding or removing participants.
- Adding, removing, or reordering turns.
- Changing scene progress status.
- Starting, completing, or switching active scenes.
- Closing a player join session.

The existing scene-character adjustment service may not directly know the playthrough because reusable scene characters can exist outside a live scene. Resolve affected active playthrough IDs through the participant/scene-progress relationship, or move notification orchestration into a command handler that has this context.

Avoid emitting events before `SaveChangesAsync` completes. If stronger delivery guarantees become necessary later, introduce an outbox table. An outbox is unnecessary for the first local/single-server version.

## SignalR Client

Install:

```powershell
npm install @microsoft/signalr
```

Create a focused module such as:

```text
Lunoria.Web/src/features/player-session/
  api/playerSessionApi.ts
  hooks/usePlayerSession.ts
  realtime/gameSessionConnection.ts
  pages/JoinGamePage.tsx
  pages/PlayerSessionPage.tsx
  types.ts
```

Build the connection with:

```ts
new HubConnectionBuilder()
  .withUrl(`${hubBaseUrl}/hubs/game-session`, {
    accessTokenFactory: () => playerToken,
  })
  .withAutomaticReconnect()
  .build();
```

Client behavior:

- Fetch a snapshot before or immediately after connecting.
- Subscribe to handlers before calling `start()`.
- Refetch after every `GameStateChanged` event.
- Refetch after reconnection.
- Display connecting, connected, reconnecting, and disconnected status.
- Debounce bursts of refresh notifications.
- Stop and dispose the connection on logout, token expiry, or component unmount.
- Do not store a player token under the game-master `auth_token` key.

Use a distinct storage key such as `player_session_token`. Session storage is preferable if players should rejoin for every browser session; local storage is preferable if page refresh and browser reopening should preserve access until expiry.

## Mobile Routes and UI

Add public routes:

```text
/join
/join/:code
/play
```

`/join/:code` should:

- Prepopulate the code from the URL.
- Allow an optional display name.
- Exchange the code for a player token.
- Navigate to `/play`.

`/play` should be designed mobile-first and show:

- Journey name.
- Active scene name and status.
- Connection status.
- Player and visible NPC cards.
- Current and maximum HP/MP.
- Clearly highlighted current turn.
- Compact turn order.
- A message when no scene is active.
- A message when the game master closes the session.

Initial implementation should remain read-only. Player-initiated actions require command authorization, validation, and conflict handling and should be a separate phase.

## CORS and Hosting

Update the API CORS policy for every deployed `Lunoria.Web` origin. SignalR requires the normal HTTP negotiation request as well as the real-time transport.

For local phone testing:

- The phone and development computer must be on the same network.
- Vite must listen on the LAN interface, not only localhost.
- The API must listen on a reachable interface.
- `VITE_API_BASE_URL` and any hub base URL must use the computer's LAN address.
- Development HTTPS certificates are usually not trusted by phones. Use a trusted development tunnel or a deliberately configured HTTP-only local profile rather than training users to bypass certificate warnings.

For Azure deployment:

- A single API instance can use in-process SignalR.
- Multiple API instances require sticky sessions or, preferably, Azure SignalR Service.
- The database remains authoritative regardless of SignalR topology.

## Implementation Phases

### Phase 1: Read-only live stats

1. Add `PlayerJoinSession`, EF configuration, repository/query support, and migration.
2. Add secure join-code generation and lifecycle service.
3. Add player JWT issuance and a player authorization scheme/policy.
4. Add game-master session management endpoints.
5. Add player join and snapshot endpoints.
6. Add a player-visible snapshot query.
7. Add SignalR hub registration, authentication, and group membership.
8. Add the notifier abstraction and SignalR implementation.
9. Publish notifications for HP/MP changes.
10. Add the mobile join and player session pages.
11. Add reconnect and snapshot-refresh behavior.

### Phase 2: Complete scene synchronization

1. Publish participant changes.
2. Publish turn-order changes.
3. Publish active-scene and scene-status changes.
4. Add hidden/revealed participant rules.
5. Add QR code presentation to the game-master UI.
6. Add join-code rotation and session closure UI.

### Phase 3: Optional player identity and actions

1. Allow a player to claim or be assigned a journey character.
2. Restrict private details to the assigned player.
3. Add narrowly scoped commands such as spending MP or using an item.
4. Validate commands server-side against the live playthrough.
5. Add audit history and game-master approval where appropriate.

## Testing Strategy

### Application tests

- Only the journey owner can create or close a join session.
- Expired, closed, malformed, and incorrect codes cannot be joined.
- Join-code comparisons do not expose stored hashes.
- Player tokens can access only their assigned playthrough.
- Player tokens cannot invoke game-master mutation endpoints.
- Snapshots exclude hidden or private fields.
- Snapshots select only the active playthrough and active scene.

### API/integration tests

- Joining returns a valid scoped token.
- Player snapshot access fails without a player token.
- A token for playthrough A cannot access playthrough B.
- SignalR rejects unauthenticated connections.
- An authorized connection joins only its claim-derived group.
- A committed HP/MP mutation produces `GameStateChanged`.
- Closing a session prevents later REST and hub access.

### Client tests

- A valid code enters the player view.
- An invalid or expired code displays an actionable error.
- Initial snapshot renders all visible participants.
- A state-change event causes one debounced refetch.
- Reconnection causes a refetch.
- Token expiry returns the player to the join screen.
- The layout works at narrow phone widths.

### Manual acceptance test

1. Start the API and `Lunoria.Web`.
2. Log in as the game master.
3. Start a playthrough and activate a scene.
4. Generate a join code.
5. Join from two separate phones or private browser windows.
6. Change a participant's HP.
7. Confirm both clients update without manual refresh.
8. Change turn order and active scene.
9. Disable one phone's network temporarily.
10. Restore its network and confirm it reconnects with current state.
11. Close the player session and confirm both clients lose access.

## Completion Criteria

The first release is complete when:

- The game master can create and close a join session.
- A player can join using a short code or QR-derived URL.
- Player access is scoped to one playthrough and does not grant mutation privileges.
- The player sees a mobile-friendly current-scene snapshot.
- HP/MP, participants, turn order, and scene changes appear without manual refresh.
- Temporary connection loss automatically recovers to authoritative current state.
- Hidden and game-master-only information is not exposed.
- Automated tests cover authorization boundaries and core synchronization behavior.

## Important Decisions to Preserve

- Group connections by playthrough, not scene, so scene transitions do not require a new session.
- Keep REST snapshots authoritative; use SignalR as an invalidation/notification mechanism.
- Use a distinct player credential rather than sharing or weakening game-master authentication.
- Build a dedicated player-visible DTO rather than reusing administrative dashboard data.
- Begin read-only. Player mutations are a separate security and gameplay feature.
- Broadcast after successful persistence.
- Do not trust client-supplied group or playthrough identifiers.

