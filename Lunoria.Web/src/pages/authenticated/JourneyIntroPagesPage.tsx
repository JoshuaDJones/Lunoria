import { useEffect, useState } from "react";
import { Link, Navigate, useNavigate, useParams } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faPlay,
  faPlus,
  faSave,
  faSpinner,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import AppLayout from "@/app/layouts";
import { useConfirmDialog, useToast } from "@/app/providers";
import { ApiLoadError, Button, Drawer } from "@/components/ui";
import {
  createIntroPage,
  deleteIntroPage,
  getJourney,
  IntroPageEditor,
  IntroPageViewer,
  listIntroPages,
  reorderIntroPages,
  updateIntroPage,
  type IntroPage,
  type IntroPageType,
  type IntroPageConfig,
  type Journey,
} from "@/features/journeys";
import { IntroPageLayoutPicker } from "@/features/journeys/components/IntroPageLayoutPicker";
import { IntroPageRail } from "@/features/journeys/components/IntroPageRail";
import { getApiError } from "@/lib/apiClient";
import { introPageTypeLabels } from "@/features/journeys/introPageConfig";

interface EditingPage {
  type: IntroPageType;
  page?: IntroPage;
}

export function JourneyIntroPagesPage() {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const navigate = useNavigate();
  const { seriesId, journeyId: journeyIdParam } = useParams();
  const journeyId = Number(journeyIdParam);
  const [journey, setJourney] = useState<Journey>();
  const [pages, setPages] = useState<IntroPage[]>([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isChoosingType, setIsChoosingType] = useState(false);
  const [editing, setEditing] = useState<EditingPage>();
  const [revision, setRevision] = useState(0);
  const [dirty, setDirty] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isSavingOrder, setIsSavingOrder] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [viewingPageId, setViewingPageId] = useState<number>();
  const busy = isSaving || isSavingOrder || isDeleting;

  const acceptLoaded = (loadedJourney: Journey, loadedPages: IntroPage[]) => {
    const ordered = [...loadedPages].sort((a, b) => a.sortOrder - b.sortOrder);
    setJourney(loadedJourney);
    setPages(ordered);
    setEditing(
      ordered[0] ? { type: ordered[0].type, page: ordered[0] } : undefined,
    );
    setDirty(false);
  };

  const load = async () => {
    setIsLoading(true);
    setError("");
    try {
      const [loadedJourney, loadedPages] = await Promise.all([
        getJourney(journeyId),
        listIntroPages(journeyId),
      ]);
      acceptLoaded(loadedJourney, loadedPages);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (!Number.isInteger(journeyId) || journeyId <= 0) return;
    let isCurrent = true;
    void Promise.all([getJourney(journeyId), listIntroPages(journeyId)])
      .then(([loadedJourney, loadedPages]) => {
        if (!isCurrent) return;
        acceptLoaded(loadedJourney, loadedPages);
        setError("");
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setError(getApiError(requestError).message);
      })
      .finally(() => {
        if (isCurrent) setIsLoading(false);
      });
    return () => {
      isCurrent = false;
    };
  }, [journeyId]);

  useEffect(() => {
    if (!dirty && !busy) return;
    const warn = (event: BeforeUnloadEvent) => {
      event.preventDefault();
      event.returnValue = "";
    };
    window.addEventListener("beforeunload", warn);
    return () => window.removeEventListener("beforeunload", warn);
  }, [dirty, busy]);

  if (!Number.isInteger(journeyId) || journeyId <= 0 || !seriesId) {
    return <Navigate to="/home" replace />;
  }

  const allowDiscard = async () => {
    if (busy) return false;
    return (
      !dirty ||
      (await confirm({
        title: "Discard unsaved changes?",
        message: "Your current page has changes that have not been saved.",
        confirmLabel: "Discard Changes",
        variant: "danger",
      }))
    );
  };

  const selectPage = async (page: IntroPage) => {
    if (editing?.page?.id === page.id || !(await allowDiscard())) return;
    setEditing({ type: page.type, page });
    setDirty(false);
    setRevision((value) => value + 1);
  };

  const savePage = async (config: IntroPageConfig, image?: File) => {
    if (!editing) return;
    let saved: IntroPage;
    if (editing.page) {
      saved = await updateIntroPage(editing.page.id, {
        journeyId,
        type: editing.type,
        config: JSON.stringify(config),
        image,
      });
      setPages((current) =>
        current.map((page) => (page.id === saved.id ? saved : page)),
      );
    } else {
      if (!image) throw new Error("An image is required.");
      saved = await createIntroPage({
        journeyId,
        type: editing.type,
        config: JSON.stringify(config),
        image,
      });
      setPages((current) =>
        [...current, saved].sort((a, b) => a.sortOrder - b.sortOrder),
      );
    }
    setEditing({ type: saved.type, page: saved });
    setDirty(false);
    setRevision((value) => value + 1);
    toast.success("Intro page was saved.");
  };

  const saveOrder = async (orderedPages: IntroPage[]) => {
    if (busy) return;
    setIsSavingOrder(true);
    try {
      await reorderIntroPages(
        journeyId,
        orderedPages.map((page, sortOrder) => ({ id: page.id, sortOrder })),
      );
      setPages(orderedPages.map((page, sortOrder) => ({ ...page, sortOrder })));
      toast.success("Page order was saved.");
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to reorder pages");
    } finally {
      setIsSavingOrder(false);
    }
  };

  const removePage = async () => {
    const page = editing?.page;
    if (!page || busy) return;
    const confirmed = await confirm({
      title: "Delete intro page?",
      message: dirty
        ? "This page and its unsaved changes will be discarded. This cannot be undone."
        : "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });
    if (!confirmed) return;
    setIsDeleting(true);
    try {
      await deleteIntroPage(page.id, journeyId);
      const remaining = pages.filter((item) => item.id !== page.id);
      setPages(remaining);
      const next =
        remaining[
          Math.min(
            pages.findIndex((item) => item.id === page.id),
            remaining.length - 1,
          )
        ];
      setEditing(next ? { type: next.type, page: next } : undefined);
      setDirty(false);
      setRevision((value) => value + 1);
      toast.success("Intro page was deleted.");
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to delete intro page",
      );
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <AppLayout
      fixedViewport
      bottomPadding
      background={<div className="stone-image absolute inset-0 z-0" />}
    >
      <main className="flex min-h-0 w-full flex-1 flex-col px-4 pt-6 pb-4 sm:px-6">
        <header className="mb-5 flex shrink-0 flex-wrap items-center justify-between gap-4">
          <div className="min-w-0">
            <h1 className="text-3xl text-content sm:text-4xl">
              Intro Pages Editor{journey ? ` - ${journey.name}` : ""}
            </h1>
            <Link
              to={`/series/${seriesId}/journeys/${journeyId}`}
              onClick={(event) => {
                if (!dirty && !busy) return;
                event.preventDefault();
                void allowDiscard().then((allowed) => {
                  if (allowed)
                    navigate(`/series/${seriesId}/journeys/${journeyId}`);
                });
              }}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to journey editor
            </Link>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <span role="status" className="mr-2 text-xs text-content-muted">
              {isSavingOrder
                ? "Saving order…"
                : isDeleting
                  ? "Deleting page…"
                  : dirty
                    ? "Unsaved changes"
                    : editing
                      ? "Saved"
                      : ""}
            </span>
            <Button
              disabled={busy || isLoading || Boolean(error)}
              leftIcon={<FontAwesomeIcon icon={faPlus} />}
              onClick={() => setIsChoosingType(true)}
              className="h-11 bg-surface/90"
            >
              Add Page
            </Button>
            <Button
              disabled={busy || dirty || pages.length === 0}
              title={
                dirty
                  ? "Save your page before previewing the intro"
                  : "Preview the full intro"
              }
              onClick={() => setViewingPageId(pages[0]?.id)}
              leftIcon={<FontAwesomeIcon icon={faPlay} />}
              className="h-11 bg-surface/90"
            >
              Preview Intro
            </Button>
            <Button
              className="h-11 bg-surface/90"
              disabled={busy || !editing || !dirty}
              onClick={() => {
                void allowDiscard().then((allowed) => {
                  if (!allowed || !editing) return;
                  const original = editing.page
                    ? pages.find((page) => page.id === editing.page?.id)
                    : pages[0];
                  setEditing(original ? { type: original.type, page: original } : undefined);
                  setDirty(false);
                  setRevision((value) => value + 1);
                });
              }}
            >
              {editing && !editing.page ? "Cancel New Page" : "Discard Changes"}
            </Button>
            <Button
              type="submit"
              form="intro-page-editor"
              variant="add"
              className="h-11"
              disabled={busy || !editing || !dirty}
              leftIcon={
                <FontAwesomeIcon
                  icon={isSaving ? faSpinner : faSave}
                  spin={isSaving}
                />
              }
            >
              Save Page
            </Button>
          </div>
        </header>
        {isLoading && <p role="status">Loading intro pages...</p>}
        {!isLoading && error && <ApiLoadError error={error} onRetry={load} />}
        {!isLoading && !error && (
          <div className="flex min-h-0 flex-1 flex-col gap-4 overflow-y-auto lg:flex-row lg:overflow-hidden">
            <div className="flex min-w-0 shrink-0 flex-col lg:min-h-0 lg:flex-1 lg:overflow-y-auto xl:overflow-hidden">
              {editing ? (
                <>
                  <div
                    inert={isSavingOrder || isDeleting}
                    className="flex shrink-0 flex-col xl:min-h-0 xl:flex-1"
                  >
                    <IntroPageEditor
                      key={`${editing.page?.id ?? "new"}-${revision}`}
                      type={editing.type}
                      page={editing.page}
                      previewToolbar={
                        <div className="flex min-h-10 shrink-0 items-center justify-between gap-3 px-1">
                          <h2 className="text-lg">
                            {editing.page
                              ? `Page ${pages.findIndex((page) => page.id === editing.page?.id) + 1}`
                              : "New Page"}
                            <span className="text-sm text-content-secondary">
                              {" · "}
                              {introPageTypeLabels[editing.type]}
                            </span>
                          </h2>
                          {editing.page && (
                            <Button
                              disabled={busy}
                              variant="danger"
                              aria-label="Delete intro page"
                              className="h-10 w-10 shrink-0 p-0"
                              onClick={() => void removePage()}
                            >
                              <FontAwesomeIcon icon={faTrash} />
                            </Button>
                          )}
                        </div>
                      }
                      onSave={savePage}
                      onDirtyChange={setDirty}
                      onBusyChange={setIsSaving}
                      pageNavigation={
                        pages.length > 0 && (
                          <IntroPageRail
                            pages={pages}
                            selectedId={editing.page?.id}
                            disabled={busy}
                            onSelect={(page) => void selectPage(page)}
                            onReorder={(ordered) => void saveOrder(ordered)}
                          />
                        )
                      }
                    />
                  </div>
                </>
              ) : (
                <div className="flex min-h-64 flex-1 flex-col items-center justify-center rounded-2xl border border-border bg-surface/90 p-8 text-center">
                  <h2 className="text-2xl">Create your journey’s opening</h2>
                  <p className="mt-3 max-w-md text-content-secondary">
                    Combine images and story text into the intro players see
                    when a new playthrough begins.
                  </p>
                  <Button
                    variant="add"
                    className="mt-6"
                    onClick={() => setIsChoosingType(true)}
                  >
                    Add Your First Page
                  </Button>
                </div>
              )}
            </div>
          </div>
        )}
      </main>
      {isChoosingType && (
        <Drawer
          title="Choose a Page Layout"
          onClose={() => setIsChoosingType(false)}
        >
          <p className="mb-5 text-sm text-content-secondary">
            Choose how your image and story will appear.
          </p>
          <IntroPageLayoutPicker
            onSelect={(type) => {
              void allowDiscard().then((allowed) => {
                if (!allowed) return;
                setEditing({ type });
                setRevision((value) => value + 1);
                setDirty(true);
                setIsChoosingType(false);
              });
            }}
          />
        </Drawer>
      )}
      {viewingPageId !== undefined && (
        <IntroPageViewer
          pages={pages}
          initialPageId={viewingPageId}
          title={`${journey?.name ?? "Journey"} Intro`}
          onClose={() => setViewingPageId(undefined)}
        />
      )}
    </AppLayout>
  );
}
