import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowsRotate } from "@fortawesome/free-solid-svg-icons";
import { Button } from "@/components/ui";
import { CharacterSyncDialog } from "@/features/characterSync/components/CharacterSyncDialog";
import type {
  CharacterSyncKind,
  CharacterSyncStatus,
} from "@/features/characterSync/types";

interface Props {
  kind: CharacterSyncKind;
  assignmentId: number;
  name: string;
  status?: CharacterSyncStatus | null;
  onUpdated: () => Promise<void>;
  disabled?: boolean;
}

export function CharacterSyncButton({
  kind,
  assignmentId,
  name,
  status,
  onUpdated,
  disabled,
}: Props) {
  const [open, setOpen] = useState(false);
  const baseUpdates =
    status &&
    [status.stats, status.spellAssignments, status.alternateForm].some(
      (item) => item.updateAvailable,
    );
  const spellUpdates = status?.sharedSpells.some(
    (item) => item.updateAvailable,
  );
  const pendingCategories = status
    ? [status.stats, status.spellAssignments, status.alternateForm].filter(
        (item) => item.updateAvailable || item.requiresReview,
      ).length +
      Number(
        status.sharedSpells.some(
          (item) => item.updateAvailable || item.requiresReview,
        ),
      )
    : 0;
  const label =
    status && !status.baseAvailable
      ? "Base unavailable"
      : status?.hasUpdates && pendingCategories > 1
        ? "Review updates"
        : baseUpdates
          ? "Base updated"
          : spellUpdates
            ? "Spells updated"
            : status?.requiresReview
              ? "Review base"
              : "Compare with base";
  return (
    <>
      <Button
        size="sm"
        disabled={disabled || status?.baseAvailable === false}
        onClick={() => setOpen(true)}
        aria-label={`${label} — ${name}`}
        className={
          status?.hasUpdates || status?.requiresReview
            ? "border-amber-400/40 bg-amber-400/10 text-amber-200 hover:bg-amber-400/20"
            : undefined
        }
        leftIcon={<FontAwesomeIcon icon={faArrowsRotate} />}
      >
        {label}
      </Button>
      {open && (
        <CharacterSyncDialog
          kind={kind}
          assignmentId={assignmentId}
          name={name}
          onUpdated={onUpdated}
          onClose={() => setOpen(false)}
        />
      )}
    </>
  );
}
