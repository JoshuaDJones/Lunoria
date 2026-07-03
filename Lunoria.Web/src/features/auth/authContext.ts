import { createContext } from "react";

export interface AuthContextValue {
  token: string | null;
  isAuthenticated: boolean;
  setAuthToken: (token: string) => void;
  signOut: () => void;
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
);
