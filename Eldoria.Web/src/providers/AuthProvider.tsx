import React, {
  createContext,
  useContext,
  useState,
  ReactNode,
  useMemo,
} from "react";

const AUTH_KEY = "auth_token";

interface AuthContextProps {
  token: string | undefined;
  setAuthToken: (token: string | undefined) => void;
}

const AuthContext = createContext<AuthContextProps | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [token, setToken] = useState<string | undefined>(() => {
    return localStorage.getItem(AUTH_KEY) ?? undefined;
  });

  const setAuthToken = (newToken: string | undefined) => {
    setToken(newToken);
    if (newToken) {
      localStorage.setItem(AUTH_KEY, newToken);
    } else {
      localStorage.removeItem(AUTH_KEY);
    }
  };

  const value = useMemo(() => ({ token, setAuthToken }), [token]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = (): AuthContextProps => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within the AuthProvider");
  return context;
};
