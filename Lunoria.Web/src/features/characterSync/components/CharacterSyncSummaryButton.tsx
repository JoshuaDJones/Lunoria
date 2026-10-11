import { useState } from "react";
import {
  autoUpdate,
  flip,
  FloatingPortal,
  hide,
  offset,
  safePolygon,
  shift,
  useDismiss,
  useFloating,
  useFocus,
  useHover,
  useInteractions,
  useRole,
} from "@floating-ui/react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowsRotate,
  faCircleExclamation,
  faSpinner,
} from "@fortawesome/free-solid-svg-icons";
import { Button, type ButtonProps } from "@/components/ui";
import type {
  CharacterSyncSummaryEntry,
  CharacterSyncSummaryReason,
} from "@/features/characterSync/summaryTypes";

interface Props extends Omit<ButtonProps, "ref"> {
  entries: CharacterSyncSummaryEntry[];
  checking: boolean;
  checkError: string;
}

const labels: Record<CharacterSyncSummaryReason, string> = {
  stats: "base stats",
  spellAssignments: "spell assignments",
  alternateForm: "alternate form",
  sharedSpells: "shared spell definitions",
  baseCharacter: "base character",
};

export function CharacterSyncSummaryButton({
  entries,
  checking,
  checkError,
  children,
  size,
  variant,
  leftIcon,
  ...buttonProps
}: Props) {
  const [open, setOpen] = useState(false);
  const {
    refs: { setReference, setFloating },
    floatingStyles,
    context,
    middlewareData,
  } = useFloating({
    open,
    onOpenChange: setOpen,
    placement: "top",
    strategy: "fixed",
    whileElementsMounted: autoUpdate,
    middleware: [
      offset(8),
      flip({ padding: 12 }),
      shift({ padding: 12 }),
      hide(),
    ],
  });
  const hover = useHover(context, { handleClose: safePolygon() });
  const focus = useFocus(context);
  const dismiss = useDismiss(context, { bubbles: false });
  const role = useRole(context, { role: "tooltip" });
  const { getReferenceProps, getFloatingProps } = useInteractions([
    hover,
    focus,
    dismiss,
    role,
  ]);
  const updates = entries.filter(
    (entry) => entry.status === "updatesAvailable",
  ).length;
  const reviews = entries.filter(
    (entry) => entry.status === "reviewNeeded",
  ).length;
  const unavailable = entries.filter(
    (entry) => entry.status === "baseUnavailable",
  ).length;
  const description = checking
    ? "Checking for character updates…"
    : checkError
      ? "Unable to check character updates. Use Retry check on this page."
      : entries.length === 0
        ? "No unreviewed base character or spell changes."
        : [
            updates ? `${updates} with updates available` : "",
            reviews ? `${reviews} need an initial review` : "",
            unavailable ? `${unavailable} with an unavailable base` : "",
          ]
            .filter(Boolean)
            .join(" · ");
  return (
    <>
      <Button
        ref={setReference}
        size={size}
        variant={variant}
        leftIcon={leftIcon}
        {...getReferenceProps(buttonProps)}
      >
        {children}
        {(checking || checkError || entries.length > 0) && (
          <FontAwesomeIcon
            icon={
              checking
                ? faSpinner
                : checkError || reviews || unavailable
                  ? faCircleExclamation
                  : faArrowsRotate
            }
            spin={checking}
            aria-hidden="true"
            className={`ml-2 ${checking ? "text-content-muted" : "text-amber-300"}`}
          />
        )}
        <span className="sr-only"> — {description}</span>
      </Button>
      {open && (
        <FloatingPortal>
          <div
            ref={setFloating}
            style={{
              ...floatingStyles,
              visibility: middlewareData.hide?.referenceHidden
                ? "hidden"
                : undefined,
            }}
            className="z-[1000] max-h-[min(24rem,60dvh)] w-80 max-w-[calc(100vw-1.5rem)] overflow-y-auto rounded-xl border border-border bg-canvas p-3 text-sm text-content shadow-xl"
            {...getFloatingProps()}
          >
            <p className="font-semibold">{description}</p>
            {!checking && !checkError && entries.length > 0 && (
              <>
                <ul className="mt-2 space-y-3">
                  {entries.map((entry) => (
                    <li key={entry.assignmentId}>
                      <p className="font-semibold">{entry.name}</p>
                      {entry.status === "baseUnavailable" ? (
                        <p className="text-amber-200">
                          Base character is unavailable.
                        </p>
                      ) : (
                        <>
                          {entry.reasons.length > 0 && (
                            <p className="text-content-secondary">
                              Updates available:{" "}
                              {entry.reasons
                                .map((reason) => labels[reason])
                                .join(", ")}
                              .
                            </p>
                          )}
                          {entry.reviewReasons.length > 0 && (
                            <p className="text-content-secondary">
                              Review needed:{" "}
                              {entry.reviewReasons
                                .map((reason) => labels[reason])
                                .join(", ")}{" "}
                              (no previous review recorded).
                            </p>
                          )}
                        </>
                      )}
                    </li>
                  ))}
                </ul>
                <p className="mt-3 border-t border-border pt-2 text-xs text-content-muted">
                  Open to review. An initial review does not necessarily mean
                  values differ. Shared spell edits already apply to authoring
                  assignments; existing playthroughs are unchanged.
                </p>
              </>
            )}
          </div>
        </FloatingPortal>
      )}
    </>
  );
}
