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
}

export function Stat({ label, value }: StatProps) {
  return (
    <div className="rounded-lg bg-surface-raised/70 px-3 py-2">
      <dt className="text-xs text-content-muted">{label}</dt>
      <dd className="mt-1 wrap-break-word font-semibold text-content">
        {value}
      </dd>
    </div>
  );
}
