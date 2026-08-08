import { useEffect, useMemo, useState } from "react";
import { PhotoDropzone } from "@/components/forms/PhotoDropzone";
import { Button, Input } from "@/components/ui";
import type { IntroPage, IntroPageType } from "@/features/journeys/types";
import {
  emptyIntroPageConfig,
  parseIntroPageConfig,
  type IntroPageConfig,
} from "@/features/journeys/introPageConfig";
import { IntroPageContent } from "./IntroPageContent";
import { IntroPagePreview } from "./IntroPagePreview";
import { getApiError } from "@/lib/apiClient";

interface IntroPageEditorProps {
  type: IntroPageType;
  page?: IntroPage;
  onSave: (config: IntroPageConfig, image?: File) => Promise<void>;
  onCancel: () => void;
}

export function IntroPageEditor({
  type,
  page,
  onSave,
  onCancel,
}: IntroPageEditorProps) {
  const [config, setConfig] = useState<IntroPageConfig>(() =>
    page
      ? parseIntroPageConfig(page.config)
      : structuredClone(emptyIntroPageConfig),
  );
  const [image, setImage] = useState<File>();
  const [imageError, setImageError] = useState("");
  const [error, setError] = useState("");
  const [isSaving, setIsSaving] = useState(false);
  const previewUrl = useMemo(
    () => (image ? URL.createObjectURL(image) : page?.previewPhotoUrl),
    [image, page?.previewPhotoUrl],
  );

  useEffect(() => {
    return () => {
      if (image && previewUrl) URL.revokeObjectURL(previewUrl);
    };
  }, [image, previewUrl]);

  const save = async () => {
    if (!image && !page?.previewPhotoUrl) {
      setImageError("An image is required.");
      return;
    }

    setIsSaving(true);
    setError("");

    try {
      await onSave(config, image);
    } catch (requestError) {
      setError(getApiError(requestError).message);
      setIsSaving(false);
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h3 className="mb-3 text-lg font-semibold text-content">
          Live preview
        </h3>
        <IntroPagePreview type={type} config={config} imageUrl={previewUrl} />
      </div>

      <PhotoDropzone
        file={image}
        hasExistingPhoto={Boolean(page?.previewPhotoUrl)}
        onChange={setImage}
        onError={setImageError}
      />
      {imageError && <p className="text-sm text-danger">{imageError}</p>}

      <label className="block">
        <span className="mb-2 block text-sm font-medium text-content-secondary">
          Image description
        </span>
        <Input
          value={config.imageAlt}
          onChange={(event) =>
            setConfig((current) => ({
              ...current,
              imageAlt: event.target.value,
            }))
          }
          placeholder="Describe the image for accessibility"
        />
      </label>

      <div>
        <span className="mb-2 block text-sm font-medium text-content-secondary">
          Page content
        </span>
        <IntroPageContent
          editable
          content={config.content}
          onChange={(content) =>
            setConfig((current) => ({ ...current, content }))
          }
        />
      </div>

      {error && (
        <p
          className="rounded-lg border border-danger/40 p-3 text-danger"
          role="alert"
        >
          {error}
        </p>
      )}

      <div className="flex justify-end gap-3 border-t border-border pt-4">
        <Button onClick={onCancel} disabled={isSaving} size="lg">
          Cancel
        </Button>
        <Button
          onClick={() => void save()}
          disabled={isSaving}
          variant="primary"
          size="lg"
        >
          {isSaving ? "Saving..." : "Save Page"}
        </Button>
      </div>
    </div>
  );
}
