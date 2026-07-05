import type { FormValues } from "@/components/forms/ResourceForm";

export function textValue(values: FormValues, name: string): string {
  return String(values[name] ?? "");
}

export function numberValue(values: FormValues, name: string): number {
  return Number(values[name] ?? 0);
}

export function nullableNumberValue(
  values: FormValues,
  name: string,
): number | null {
  const value = values[name];
  return value === "" || value === undefined ? null : Number(value);
}

export function booleanValue(values: FormValues, name: string): boolean {
  return Boolean(values[name]);
}

export function requiredPhoto(photo?: File): File {
  if (!photo) {
    throw new Error("A photo is required.");
  }

  return photo;
}
