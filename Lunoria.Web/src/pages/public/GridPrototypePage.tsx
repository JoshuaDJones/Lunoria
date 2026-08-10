import {
  useCallback,
  useEffect,
  useRef,
  useState,
  type ChangeEvent,
  type FormEvent,
} from "react";
import imageCompression from "browser-image-compression";
import { HubConnectionState, type HubConnection } from "@microsoft/signalr";
import { Button } from "@/components/ui/Button";
import { CharacterPrototypeDrawer } from "@/features/gridPrototype/CharacterPrototypeDrawer";
import { PrototypeGridBoard } from "@/features/gridPrototype/PrototypeGridBoard";
import { listGridPrototypeCharacters } from "@/features/gridPrototype/gridPrototypeApi";
import { createGridPrototypeConnection } from "@/features/gridPrototype/gridPrototypeConnection";
import type {
  CreateGridPrototypeSessionResult,
  GridPrototypeCharacter,
  GridPrototypeSession,
} from "@/features/gridPrototype/types";
import type { SceneGridConfiguration } from "@/features/scenes";

const hostTokenKey = (code: string) => `grid-prototype-host:${code}`;

function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : "The grid request failed.";
}

function readAsDataUrl(file: Blob): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(String(reader.result));
    reader.onerror = () => reject(reader.error ?? new Error("Could not read image."));
    reader.readAsDataURL(file);
  });
}

interface GridPrototypePageProps {
  initialGrid?: SceneGridConfiguration;
}

export function GridPrototypePage({ initialGrid }: GridPrototypePageProps = {}) {
  const connectionRef = useRef<HubConnection | null>(null);
  const activeCodeRef = useRef<string | null>(null);
  const backgroundInputRef = useRef<HTMLInputElement>(null);

  const [connectionState, setConnectionState] = useState("Connecting…");
  const [session, setSession] = useState<GridPrototypeSession | null>(null);
  const [hostToken, setHostToken] = useState<string | null>(null);
  const [joinCode, setJoinCode] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [characters, setCharacters] = useState<GridPrototypeCharacter[]>([]);
  const [charactersLoading, setCharactersLoading] = useState(false);
  const [selectedTokenId, setSelectedTokenId] = useState<string | null>(null);
  const [backgroundLoading, setBackgroundLoading] = useState(false);

  const applySession = useCallback((nextSession: GridPrototypeSession) => {
    activeCodeRef.current = nextSession.code;
    setSession(nextSession);
    setSelectedTokenId((current) =>
      current && nextSession.tokens.some((token) => token.id === current)
        ? current
        : null,
    );
  }, []);

  useEffect(() => {
    let active = true;
    const connection = createGridPrototypeConnection();
    connectionRef.current = connection;

    connection.on("BoardUpdated", applySession);
    connection.on("SessionClosed", () => {
      activeCodeRef.current = null;
      setSession(null);
      setHostToken(null);
      setError("The host closed this grid session.");
    });
    connection.onreconnecting(() => setConnectionState("Reconnecting…"));
    connection.onreconnected(async () => {
      setConnectionState("Connected");
      const code = activeCodeRef.current;
      if (!code) return;

      try {
        const snapshot = await connection.invoke<GridPrototypeSession>(
          "JoinSession",
          code,
        );
        applySession(snapshot);
      } catch (rejoinError) {
        setError(errorMessage(rejoinError));
      }
    });
    connection.onclose(() => setConnectionState("Disconnected"));

    void connection
      .start()
      .then(async () => {
        if (!active) return;
        setConnectionState("Connected");

        if (initialGrid) {
          const result = await connection.invoke<CreateGridPrototypeSessionResult>(
            "CreateConfiguredSession",
            initialGrid.rows,
            initialGrid.columns,
            initialGrid.gridColor,
            initialGrid.backgroundImageUrl,
          );
          if (!active) return;
          sessionStorage.setItem(
            hostTokenKey(result.session.code),
            result.hostToken,
          );
          setHostToken(result.hostToken);
          applySession(result.session);
        }
      })
      .catch((startError: unknown) => {
        if (!active) return;
        setConnectionState("Disconnected");
        setError(errorMessage(startError));
      });

    return () => {
      active = false;
      connection.off("BoardUpdated");
      connection.off("SessionClosed");
      void connection.stop();
      connectionRef.current = null;
    };
  }, [applySession, initialGrid]);

  const connected = connectionState === "Connected";

  const invoke = async <T,>(method: string, ...args: unknown[]): Promise<T> => {
    const connection = connectionRef.current;
    if (!connection || connection.state !== HubConnectionState.Connected) {
      throw new Error("The live grid is not connected yet.");
    }
    return connection.invoke<T>(method, ...args);
  };

  const handleCreate = async () => {
    setError(null);
    try {
      const result = await invoke<CreateGridPrototypeSessionResult>(
        "CreateSession",
      );
      sessionStorage.setItem(hostTokenKey(result.session.code), result.hostToken);
      setHostToken(result.hostToken);
      applySession(result.session);
    } catch (createError) {
      setError(errorMessage(createError));
    }
  };

  const handleJoin = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      const snapshot = await invoke<GridPrototypeSession>(
        "JoinSession",
        joinCode.trim().toUpperCase(),
      );
      setHostToken(sessionStorage.getItem(hostTokenKey(snapshot.code)));
      applySession(snapshot);
    } catch (joinError) {
      setError(errorMessage(joinError));
    }
  };

  const handleOpenCharacters = async () => {
    setDrawerOpen(true);
    if (characters.length > 0) return;

    setCharactersLoading(true);
    try {
      setCharacters(await listGridPrototypeCharacters());
    } catch (loadError) {
      setError(errorMessage(loadError));
    } finally {
      setCharactersLoading(false);
    }
  };

  const handleAddCharacter = async (character: GridPrototypeCharacter) => {
    if (!session || !hostToken) return;
    try {
      await invoke("AddToken", session.code, hostToken, character);
    } catch (addError) {
      setError(errorMessage(addError));
    }
  };

  const handleRemoveSelected = async () => {
    if (!session || !hostToken || !selectedTokenId) return;
    try {
      await invoke("RemoveToken", session.code, hostToken, selectedTokenId);
    } catch (removeError) {
      setError(errorMessage(removeError));
    }
  };

  const handleBackground = async (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    event.target.value = "";
    if (!file || !session || !hostToken) return;

    setBackgroundLoading(true);
    setError(null);
    try {
      const compressed = await imageCompression(file, {
        maxSizeMB: 1.5,
        maxWidthOrHeight: 2400,
        useWebWorker: true,
      });
      const dataUrl = await readAsDataUrl(compressed);
      await invoke("SetBackground", session.code, hostToken, dataUrl);
    } catch (backgroundError) {
      setError(errorMessage(backgroundError));
    } finally {
      setBackgroundLoading(false);
    }
  };

  const handleGridColor = async (gridColor: string) => {
    if (!session || !hostToken) return;
    try {
      await invoke("SetGridColor", session.code, hostToken, gridColor);
    } catch (colorError) {
      setError(errorMessage(colorError));
    }
  };

  const handleCloseSession = async () => {
    if (!session || !hostToken) return;
    try {
      await invoke("CloseSession", session.code, hostToken);
      sessionStorage.removeItem(hostTokenKey(session.code));
    } catch (closeError) {
      setError(errorMessage(closeError));
    }
  };

  const handleBeginMove = async (tokenId: string): Promise<boolean> => {
    if (!session) return false;
    try {
      const allowed = await invoke<boolean>("BeginMove", session.code, tokenId);
      if (!allowed) setError("Another participant is moving that token.");
      return allowed;
    } catch (moveError) {
      setError(errorMessage(moveError));
      return false;
    }
  };

  const handleMoveToken = async (
    tokenId: string,
    row: number,
    column: number,
  ) => {
    if (!session) return;
    try {
      await invoke("MoveToken", session.code, tokenId, row, column);
    } catch (moveError) {
      setError(errorMessage(moveError));
    }
  };

  const handleEndMove = async (tokenId: string) => {
    if (!session) return;
    try {
      await invoke("EndMove", session.code, tokenId);
    } catch {
      // The short server-side lock also expires automatically.
    }
  };

  if (!session) {
    if (initialGrid) {
      return (
        <main className="flex min-h-screen items-center justify-center bg-canvas text-content-muted">
          {error ?? "Opening live grid…"}
        </main>
      );
    }

    return (
      <main className="stone-image flex min-h-screen items-center justify-center p-6">
        <section className="w-full max-w-lg rounded-2xl border border-border bg-surface/95 p-8 shadow-2xl backdrop-blur-sm">
          <p className="text-xs font-semibold tracking-[0.25em] text-brand-hover uppercase">
            Lunoria prototype
          </p>
          <h1 className="mt-2 text-3xl font-bold text-content">Live Grid</h1>
          <p className="mt-3 text-sm leading-6 text-content-muted">
            Create a temporary board or enter a code to join one. Anyone with
            the code can move pieces.
          </p>

          <div className="mt-8 grid gap-6">
            <Button
              variant="add"
              size="lg"
              disabled={!connected}
              onClick={() => void handleCreate()}
            >
              Create a board
            </Button>

            <div className="flex items-center gap-3 text-xs text-content-muted">
              <span className="h-px flex-1 bg-border" /> OR
              <span className="h-px flex-1 bg-border" />
            </div>

            <form className="grid gap-3" onSubmit={(event) => void handleJoin(event)}>
              <label htmlFor="grid-code" className="text-sm text-content-secondary">
                Session code
              </label>
              <input
                id="grid-code"
                value={joinCode}
                onChange={(event) =>
                  setJoinCode(event.target.value.toUpperCase().slice(0, 8))
                }
                className="rounded-lg border border-border bg-surface-raised px-4 py-3 text-center text-xl tracking-[0.3em] text-content uppercase outline-none focus:border-brand-hover"
                placeholder="ABCD2345"
                autoComplete="off"
              />
              <Button
                type="submit"
                variant="primary"
                size="lg"
                disabled={!connected || joinCode.trim().length !== 8}
              >
                Join board
              </Button>
            </form>
          </div>

          <p className="mt-6 text-center text-xs text-content-muted">
            SignalR: {connectionState}
          </p>
          {error && (
            <p className="mt-4 rounded-lg border border-danger/40 bg-danger/10 p-3 text-sm text-danger">
              {error}
            </p>
          )}
        </section>
      </main>
    );
  }

  const isHost = hostToken !== null;

  return (
    <main
      className={
        initialGrid
          ? "relative flex h-screen w-screen overflow-hidden bg-canvas"
          : "flex min-h-screen flex-col bg-canvas p-3 lg:p-5"
      }
    >
      <header
        className={`flex flex-wrap items-center gap-3 rounded-xl border border-border bg-surface-raised px-4 py-3 ${
          initialGrid
            ? "absolute top-3 right-3 left-3 z-30 shadow-2xl"
            : "mb-4"
        }`}
      >
        <div className="mr-auto">
          <p className="text-xs text-content-muted">Session code</p>
          <button
            type="button"
            className="font-mono text-xl font-bold tracking-[0.22em] text-brand-hover"
            title="Copy session code"
            onClick={() => void navigator.clipboard.writeText(session.code)}
          >
            {session.code}
          </button>
        </div>

        {isHost && (
          <>
            <Button variant="add" onClick={() => void handleOpenCharacters()}>
              Add characters
            </Button>
            <Button
              variant="utility"
              disabled={backgroundLoading}
              onClick={() => backgroundInputRef.current?.click()}
            >
              {backgroundLoading ? "Preparing background…" : "Add background"}
            </Button>
            <input
              ref={backgroundInputRef}
              type="file"
              accept="image/*"
              className="hidden"
              onChange={(event) => void handleBackground(event)}
            />
            {session.backgroundImage && (
              <Button
                size="sm"
                onClick={() =>
                  void invoke("SetBackground", session.code, hostToken, null)
                }
              >
                Clear background
              </Button>
            )}
            <label className="flex items-center gap-2 rounded-lg border border-border px-3 py-2 text-xs text-content-secondary">
              Grid color
              <input
                type="color"
                value={session.gridColor}
                className="h-7 w-10 cursor-pointer border-0 bg-transparent"
                onChange={(event) => void handleGridColor(event.target.value)}
              />
            </label>
            <Button
              variant="danger"
              disabled={!selectedTokenId}
              onClick={() => void handleRemoveSelected()}
            >
              Remove selected
            </Button>
            <Button variant="danger" size="sm" onClick={() => void handleCloseSession()}>
              Close session
            </Button>
          </>
        )}

        {!isHost && (
          <p className="text-xs text-content-muted">
            Drag any piece. Changes are shared live.
          </p>
        )}
      </header>

      {error && (
        <button
          type="button"
          className="mb-3 rounded-lg border border-danger/40 bg-danger/10 px-4 py-2 text-left text-sm text-danger"
          onClick={() => setError(null)}
        >
          {error} <span className="float-right">Dismiss</span>
        </button>
      )}

      <section className="flex min-h-0 flex-1 items-stretch justify-center">
        <div className={initialGrid ? "h-full w-full" : "w-full max-w-[1800px]"}>
          <PrototypeGridBoard
            session={session}
            selectedTokenId={selectedTokenId}
            onSelectToken={setSelectedTokenId}
            onBeginMove={handleBeginMove}
            onMoveToken={handleMoveToken}
            onEndMove={handleEndMove}
            fillViewport={Boolean(initialGrid)}
          />
          {!initialGrid && (
            <div className="mt-2 flex justify-between text-xs text-content-muted">
              <span>{session.rows} rows × {session.columns} columns</span>
              <span>{session.tokens.length} pieces · {connectionState}</span>
            </div>
          )}
        </div>
      </section>

      {drawerOpen && (
        <CharacterPrototypeDrawer
          characters={characters}
          loading={charactersLoading}
          onAdd={(character) => void handleAddCharacter(character)}
          onClose={() => setDrawerOpen(false)}
        />
      )}
    </main>
  );
}
