import clsx from "clsx";
import { PropsWithChildren } from "react";

interface DialogItemButtonProps {
  onClick: () => void;
  className?: string;
}

const DialogItemButton = ({
  onClick,
  children,
  className,
}: PropsWithChildren<DialogItemButtonProps>) => {
  return (
    <button
      onClick={onClick}
      className={clsx("flex w-full px-5 py-3", className)}
    >
      {children}
    </button>
  );
};

export default DialogItemButton;
