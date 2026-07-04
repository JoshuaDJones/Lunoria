import { createBrowserRouter } from "react-router-dom";
import { HomePage } from "@/pages/HomePage";
import { ComponentShowCasePage } from "@/pages/ComponentShowCasePage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <HomePage />,
  },
  {
    path: "/componentshowcase",
    element: <ComponentShowCasePage />,
  },
]);
