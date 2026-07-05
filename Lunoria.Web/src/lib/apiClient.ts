import axios, { AxiosError } from "axios";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

if (!apiBaseUrl) {
  throw new Error("VITE_API_BASE_URL is not defined");
}

const AUTH_TOKEN_KEY = "auth_token";

export interface ApiError {
  code: string;
  message: string;
}

export interface ApiResult<T = never> {
  success: boolean;
  value: T | null;
  error: ApiError;
}

export const apiClient = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    Accept: "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(AUTH_TOKEN_KEY);

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export function setAccessToken(token?: string): void {
  if (token) {
    localStorage.setItem(AUTH_TOKEN_KEY, token);
  } else {
    localStorage.removeItem(AUTH_TOKEN_KEY);
  }
}

export function getAccessToken(): string | null {
  return localStorage.getItem(AUTH_TOKEN_KEY);
}

export function getApiError(error: unknown): ApiError {
  if (error instanceof AxiosError) {
    const responseError = error.response?.data as Partial<ApiError> | undefined;

    return {
      code:
        responseError?.code ?? `Http.${error.response?.status ?? "Unknown"}`,
      message: responseError?.message ?? error.message,
    };
  }

  return {
    code: "Client.Unknown",
    message:
      error instanceof Error ? error.message : "An unknown error occurred.",
  };
}

export function unwrapApiResult<T>(result: ApiResult<T>): T {
  if (!result.success || result.value === null) {
    throw result.error;
  }

  return result.value;
}
