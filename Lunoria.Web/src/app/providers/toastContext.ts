import { createContext } from "react";

export type ToastVariant = "success" | "info" | "error";

export interface ToastOptions {
  title?: string;
  message: string;
  variant?: ToastVariant;
  duration?: number;
}

export interface ToastContextValue {
  show: (options: ToastOptions) => number;
  success: (message: string, title?: string) => number;
  info: (message: string, title?: string) => number;
  error: (message: string, title?: string) => number;
  dismiss: (id: number) => void;
  dismissAll: () => void;
}

export const ToastContext = createContext<ToastContextValue | undefined>(
  undefined,
);
