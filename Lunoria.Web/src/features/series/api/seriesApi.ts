import { ListSeriesParams, Series, UpdateSeriesInput, CreateSeriesInput } from "../types";
import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";

export async function listSeries(
    params?: ListSeriesParams,
): Promise<Series[]> {
    const { data } = await apiClient.get<Series[]>("/Series", { params });
    return data;
}

export async function getSeries(id: number): Promise<Series> {
    const { data } = await apiClient.get<Series>(`/Series/${id}`);
    return data;
}

export async function createSeries(
    input: CreateSeriesInput,
): Promise<Series> {
    const { data } = await apiClient.post<Series>("/Series", toFormData(input));
    return data;
}

export async function updateSeries(
    id: number,
    input: UpdateSeriesInput,
): Promise<Series> {
    const { data } = await apiClient.put<Series>(`/Series/${id}`, toFormData(input));
    return data;
}

export async function deleteSeries(id: number): Promise<void> {
    await apiClient.delete(`/Series/${id}`);
}