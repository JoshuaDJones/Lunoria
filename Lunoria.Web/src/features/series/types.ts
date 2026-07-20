import type { Journey } from "@/features/journeys/types";

export interface Series{
    id: number;
    name: string;
    description: string | null;
    photoUrl: string | null;
    fileName: string | null;
    CreatedAt: string;
    UpdatedAt: string;
    journeys: Journey[];
}

export interface CreateSeriesInput {
  name: string;
  description?: string;
  photo?: File;
}

export interface UpdateSeriesInput {
  name: string;
  description?: string;
  photo?: File;
}

export interface ListSeriesParams {
  skip?: number;
  take?: number;
}