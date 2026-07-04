import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "@/features/auth";

export function RequireAuth() {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  return isAuthenticated ? (
    <Outlet />
  ) : (
    <Navigate to="/" replace state={{ from: location.pathname }} />
  );
}
