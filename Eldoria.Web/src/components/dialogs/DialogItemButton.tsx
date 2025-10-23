import { PropsWithChildren } from "react";

interface DialogItemButtonProps {
  onClick: () => void;
}

const DialogItemButton = ({
  onClick,
  children,
}: PropsWithChildren<DialogItemButtonProps>) => {
  return (
    <button onClick={onClick} className="flex w-full px-5 py-3">
      {children}
    </button>
  );
};

export default DialogItemButton;
