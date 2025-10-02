import { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../providers/AuthProvider";

interface ProtectedRouteProps {
  element: ReactNode;
}

export const ProtectedRoute = ({ element }: ProtectedRouteProps) => {
  const { token } = useAuth();
  const hasToken = !!token;

  if (!hasToken) {
    return <Navigate to="/" replace />;
  }

  return <>{element}</>;
};
