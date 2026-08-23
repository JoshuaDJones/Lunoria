import { createBrowserRouter, Navigate } from "react-router-dom";
import { AllDialogsPage } from "@/pages/authenticated/AllDialogsPage";
import { RequireAuth } from "@/app/routing/RequireAuth";
import { RequireGuest } from "@/app/routing/RequireGuest";
import { HomePage } from "@/pages/authenticated/HomePage";
import { LoginPage } from "@/pages/auth/LoginPage";
import { RegisterPage } from "@/pages/auth/RegisterPage";
import { ForgotPasswordPage } from "@/pages/auth/ForgotPasswordPage";
import { LandingPage } from "@/pages/public/LandingPage";
import { CharactersPage } from "@/pages/authenticated/CharactersPage";
import { SpellsPage } from "@/pages/authenticated/SpellsPage";
import { ConsumablesPage } from "@/pages/authenticated/ConsumablesPage";
import { EquipmentPage } from "@/pages/authenticated/EquipmentPage";
import { SceneDialogsPage } from "@/pages/authenticated/SceneDialogsPage";
import { ComponentDisplayPage } from "@/pages/authenticated/ComponentDisplayPage";
import { JourneysPage } from "@/pages/authenticated/JourneysPage";
import { JourneyEditorPage } from "@/pages/authenticated/JourneyEditorPage";
import { JourneyIntroPagesPage } from "@/pages/authenticated/JourneyIntroPagesPage";
import { PlayHubPage } from "@/pages/authenticated/PlayHubPage";
import { PlaythroughPage } from "@/pages/authenticated/PlaythroughPage";
import { ScenePlaythroughPage } from "@/pages/authenticated/ScenePlaythroughPage";
import { GridPrototypePage } from "@/pages/public/GridPrototypePage";
import { PlaythroughGuestPage } from "@/pages/public/PlaythroughGuestPage";
import { SceneGridPage } from "@/pages/authenticated/SceneGridPage";

export const router = createBrowserRouter([
  {
    path: "/grid-prototype",
    element: <GridPrototypePage />,
  },
  {
    path: "/join/:token",
    element: <PlaythroughGuestPage />,
  },
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
        path: "/scene-grids/:sceneId",
        element: <SceneGridPage />,
      },
      {
        path: "/journeys",
        element: <JourneysPage />,
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
      {
        path: "/components",
        element: <ComponentDisplayPage />,
      },
      {
        path: "/journeys/:journeyId/all-dialogs",
        element: <AllDialogsPage />,
      },
      {
        path: "/series/:seriesId/journeys",
        element: <JourneysPage />,
      },
      {
        path: "/series/:seriesId/journeys/:journeyId",
        element: <JourneyEditorPage />,
      },
      {
        path: "/series/:seriesId/journeys/:journeyId/scenes/:sceneId/dialogs",
        element: <SceneDialogsPage />,
      },
      {
        path: "/series/:seriesId/journeys/:journeyId/intro-pages",
        element: <JourneyIntroPagesPage />,
      },
      {
        path: "/series/:seriesId/journeys/:journeyId/play",
        element: <PlayHubPage />,
      },
      {
        path: "/series/:seriesId/journeys/:journeyId/playthroughs/:playthroughId",
        element: <PlaythroughPage />,
      },
      {
        path: "/series/:seriesId/journeys/:journeyId/playthroughs/:playthroughId/scenes/:sceneId",
        element: <ScenePlaythroughPage />,
      },
    ],
  },
  {
    path: "*",
    element: <Navigate to="/" replace />,
  },
]);
