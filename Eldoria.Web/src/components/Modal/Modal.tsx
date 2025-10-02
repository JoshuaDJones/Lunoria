import React, { PropsWithChildren } from "react";

interface ModalProps {
  visible: boolean;
  onBackgroundClose?: () => void;
}

const Modal = ({
  visible,
  onBackgroundClose,
  children,
}: PropsWithChildren<ModalProps>) => {
  if (!visible) return null;

  return (
    <div
      className="z-50 w-full h-full flex items-center justify-center fixed top-0 right-0 left-0 bottom-0 bg-gray-400 dark:bg-gray-700 bg-opacity-50 dark:bg-opacity-80"
      onClick={onBackgroundClose}
    >
      {children}
    </div>
  );
};

export default Modal;
