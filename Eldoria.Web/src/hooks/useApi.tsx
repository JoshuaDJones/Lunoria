import { useCallback } from "react";
import axios from "axios";
import { useAuth } from "../providers/AuthProvider";

export const BASE_URL = import.meta.env.VITE_API_BASE_URL;

if (!BASE_URL) {
  throw new Error("VITE_API_BASE_URL is not defined");
}

type QueryParams = { [key: string]: string | number | boolean | undefined };
type ContentType = "application/json" | "multipart/form-data";

const AttachParamsAndHeaders = (
  token: string,
  params?: QueryParams,
  contentType?: ContentType,
) => {
  return {
    headers: {
      "Content-Type":
        contentType === "application/json" || contentType === undefined
          ? "application/json"
          : "multipart/form-data",
      Authorization: `Bearer ${token}`,
    },
    params,
  };
};

export function useApi() {
  const { token } = useAuth();

  const post = useCallback(
    async (
      url: string,
      body: object,
      params?: QueryParams,
      contentType?: ContentType,
    ) => {
      const config = AttachParamsAndHeaders(token!, params, contentType);
      const response = await axios.post(url, body, config);
      return response.data;
    },
    [token],
  );

  const get = useCallback(
    async (url: string, params?: QueryParams, contentType?: ContentType) => {
      const config = AttachParamsAndHeaders(token!, params, contentType);
      const response = await axios.get(url, config);
      return response.data;
    },
    [token],
  );

  const put = useCallback(
    async (url: string, body: object, params?: QueryParams) => {
      const config = AttachParamsAndHeaders(token!, params);
      const response = await axios.put(url, body, config);
      return response.data;
    },
    [token],
  );

  const patch = useCallback(
    async (url: string, body: object, params?: QueryParams) => {
      const config = AttachParamsAndHeaders(token!, params);
      const response = await axios.patch(url, body, config);
      return response.data;
    },
    [token],
  );

  const del = useCallback(
    async (url: string, params?: QueryParams) => {
      const config = AttachParamsAndHeaders(token!, params);
      const response = await axios.delete(url, config);
      return response.data;
    },
    [token],
  );

  const postForm = useCallback(
    async (url: string, form: FormData) => {
      const headers: Record<string, string> = {
        Authorization: `Bearer ${token}`,
      };

      const res = await fetch(url, { method: "POST", body: form, headers });
      const text = await res.text();
      if (!res.ok)
        throw new Error(
          `POST ${url} failed: ${res.status} ${res.statusText} — ${text}`,
        );
      try {
        return JSON.parse(text);
      } catch {
        return text;
      }
    },
    [token],
  );

  const putForm = useCallback(
    async (url: string, form: FormData) => {
      const headers: Record<string, string> = {
        Authorization: `Bearer ${token}`,
      };

      const res = await fetch(url, { method: "PUT", body: form, headers });
      const text = await res.text();
      if (!res.ok)
        throw new Error(
          `PUT ${url} failed: ${res.status} ${res.statusText} — ${text}`,
        );
      try {
        return JSON.parse(text);
      } catch {
        return text;
      }
    },
    [token],
  );

  return { get, post, put, patch, del, postForm, putForm };
}
