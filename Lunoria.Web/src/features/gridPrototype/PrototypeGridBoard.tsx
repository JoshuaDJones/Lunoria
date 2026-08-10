import {
  useRef,
  useState,
  type CSSProperties,
  type PointerEvent as ReactPointerEvent,
} from "react";
import type {
  GridPrototypeSession,
  GridPrototypeToken,
} from "@/features/gridPrototype/types";

interface DragState {
  tokenId: string;
  pointerId: number;
  row: number;
  column: number;
}

interface PrototypeGridBoardProps {
  session: GridPrototypeSession;
  selectedTokenId: string | null;
  onSelectToken: (tokenId: string) => void;
  onBeginMove: (tokenId: string) => Promise<boolean>;
  onMoveToken: (tokenId: string, row: number, column: number) => Promise<void>;
  onEndMove: (tokenId: string) => Promise<void>;
  fillViewport?: boolean;
}

export function PrototypeGridBoard({
  session,
  selectedTokenId,
  onSelectToken,
  onBeginMove,
  onMoveToken,
  onEndMove,
  fillViewport = false,
}: PrototypeGridBoardProps) {
  const boardRef = useRef<HTMLDivElement>(null);
  const [drag, setDrag] = useState<DragState | null>(null);

  const getCell = (clientX: number, clientY: number) => {
    const bounds = boardRef.current?.getBoundingClientRect();
    if (!bounds) return null;

    return {
      column: Math.max(
        0,
        Math.min(
          session.columns - 1,
          Math.floor(((clientX - bounds.left) / bounds.width) * session.columns),
        ),
      ),
      row: Math.max(
        0,
        Math.min(
          session.rows - 1,
          Math.floor(((clientY - bounds.top) / bounds.height) * session.rows),
        ),
      ),
    };
  };

  const handlePointerDown = async (
    event: ReactPointerEvent<HTMLButtonElement>,
    token: GridPrototypeToken,
  ) => {
    if (event.button !== 0) return;
    event.preventDefault();
    onSelectToken(token.id);
    const tokenElement = event.currentTarget;

    const allowed = await onBeginMove(token.id);
    if (!allowed) return;

    tokenElement.setPointerCapture(event.pointerId);
    const cell = getCell(event.clientX, event.clientY);
    setDrag({
      tokenId: token.id,
      pointerId: event.pointerId,
      row: cell?.row ?? token.row,
      column: cell?.column ?? token.column,
    });
  };

  const handlePointerMove = (event: ReactPointerEvent<HTMLDivElement>) => {
    if (!drag || drag.pointerId !== event.pointerId) return;
    const cell = getCell(event.clientX, event.clientY);
    if (!cell) return;
    setDrag((current) => (current ? { ...current, ...cell } : null));
  };

  const finishDrag = async (
    event: ReactPointerEvent<HTMLDivElement>,
    commit: boolean,
  ) => {
    if (!drag || drag.pointerId !== event.pointerId) return;
    const completed = drag;

    try {
      if (commit) {
        await onMoveToken(completed.tokenId, completed.row, completed.column);
      }
    } finally {
      setDrag((current) =>
        current?.tokenId === completed.tokenId ? null : current,
      );
      await onEndMove(completed.tokenId);
    }
  };

  const gridStyle: CSSProperties = {
    backgroundImage: `linear-gradient(to right, ${session.gridColor} 1px, transparent 1px), linear-gradient(to bottom, ${session.gridColor} 1px, transparent 1px)`,
    backgroundSize: `${100 / session.columns}% ${100 / session.rows}%`,
  };

  return (
    <div
      ref={boardRef}
      className={`relative w-full touch-none overflow-hidden bg-black shadow-2xl select-none ${
        fillViewport
          ? "h-full border-0"
          : "aspect-[36/20] rounded-xl border border-border"
      }`}
      onPointerMove={handlePointerMove}
      onPointerUp={(event) => void finishDrag(event, true)}
      onPointerCancel={(event) => void finishDrag(event, false)}
    >
      {session.backgroundImage && (
        <img
          src={session.backgroundImage}
          alt="Board background"
          className="pointer-events-none absolute inset-0 size-full object-cover"
          draggable={false}
        />
      )}
      <div className="pointer-events-none absolute inset-0" style={gridStyle} />

      {session.tokens.length === 0 && (
        <div className="pointer-events-none absolute inset-0 flex items-center justify-center">
          <p className="rounded-lg bg-canvas/75 px-4 py-2 text-sm text-content-muted backdrop-blur-sm">
            The host can add characters from the drawer.
          </p>
        </div>
      )}

      {session.tokens.map((token) => {
        const position = drag?.tokenId === token.id ? drag : token;
        const tokenStyle: CSSProperties = {
          left: `${(position.column / session.columns) * 100}%`,
          top: `${(position.row / session.rows) * 100}%`,
          width: `${100 / session.columns}%`,
          height: `${100 / session.rows}%`,
        };

        return (
          <button
            key={token.id}
            type="button"
            title={token.name}
            aria-label={`Move ${token.name}`}
            className={`absolute z-10 flex cursor-grab items-center justify-center p-[0.08%] active:cursor-grabbing ${
              selectedTokenId === token.id
                ? "rounded-sm ring-2 ring-utility ring-offset-1 ring-offset-black"
                : ""
            }`}
            style={tokenStyle}
            onPointerDown={(event) => void handlePointerDown(event, token)}
          >
            {token.imageUrl ? (
              <img
                src={token.imageUrl}
                alt=""
                className="pointer-events-none size-full object-contain drop-shadow-[0_2px_2px_rgba(0,0,0,0.85)]"
                draggable={false}
              />
            ) : (
              <span className="pointer-events-none size-3/4 rounded-full bg-utility" />
            )}
          </button>
        );
      })}
    </div>
  );
}
