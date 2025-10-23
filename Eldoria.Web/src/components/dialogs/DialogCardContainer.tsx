import clsx from "clsx";
import { PropsWithChildren } from "react";

interface DialogCardContainerProps {
  isActive: boolean;
}

const DialogCardContainer = ({
  isActive,
  children,
}: PropsWithChildren<DialogCardContainerProps>) => {
  return (
    <div
      className={clsx("flex flex-col mt-3 rounded-lg bg-stone-900/75", {
        "outline outline-blue-400": isActive,
      })}
    >
      {children}
    </div>
  );
};

export default DialogCardContainer;
