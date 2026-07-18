import { useCallback } from "react";
import { Navigate, useParams } from "react-router-dom";
import { CollectionPage } from "@/components/layout/CollectionPage";
import { Card } from "@/components/ui";
import { tempGetAllDialogs } from "@/features/journeys/api/journeysApi";
import type {
  DialogPage,
  DialogPageSection,
  SceneDialog,
} from "@/features/scenes/types";

function sortByOrder<T extends { orderNum: number }>(
  items: T[] | null,
): T[] {
  return [...(items ?? [])].sort((a, b) => a.orderNum - b.orderNum);
}

function DialogSectionRow({ section }: { section: DialogPageSection }) {
  const speaker = section.isNarrator
    ? "Narrator"
    : (section.character?.name ?? "Unknown character");

  return (
    <div className="flex gap-3 rounded-lg bg-surface-raised/90 p-3">
      {!section.isNarrator && section.character?.photoUrl && (
        <img
          src={section.character.photoUrl}
          alt=""
          className="h-12 w-12 shrink-0 rounded-full object-cover"
        />
      )}
      <div className="min-w-0 flex-1">
        <div className="flex items-center justify-between gap-2">
          <span
            className={
              section.isNarrator
                ? "text-sm font-semibold italic text-content-muted"
                : "text-sm font-semibold text-brand-hover"
            }
          >
            {speaker}
          </span>
          <span className="text-xs text-content-muted">
            #{section.orderNum}
          </span>
        </div>
        <p className="mt-1 wrap-break-word text-sm text-content-secondary">
          {section.readingText}
        </p>
      </div>
    </div>
  );
}

function DialogPageBlock({ page }: { page: DialogPage }) {
  const sections = sortByOrder(page.dialogPageSections);

  return (
    <div className="rounded-xl border border-border bg-surface/80 p-4">
      <div className="mb-3 flex items-center justify-between gap-3">
        <h3 className="text-lg font-semibold text-content">
          Page {page.orderNum}
        </h3>
        {page.photoUrl && (
          <img
            src={page.photoUrl}
            alt=""
            className="h-16 w-24 shrink-0 rounded-lg object-cover"
          />
        )}
      </div>

      {sections.length > 0 ? (
        <div className="space-y-2">
          {sections.map((section) => (
            <DialogSectionRow key={section.id} section={section} />
          ))}
        </div>
      ) : (
        <p className="text-sm text-content-muted">No dialog sections.</p>
      )}
    </div>
  );
}

function DialogCard({ dialog }: { dialog: SceneDialog }) {
  const pages = sortByOrder(dialog.dialogPages);

  return (
    <Card className="p-5">
      <div className="flex items-center justify-between gap-2">
        <h2 className="text-2xl font-semibold text-content">
          {dialog.title}
        </h2>
        <span className="text-xs text-content-muted">#{dialog.id}</span>
      </div>

      {pages.length > 0 ? (
        <div className="mt-4 space-y-4">
          {pages.map((page) => (
            <DialogPageBlock key={page.id} page={page} />
          ))}
        </div>
      ) : (
        <p className="mt-2 text-sm text-content-muted">No pages yet.</p>
      )}
    </Card>
  );
}

export function AllDialogsPage() {
  const { journeyId: journeyIdParam } = useParams();
  const journeyId = Number(journeyIdParam);

  const loadDialogs = useCallback(
    () => tempGetAllDialogs(journeyId),
    [journeyId],
  );

  if (!Number.isInteger(journeyId) || journeyId <= 0) {
    return <Navigate to="/home" replace />;
  }

  return (
    <CollectionPage
      title="All Dialogs"
      itemName="dialog"
      loadItems={loadDialogs}
      renderItems={(dialogs) => (
        <div className="space-y-6">
          {dialogs.map((dialog) => (
            <DialogCard key={dialog.id} dialog={dialog} />
          ))}
        </div>
      )}
    />
  );
}
