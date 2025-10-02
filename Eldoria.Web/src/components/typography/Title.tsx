import { PropsWithChildren } from "react";
import clsx from "clsx";

export enum TitleSize {
  small = "small",
  medium = "medium",
  large = "large",
  xlarge = "xlarge",
}

export enum TitleColor {
  default = "default",
  stone800 = "stone800",
  stone700 = "stone700",
  white = "white",
}

interface TitleProps {
  className?: string;
  size?: TitleSize;
  color?: TitleColor;
}

const Title = ({
  children,
  className,
  size = TitleSize.large,
  color = TitleColor.default,
}: PropsWithChildren<TitleProps>) => {
  const sizeClasses: Record<TitleSize, string> = {
    [TitleSize.small]: "text-2xl",
    [TitleSize.medium]: "text-5xl",
    [TitleSize.large]: "text-7xl",
    [TitleSize.xlarge]: "text-9xl",
  };

  const colorClasses: Record<TitleColor, string> = {
    [TitleColor.default]: "text-black",
    [TitleColor.stone800]: "text-stone-800",
    [TitleColor.stone700]: "text-stone-700",
    [TitleColor.white]: "text-white",
  };

  return (
    <h1
      className={clsx(
        "font-cinzel tracking-wider",
        sizeClasses[size],
        colorClasses[color],
        className,
      )}
    >
      {children}
    </h1>
  );
};

export default Title;
