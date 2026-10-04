import { useEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { Button } from "@/components/ui";
import { getCharacter } from "@/features/characters/api/charactersApi";
import {
  applyCharacterSync,
  getCharacterSyncPreview,
} from "@/features/characterSync/api";
import { CharacterSyncDetails } from "@/features/characterSync/components/CharacterSyncDetails";
import type {
  CharacterSyncKind,
  CharacterSyncPreview,
  CharacterSyncSelection,
} from "@/features/characterSync/types";
import { getApiError } from "@/lib/apiClient";

const emptySelection: CharacterSyncSelection = {
  stats: false,
  spellAssignments: false,
  alternateForm: false,
  sharedSpellUpdates: false,
};

export function CharacterSyncDialog({
  kind,
  assignmentId,
  name,
  onClose,
  onUpdated,
}: {
  kind: CharacterSyncKind;
  assignmentId: number;
  name: string;
  onClose: () => void;
  onUpdated: () => Promise<void>;
}) {
  const dialog = useRef<HTMLDialogElement>(null);
  const working = useRef(false);
  const [preview, setPreview] = useState<CharacterSyncPreview>();
  const [alternateNames, setAlternateNames] = useState<Record<number, string>>(
    {},
  );
  const [selection, setSelection] = useState(emptySelection);
  const [loading, setLoading] = useState(true);
  const [reloadKey, setReloadKey] = useState(0);
  const [busy, setBusy] = useState<"apply" | "keep" | "refresh" | null>(null);
  const [error, setError] = useState("");
  const [stale, setStale] = useState(false);
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    const element = dialog.current;
    element?.showModal();
    return () => element?.close();
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    void getCharacterSyncPreview(kind, assignmentId, controller.signal)
      .then(async (result) => {
        const ids = [
          ...new Set(
            [result.assignedAlternateFormId, result.baseAlternateFormId].filter(
              (id): id is number => id !== null,
            ),
          ),
        ];
        const characters = await Promise.all(
          ids.map((id) => getCharacter(id).catch(() => null)),
        );
        if (controller.signal.aborted) return;
        setAlternateNames(
          Object.fromEntries(
            characters
              .filter((item) => item !== null)
              .map((item) => [item.id, item.name]),
          ),
        );
        setPreview(result);
        setSelection(emptySelection);
        setStale(false);
      })
      .catch((reason) => {
        if (!controller.signal.aborted) setError(getApiError(reason).message);
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, [kind, assignmentId, reloadKey]);

  const reload = () => {
    setLoading(true);
    setError("");
    setPreview(undefined);
    setSelection(emptySelection);
    setReloadKey((value) => value + 1);
  };
  const close = () => {
    if (!working.current) onClose();
  };
  const refreshAfterSave = async () => {
    try {
      await onUpdated();
      onClose();
    } catch (reason) {
      setError(
        `Changes were saved, but the character list could not refresh. ${getApiError(reason).message}`,
      );
    }
  };
  const submit = async (acknowledgeOnly: boolean) => {
    if (!preview || working.current || stale || saved) return;
    working.current = true;
    setBusy(acknowledgeOnly ? "keep" : "apply");
    setError("");
    try {
      const result = await applyCharacterSync(
        kind,
        assignmentId,
        { ...selection, expectedReviewToken: preview.reviewToken },
        acknowledgeOnly,
      );
      setPreview(result);
      setSaved(true);
      await refreshAfterSave();
    } catch (reason) {
      const failure = getApiError(reason);
      setStale(
        failure.code === "CharacterSync.Conflict" ||
          failure.code === "Http.409",
      );
      setError(failure.message);
    } finally {
      working.current = false;
      setBusy(null);
    }
  };
  const retryRefresh = async () => {
    if (working.current) return;
    working.current = true;
    setBusy("refresh");
    try {
      await refreshAfterSave();
    } finally {
      working.current = false;
      setBusy(null);
    }
  };
  const replacements =
    selection.stats || selection.spellAssignments || selection.alternateForm;
  const hasSelection = replacements || selection.sharedSpellUpdates;
  const blocked =
    busy !== null || loading || stale || saved || !preview || !hasSelection;
  const spinner = <FontAwesomeIcon icon={faSpinner} spin aria-hidden="true" />;

  return createPortal(
    <dialog
      ref={dialog}
      data-nested-dialog="true"
      aria-labelledby="character-sync-title"
      onCancel={(event) => {
        event.preventDefault();
        close();
      }}
      onKeyDown={(event) => {
        if (event.key === "Escape") event.stopPropagation();
      }}
      onClick={(event) => {
        if (event.target === event.currentTarget) close();
      }}
      className="fixed inset-0 m-auto max-h-[90dvh] w-[calc(100%_-_2rem)] max-w-3xl overflow-hidden rounded-2xl border border-border bg-surface-raised p-0 text-content shadow-2xl backdrop:bg-black/70"
    >
      <div className="flex max-h-[90dvh] flex-col" aria-busy={busy !== null}>
        <header className="shrink-0 border-b border-border p-5">
          <h2 id="character-sync-title" className="text-xl font-semibold">
            Review updates — {name}
          </h2>
          <p className="mt-2 text-sm text-content-secondary">
            Choose what to review or replace. Existing playthroughs won’t be
            affected.
          </p>
        </header>
        <div className="min-h-0 flex-1 overflow-y-auto p-5">
          {loading && (
            <p
              role="status"
              className="flex items-center gap-2 text-content-secondary"
            >
              {spinner} Loading comparison…
            </p>
          )}
          {error && (
            <div
              role="alert"
              className="mb-4 rounded-lg border border-danger/40 p-3 text-sm text-danger"
            >
              <p>{error}</p>
              {stale && (
                <p className="mt-2">
                  Reload the latest changes and select the categories again
                  before confirming.
                </p>
              )}
              {(!busy || busy === "refresh") && (
                <Button
                  className="mt-3"
                  disabled={busy !== null}
                  leftIcon={busy === "refresh" ? spinner : undefined}
                  onClick={saved ? () => void retryRefresh() : reload}
                >
                  {saved ? "Retry refresh" : "Reload review"}
                </Button>
              )}
            </div>
          )}
          {preview && !loading && (
            <fieldset
              disabled={busy !== null || stale || saved}
              className="min-w-0 disabled:opacity-60"
            >
              <legend className="sr-only">
                Categories to review or synchronize
              </legend>
              <CharacterSyncDetails
                preview={preview}
                selection={selection}
                alternateNames={alternateNames}
                onChange={(key, checked) =>
                  setSelection((current) => ({ ...current, [key]: checked }))
                }
              />
            </fieldset>
          )}
        </div>
        <footer className="shrink-0 border-t border-border p-5">
          <p className="mb-3 text-xs text-content-muted">
            Keep current dismisses selected warnings without replacing your
            customizations. Apply replaces only selected categories. Shared
            spell changes can only be marked reviewed.
          </p>
          <div className="flex flex-wrap justify-end gap-3">
            <Button onClick={close} disabled={busy !== null}>
              Close
            </Button>
            {replacements && (
              <Button
                onClick={() => void submit(true)}
                disabled={blocked}
                leftIcon={busy === "keep" ? spinner : undefined}
              >
                Keep current
              </Button>
            )}
            <Button
              variant="primary"
              onClick={() => void submit(!replacements)}
              disabled={blocked}
              leftIcon={
                busy === "apply" || (busy === "keep" && !replacements)
                  ? spinner
                  : undefined
              }
            >
              {replacements ? "Apply selected updates" : "Mark reviewed"}
            </Button>
          </div>
        </footer>
      </div>
    </dialog>,
    document.body,
  );
}
