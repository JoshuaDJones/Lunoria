import { useRef, useState, type DragEvent } from "react";
import clsx from "clsx";

const acceptedTypes = new Set(["image/jpeg", "image/png", "image/webp"]);

interface PhotoDropzoneProps {
  file?: File;
  hasExistingPhoto?: boolean;
  onChange: (file?: File) => void;
  onError: (message: string) => void;
}

export function PhotoDropzone({
  file,
  hasExistingPhoto,
  onChange,
  onError,
}: PhotoDropzoneProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [isDragging, setIsDragging] = useState(false);

  const selectFile = (nextFile?: File) => {
    if (!nextFile) return;

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
      <span className="mb-2 block text-sm font-medium text-content-secondary">
        Photo
        {hasExistingPhoto ? " (leave empty to keep current photo)" : ""}
      </span>
      <div
        onDragEnter={(event) => {
          event.preventDefault();
          setIsDragging(true);
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
          accept="image/jpeg,image/png,image/webp"
          onChange={(event) => selectFile(event.target.files?.[0])}
          className="sr-only"
        />
        <p className="font-medium text-content">
          {isDragging ? "Drop the image here" : "Drag and drop an image here"}
        </p>
        <p className="mt-1 text-sm text-content-muted">
          JPEG, PNG, or WebP
        </p>
        <button
          type="button"
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
