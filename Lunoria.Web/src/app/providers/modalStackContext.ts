import { createContext, type ReactNode } from "react";

export type ModalPlacement = "drawer" | "center";

export interface ModalStackOptions {
  title: string;
  content: ReactNode;
  placement?: ModalPlacement;
  closeOnBackdrop?: boolean;
}

export interface ModalStackEntry extends Required<ModalStackOptions> {
  id: number;
}

export interface ModalStackContextValue {
  push: (options: ModalStackOptions) => number;
  pop: () => void;
  popTo: (id: number) => void;
  dismissAll: () => void;
  depth: number;
}

export const ModalStackContext = createContext<
  ModalStackContextValue | undefined
>(undefined);
