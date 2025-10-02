import axios from "axios";

export const BASE_URL = "http://localhost:5014/api/v1/";
export const register_url = `${BASE_URL}account/register`;
export const login_url = `${BASE_URL}account/login`;
export const create_recipe_url = `${BASE_URL}recipe/create`;
export const get_all_recipes_url = `${BASE_URL}recipe/`;
export const get_recipe_url = `${BASE_URL}recipe/get?id=`;

export const get_all_journeys_url = `${BASE_URL}journeys/`;
export const get_journey_url = `${BASE_URL}journeys/`;
export const create_journey_url = `${BASE_URL}journeys/`;

export const create_scene_url = `${BASE_URL}scenes/`;
export const get_scene_url = `${BASE_URL}scenes/`;

export const get_all_spells_url = `${BASE_URL}spells/`;
export const create_spell_url = `${BASE_URL}spells/`;

export const get_all_characters_url = `${BASE_URL}characters/`;
export const create_character_url = `${BASE_URL}characters/`;

export const get_all_items_url = `${BASE_URL}items/`;
export const create_items_url = `${BASE_URL}items/`;

type QueryParams = { [key: string]: string | number | boolean | undefined };
type ContentType = "application/json" | "multipart/form-data";

const AttachParamsAndHeaders = (
  token?: string,
  params?: QueryParams,
  contentType?: ContentType,
) => {
  return {
    headers: {
      "Content-Type":
        contentType === "application/json" || contentType === undefined
          ? "application/json"
          : "multipart/form-data",
      ...(token && { Authorization: `Bearer ${token}` }),
    },
    params: params,
  };
};

export const post = async (
  url: string,
  body: object,
  params?: QueryParams,
  token?: string,
  contentType?: ContentType,
) => {
  try {
    const config = AttachParamsAndHeaders(token, params, contentType);

    const response = await axios.post(url, body, config);
    return response.data;
  } catch (err) {
    console.error("Error posting data:", err);
    throw err;
  }
};

export const get = async (
  url: string,
  params?: QueryParams,
  token?: string,
  contentType?: ContentType,
) => {
  try {
    const config = AttachParamsAndHeaders(token, params, contentType);
    const response = await axios.get(url, config);
    return response.data;
  } catch (err) {
    console.error("Error getting data:", err);
    throw err;
  }
};

export const put = async (
  url: string,
  body: object,
  params?: QueryParams,
  token?: string,
) => {
  try {
    const config = AttachParamsAndHeaders(token, params);
    const response = await axios.put(url, body, config);
    return response.data;
  } catch (err) {
    console.error("Error putting data:", err);
    throw err;
  }
};

export const patch = async (
  url: string,
  body: object,
  params?: QueryParams,
  token?: string,
) => {
  try {
    const config = AttachParamsAndHeaders(token, params);
    const response = await axios.patch(url, body, config);
    return response.data;
  } catch (err) {
    console.error("Error patching data:", err);
    throw err;
  }
};

export const del = async (
  url: string,
  params?: QueryParams,
  token?: string,
) => {
  try {
    const config = AttachParamsAndHeaders(token, params);
    const response = await axios.delete(url, config);
    return response.data;
  } catch (err) {
    console.error("Error deleting data:", err);
    throw err;
  }
};

export async function postForm(url: string, form: FormData, token?: string) {
  const headers: Record<string, string> = {};
  if (token) headers.Authorization = `Bearer ${token}`;

  const res = await fetch(url, { method: "POST", body: form, headers });
  const text = await res.text(); // good for debugging
  if (!res.ok)
    throw new Error(
      `POST ${url} failed: ${res.status} ${res.statusText} — ${text}`,
    );
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}
