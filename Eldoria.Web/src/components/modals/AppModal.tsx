import clsx from "clsx";
import { PropsWithChildren } from "react";

interface AppModalProps {
  onBackgroundClose?: () => void;
  backgroundOverride?: string;
  centerContent?: boolean;
}

const AppModal = ({
  onBackgroundClose,
  centerContent = false,
  backgroundOverride,
  children,
}: PropsWithChildren<AppModalProps>) => {
  return (
    <div
      className={clsx(
        "z-50 w-full h-full fixed top-0 right-0 left-0 bottom-0",
        backgroundOverride,
        {
          "items-center justify-center flex": centerContent,
          "bg-slate-900/60": !backgroundOverride,
        },
      )}
      onClick={onBackgroundClose}
    >
      {children}
    </div>
  );
};

export default AppModal;
