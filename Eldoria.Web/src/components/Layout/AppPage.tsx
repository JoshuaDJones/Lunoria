import { PropsWithChildren, ReactElement } from "react";
import NavBar from "./NavBar";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";

interface AppPageProps {
  hasNav?: boolean;
  hasBackButton?: boolean;
  backgroundImage?: string;
  pane?: ReactElement;
}

const AppPage = ({
  hasNav = false,
  hasBackButton = false,
  backgroundImage,
  pane,
  children,
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

      <div className="absolute inset-0 flex flex-col pb-20 overflow-y-auto scrollbar-hide z-10">
        {children}
      </div>

      {hasNav && <NavBar hasBackButton={hasBackButton} />}
    </div>
  );
};

export default AppPage;
