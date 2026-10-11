import { useEffect, useRef, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGripVertical } from "@fortawesome/free-solid-svg-icons";
import { Button } from "@/components/ui";
import { getApiError } from "@/lib/apiClient";
import {
  getSceneObjectives,
  saveSceneObjectives,
} from "@/features/scenes/objectives";

type DraftSet = { key: number; points: { key: number; text: string }[] };
const fieldClass =
  "w-full rounded-lg border border-border bg-surface p-3 text-content";

export function SceneObjectiveEditor({
  sceneId,
  onBusyChange,
}: {
  sceneId: number;
  onBusyChange: (busy: boolean) => void;
}) {
  const nextKey = useRef(0);
  const [sets, setSets] = useState<DraftSet[]>([]);
  const [draggedKey, setDraggedKey] = useState<number>();
  const [dropKey, setDropKey] = useState<number>();
  const [loading, setLoading] = useState(true);
  const [loadFailed, setLoadFailed] = useState(false);
  const [retry, setRetry] = useState(0);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [saved, setSaved] = useState(false);
  const submitting = useRef(false);

  useEffect(() => {
    let current = true;
    void getSceneObjectives(sceneId)
      .then((data) => {
        if (current)
          setSets(
            data.map((set) => ({
              key: nextKey.current++,
              points: set.points.map((text) => ({
                key: nextKey.current++,
                text,
              })),
            })),
          );
      })
      .catch((e) => {
        if (current) {
          setError(getApiError(e).message);
          setLoadFailed(true);
        }
      })
      .finally(() => {
        if (current) setLoading(false);
      });
    return () => {
      current = false;
    };
  }, [sceneId, retry]);

  function update(next: DraftSet[]) {
    setSets(next);
    setSaved(false);
  }
  function moveSet(index: number, target: number) {
    if (
      busy ||
      submitting.current ||
      index < 0 ||
      target < 0 ||
      target >= sets.length ||
      index === target
    )
      return;
    const next = [...sets];
    const [moved] = next.splice(index, 1);
    next.splice(target, 0, moved);
    update(next);
  }

  if (loading) return <p role="status">Loading objectives…</p>;
  if (loadFailed)
    return (
      <div>
        <p role="alert">{error}</p>
        <Button
          onClick={() => {
            setLoading(true);
            setLoadFailed(false);
            setError("");
            setRetry((v) => v + 1);
          }}
        >
          Retry
        </Button>
      </div>
    );

  return (
    <form
      className="space-y-5"
      onSubmit={async (event) => {
        event.preventDefault();
        if (submitting.current) return;
        submitting.current = true;
        setBusy(true);
        onBusyChange(true);
        setError("");
        setSaved(false);
        try {
          await saveSceneObjectives(
            sceneId,
            sets.map((set) => ({
              points: set.points.map((p) => p.text.trim()),
            })),
          );
          setSaved(true);
        } catch (e) {
          setError(getApiError(e).message);
        } finally {
          submitting.current = false;
          setBusy(false);
          onBusyChange(false);
        }
      }}
    >
      <p className="text-content-secondary">
        Each set is shown separately. During the scene, use the arrows to reveal
        the next set or revisit a previous one. Changes apply to new
        playthroughs.
      </p>
      <fieldset disabled={busy} className="space-y-5">
        {sets.map((set, index) => (
          <section
            key={set.key}
            className={`relative space-y-3 rounded-xl border border-border p-4 ${draggedKey === set.key ? "opacity-50" : ""}`}
            onDragOver={(event) => {
              if (busy || draggedKey === undefined || draggedKey === set.key)
                return;
              event.preventDefault();
              event.dataTransfer.dropEffect = "move";
              setDropKey(set.key);
            }}
            onDragLeave={(event) => {
              if (
                !event.currentTarget.contains(
                  event.relatedTarget as Node | null,
                )
              )
                setDropKey(undefined);
            }}
            onDrop={(event) => {
              if (draggedKey === undefined) return;
              event.preventDefault();
              moveSet(
                sets.findIndex((item) => item.key === draggedKey),
                index,
              );
              setDraggedKey(undefined);
              setDropKey(undefined);
            }}
          >
            {dropKey === set.key &&
              draggedKey !== undefined &&
              draggedKey !== set.key && (
                <div
                  className={`pointer-events-none absolute right-0 left-0 h-1 rounded bg-amber-300 ${sets.findIndex((item) => item.key === draggedKey) < index ? "-bottom-3" : "-top-3"}`}
                />
              )}
            <div className="flex flex-wrap items-center gap-2">
              <button
                type="button"
                draggable={!busy && sets.length > 1}
                disabled={busy || sets.length < 2}
                aria-label={`Drag set ${index + 1} to reorder; use arrow keys to move`}
                className="cursor-grab p-2 text-content-muted active:cursor-grabbing disabled:opacity-40"
                onDragStart={(event) => {
                  event.dataTransfer.effectAllowed = "move";
                  event.dataTransfer.setData("text/plain", String(set.key));
                  const card = event.currentTarget.closest("section");
                  if (card) event.dataTransfer.setDragImage(card, 20, 20);
                  setDraggedKey(set.key);
                }}
                onDragEnd={() => {
                  setDraggedKey(undefined);
                  setDropKey(undefined);
                }}
                onKeyDown={(event) => {
                  if (event.key === "ArrowUp" || event.key === "ArrowDown") {
                    event.preventDefault();
                    moveSet(index, index + (event.key === "ArrowUp" ? -1 : 1));
                  }
                }}
              >
                <FontAwesomeIcon icon={faGripVertical} aria-hidden="true" />
              </button>
              <h3 className="mr-auto text-xl">Set {index + 1}</h3>
              <Button
                type="button"
                size="sm"
                variant="danger"
                onClick={() => update(sets.filter((s) => s.key !== set.key))}
              >
                Remove set
              </Button>
            </div>
            {set.points.map((point, pointIndex) => (
              <div key={point.key} className="space-y-2">
                <label className="block">
                  <span className="text-sm text-content-secondary">
                    Objective {pointIndex + 1}
                  </span>
                  <textarea
                    className={fieldClass}
                    required
                    maxLength={1000}
                    rows={2}
                    value={point.text}
                    onChange={(e) =>
                      update(
                        sets.map((s) =>
                          s.key === set.key
                            ? {
                                ...s,
                                points: s.points.map((p) =>
                                  p.key === point.key
                                    ? { ...p, text: e.target.value }
                                    : p,
                                ),
                              }
                            : s,
                        ),
                      )
                    }
                  />
                </label>
                <div className="flex justify-end gap-2">
                  {[-1, 1].map((direction) => (
                    <Button
                      key={direction}
                      type="button"
                      size="sm"
                      aria-label={`Move objective ${pointIndex + 1} ${direction === -1 ? "up" : "down"}`}
                      disabled={
                        pointIndex + direction < 0 ||
                        pointIndex + direction >= set.points.length
                      }
                      onClick={() => {
                        const points = [...set.points];
                        [points[pointIndex], points[pointIndex + direction]] = [
                          points[pointIndex + direction],
                          points[pointIndex],
                        ];
                        update(
                          sets.map((s) =>
                            s.key === set.key ? { ...s, points } : s,
                          ),
                        );
                      }}
                    >
                      {direction === -1 ? "↑" : "↓"}
                    </Button>
                  ))}
                  <Button
                    type="button"
                    size="sm"
                    disabled={set.points.length === 1}
                    onClick={() =>
                      update(
                        sets.map((s) =>
                          s.key === set.key
                            ? {
                                ...s,
                                points: s.points.filter(
                                  (p) => p.key !== point.key,
                                ),
                              }
                            : s,
                        ),
                      )
                    }
                  >
                    Remove
                  </Button>
                </div>
              </div>
            ))}
            <Button
              type="button"
              size="sm"
              disabled={set.points.length >= 50}
              onClick={() =>
                update(
                  sets.map((s) =>
                    s.key === set.key
                      ? {
                          ...s,
                          points: [
                            ...s.points,
                            { key: nextKey.current++, text: "" },
                          ],
                        }
                      : s,
                  ),
                )
              }
            >
              Add objective
            </Button>
          </section>
        ))}
        {sets.length === 0 && (
          <p className="text-content-muted">This scene has no objectives.</p>
        )}
        <Button
          type="button"
          disabled={sets.length >= 50}
          onClick={() =>
            update([
              ...sets,
              {
                key: nextKey.current++,
                points: [{ key: nextKey.current++, text: "" }],
              },
            ])
          }
        >
          Add objective set
        </Button>
        <Button
          type="submit"
          variant="primary"
          className="ml-3"
          aria-busy={busy}
          leftIcon={
            busy ? (
              <span
                aria-hidden="true"
                className="h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent"
              />
            ) : undefined
          }
        >
          Save objectives
        </Button>
      </fieldset>
      {error && (
        <p role="alert" className="text-danger">
          {error}
        </p>
      )}
      {saved && (
        <p role="status" className="text-content-secondary">
          Objectives saved.
        </p>
      )}
    </form>
  );
}
