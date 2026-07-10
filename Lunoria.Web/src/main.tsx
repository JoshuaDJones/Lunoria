import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import { router } from "@/app/router";
import "@/styles/index.css";
import {
  AuthProvider,
  ConfirmDialogProvider,
  ModalStackProvider,
  ToastProvider,
} from "@/app/providers";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <AuthProvider>
      <ToastProvider>
        <ConfirmDialogProvider>
          <ModalStackProvider>
            <RouterProvider router={router} />
          </ModalStackProvider>
        </ConfirmDialogProvider>
      </ToastProvider>
    </AuthProvider>
  </StrictMode>,
);
