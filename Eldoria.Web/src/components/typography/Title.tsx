import { PropsWithChildren } from "react";
import clsx from "clsx";

export enum TitleSize {
  small = "small",
  medium = "medium",
  large = "large",
  xlarge = "xlarge",
  custom = "custom",
}

export enum TitleColor {
  default = "default",
  stone300 = "stone300",
  stone400 = "stone400",
  stone500 = "stone500",
  stone600 = "stone600",
  stone700 = "stone700",
  stone800 = "stone800",
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
    [TitleSize.custom]: "",
  };

  const colorClasses: Record<TitleColor, string> = {
    [TitleColor.default]: "text-black",
    [TitleColor.stone300]: "text-stone-300",
    [TitleColor.stone400]: "text-stone-400",
    [TitleColor.stone500]: "text-stone-500",
    [TitleColor.stone600]: "text-stone-600",
    [TitleColor.stone700]: "text-stone-700",
    [TitleColor.stone800]: "text-stone-800",
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
