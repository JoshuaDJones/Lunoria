import { useEffect, useState } from "react";
import { Link, Navigate, useParams } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGripVertical } from "@fortawesome/free-solid-svg-icons";
import AppLayout from "@/app/layouts";
import { useConfirmDialog, useToast } from "@/app/providers";
import { ApiLoadError, Button, Drawer } from "@/components/ui";
import {
  createIntroPage,
  deleteIntroPage,
  getJourney,
  IntroPageEditor,
  IntroPagePreview,
  IntroPageViewer,
  IntroPageType,
  introPageTypeLabels,
  listIntroPages,
  parseIntroPageConfig,
  reorderIntroPages,
  updateIntroPage,
  type IntroPage,
  type IntroPageConfig,
  type Journey,
} from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";

const introPageTypes = Object.values(IntroPageType).filter(
  (value): value is IntroPageType => typeof value === "number",
);

interface EditingPage {
  type: IntroPageType;
  page?: IntroPage;
}

export function JourneyIntroPagesPage() {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const { seriesId, journeyId: journeyIdParam } = useParams();
  const journeyId = Number(journeyIdParam);
  const [journey, setJourney] = useState<Journey>();
  const [pages, setPages] = useState<IntroPage[]>([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isChoosingType, setIsChoosingType] = useState(false);
  const [editing, setEditing] = useState<EditingPage>();
  const [isOrdering, setIsOrdering] = useState(false);
  const [orderedPages, setOrderedPages] = useState<IntroPage[]>([]);
  const [draggedPageId, setDraggedPageId] = useState<number>();
  const [isSavingOrder, setIsSavingOrder] = useState(false);
  const [orderError, setOrderError] = useState("");
  const [viewingPageId, setViewingPageId] = useState<number>();

  const load = async () => {
    setIsLoading(true);
    setError("");

    try {
      const [loadedJourney, loadedPages] = await Promise.all([
        getJourney(journeyId),
        listIntroPages(journeyId),
      ]);
      setJourney(loadedJourney);
      setPages(loadedPages);
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
        if (isCurrent) {
          setJourney(loadedJourney);
          setPages(loadedPages);
          setError("");
        }
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

  if (!Number.isInteger(journeyId) || journeyId <= 0 || !seriesId) {
    return <Navigate to="/home" replace />;
  }

  const removePage = async (page: IntroPage) => {
    const confirmed = await confirm({
      title: "Delete intro page?",
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });
    if (!confirmed) return;

    try {
      await deleteIntroPage(page.id, journeyId);
      setPages((current) => current.filter((item) => item.id !== page.id));
      toast.success("Intro page was deleted.");
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to delete intro page",
      );
    }
  };

  const openOrder = () => {
    setOrderedPages([...pages].sort((a, b) => a.sortOrder - b.sortOrder));
    setOrderError("");
    setIsOrdering(true);
  };

  const moveDraggedPage = (targetId: number) => {
    if (draggedPageId === undefined || draggedPageId === targetId) return;
    setOrderedPages((current) => {
      const from = current.findIndex((page) => page.id === draggedPageId);
      const to = current.findIndex((page) => page.id === targetId);
      if (from < 0 || to < 0) return current;
      const next = [...current];
      const [moved] = next.splice(from, 1);
      next.splice(to, 0, moved);
      return next;
    });
  };

  const saveOrder = async () => {
    setIsSavingOrder(true);
    setOrderError("");
    try {
      await reorderIntroPages(
        journeyId,
        orderedPages.map((page, sortOrder) => ({ id: page.id, sortOrder })),
      );
      setPages(orderedPages.map((page, sortOrder) => ({ ...page, sortOrder })));
      setIsOrdering(false);
      toast.success("Intro page order was updated.");
    } catch (requestError) {
      setOrderError(getApiError(requestError).message);
      setIsSavingOrder(false);
    }
  };

  return (
    <AppLayout
      scrolling
      background={<div className="stone-image absolute inset-0 z-0" />}
    >
      <main className="w-full p-6 sm:p-10">
        <header className="mb-6 flex flex-wrap items-center justify-between gap-4">
          <div>
            <h1 className="text-4xl text-content sm:text-5xl">
              {journey ? `${journey.name} Intro Pages` : "Intro Pages"}
            </h1>
            <Link
              to={`/series/${seriesId}/journeys/${journeyId}`}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to journey
            </Link>
          </div>
          <div className="flex gap-3">
            <Button
              onClick={openOrder}
              disabled={pages.length < 2}
              variant="utility"
              size="lg"
            >
              Page Order
            </Button>
            <Button
              onClick={() => setIsChoosingType(true)}
              variant="add"
              size="lg"
            >
              New Intro Page
            </Button>
          </div>
        </header>

        {isLoading && <p role="status">Loading intro pages...</p>}
        {!isLoading && error && <ApiLoadError error={error} onRetry={load} />}

        {!isLoading && !error && pages.length === 0 && (
          <div className="rounded-xl border border-border bg-surface/80 p-8 text-center">
            <h2 className="text-2xl font-semibold text-content">
              No intro pages yet
            </h2>
            <p className="mt-2 text-content-muted">
              Create the opening slideshow for this journey.
            </p>
          </div>
        )}

        {!isLoading && !error && pages.length > 0 && (
          <div className="grid gap-6 xl:grid-cols-2">
            {pages.map((page, index) => (
              <article
                key={page.id}
                className="rounded-2xl border border-border bg-surface/90 p-4"
              >
                <div className="mb-3 flex items-center justify-between gap-3">
                  <div>
                    <span className="text-xs text-content-muted">
                      Page {index + 1}
                    </span>
                    <h2 className="font-semibold text-content">
                      {introPageTypeLabels[page.type]}
                    </h2>
                  </div>
                  <div className="flex gap-2">
                    <Button
                      size="sm"
                      variant="accent"
                      onClick={() => setViewingPageId(page.id)}
                    >
                      View
                    </Button>
                    <Button
                      size="sm"
                      onClick={() => setEditing({ type: page.type, page })}
                    >
                      Edit
                    </Button>
                    <Button
                      size="sm"
                      variant="danger"
                      onClick={() => void removePage(page)}
                    >
                      Delete
                    </Button>
                  </div>
                </div>
                <IntroPagePreview
                  type={page.type}
                  config={parseIntroPageConfig(page.config)}
                  imageUrl={page.previewPhotoUrl}
                />
              </article>
            ))}
          </div>
        )}
      </main>

      {isChoosingType && (
        <Drawer
          title="Choose an Intro Page Type"
          onClose={() => setIsChoosingType(false)}
        >
          <div className="grid gap-3">
            {introPageTypes.map((type) => (
              <Button
                key={type}
                size="lg"
                className="justify-start py-5 text-left"
                onClick={() => {
                  setIsChoosingType(false);
                  setEditing({ type });
                }}
              >
                {introPageTypeLabels[type]}
              </Button>
            ))}
          </div>
        </Drawer>
      )}

      {editing && (
        <Drawer
          title={editing.page ? "Edit Intro Page" : "Create Intro Page"}
          onClose={() => setEditing(undefined)}
        >
          <IntroPageEditor
            type={editing.type}
            page={editing.page}
            onCancel={() => setEditing(undefined)}
            onSave={async (config: IntroPageConfig, image?: File) => {
              if (editing.page) {
                const updated = await updateIntroPage(editing.page.id, {
                  journeyId,
                  type: editing.type,
                  config: JSON.stringify(config),
                  image,
                });
                setPages((current) =>
                  current.map((page) =>
                    page.id === updated.id ? updated : page,
                  ),
                );
                toast.success("Intro page was updated.");
              } else {
                if (!image) throw new Error("An image is required.");
                const created = await createIntroPage({
                  journeyId,
                  type: editing.type,
                  config: JSON.stringify(config),
                  image,
                });
                setPages((current) => [...current, created]);
                toast.success("Intro page was created.");
              }
              setEditing(undefined);
            }}
          />
        </Drawer>
      )}

      {isOrdering && (
        <Drawer title="Page Order" onClose={() => setIsOrdering(false)}>
          <div className="flex min-h-full flex-col">
            <p className="mb-5 text-sm text-content-secondary">
              Drag pages into slideshow order, then save.
            </p>
            {orderError && (
              <p className="mb-4 text-danger" role="alert">
                {orderError}
              </p>
            )}
            <ol className="flex-1 space-y-3">
              {orderedPages.map((page, index) => (
                <li
                  key={page.id}
                  draggable={!isSavingOrder}
                  onDragStart={() => setDraggedPageId(page.id)}
                  onDragOver={(event) => {
                    event.preventDefault();
                    moveDraggedPage(page.id);
                  }}
                  onDragEnd={() => setDraggedPageId(undefined)}
                  className={`flex cursor-grab items-center gap-4 rounded-xl border bg-surface p-3 transition active:cursor-grabbing ${
                    draggedPageId === page.id
                      ? "border-brand opacity-50"
                      : "border-border"
                  }`}
                >
                  <FontAwesomeIcon
                    icon={faGripVertical}
                    className="shrink-0 text-content-muted"
                  />
                  <span className="w-7 shrink-0 text-center text-sm text-content-muted">
                    {index + 1}
                  </span>
                  {page.previewPhotoUrl && (
                    <img
                      src={page.previewPhotoUrl}
                      alt=""
                      className="size-12 shrink-0 rounded-lg object-cover"
                    />
                  )}
                  <span className="min-w-0 flex-1 truncate font-semibold text-content">
                    {introPageTypeLabels[page.type]}
                  </span>
                </li>
              ))}
            </ol>
            <div className="mt-6 flex justify-end gap-3 border-t border-border pt-4">
              <Button
                onClick={() => setIsOrdering(false)}
                disabled={isSavingOrder}
              >
                Cancel
              </Button>
              <Button
                variant="primary"
                disabled={isSavingOrder}
                onClick={() => void saveOrder()}
              >
                {isSavingOrder ? "Saving..." : "Save Order"}
              </Button>
            </div>
          </div>
        </Drawer>
      )}

      {viewingPageId !== undefined && (
        <IntroPageViewer
          pages={pages}
          initialPageId={viewingPageId}
          title={`${journey?.name ?? "Journey"} Intro Pages`}
          onClose={() => setViewingPageId(undefined)}
        />
      )}
    </AppLayout>
  );
}
