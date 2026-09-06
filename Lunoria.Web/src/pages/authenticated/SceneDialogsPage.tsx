import {
  useEffect,
  useRef,
  useState,
  type FormEvent,
  type ReactNode,
} from "react";
import { Link, Navigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts/AppLayout";
import {
  booleanValue,
  nullableNumberValue,
  numberValue,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import {
  ApiLoadError,
  Button,
  Drawer,
  FormField,
  Input,
  Select,
} from "@/components/ui";
import {
  CharacterSelectionField,
  CharacterType,
  listCharacters,
} from "@/features/characters";
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
  DialogPageType,
  type SceneDialog,
} from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";
import { useConfirmDialog, useToast } from "@/app/providers";

const dialogFields: ResourceFormField[] = [
  { name: "title", label: "Title", required: true },
];

const sectionFields: ResourceFormField[] = [
  { name: "orderNum", label: "Order", type: "number", required: true },
  {
    name: "readingText",
    label: "Reading text",
    type: "textarea",
    required: true,
  },
  { name: "isNarrator", label: "Narrator", type: "checkbox" },
  {
    name: "characterId",
    label: "Character",
  },
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

const IMAGE_ACCEPT =
  ".jpg,.jpeg,.png,.gif,.webp,image/jpeg,image/png,image/gif,image/webp";
const VIDEO_ACCEPT = ".mp4,video/mp4";
const MAX_IMAGE_BYTES = 10 * 1024 * 1024;
const MAX_VIDEO_BYTES = 100 * 1024 * 1024;

function DialogPageForm({
  page,
  defaultOrderNum,
  onSubmit,
}: {
  page?: DialogPage | null;
  defaultOrderNum: number;
  onSubmit: (input: {
    orderNum: number;
    pageType: DialogPageType;
    media?: File;
  }) => Promise<void>;
}) {
  const [orderNum, setOrderNum] = useState(page?.orderNum ?? defaultOrderNum);
  const [pageType, setPageType] = useState(
    page?.pageType ?? DialogPageType.Image,
  );
  const [media, setMedia] = useState<File>();
  const [previewUrl, setPreviewUrl] = useState("");
  const [error, setError] = useState("");
  const [hasInvalidMedia, setHasInvalidMedia] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const objectUrlRef = useRef("");

  useEffect(() => {
    return () => {
      if (objectUrlRef.current) URL.revokeObjectURL(objectUrlRef.current);
    };
  }, []);

  const chooseMedia = (file?: File) => {
    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = "";
    }

    setMedia(file);
    setPreviewUrl("");
    setError("");
    setHasInvalidMedia(false);

    if (file) {
      const maximumBytes =
        pageType === DialogPageType.Video
          ? MAX_VIDEO_BYTES
          : MAX_IMAGE_BYTES;
      if (file.size > maximumBytes) {
        setError(
          `${pageType === DialogPageType.Video ? "Videos" : "Images"} must be ${maximumBytes / 1024 / 1024} MB or smaller.`,
        );
        setMedia(undefined);
        setHasInvalidMedia(true);
        return;
      }

      const url = URL.createObjectURL(file);
      objectUrlRef.current = url;
      setPreviewUrl(url);
    }
  };

  const changePageType = (nextType: DialogPageType) => {
    if (
      nextType === DialogPageType.Video &&
      (page?.dialogPageSections?.length ?? 0) > 0
    ) {
      setError("Remove all dialog sections before changing this page to video.");
      return;
    }

    setPageType(nextType);
    chooseMedia(undefined);
  };

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (hasInvalidMedia) {
      return;
    }

    setError("");

    if (!Number.isInteger(orderNum) || orderNum < 1) {
      setError("Order must be a positive whole number.");
      return;
    }

    if ((!page || pageType !== page.pageType) && !media) {
      setError(
        `Select ${pageType === DialogPageType.Video ? "an MP4 video" : "an image"}.`,
      );
      return;
    }

    setIsSubmitting(true);
    try {
      await onSubmit({ orderNum, pageType, media });
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const displayedMediaUrl =
    previewUrl || (page?.pageType === pageType ? page.mediaUrl : "");

  return (
    <form className="space-y-5" onSubmit={(event) => void submit(event)}>
      <FormField htmlFor="dialog-page-order" label="Order">
        <Input
          id="dialog-page-order"
          type="number"
          min={1}
          required
          value={orderNum}
          onChange={(event) => setOrderNum(Number(event.target.value))}
        />
      </FormField>

      <FormField htmlFor="dialog-page-type" label="Page type">
        <Select
          id="dialog-page-type"
          value={pageType}
          onChange={(event) =>
            changePageType(Number(event.target.value) as DialogPageType)
          }
        >
          <option value={DialogPageType.Image}>Image with dialog</option>
          <option value={DialogPageType.Video}>Video</option>
        </Select>
      </FormField>

      <FormField
        htmlFor="dialog-page-media"
        label={pageType === DialogPageType.Video ? "MP4 video" : "Image"}
      >
        <Input
          key={pageType}
          id="dialog-page-media"
          type="file"
          accept={pageType === DialogPageType.Video ? VIDEO_ACCEPT : IMAGE_ACCEPT}
          required={!page || pageType !== page.pageType}
          onChange={(event) => chooseMedia(event.target.files?.[0])}
        />
      </FormField>

      {displayedMediaUrl && (
        <figure className="rounded-xl border border-border bg-surface p-3">
          <figcaption className="mb-3 text-sm font-semibold text-content-secondary">
            {previewUrl ? "Selected media preview" : "Current media"}
          </figcaption>
          {pageType === DialogPageType.Video ? (
            <video
              src={displayedMediaUrl}
              controls
              playsInline
              preload="metadata"
              className="max-h-72 w-full rounded-lg bg-canvas object-contain"
            />
          ) : (
            <img
              src={displayedMediaUrl}
              alt=""
              className="max-h-72 max-w-full rounded-lg object-contain"
            />
          )}
        </figure>
      )}

      {pageType === DialogPageType.Video && (
        <p className="text-sm text-content-muted">
          Video pages cannot contain dialog sections. Use an H.264 MP4 with
          fast-start metadata, up to 100 MB.
        </p>
      )}

      {error && (
        <p className="text-sm text-danger" role="alert">
          {error}
        </p>
      )}

      <Button
        type="submit"
        variant="primary"
        size="lg"
        className="w-full"
        disabled={isSubmitting}
      >
        {isSubmitting ? "Saving..." : "Save"}
      </Button>
    </form>
  );
}

export function SceneDialogsPage() {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const {
    seriesId: seriesIdParam,
    journeyId: journeyIdParam,
    sceneId: sceneIdParam,
  } = useParams();
  const seriesId = Number(seriesIdParam);
  const journeyId = Number(journeyIdParam);
  const sceneId = Number(sceneIdParam);

  const [dialogs, setDialogs] = useState<SceneDialog[]>([]);
  const [selectedDialogId, setSelectedDialogId] = useState<number>();
  const [selectedPageId, setSelectedPageId] = useState<number>();
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [editingDialog, setEditingDialog] = useState<
    SceneDialog | null | undefined
  >();
  const [editingPage, setEditingPage] = useState<
    DialogPage | null | undefined
  >();
  const [editingSection, setEditingSection] = useState<
    DialogPageSection | null | undefined
  >();
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
    !Number.isInteger(seriesId) ||
    seriesId <= 0 ||
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
  const sections =
    selectedPage?.pageType === DialogPageType.Image
      ? [...(selectedPage.dialogPageSections ?? [])].sort(
          (a, b) => a.orderNum - b.orderNum,
        )
      : [];

  const refresh = async () => {
    setDialogs(await listSceneDialogs(sceneId));
    setError("");
  };

  const retryLoad = async () => {
    setIsLoading(true);
    setError("");

    try {
      setDialogs(await listSceneDialogs(sceneId));
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  const remove = async (
    message: string,
    successMessage: string,
    operation: () => Promise<void>,
  ) => {
    const confirmed = await confirm({
      title: message,
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) {
      return;
    }

    try {
      await operation();
      await refresh();
      toast.success(successMessage);
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
              to={`/series/${seriesId}/journeys/${journeyId}`}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to journey
            </Link>
          </div>
        </header>

        {!isLoading && error && (
          <div className="mb-4">
            <ApiLoadError error={error} onRetry={retryLoad} />
          </div>
        )}

        {isLoading ? (
          <p className="text-content-secondary" role="status">
            Loading dialog editor...
          </p>
        ) : !error ? (
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
                      void remove(
                        `Delete "${dialog.title}"?`,
                        `Dialog "${dialog.title}" was deleted.`,
                        () => deleteSceneDialog(dialog.id),
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
                  {page.pageType === DialogPageType.Video ? (
                    <video
                      src={page.mediaUrl}
                      muted
                      playsInline
                      preload="metadata"
                      className="mb-3 h-24 w-32 rounded-lg bg-canvas object-contain"
                    />
                  ) : (
                    <img
                      src={page.mediaUrl}
                      alt=""
                      className="mb-3 h-24 w-32 rounded-lg bg-canvas object-contain"
                    />
                  )}
                  <h3 className="font-semibold text-content">
                    Page {page.orderNum}
                  </h3>
                  <p className="mt-1 text-xs text-content-muted">
                    {page.pageType === DialogPageType.Video
                      ? "Video"
                      : `${page.dialogPageSections?.length ?? 0} sections`}
                  </p>
                  <ItemActions
                    onEdit={() => setEditingPage(page)}
                    onDelete={() =>
                      void remove(
                        `Delete page ${page.orderNum}?`,
                        `Page ${page.orderNum} was deleted.`,
                        () => deleteDialogPage(page.id),
                      )
                    }
                  />
                </article>
              ))}
            </EditorColumn>

            <EditorColumn
              title="Sections"
              addLabel="Add section"
              canAdd={Boolean(
                selectedPage && selectedPage.pageType === DialogPageType.Image,
              )}
              onAdd={() => setEditingSection(null)}
              emptyMessage={
                selectedPage?.pageType === DialogPageType.Video
                  ? "Video pages do not have dialog sections."
                  : selectedPage
                    ? "No sections yet."
                    : "Select a page first."
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
                        : (section.character?.name ?? "No character")}
                    </span>
                  </div>
                  <p className="mt-3 whitespace-pre-wrap text-sm text-content-secondary">
                    {section.readingText}
                  </p>
                  <ItemActions
                    onEdit={() => setEditingSection(section)}
                    onDelete={() =>
                      void remove(
                        `Delete section ${section.orderNum}?`,
                        `Section ${section.orderNum} was deleted.`,
                        () => deleteDialogPageSection(section.id),
                      )
                    }
                  />
                </article>
              ))}
            </EditorColumn>
          </div>
        ) : null}
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
                toast.success(`Dialog "${title}" was updated.`);
              } else {
                await createSceneDialog(sceneId, title);
                toast.success(`Dialog "${title}" was created.`);
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
          <DialogPageForm
            page={editingPage}
            defaultOrderNum={pages.length + 1}
            onSubmit={async ({ orderNum, pageType, media }) => {
              if (editingPage) {
                await updateDialogPage(editingPage.id, {
                  orderNum,
                  pageType,
                  media,
                });
                toast.success(`Page ${orderNum} was updated.`);
              } else {
                await createDialogPage(
                  selectedDialog.id,
                  orderNum,
                  pageType,
                  media!,
                );
                toast.success(`Page ${orderNum} was created.`);
              }

              setEditingPage(undefined);
              await refresh();
            }}
          />
        </Drawer>
      )}

      {editingSection !== undefined &&
        selectedPage?.pageType === DialogPageType.Image && (
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
                orderNum: String(
                  editingSection?.orderNum ?? sections.length + 1,
                ),
                readingText: editingSection?.readingText ?? "",
                characterId: String(editingSection?.character?.id ?? ""),
                isNarrator: editingSection?.isNarrator ?? false,
              }}
              customFields={{
                characterId: ({ field, value, values, setValue }) => {
                  const selectedId = Number(value);

                  return (
                    <CharacterSelectionField
                      id={field.name}
                      selectedId={
                        Number.isInteger(selectedId) && selectedId > 0
                          ? selectedId
                          : null
                      }
                      initialSelectedCharacter={editingSection?.character}
                      pickerTitle="Choose dialog character"
                      disabled={Boolean(values.isNarrator)}
                      disabledMessage="Narrator sections do not use a character."
                      loadCharacters={() =>
                        listCharacters({ typeFilter: CharacterType.Any })
                      }
                      onChange={(characterId) =>
                        setValue(
                          characterId === null ? "" : String(characterId),
                        )
                      }
                    />
                  );
                },
              }}
              onSubmit={async (values) => {
                const isNarrator = booleanValue(values, "isNarrator");
                const request = {
                  orderNum: numberValue(values, "orderNum"),
                  readingText: textValue(values, "readingText"),
                  characterId: isNarrator
                    ? null
                    : nullableNumberValue(values, "characterId"),
                  isNarrator,
                };

                if (editingSection) {
                  await updateDialogPageSection(editingSection.id, request);
                  toast.success(`Section ${request.orderNum} was updated.`);
                } else {
                  await createDialogPageSection(selectedPage.id, request);
                  toast.success(`Section ${request.orderNum} was created.`);
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
