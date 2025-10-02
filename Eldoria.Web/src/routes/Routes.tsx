import { createBrowserRouter } from "react-router";
import App from "../App";
import HomePage from "../pages/HomePage";
import ForgotPasswordPage from "../pages/ForgotPasswordPage";
import RegisterPage from "../pages/RegisterPage";
import LoginPage from "../pages/LoginPage";
import { ProtectedRoute } from "../components/ProtectedRoute";
import JourneysPage from "../pages/authenticated/JourneysPage";
import ItemsPage from "../pages/authenticated/ItemsPage";
import SpellsPage from "../pages/authenticated/SpellsPage";
import CharactersPage from "../pages/authenticated/CharactersPage";
import JourneyDetailsPage from "../pages/authenticated/JourneyDetailsPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { index: true, element: <HomePage /> },
      { path: "Login", element: <LoginPage /> },
      { path: "Register", element: <RegisterPage /> },
      { path: "ForgotPassword", element: <ForgotPasswordPage /> },

      {
        path: "Journeys",
        element: <ProtectedRoute element={<JourneysPage />} />,
      },
      { path: "Items", element: <ProtectedRoute element={<ItemsPage />} /> },
      { path: "Spells", element: <ProtectedRoute element={<SpellsPage />} /> },
      {
        path: "Characters",
        element: <ProtectedRoute element={<CharactersPage />} />,
      },
      {
        path: "JourneyDetails/:id",
        element: <ProtectedRoute element={<JourneyDetailsPage />} />,
      },
    ],
  },
]);

// export const router = createBrowserRouter([
//   {
//     path: "/",
//     element: <App />,
//     children: [
//       { path: "", element: <HomePage /> },
//       // {
//       //   path: "Scenes/:id",
//       //   element: <ProtectedRoute element={<ScenePage />} />,
//       // },
//       // {
//       //   path: "Characters/",
//       //   element: <ProtectedRoute element={<CharactersPage />} />,
//       // },
//       // { path: "Spells", element: <ProtectedRoute element={<SpellsPage />} /> },
//       // { path: "Items", element: <ProtectedRoute element={<ItemsPage />} /> },
//       // {
//       //   path: "SceneDetails/:id",
//       //   element: <ProtectedRoute element={<SceneDetails />} />,
//       // },
//       // {
//       //   path: "Profile",
//       //   element: <ProtectedRoute element={<ProfilePage />} />,
//       // },

//       { path: "Login", element: <LoginPage /> },
//       { path: "Register", element: <RegisterPage /> },
//       { path: "ForgotPassword", element: <ForgotPasswordPage /> },
//     ],
//   },
// ]);
