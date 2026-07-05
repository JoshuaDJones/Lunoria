import { createBrowserRouter, Navigate } from "react-router-dom";
import { RequireAuth } from "@/app/routing/RequireAuth";
import { RequireGuest } from "@/app/routing/RequireGuest";
import { HomePage } from "@/pages/authenticated/HomePage";
import { ComponentShowCasePage } from "@/pages/authenticated/ComponentShowCasePage";
import { LoginPage } from "@/pages/auth/LoginPage";
import { RegisterPage } from "@/pages/auth/RegisterPage";
import { ForgotPasswordPage } from "@/pages/auth/ForgotPasswordPage";
import { LandingPage } from "@/pages/public/LandingPage";
import { CharactersPage } from "@/pages/authenticated/CharactersPage";
import { SpellsPage } from "@/pages/authenticated/SpellsPage";
import { ConsumablesPage } from "@/pages/authenticated/ConsumablesPage";
import { EquipmentPage } from "@/pages/authenticated/EquipmentPage";

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
      {
        path: "/characters",
        element: <CharactersPage />,
      },
      {
        path: "/spells",
        element: <SpellsPage />,
      },
      {
        path: "/consumables",
        element: <ConsumablesPage />,
      },
      {
        path: "/equipment",
        element: <EquipmentPage />,
      },
    ],
  },
  {
    path: "*",
    element: <Navigate to="/" replace />,
  },
]);
