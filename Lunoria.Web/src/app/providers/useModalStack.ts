import { useContext } from "react";
import { ModalStackContext } from "./modalStackContext";

export function useModalStack() {
  const context = useContext(ModalStackContext);

  if (!context) {
    throw new Error("useModalStack must be used within ModalStackProvider");
  }

  return context;
}
