import { type ReactNode, useCallback, useMemo, useState } from "react";
import {
  AuthContext,
  type AuthContextValue,
} from "@/features/auth/authContext";
import { getAccessToken, setAccessToken } from "@/lib/apiClient";

interface AuthProviderProps {
  children: ReactNode;
}

function isTokenValid(token: string | null): token is string {
  if (!token) {
    return false;
  }

  try {
    const payload = JSON.parse(atob(token.split(".")[1])) as { exp?: number };
    return typeof payload.exp === "number" && payload.exp * 1000 > Date.now();
  } catch {
    return false;
  }
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [token, setToken] = useState<string | null>(() => {
    const storedToken = getAccessToken();

    if (isTokenValid(storedToken)) {
      return storedToken;
    }

    setAccessToken();
    return null;
  });

  const setAuthToken = useCallback((newToken: string) => {
    setAccessToken(newToken);
    setToken(newToken);
  }, []);

  const signOut = useCallback(() => {
    setAccessToken();
    setToken(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      token,
      isAuthenticated: token !== null,
      setAuthToken,
      signOut,
    }),
    [signOut, setAuthToken, token],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
