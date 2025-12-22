import clsx from "clsx";
import { PropsWithChildren } from "react";

export enum TextColor {
  primary,
  invert,
  white,
  black,
  custom,
}

interface EasyTextProps {
  textColor?: TextColor;
  className?: string;
}

const EasyText = ({
  textColor = TextColor.primary,
  className,
  children,
}: PropsWithChildren<EasyTextProps>) => {
  return (
    <label
      className={clsx(className, {
        "text-black dark:text-white": textColor === TextColor.primary,
        "text-white dark:text-black": textColor === TextColor.invert,
        "text-white": textColor === TextColor.white,
        "text-black": textColor === TextColor.black,
      })}
    >
      {children}
    </label>
  );
};

export default EasyText;
