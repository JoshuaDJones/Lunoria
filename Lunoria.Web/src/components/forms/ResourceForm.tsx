import {
  useEffect,
  useRef,
  useState,
  type FormEvent,
  type ReactNode,
} from "react";
import { getApiError } from "@/lib/apiClient";

export type FormValue = string | boolean;
export type FormValues = Record<string, FormValue>;

export interface ResourceFormField {
  name: string;
  label: string;
  type?: "text" | "textarea" | "number" | "checkbox";
  required?: boolean;
}

interface ResourceFormProps {
  fields: ResourceFormField[];
  initialValues: FormValues;
  existingPhotoUrl?: string;
  requirePhoto?: boolean;
  onSubmit: (values: FormValues, photo?: File) => Promise<void>;
  children?: ReactNode;
}

export function ResourceForm({
  fields,
  initialValues,
  existingPhotoUrl,
  requirePhoto,
  onSubmit,
  children,
}: ResourceFormProps) {
  const [values, setValues] = useState(initialValues);
  const [photo, setPhoto] = useState<File>();
  const [photoPreviewUrl, setPhotoPreviewUrl] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const objectUrlRef = useRef("");

  useEffect(() => {
    return () => {
      if (objectUrlRef.current) {
        URL.revokeObjectURL(objectUrlRef.current);
      }
    };
  }, []);

  const handlePhotoChange = (file?: File) => {
    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = "";
    }

    setPhoto(file);

    if (file) {
      const objectUrl = URL.createObjectURL(file);
      objectUrlRef.current = objectUrl;
      setPhotoPreviewUrl(objectUrl);
    } else {
      setPhotoPreviewUrl("");
    }
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError("");
    setIsSubmitting(true);

    try {
      await onSubmit(values, photo);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={(event) => void handleSubmit(event)} className="space-y-5">
      {fields.map((field) => {
        const value = values[field.name];

        if (field.type === "checkbox") {
          return (
            <label
              key={field.name}
              className="flex items-center gap-3 text-sm font-medium text-content-secondary"
            >
              <input
                type="checkbox"
                checked={Boolean(value)}
                onChange={(event) =>
                  setValues((current) => ({
                    ...current,
                    [field.name]: event.target.checked,
                  }))
                }
                className="size-4 accent-brand"
              />
              {field.label}
            </label>
          );
        }

        const commonProps = {
          id: field.name,
          name: field.name,
          required: field.required,
          value: String(value ?? ""),
          onChange: (
            event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
          ) =>
            setValues((current) => ({
              ...current,
              [field.name]: event.target.value,
            })),
          className:
            "w-full rounded-lg border border-border bg-surface px-4 py-3 text-content outline-none transition focus:border-brand-hover focus:ring-2 focus:ring-brand-hover/20",
        };

        return (
          <label
            key={field.name}
            htmlFor={field.name}
            className="block text-sm font-medium text-content-secondary"
          >
            <span className="mb-2 block">{field.label}</span>
            {field.type === "textarea" ? (
              <textarea {...commonProps} rows={4} />
            ) : (
              <input {...commonProps} type={field.type ?? "text"} />
            )}
          </label>
        );
      })}

      {children}

      <label className="block text-sm font-medium text-content-secondary">
        <span className="mb-2 block">
          Photo{existingPhotoUrl ? " (leave empty to keep current photo)" : ""}
        </span>
        <input
          type="file"
          accept="image/jpeg,image/png,image/webp"
          required={requirePhoto}
          onChange={(event) => handlePhotoChange(event.target.files?.[0])}
          className="block w-full rounded-lg border border-border bg-surface px-4 py-3 text-sm text-content file:mr-4 file:rounded-md file:border-0 file:bg-brand file:px-3 file:py-2 file:font-semibold file:text-on-brand"
        />
      </label>

      {(photoPreviewUrl || existingPhotoUrl) && (
        <figure className="rounded-xl border border-border bg-surface p-3">
          <div className="mb-3 flex items-center justify-between gap-3">
            <figcaption className="text-sm font-semibold text-content-secondary">
              {photoPreviewUrl ? "Selected image preview" : "Current image"}
            </figcaption>
            {photoPreviewUrl && (
              <span className="rounded-full border border-brand-subtle/40 bg-brand/10 px-3 py-1 text-xs font-semibold text-brand-hover">
                Not saved yet
              </span>
            )}
          </div>
          <img
            src={photoPreviewUrl || existingPhotoUrl}
            alt={photoPreviewUrl ? "Selected unsaved preview" : "Current saved"}
            className="max-h-72 max-w-full rounded-lg object-contain"
          />
        </figure>
      )}

      {error && (
        <p className="text-sm text-danger" role="alert">
          {error}
        </p>
      )}

      <button
        type="submit"
        disabled={isSubmitting}
        className="w-full rounded-lg bg-brand px-5 py-3 font-semibold text-on-brand transition hover:bg-brand-hover disabled:cursor-not-allowed disabled:opacity-60"
      >
        {isSubmitting ? "Saving..." : "Save"}
      </button>
    </form>
  );
}
