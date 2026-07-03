type FormValue =
  | string
  | number
  | boolean
  | File
  | null
  | undefined
  | readonly (string | number)[];

export function toFormData<T extends object>(values: T): FormData {
  const formData = new FormData();

  for (const [key, value] of Object.entries(values) as [string, FormValue][]) {
    if (value === null || value === undefined) {
      continue;
    }

    if (Array.isArray(value)) {
      for (const item of value) {
        formData.append(key, String(item));
      }
      continue;
    }

    formData.append(key, value instanceof File ? value : String(value));
  }

  return formData;
}
