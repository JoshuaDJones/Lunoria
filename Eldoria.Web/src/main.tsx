import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { RouterProvider } from "react-router-dom";
import { router } from "./routes/Routes.tsx";
import { AuthProvider } from "./providers/AuthProvider.tsx";
import { ToastProvider } from "./providers/ToastProvider.tsx";
import { ModalRouterProvider } from "./providers/ModalRouterProvider.tsx";
import { LoadingProvider } from "./providers/LoadingProvider.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ToastProvider>
      <AuthProvider>
        <LoadingProvider>
          <ModalRouterProvider>
            <RouterProvider router={router} />
          </ModalRouterProvider>
        </LoadingProvider>
      </AuthProvider>
    </ToastProvider>
  </StrictMode>,
);
