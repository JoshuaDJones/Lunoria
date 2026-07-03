import { apiClient } from "@/lib/apiClient";
import type {
  AuthenticationToken,
  LoginRequest,
  RegisterRequest,
} from "@/features/auth/types";

export async function login(request: LoginRequest): Promise<AuthenticationToken> {
  const { data } = await apiClient.post<AuthenticationToken>("/Auth/login", request);
  return data;
}

export async function register(
  request: RegisterRequest,
): Promise<AuthenticationToken> {
  const { data } = await apiClient.post<AuthenticationToken>(
    "/Auth/register",
    request,
  );
  return data;
}
