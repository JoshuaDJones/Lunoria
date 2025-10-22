import { PropsWithChildren, ReactElement } from "react";
import NavBar from "./NavBar";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import clsx from "clsx";

interface AppPageProps {
  hasNav?: boolean;
  hasBackButton?: boolean;
  backgroundImage?: string;
  pane?: ReactElement;
  noBottomPadding?: boolean;
}

const AppPage = ({
  hasNav = false,
  hasBackButton = false,
  backgroundImage,
  pane,
  children,
  noBottomPadding,
}: PropsWithChildren<AppPageProps>) => {
  return (
    <div className="bg-slate-800 h-screen w-screen relative overflow-hidden">
      {backgroundImage && (
        <img
          className="absolute right-0 left-0 w-full h-full top-0 bottom-0 z-0"
          src={backgroundImage}
        />
      )}

      {pane}

      <div
        className={clsx(
          "absolute inset-0 flex flex-col overflow-y-auto scrollbar-hide z-10",
          {
            "pb-20": !noBottomPadding,
          },
        )}
      >
        {children}
      </div>

      {hasNav && <NavBar hasBackButton={hasBackButton} />}
    </div>
  );
};

export default AppPage;
