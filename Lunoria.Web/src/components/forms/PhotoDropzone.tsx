import { useRef, useState, type DragEvent } from "react";
import clsx from "clsx";
import { InfoTooltip } from "@/components/ui/InfoTooltip";

const acceptedTypes = new Set(["image/jpeg", "image/png", "image/webp"]);

interface PhotoDropzoneProps {
  file?: File;
  hasExistingPhoto?: boolean;
  tooltip?: string;
  label?: string;
  disabled?: boolean;
  onChange: (file?: File) => void;
  onError: (message: string) => void;
}

export function PhotoDropzone({
  file,
  hasExistingPhoto,
  tooltip,
  label = "Photo",
  disabled = false,
  onChange,
  onError,
}: PhotoDropzoneProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [isDragging, setIsDragging] = useState(false);

  const selectFile = (nextFile?: File) => {
    if (disabled || !nextFile) return;

    if (!acceptedTypes.has(nextFile.type)) {
      onError("Photo must be a JPEG, PNG, or WebP image.");
      return;
    }

    onError("");
    onChange(nextFile);
  };

  const handleDrop = (event: DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    setIsDragging(false);
    selectFile(event.dataTransfer.files[0]);
  };

  return (
    <div>
      <div className="mb-2 flex flex-wrap items-center gap-2 text-sm font-medium text-content-secondary">
        <span className="inline-flex items-center gap-2 leading-5">
          <span>{label}</span>
          {tooltip && (
            <InfoTooltip text={tooltip} label={`${label} information`} plain />
          )}
        </span>
        {hasExistingPhoto && <span>(leave empty to keep current photo)</span>}
      </div>
      <div
        onDragEnter={(event) => {
          event.preventDefault();
          if (!disabled) setIsDragging(true);
        }}
        onDragOver={(event) => event.preventDefault()}
        onDragLeave={(event) => {
          if (!event.currentTarget.contains(event.relatedTarget as Node)) {
            setIsDragging(false);
          }
        }}
        onDrop={handleDrop}
        className={clsx(
          "rounded-xl border-2 border-dashed bg-surface p-6 text-center transition",
          isDragging
            ? "border-brand bg-brand/10"
            : "border-border hover:border-brand-subtle/70",
        )}
      >
        <input
          ref={inputRef}
          type="file"
          aria-label={label}
          disabled={disabled}
          accept="image/jpeg,image/png,image/webp"
          onChange={(event) => selectFile(event.target.files?.[0])}
          className="sr-only"
        />
        <p className="font-medium text-content">
          {isDragging ? "Drop the image here" : "Drag and drop an image here"}
        </p>
        <p className="mt-1 text-sm text-content-muted">JPEG, PNG, or WebP</p>
        <button
          type="button"
          disabled={disabled}
          onClick={() => {
            if (inputRef.current) {
              inputRef.current.value = "";
              inputRef.current.click();
            }
          }}
          className="mt-4 rounded-lg border border-brand-subtle/50 px-4 py-2 text-sm font-semibold text-brand-hover transition hover:bg-brand/10"
        >
          Browse files
        </button>
        {file && (
          <p className="mt-3 truncate text-sm text-content-secondary">
            Selected: {file.name}
          </p>
        )}
      </div>
    </div>
  );
}
