import { useEffect, useRef, useState } from "react";
import { Button } from "@/components/ui";
import { PhotoDropzone } from "@/components/forms/PhotoDropzone";

interface Props {
  label: string;
  tooltip?: string;
  file?: File;
  existingUrl?: string | null;
  disabled: boolean;
  allowRemove?: boolean;
  removeExisting?: boolean;
  onRemoveExisting?: (remove: boolean) => void;
  onChange: (file?: File) => void;
}

export function SceneImageField({
  label,
  tooltip,
  file,
  existingUrl,
  disabled,
  allowRemove,
  removeExisting,
  onRemoveExisting,
  onChange,
}: Props) {
  const [preview, setPreview] = useState("");
  const [error, setError] = useState("");
  const objectUrl = useRef("");
  useEffect(
    () => () => {
      if (objectUrl.current) URL.revokeObjectURL(objectUrl.current);
    },
    [],
  );
  const change = (next?: File) => {
    if (disabled) return;
    if (objectUrl.current) URL.revokeObjectURL(objectUrl.current);
    objectUrl.current = next ? URL.createObjectURL(next) : "";
    setPreview(objectUrl.current);
    setError("");
    if (next) onRemoveExisting?.(false);
    onChange(next);
  };
  const imageUrl = preview || (!removeExisting ? existingUrl : undefined);
  return (
    <div className="space-y-3">
      <PhotoDropzone
        label={label}
        tooltip={tooltip}
        file={file}
        hasExistingPhoto={!!existingUrl && !removeExisting}
        disabled={disabled}
        onChange={change}
        onError={setError}
      />
      {error && (
        <p role="alert" className="text-sm text-danger">
          {error}
        </p>
      )}
      {imageUrl && (
        <figure className="rounded-xl border border-border bg-surface p-3">
          <figcaption className="mb-2 text-sm text-content-muted">
            {preview ? "Selected image preview" : "Current image"}
          </figcaption>
          <img
            src={imageUrl}
            alt={`${label} preview`}
            className="max-h-56 max-w-full rounded-lg object-contain"
          />
          {file ? (
            <Button
              size="sm"
              className="mt-3"
              disabled={disabled}
              onClick={() => change(undefined)}
            >
              Remove selected image
            </Button>
          ) : (
            allowRemove && (
              <Button
                size="sm"
                className="mt-3"
                disabled={disabled}
                onClick={() => onRemoveExisting?.(true)}
              >
                Remove current image
              </Button>
            )
          )}
        </figure>
      )}
      {removeExisting && (
        <div className="flex flex-wrap items-center justify-between gap-2 text-sm text-content-secondary">
          <p>The grid background will be removed when you save.</p>
          <Button
            size="sm"
            disabled={disabled}
            onClick={() => onRemoveExisting?.(false)}
          >
            Undo
          </Button>
        </div>
      )}
    </div>
  );
}
