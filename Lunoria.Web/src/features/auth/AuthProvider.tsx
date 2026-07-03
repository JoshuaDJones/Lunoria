import {
  type ReactNode,
  useCallback,
  useMemo,
  useState,
} from "react";
import { AuthContext, type AuthContextValue } from "@/features/auth/authContext";
import { getAccessToken, setAccessToken } from "@/lib/apiClient";

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [token, setToken] = useState<string | null>(getAccessToken);

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
