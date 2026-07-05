import { type ComponentPropsWithRef } from "react";
import { twMerge } from "tailwind-merge";

type ButtonVariant = "primary" | "secondary" | "accent" | "danger";
type ButtonSize = "sm" | "md" | "lg";

export type ButtonProps = ComponentPropsWithRef<"button"> & {
  variant?: ButtonVariant;
  size?: ButtonSize;
};

const variantClasses: Record<ButtonVariant, string> = {
  primary:
    "border-transparent bg-brand font-semibold text-on-brand hover:bg-brand-hover",
  secondary:
    "border-border text-content-secondary hover:border-brand-hover hover:text-brand-hover",
  accent:
    "border-brand-subtle/50 font-semibold text-brand-hover hover:bg-brand/10",
  danger: "border-danger/50 text-danger hover:bg-danger/10",
};

const sizeClasses: Record<ButtonSize, string> = {
  sm: "rounded-md px-3 py-1.5 text-xs",
  md: "rounded-lg px-4 py-2 text-sm",
  lg: "rounded-lg px-5 py-3",
};

export function Button({
  variant = "secondary",
  size = "md",
  type = "button",
  className,
  ...props
}: ButtonProps) {
  return (
    <button
      type={type}
      className={twMerge(
        "border transition cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-hover/40 disabled:cursor-not-allowed disabled:opacity-40",
        variantClasses[variant],
        sizeClasses[size],
        className,
      )}
      {...props}
    />
  );
}
