import clsx from "clsx";
import { PropsWithChildren, ReactElement } from "react";
import BackIconButton from "../buttons/BackIconButton";
import Title, { TitleColor } from "../typography/Title";

interface PageContentProps extends PropsWithChildren {
  title?: string;
  titleColor?: TitleColor;
  noCentering?: boolean;
  noTopMargin?: boolean;
  useBackButton?: boolean;
  leftPane?: ReactElement;
  rightPane?: ReactElement;
  className?: string;
}

const PageContent = ({
  title,
  titleColor,
  children,
  noCentering,
  noTopMargin,
  useBackButton = false,
  leftPane,
  rightPane,
  className,
}: PageContentProps) => {
  return (
    <div
      className={clsx(
        "flex flex-1 gap-2",
        {
          "items-center justify-center": !noCentering,
          "mt-20": !noTopMargin,
        },
        className,
      )}
    >
      <div className="flex w-[150px] h-full">{leftPane}</div>

      <div className="flex flex-1 flex-col">{children}</div>

      <div className="flex w-[150px] h-full">{rightPane}</div>

      <div className="absolute left-5">
        {useBackButton && <BackIconButton />}
      </div>
    </div>
  );
};

export default PageContent;
