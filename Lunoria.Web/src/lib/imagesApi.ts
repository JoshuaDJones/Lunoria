import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";

export interface ImageUploadResult {
  photoUrl: string;
  fileName: string;
}

export async function uploadImage(
  file: File,
  name?: string,
): Promise<ImageUploadResult> {
  const { data } = await apiClient.post<ImageUploadResult>(
    "/Images",
    toFormData({ file, name }),
  );
  return data;
}

export async function deleteImage(imagePath: string): Promise<void> {
  await apiClient.delete("/Images", { params: { imagePath } });
}
