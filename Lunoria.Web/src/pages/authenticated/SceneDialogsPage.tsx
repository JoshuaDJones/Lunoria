import { useEffect, useState, type ReactNode } from "react";
import { Link, Navigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts/AppLayout";
import {
  booleanValue,
  nullableNumberValue,
  numberValue,
  requiredPhoto,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Button, Drawer } from "@/components/ui";
import {
  createDialogPage,
  createDialogPageSection,
  createSceneDialog,
  deleteDialogPage,
  deleteDialogPageSection,
  deleteSceneDialog,
  listSceneDialogs,
  updateDialogPage,
  updateDialogPageSection,
  updateSceneDialog,
  DialogViewer,
  type DialogPage,
  type DialogPageSection,
  type SceneDialog,
} from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";

const dialogFields: ResourceFormField[] = [
  { name: "title", label: "Title", required: true },
];

const pageFields: ResourceFormField[] = [
  { name: "orderNum", label: "Order", type: "number", required: true },
];

const sectionFields: ResourceFormField[] = [
  { name: "orderNum", label: "Order", type: "number", required: true },
  {
    name: "readingText",
    label: "Reading text",
    type: "textarea",
    required: true,
  },
  { name: "characterId", label: "Character ID", type: "number" },
  { name: "isNarrator", label: "Narrator", type: "checkbox" },
];

interface EditorColumnProps {
  title: string;
  addLabel: string;
  canAdd: boolean;
  onAdd: () => void;
  emptyMessage: string;
  hasItems: boolean;
  children: ReactNode;
}

function EditorColumn({
  title,
  addLabel,
  canAdd,
  onAdd,
  emptyMessage,
  hasItems,
  children,
}: EditorColumnProps) {
  return (
    <section className="flex min-h-80 flex-col rounded-2xl border border-border bg-surface/90">
      <header className="flex items-center justify-between gap-3 border-b border-border p-4">
        <h2 className="text-xl font-semibold text-content">{title}</h2>
        <Button
          onClick={onAdd}
          disabled={!canAdd}
          variant="primary"
          className="px-3"
        >
          {addLabel}
        </Button>
      </header>
      <div className="flex-1 space-y-3 overflow-y-auto p-4">
        {hasItems ? (
          children
        ) : (
          <p className="text-sm text-content-muted">{emptyMessage}</p>
        )}
      </div>
    </section>
  );
}

interface ItemActionsProps {
  onView?: () => void;
  onEdit: () => void;
  onDelete: () => void;
}

function ItemActions({ onView, onEdit, onDelete }: ItemActionsProps) {
  return (
    <div className="mt-3 flex gap-2">
      {onView && (
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onView();
          }}
          variant="accent"
          size="sm"
        >
          View
        </Button>
      )}
      <Button
        onClick={(event) => {
          event.stopPropagation();
          onEdit();
        }}
        size="sm"
      >
        Edit
      </Button>
      <Button
        onClick={(event) => {
          event.stopPropagation();
          onDelete();
        }}
        variant="danger"
        size="sm"
      >
        Delete
      </Button>
    </div>
  );
}

export function SceneDialogsPage() {
  const { journeyId: journeyIdParam, sceneId: sceneIdParam } = useParams();
  const journeyId = Number(journeyIdParam);
  const sceneId = Number(sceneIdParam);

  const [dialogs, setDialogs] = useState<SceneDialog[]>([]);
  const [selectedDialogId, setSelectedDialogId] = useState<number>();
  const [selectedPageId, setSelectedPageId] = useState<number>();
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [editingDialog, setEditingDialog] =
    useState<SceneDialog | null | undefined>();
  const [editingPage, setEditingPage] =
    useState<DialogPage | null | undefined>();
  const [editingSection, setEditingSection] =
    useState<DialogPageSection | null | undefined>();
  const [viewingDialog, setViewingDialog] = useState<SceneDialog>();

  useEffect(() => {
    let isCurrent = true;

    void listSceneDialogs(sceneId)
      .then((loadedDialogs) => {
        if (isCurrent) {
          setDialogs(loadedDialogs);
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) {
          setError(getApiError(requestError).message);
        }
      })
      .finally(() => {
        if (isCurrent) {
          setIsLoading(false);
        }
      });

    return () => {
      isCurrent = false;
    };
  }, [sceneId]);

  if (
    !Number.isInteger(journeyId) ||
    journeyId <= 0 ||
    !Number.isInteger(sceneId) ||
    sceneId <= 0
  ) {
    return <Navigate to="/home" replace />;
  }

  const selectedDialog = dialogs.find(
    (dialog) => dialog.id === selectedDialogId,
  );
  const pages = [...(selectedDialog?.dialogPages ?? [])].sort(
    (a, b) => a.orderNum - b.orderNum,
  );
  const selectedPage = pages.find((page) => page.id === selectedPageId);
  const sections = [...(selectedPage?.dialogPageSections ?? [])].sort(
    (a, b) => a.orderNum - b.orderNum,
  );

  const refresh = async () => {
    setDialogs(await listSceneDialogs(sceneId));
    setError("");
  };

  const remove = async (
    message: string,
    operation: () => Promise<void>,
  ) => {
    if (!window.confirm(message)) {
      return;
    }

    try {
      await operation();
      await refresh();
    } catch (requestError) {
      setError(getApiError(requestError).message);
    }
  };

  return (
    <AppLayout
      scrolling
      bottomPadding
      background={<div className="stone-image absolute inset-0 z-0" />}
    >
      <main className="flex min-h-full flex-col p-5 sm:p-8">
        <header className="mb-5 flex flex-wrap items-center justify-between gap-4">
          <div>
            <h1 className="mt-2 text-4xl font-semibold text-content">
              Dialog editor
            </h1>
                        <Link
              to={`/journeys/${journeyId}/scenes`}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to scenes
            </Link>
          </div>
        </header>

        {error && (
          <p
            className="mb-4 rounded-lg border border-danger/40 bg-surface p-3 text-danger"
            role="alert"
          >
            {error}
          </p>
        )}

        {isLoading ? (
          <p className="text-content-secondary" role="status">
            Loading dialog editor...
          </p>
        ) : (
          <div className="grid flex-1 gap-4 lg:grid-cols-3">
            <EditorColumn
              title="Dialogs"
              addLabel="Add dialog"
              canAdd
              onAdd={() => setEditingDialog(null)}
              emptyMessage="No dialogs yet."
              hasItems={dialogs.length > 0}
            >
              {dialogs.map((dialog) => (
                <article
                  key={dialog.id}
                  onClick={() => {
                    setSelectedDialogId(dialog.id);
                    setSelectedPageId(undefined);
                  }}
                  className={`cursor-pointer rounded-xl border p-4 transition ${
                    dialog.id === selectedDialogId
                      ? "border-brand bg-brand/10"
                      : "border-border bg-surface-raised/70 hover:border-brand-subtle/60"
                  }`}
                >
                  <h3 className="font-semibold text-content">{dialog.title}</h3>
                  <p className="mt-1 text-xs text-content-muted">
                    {dialog.dialogPages?.length ?? 0} pages
                  </p>
                  <ItemActions
                    onView={() => setViewingDialog(dialog)}
                    onEdit={() => setEditingDialog(dialog)}
                    onDelete={() =>
                      void remove(`Delete "${dialog.title}"?`, () =>
                        deleteSceneDialog(dialog.id),
                      )
                    }
                  />
                </article>
              ))}
            </EditorColumn>

            <EditorColumn
              title="Pages"
              addLabel="Add page"
              canAdd={Boolean(selectedDialog)}
              onAdd={() => setEditingPage(null)}
              emptyMessage={
                selectedDialog ? "No pages yet." : "Select a dialog first."
              }
              hasItems={pages.length > 0}
            >
              {pages.map((page) => (
                <article
                  key={page.id}
                  onClick={() => setSelectedPageId(page.id)}
                  className={`cursor-pointer rounded-xl border p-3 transition ${
                    page.id === selectedPageId
                      ? "border-brand bg-brand/10"
                      : "border-border bg-surface-raised/70 hover:border-brand-subtle/60"
                  }`}
                >
                  {page.photoUrl && (
                    <img
                      src={page.photoUrl}
                      alt=""
                      className="mb-3 h-24 w-32 rounded-lg bg-canvas object-contain"
                    />
                  )}
                  <h3 className="font-semibold text-content">
                    Page {page.orderNum}
                  </h3>
                  <p className="mt-1 text-xs text-content-muted">
                    {page.dialogPageSections?.length ?? 0} sections
                  </p>
                  <ItemActions
                    onEdit={() => setEditingPage(page)}
                    onDelete={() =>
                      void remove(`Delete page ${page.orderNum}?`, () =>
                        deleteDialogPage(page.id),
                      )
                    }
                  />
                </article>
              ))}
            </EditorColumn>

            <EditorColumn
              title="Sections"
              addLabel="Add section"
              canAdd={Boolean(selectedPage)}
              onAdd={() => setEditingSection(null)}
              emptyMessage={
                selectedPage ? "No sections yet." : "Select a page first."
              }
              hasItems={sections.length > 0}
            >
              {sections.map((section) => (
                <article
                  key={section.id}
                  className="rounded-xl border border-border bg-surface-raised/70 p-4"
                >
                  <div className="flex items-center justify-between gap-2">
                    <h3 className="font-semibold text-content">
                      Section {section.orderNum}
                    </h3>
                    <span className="text-xs text-content-muted">
                      {section.isNarrator
                        ? "Narrator"
                        : section.character?.name ?? "No character"}
                    </span>
                  </div>
                  <p className="mt-3 whitespace-pre-wrap text-sm text-content-secondary">
                    {section.readingText}
                  </p>
                  <ItemActions
                    onEdit={() => setEditingSection(section)}
                    onDelete={() =>
                      void remove(`Delete section ${section.orderNum}?`, () =>
                        deleteDialogPageSection(section.id),
                      )
                    }
                  />
                </article>
              ))}
            </EditorColumn>
          </div>
        )}
      </main>

      {editingDialog !== undefined && (
        <Drawer
          title={editingDialog ? "Edit dialog" : "Create dialog"}
          onClose={() => setEditingDialog(undefined)}
        >
          <ResourceForm
            fields={dialogFields}
            initialValues={{ title: editingDialog?.title ?? "" }}
            showPhoto={false}
            onSubmit={async (values) => {
              const title = textValue(values, "title");

              if (editingDialog) {
                await updateSceneDialog(editingDialog.id, title);
              } else {
                await createSceneDialog(sceneId, title);
              }

              setEditingDialog(undefined);
              await refresh();
            }}
          />
        </Drawer>
      )}

      {editingPage !== undefined && selectedDialog && (
        <Drawer
          title={editingPage ? "Edit dialog page" : "Create dialog page"}
          onClose={() => setEditingPage(undefined)}
        >
          <ResourceForm
            fields={pageFields}
            initialValues={{
              orderNum: String(editingPage?.orderNum ?? pages.length + 1),
            }}
            existingPhotoUrl={editingPage?.photoUrl ?? undefined}
            requirePhoto={!editingPage}
            onSubmit={async (values, photo) => {
              const orderNum = numberValue(values, "orderNum");

              if (editingPage) {
                await updateDialogPage(editingPage.id, { orderNum, photo });
              } else {
                await createDialogPage(
                  selectedDialog.id,
                  orderNum,
                  requiredPhoto(photo),
                );
              }

              setEditingPage(undefined);
              await refresh();
            }}
          />
        </Drawer>
      )}

      {editingSection !== undefined && selectedPage && (
        <Drawer
          title={
            editingSection ? "Edit dialog section" : "Create dialog section"
          }
          onClose={() => setEditingSection(undefined)}
        >
          <ResourceForm
            fields={sectionFields}
            showPhoto={false}
            initialValues={{
              orderNum: String(editingSection?.orderNum ?? sections.length + 1),
              readingText: editingSection?.readingText ?? "",
              characterId: String(editingSection?.character?.id ?? ""),
              isNarrator: editingSection?.isNarrator ?? false,
            }}
            onSubmit={async (values) => {
              const request = {
                orderNum: numberValue(values, "orderNum"),
                readingText: textValue(values, "readingText"),
                characterId: nullableNumberValue(values, "characterId"),
                isNarrator: booleanValue(values, "isNarrator"),
              };

              if (editingSection) {
                await updateDialogPageSection(editingSection.id, request);
              } else {
                await createDialogPageSection(selectedPage.id, request);
              }

              setEditingSection(undefined);
              await refresh();
            }}
          />
        </Drawer>
      )}

      {viewingDialog && (
        <DialogViewer
          dialog={viewingDialog}
          onClose={() => setViewingDialog(undefined)}
        />
      )}
    </AppLayout>
  );
}
