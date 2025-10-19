import clsx from "clsx";
import React, { ReactElement, ReactNode } from "react";

export enum AppButtonVariant {
  primary,
  secondary,
  warning,
  go,
  outline,
  disabled,
  ghost,
  custom,
}

export enum AppButtonSize {
  xs,
  sm,
  md,
  lg,
  xl,
  custom,
}

interface AppButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  title: string;
  variant: AppButtonVariant;
  size: AppButtonSize;
  rightIcon?: ReactElement;
  leftIcon?: ReactElement;
  noRounded?: boolean;
  className?: string;
}

const AppButton = ({
  title,
  variant,
  size,
  rightIcon,
  leftIcon,
  noRounded,
  className,
  ...props
}: AppButtonProps) => {
  return (
    <button
      {...props}
      className={clsx(
        "flex flex-row font-cinzel font-bold tracking-widest items-center justify-center border-box hover:bg-opacity-70",
        {
          "bg-blue-900 text-white": variant === AppButtonVariant.primary,
          "bg-stone-800 text-white": variant === AppButtonVariant.secondary,
          "bg-red-900 text-white": variant === AppButtonVariant.warning,
          "bg-green-900 text-white": variant === AppButtonVariant.go,
          "border-4 border-black text-black":
            variant === AppButtonVariant.outline,
          "bg-gray-400/50 text-white": variant === AppButtonVariant.disabled,
          "bg-transparent text-white": variant === AppButtonVariant.ghost,
          "text-xs px-2 py-1": size === AppButtonSize.xs,
          "text-sm px-3 py-2": size === AppButtonSize.sm,
          "text-lg px-4 py-2": size === AppButtonSize.md,
          "text-2xl px-6 py-3": size === AppButtonSize.lg,
          "text-4xl px-8 py-4": size === AppButtonSize.xl,
          "rounded-xl": !noRounded,
        },
        className,
      )}
    >
      {leftIcon}
      {title}
      {rightIcon}
    </button>
  );
};

export default AppButton;
