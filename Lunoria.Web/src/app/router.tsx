import { createBrowserRouter, Navigate } from "react-router-dom";
import { RequireAuth } from "@/app/routing/RequireAuth";
import { RequireGuest } from "@/app/routing/RequireGuest";
import { HomePage } from "@/pages/HomePage";
import { LandingPage } from "@/pages/LandingPage";
import { LoginPage } from "@/pages/LoginPage";
import { RegisterPage } from "@/pages/RegisterPage";
import { ForgotPasswordPage } from "@/pages/ForgotPasswordPage";
import { ComponentShowCasePage } from "@/pages/ComponentShowCasePage";

export const router = createBrowserRouter([
  {
    element: <RequireGuest />,
    children: [
      {
        path: "/",
        element: <LandingPage />,
      },
      {
        path: "/login",
        element: <LoginPage />,
      },
      {
        path: "/register",
        element: <RegisterPage />,
      },
      {
        path: "/forgot-password",
        element: <ForgotPasswordPage />,
      },
    ],
  },
  {
    element: <RequireAuth />,
    children: [
      {
        path: "/home",
        element: <HomePage />,
      },
      {
        path: "/componentshowcase",
        element: <ComponentShowCasePage />,
      },
    ],
  },
  {
    path: "*",
    element: <Navigate to="/" replace />,
  },
]);
