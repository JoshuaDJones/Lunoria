import { useEffect, useRef, type ReactNode } from "react";
import { Button } from "@/components/ui";

export function InlineDialogForm({
  title,
  busy,
  onCancel,
  children,
}: {
  title: string;
  busy: boolean;
  onCancel: () => void;
  children: ReactNode;
}) {
  const container = useRef<HTMLDivElement>(null);
  useEffect(() => {
    container.current?.scrollIntoView({
      block: "nearest",
      behavior: "instant",
    });
    container.current
      ?.querySelector<HTMLInputElement>("input")
      ?.focus({ preventScroll: true });
  }, []);
  return (
    <div
      ref={container}
      aria-busy={busy}
      className="rounded-xl border border-brand/50 bg-surface p-4 sm:p-5"
    >
      <h3 className="mb-4 text-lg font-semibold">{title}</h3>
      <div className="max-w-3xl">
        {children}
        <div className="mt-3 flex justify-end">
          <Button disabled={busy} onClick={onCancel}>
            Cancel
          </Button>
        </div>
      </div>
    </div>
  );
}
