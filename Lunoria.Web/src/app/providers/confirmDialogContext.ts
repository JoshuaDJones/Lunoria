import { createContext, type ReactNode } from "react";
import type { ButtonProps } from "@/components/ui";

export type ConfirmVariant = Extract<
  ButtonProps["variant"],
  "primary" | "danger"
>;

export interface ConfirmDialogOptions {
  title: string;
  message?: ReactNode;
  confirmLabel?: string;
  cancelLabel?: string;
  variant?: ConfirmVariant;
}

export interface PendingConfirmation extends Required<
  Omit<ConfirmDialogOptions, "message">
> {
  message?: ReactNode;
  resolve: (confirmed: boolean) => void;
}

export interface ConfirmDialogContextValue {
  confirm: (options: ConfirmDialogOptions) => Promise<boolean>;
}

export const ConfirmDialogContext = createContext<
  ConfirmDialogContextValue | undefined
>(undefined);
