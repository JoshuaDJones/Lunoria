import type { ComponentPropsWithoutRef } from "react";
import clsx from "clsx";

type CardProps = ComponentPropsWithoutRef<"article">;

export function Card({ className, ...props }: CardProps) {
  return (
    <article
      className={clsx(
        "overflow-hidden rounded-xl border border-border bg-surface/90 shadow-lg ",
        className,
      )}
      {...props}
    />
  );
}
