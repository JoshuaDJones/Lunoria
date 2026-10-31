import { useEffect, useRef, useState } from "react";
import { getApiError } from "@/lib/apiClient";
import { Button } from "@/components/ui";
import { CharacterType } from "@/features/characters";
import { type ScenePlaythroughCharacterOption } from "@/features/journeys";

const ActionDialogContent = ({
  playthroughCharacters,
  onSkip,
  onActivate,
  onSubmittingChange,
}: {
  playthroughCharacters: ScenePlaythroughCharacterOption[];
  onSkip: () => void;
  onActivate: (characterId: number) => Promise<void>;
  onSubmittingChange: (busy: boolean) => void;
}) => {
  const contentRef = useRef<HTMLDivElement>(null);
  const submitting = useRef(false);
  const [busyId, setBusyId] = useState<number>();
  const [error, setError] = useState("");
  const enemies = playthroughCharacters.filter(
    (character) => character.characterType === CharacterType.Enemy,
  );
  const activate = async (characterId: number) => {
    if (submitting.current) return;
    submitting.current = true;
    setBusyId(characterId);
    setError("");
    onSubmittingChange(true);
    try {
      await onActivate(characterId);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      submitting.current = false;
      setBusyId(undefined);
      onSubmittingChange(false);
    }
  };

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (
        !event.ctrlKey ||
        event.altKey ||
        event.shiftKey ||
        event.metaKey ||
        event.key.toLowerCase() !== "s" ||
        event.defaultPrevented ||
        contentRef.current
          ?.closest('[data-nested-dialog="true"]')
          ?.getAttribute("aria-hidden") !== "false" ||
        document.querySelector('[data-confirm-dialog="true"], dialog[open]')
      )
        return;

      event.preventDefault();
      if (!event.repeat && !submitting.current) onSkip();
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [onSkip]);

  return (
    <div ref={contentRef}>
      <Button
        className="mb-4"
        onClick={onSkip}
        disabled={busyId !== undefined}
        aria-keyshortcuts="Control+s"
        title="Skip Activation (Ctrl+S)"
      >
        Skip Activation {"\u2192"}
      </Button>
      {enemies.map((character) => (
        <div
          className="flex items-center gap-3 rounded-xl border border-border bg-surface p-3 mb-2"
          key={character.id}
        >
          {character.photoUrl ? (
            <img
              src={character.photoUrl}
              alt=""
              className="h-12 w-12 rounded-lg object-contain"
            />
          ) : (
            <div className="h-12 w-12 rounded-lg bg-surface-raised" />
          )}
          <p className="min-w-0 flex-1 truncate font-semibold text-content">
            {character.name}
          </p>
          <Button
            disabled={busyId !== undefined}
            onClick={() => void activate(character.id)}
          >
            {busyId === character.id ? (
              <span
                role="status"
                aria-label="Activating character"
                className="h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent"
              />
            ) : (
              "Activate"
            )}
          </Button>
        </div>
      ))}
      {enemies.length === 0 && (
        <p className="text-content-muted">
          No enemies are available to activate.
        </p>
      )}
      {error && (
        <p role="alert" className="mt-3 text-danger">
          {error}
        </p>
      )}
      <div className="mt-4 flex justify-end">
        <Button
          variant="primary"
          disabled={busyId !== undefined}
          onClick={onSkip}
        >
          Next
        </Button>
      </div>
    </div>
  );
};

export default ActionDialogContent;
