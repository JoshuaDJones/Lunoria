import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "@/features/auth";

export function RequireGuest() {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <Navigate to="/home" replace /> : <Outlet />;
}
