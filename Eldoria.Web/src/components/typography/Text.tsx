import clsx from "clsx";
import { PropsWithChildren } from "react";

export enum TextColor {
  primary = "primary",
  invert = "invert",
  white = "white",
  black = "black",
  red = "red",
  green = "green",
  blue = "blue",
  custom = "custom",
}

export enum TextSize {
  xs = "xs",
  sm = "sm",
  base = "base",
  lg = "lg",
  xl = "xl",
  x2l = "x2l",
  x3l = "x3l"  
}

interface TextProps {
  textColor?: TextColor;
  size?: TextSize;
  className?: string;
}

const Text = ({
  textColor = TextColor.primary,
  size = TextSize.base,
  className,
  children,
}: PropsWithChildren<TextProps>) => {
  const sizeClasses: Record<TextSize, string> = {
    [TextSize.xs]: "text-xs",
    [TextSize.sm]: "text-sm",
    [TextSize.base]: "text-base",
    [TextSize.lg]: "text-lg",
    [TextSize.xl]: "text-2xl",
    [TextSize.x2l]: "text-3xl",
    [TextSize.x3l]: "text-4xl",
  };

  const colorClasses: Record<TextColor, string> = {
    [TextColor.primary]: "text-black dark:text-white",
    [TextColor.invert]: "text-white dark:text-black",
    [TextColor.white]: "text-white",
    [TextColor.black]: "text-black",
    [TextColor.red]: "text-red-300",
    [TextColor.blue]: "text-blue-300",
    [TextColor.green]: "text-green-300",
    [TextColor.custom]: "",
  };

  return (
    <span
      className={clsx(
        "font-cinzel",
        sizeClasses[size],
        colorClasses[textColor],
        className,
      )}
    >
      {children}
    </span>
  );
};

export default Text;
