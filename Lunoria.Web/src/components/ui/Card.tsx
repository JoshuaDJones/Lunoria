import type { ComponentPropsWithoutRef } from "react";
import clsx from "clsx";

type CardProps = ComponentPropsWithoutRef<"article">;

export function Card({ className, ...props }: CardProps) {
  return (
    <article
      className={clsx(
        "overflow-hidden rounded-xl border border-border bg-surface/80 shadow-lg backdrop-blur-sm",
        className,
      )}
      {...props}
    />
  );
}
