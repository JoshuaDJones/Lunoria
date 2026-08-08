import type { ComponentPropsWithoutRef } from "react";
import { twMerge } from "tailwind-merge";

type CardGridProps = ComponentPropsWithoutRef<"div">;

export function CardGrid({ className, ...props }: CardGridProps) {
  return (
    <div
      className={twMerge(
        "grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-3",
        className,
      )}
      {...props}
    />
  );
}
