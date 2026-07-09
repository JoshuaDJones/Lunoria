import { ReactNode, type ComponentPropsWithRef } from "react";
import { twMerge } from "tailwind-merge";

type ButtonVariant =
  "primary" | "secondary" | "accent" | "add" | "magic" | "danger";
type ButtonSize = "sm" | "md" | "lg";

export type ButtonProps = ComponentPropsWithRef<"button"> & {
  variant?: ButtonVariant;
  size?: ButtonSize;
  inverted?: boolean;
  rightIcon?: ReactNode;
  leftIcon?: ReactNode;
};

const variantClasses: Record<
  ButtonVariant,
  { default: string; inverted: string }
> = {
  primary: {
    default:
      "border-transparent bg-brand font-semibold text-on-brand hover:bg-brand-hover",
    inverted:
      "border-brand bg-brand/10 font-semibold text-brand-hover hover:bg-brand hover:text-on-brand",
  },
  secondary: {
    default:
      "border-border text-content-secondary hover:border-brand-hover hover:text-brand-hover",
    inverted:
      "border-content-secondary bg-content/10 text-content hover:bg-content hover:text-canvas",
  },
  accent: {
    default:
      "border-brand-subtle/50 font-semibold text-brand-hover hover:bg-brand/10",
    inverted:
      "border-brand-subtle bg-brand/10 font-semibold text-brand-hover hover:bg-brand-hover hover:text-on-brand",
  },
  add: {
    default:
      "border-transparent bg-add font-semibold text-on-add hover:bg-add-hover",
    inverted:
      "border-add bg-add/10 font-semibold text-add-hover hover:bg-add hover:text-on-add",
  },
  magic: {
    default:
      "border-transparent bg-magic font-semibold text-on-magic hover:bg-magic-hover",
    inverted:
      "border-magic bg-magic/10 font-semibold text-magic-hover hover:bg-magic hover:text-on-magic",
  },
  danger: {
    default: "border-danger/50 text-danger hover:bg-danger/10",
    inverted:
      "border-danger bg-danger/10 font-semibold text-danger hover:bg-danger hover:text-canvas",
  },
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
  inverted = false,
  className,
  leftIcon,
  rightIcon,
  ...props
}: ButtonProps) {
  return (
    <button
      type={type}
      className={twMerge(
        "border transition cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-hover/40 disabled:cursor-not-allowed disabled:opacity-40",
        "inline-flex items-center justify-center",
        variantClasses[variant][inverted ? "inverted" : "default"],
        sizeClasses[size],
        className,
      )}
      {...props}
    >
      {leftIcon && <span className="mr-2">{leftIcon}</span>}
      {props.children}
      {rightIcon && <span className="ml-2">{rightIcon}</span>}
    </button>
  );
}
