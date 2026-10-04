import type { ReactNode } from "react";
import { DialogAccordionPanel } from "@/features/scenes/components/DialogAccordionPanel";
import { Button } from "@/components/ui";
import {
  DialogPageType,
  type DialogPage,
  type DialogPageSection,
  type SceneDialog,
} from "@/features/scenes/types";

interface DialogAccordionProps {
  dialog: SceneDialog;
  expanded: boolean;
  openPageIds: number[];
  onToggle: () => void;
  onCollapse: () => void;
  onExpand: () => void;
  onTogglePage: (id: number) => void;
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
  onAddPage: () => void;
  onEditPage: (page: DialogPage) => void;
  onDeletePage: (page: DialogPage) => void;
  onAddSection: (page: DialogPage) => void;
  onEditSection: (page: DialogPage, section: DialogPageSection) => void;
  onDeleteSection: (section: DialogPageSection) => void;
  pageForm?: ReactNode;
  pageFormPageId?: number;
  sectionForm?: ReactNode;
  sectionFormPageId?: number;
  sectionFormSectionId?: number;
}

export function DialogAccordion({
  dialog,
  expanded,
  openPageIds,
  onToggle,
  onCollapse,
  onExpand,
  onTogglePage,
  onView,
  onEdit,
  onDelete,
  onAddPage,
  onEditPage,
  onDeletePage,
  onAddSection,
  onEditSection,
  onDeleteSection,
  pageForm,
  pageFormPageId,
  sectionForm,
  sectionFormPageId,
  sectionFormSectionId,
}: DialogAccordionProps) {
  const pages = [...(dialog.dialogPages ?? [])].sort(
    (a, b) => a.orderNum - b.orderNum,
  );
  return (
    <section className="overflow-hidden rounded-2xl border border-border bg-surface/90">
      <header className="flex flex-wrap items-center gap-3 p-4">
        <h2 className="min-w-0 flex-1">
          <button
            type="button"
            onClick={onToggle}
            aria-expanded={expanded}
            aria-controls={`dialog-${dialog.id}-pages`}
            className="flex w-full items-center gap-3 rounded-lg text-left focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-hover"
          >
            <span
              aria-hidden="true"
              className={`transition-transform motion-reduce:transition-none ${expanded ? "rotate-90" : ""}`}
            >
              ›
            </span>
            <span className="min-w-0 break-words text-xl font-semibold">
              {dialog.title}
            </span>
            <span className="shrink-0 text-xs text-content-muted">
              {pages.length} pages
            </span>
          </button>
        </h2>
        <div className="flex flex-wrap gap-2">
          <Button size="sm" onClick={onCollapse}>
            Collapse
          </Button>
          <Button size="sm" onClick={onExpand}>
            Expand
          </Button>
          <Button size="sm" onClick={onView}>
            View
          </Button>
          <Button size="sm" onClick={onEdit}>
            Edit
          </Button>
          <Button size="sm" variant="danger" onClick={onDelete}>
            Delete
          </Button>
        </div>
      </header>
      <DialogAccordionPanel id={`dialog-${dialog.id}-pages`} open={expanded}>
        <div className="space-y-3 border-t border-border p-4 sm:p-5">
          <div className="flex items-center justify-between gap-3">
            <h3 className="font-semibold text-content-secondary">Pages</h3>
          </div>
          {pages.length === 0 && (
            <p className="py-4 text-sm text-content-muted">
              No pages yet. Add the first page to this dialog.
            </p>
          )}
          {pages.map((page) => {
            const open = openPageIds.includes(page.id);
            const video = page.pageType === DialogPageType.Video;
            const sections = [...(page.dialogPageSections ?? [])].sort(
              (a, b) => a.orderNum - b.orderNum,
            );
            return (
              <article
                key={page.id}
                className="overflow-hidden rounded-xl border border-border bg-surface-raised/50"
              >
                <header className="flex flex-wrap items-center gap-3 p-3">
                  <h4 className="min-w-0 flex-1">
                    <button
                      type="button"
                      onClick={() => onTogglePage(page.id)}
                      aria-expanded={open}
                      aria-controls={`dialog-page-${page.id}`}
                      className="flex w-full items-center gap-3 rounded-lg text-left focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-hover"
                    >
                      <span
                        aria-hidden="true"
                        className={`transition-transform motion-reduce:transition-none ${open ? "rotate-90" : ""}`}
                      >
                        ›
                      </span>
                      {video ? (
                        <span
                          aria-hidden="true"
                          className="flex h-12 w-20 shrink-0 items-center justify-center rounded-lg bg-canvas text-content-muted"
                        >
                          ▶
                        </span>
                      ) : (
                        <img
                          src={page.mediaUrl}
                          alt=""
                          loading="lazy"
                          className="h-12 w-20 shrink-0 rounded-lg bg-canvas object-contain"
                        />
                      )}
                      <span>
                        <span className="block font-semibold">
                          Page {page.orderNum}
                        </span>
                        <span className="text-xs text-content-muted">
                          {video ? "Video" : `${sections.length} sections`}
                        </span>
                      </span>
                    </button>
                  </h4>
                  <div className="flex gap-2">
                    <Button size="sm" onClick={() => onEditPage(page)}>
                      Edit
                    </Button>
                    <Button
                      size="sm"
                      variant="danger"
                      onClick={() => onDeletePage(page)}
                    >
                      Delete
                    </Button>
                  </div>
                </header>
                <DialogAccordionPanel id={`dialog-page-${page.id}`} open={open}>
                  <div className="space-y-3 border-t border-border p-4">
                    {pageFormPageId === page.id && pageForm ? (
                      pageForm
                    ) : video ? (
                      <video
                        src={page.mediaUrl}
                        controls
                        playsInline
                        preload="metadata"
                        className="max-h-96 w-full rounded-lg bg-canvas"
                      />
                    ) : (
                      <>
                        <div className="flex items-center justify-between gap-3">
                          <h5 className="font-semibold text-content-secondary">
                            Sections
                          </h5>
                        </div>
                        {sections.length === 0 && (
                          <p className="py-3 text-sm text-content-muted">
                            No sections yet. Add narration or character
                            dialogue.
                          </p>
                        )}
                        {sections.map((section) =>
                          sectionFormPageId === page.id &&
                          sectionFormSectionId === section.id &&
                          sectionForm ? (
                            <div key={section.id}>{sectionForm}</div>
                          ) : (
                            <div
                              key={section.id}
                              className="rounded-xl border border-border bg-surface/70 p-4"
                            >
                              <div className="flex flex-wrap items-center justify-between gap-3">
                                <h6 className="font-semibold">
                                  Section {section.orderNum}
                                  <span className="ml-3 text-sm font-normal text-content-secondary">
                                    {section.isNarrator
                                      ? "Narrator"
                                      : (section.character?.name ??
                                        "No character")}
                                  </span>
                                </h6>
                                <div className="flex gap-2">
                                  <Button
                                    size="sm"
                                    onClick={() => onEditSection(page, section)}
                                  >
                                    Edit
                                  </Button>
                                  <Button
                                    size="sm"
                                    variant="danger"
                                    onClick={() => onDeleteSection(section)}
                                  >
                                    Delete
                                  </Button>
                                </div>
                              </div>
                              <p className="mt-3 whitespace-pre-wrap break-words text-sm leading-relaxed text-content-secondary">
                                {section.readingText}
                              </p>
                            </div>
                          ),
                        )}
                        {sectionFormPageId === page.id &&
                        sectionFormSectionId === undefined &&
                        sectionForm ? (
                          sectionForm
                        ) : (
                          <div className="flex justify-end pt-2">
                            <Button onClick={() => onAddSection(page)}>
                              Add Section
                            </Button>
                          </div>
                        )}
                      </>
                    )}
                  </div>
                </DialogAccordionPanel>
              </article>
            );
          })}
          {(pageFormPageId === undefined && pageForm) || (
            <div className="flex justify-end pt-2">
              <Button onClick={onAddPage}>Add Page</Button>
            </div>
          )}
        </div>
      </DialogAccordionPanel>
    </section>
  );
}
