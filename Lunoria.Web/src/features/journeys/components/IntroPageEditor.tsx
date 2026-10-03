import {
  useEffect,
  useLayoutEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
  type CSSProperties,
} from "react";
import { IntroEditorDivider } from "@/features/journeys/components/IntroEditorDivider";
import { PhotoDropzone } from "@/components/forms/PhotoDropzone";
import { Button, Input } from "@/components/ui";
import type { IntroPage, IntroPageType } from "@/features/journeys/types";
import {
  emptyIntroPageConfig,
  parseIntroPageConfig,
  type IntroPageConfig,
} from "@/features/journeys/introPageConfig";
import { IntroPageContent } from "@/features/journeys/components/IntroPageContent";
import { IntroPageCanvas } from "@/features/journeys/components/IntroPageCanvas";
import { getApiError } from "@/lib/apiClient";

interface IntroPageEditorProps {
  type: IntroPageType;
  page?: IntroPage;
  onSave: (config: IntroPageConfig, image?: File) => Promise<void>;
  onDirtyChange: (dirty: boolean) => void;
  onBusyChange: (busy: boolean) => void;
  pageNavigation?: ReactNode;
  previewToolbar?: ReactNode;
}

export function IntroPageEditor({
  type,
  page,
  onSave,
  onDirtyChange,
  onBusyChange,
  pageNavigation,
  previewToolbar,
}: IntroPageEditorProps) {
  const workspaceRef = useRef<HTMLFormElement>(null);
  const [workspaceWidth, setWorkspaceWidth] = useState(0);
  const [preferredWidth, setPreferredWidth] = useState(() => {
    try {
      const saved = Number(localStorage.getItem("lunoria.introEditorWidth"));
      return Number.isFinite(saved) && saved >= 320 ? saved : 416;
    } catch {
      return 416;
    }
  });
  const maxEditorWidth = Math.max(320, workspaceWidth - 340);
  const editorWidth = Math.max(320, Math.min(preferredWidth, maxEditorWidth));
  useLayoutEffect(() => {
    const element = workspaceRef.current;
    if (!element) return;
    const measure = () => setWorkspaceWidth(element.getBoundingClientRect().width);
    measure();
    const observer = new ResizeObserver(([entry]) =>
      setWorkspaceWidth(entry.contentRect.width),
    );
    observer.observe(element);
    return () => observer.disconnect();
  }, []);
  const resizeEditor = (width: number) => {
    const next = Math.round(Math.max(320, Math.min(width, maxEditorWidth)));
    setPreferredWidth(next);
    try {
      localStorage.setItem("lunoria.introEditorWidth", String(next));
    } catch {
      /* Storage may be unavailable. */
    }
  };
  const [config, setConfig] = useState<IntroPageConfig>(() =>
    page
      ? parseIntroPageConfig(page.config)
      : structuredClone(emptyIntroPageConfig),
  );
  const [image, setImage] = useState<File>();
  const imageInputRef = useRef<HTMLInputElement>(null);
  const [imageError, setImageError] = useState("");
  const [error, setError] = useState("");
  const [isSaving, setIsSaving] = useState(false);
  const previewUrl = useMemo(
    () => (image ? URL.createObjectURL(image) : page?.previewPhotoUrl),
    [image, page?.previewPhotoUrl],
  );

  useEffect(
    () => () => {
      if (image && previewUrl) URL.revokeObjectURL(previewUrl);
    },
    [image, previewUrl],
  );

  const save = async () => {
    if (isSaving) return;
    if (!image && !page?.previewPhotoUrl) {
      setImageError("An image is required.");
      return;
    }
    setIsSaving(true);
    onBusyChange(true);
    setError("");
    try {
      await onSave(config, image);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsSaving(false);
      onBusyChange(false);
    }
  };

  return (
    <form
      ref={workspaceRef}
      id="intro-page-editor"
      onSubmit={(event) => {
        event.preventDefault();
        void save();
      }}
      aria-busy={isSaving}
      style={{ "--intro-editor-width": `${editorWidth}px` } as CSSProperties}
      className="grid min-w-0 shrink-0 gap-5 xl:gap-0 xl:min-h-0 xl:flex-1 xl:grid-cols-[minmax(0,1fr)_20px_minmax(0,var(--intro-editor-width))] xl:grid-rows-[minmax(0,1fr)]"
    >
      <section
        aria-label="Live preview"
        className="flex min-h-0 min-w-0 flex-col gap-4 overflow-hidden rounded-2xl border border-border bg-surface/80 p-3"
      >
        {previewToolbar}
        <div className="min-h-0 min-w-0 xl:flex-1">
          <IntroPageCanvas type={type} config={config} imageUrl={previewUrl} />
        </div>
        {pageNavigation}
      </section>
      <IntroEditorDivider
        width={editorWidth}
        min={320}
        max={maxEditorWidth}
        disabled={isSaving}
        onResize={resizeEditor}
      />
      <section
        id="intro-page-settings"
        aria-label="Page settings"
        className="min-w-0 rounded-2xl border border-border bg-surface/95 p-5 xl:overflow-y-auto"
      >
        <div className="mb-5 flex flex-wrap items-center justify-between gap-2">
          <h2 className="text-lg font-semibold">Page Content</h2>
        </div>
        <fieldset
          disabled={isSaving}
          inert={isSaving}
          className="min-w-0 space-y-5"
        >
          {previewUrl ? (
            <div className="flex items-center gap-4">
              <img
                src={previewUrl}
                alt=""
                className="h-16 w-20 rounded-lg object-cover"
              />
              <div>
                <Button onClick={() => imageInputRef.current?.click()}>
                  Replace Image
                </Button>
                <p className="mt-2 text-xs text-content-muted">
                  JPEG, PNG, or WebP
                </p>
              </div>
              <input
                ref={imageInputRef}
                type="file"
                className="hidden"
                accept="image/jpeg,image/png,image/webp"
                onChange={(event) => {
                  const file = event.target.files?.[0];
                  event.target.value = "";
                  if (!file) return;
                  if (
                    !["image/jpeg", "image/png", "image/webp"].includes(
                      file.type,
                    )
                  ) {
                    setImageError("Photo must be a JPEG, PNG, or WebP image.");
                    return;
                  }
                  setImage(file);
                  setImageError("");
                  onDirtyChange(true);
                }}
              />
            </div>
          ) : (
            <PhotoDropzone
              file={image}
              hasExistingPhoto={Boolean(page?.previewPhotoUrl)}
              onChange={(file) => {
                setImage(file);
                setImageError("");
                onDirtyChange(true);
              }}
              onError={setImageError}
            />
          )}
          {imageError && (
            <p role="alert" className="text-sm text-danger">
              {imageError}
            </p>
          )}
          <details className="text-sm text-content-secondary">
            <summary className="cursor-pointer">
              Image description (accessibility)
            </summary>
            <label className="mt-3 block">
              <span className="sr-only">Image description</span>
              <Input
                value={config.imageAlt}
                onChange={(event) => {
                  setConfig((current) => ({
                    ...current,
                    imageAlt: event.target.value,
                  }));
                  onDirtyChange(true);
                }}
                placeholder="Describe the image for accessibility"
              />
            </label>
          </details>
          <div>
            <span className="mb-2 block text-sm font-medium text-content-secondary">
              Text
            </span>
            <div inert={isSaving}>
              <IntroPageContent
                editable
                content={config.content}
                onChange={(content) => {
                  setConfig((current) => ({ ...current, content }));
                  onDirtyChange(true);
                }}
              />
            </div>
          </div>
        </fieldset>
        {error && (
          <p
            className="mt-4 rounded-lg border border-danger/40 p-3 text-danger"
            role="alert"
          >
            {error}
          </p>
        )}
      </section>
    </form>
  );
}
