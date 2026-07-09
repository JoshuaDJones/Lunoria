import type { ComponentPropsWithoutRef, ReactNode } from "react";
import clsx from "clsx";

type StatGridProps = ComponentPropsWithoutRef<"dl">;

export function StatGrid({ className, ...props }: StatGridProps) {
  return (
    <dl className={clsx("grid grid-cols-2 gap-2", className)} {...props} />
  );
}

interface StatProps {
  label: string;
  value: ReactNode;
  labelTone?: string;
  valueTone?: string;
}

export function Stat({
  label,
  value,
  labelTone = "text-content-muted",
  valueTone = "text-content",
}: StatProps) {
  return (
    <div className="rounded-lg bg-surface-raised/90 px-3 py-2">
      <dt className={clsx("text-lg", labelTone)}>{label}</dt>
      <dd className={clsx("mt-1 wrap-break-word font-semibold", valueTone)}>
        {value}
      </dd>
    </div>
  );
}
