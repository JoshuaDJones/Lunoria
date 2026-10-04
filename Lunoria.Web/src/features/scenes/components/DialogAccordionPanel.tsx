import { useEffect, useRef, useState, type ReactNode } from "react";

export function DialogAccordionPanel({
  id,
  open,
  children,
}: {
  id: string;
  open: boolean;
  children: ReactNode;
}) {
  const [retained, setRetained] = useState(open);
  const container = useRef<HTMLDivElement>(null);
  // Keep content mounted until the closing transition has finished.
  if (open && !retained) setRetained(true);

  useEffect(() => {
    if (open) return;
    container.current
      ?.querySelectorAll("video")
      .forEach((video) => video.pause());
    const delay = window.matchMedia("(prefers-reduced-motion: reduce)").matches
      ? 0
      : 250;
    const timer = setTimeout(() => setRetained(false), delay);
    return () => clearTimeout(timer);
  }, [open]);

  return (
    <div
      id={id}
      ref={container}
      inert={!open}
      aria-hidden={!open}
      className={`grid transition-[grid-template-rows,opacity] duration-250 ease-in-out motion-reduce:transition-none ${open ? "grid-rows-[1fr] opacity-100" : "grid-rows-[0fr] opacity-0"}`}
    >
      <div className="min-h-0 overflow-hidden">{retained && children}</div>
    </div>
  );
}
