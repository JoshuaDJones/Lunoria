import { useRef, type KeyboardEvent } from "react";

interface IntroEditorDividerProps {
  width: number;
  min: number;
  max: number;
  disabled: boolean;
  onResize: (width: number) => void;
}

export function IntroEditorDivider({
  width,
  min,
  max,
  disabled,
  onResize,
}: IntroEditorDividerProps) {
  const drag = useRef<{ x: number; width: number } | null>(null);
  const onKeyDown = (event: KeyboardEvent<HTMLDivElement>) => {
    if (disabled) return;
    const step = event.shiftKey ? 50 : 10;
    const next =
      event.key === "ArrowLeft"
        ? width + step
        : event.key === "ArrowRight"
          ? width - step
          : event.key === "Home"
            ? min
            : event.key === "End"
              ? max
              : event.key === "Enter"
                ? 416
                : undefined;
    if (next === undefined) return;
    event.preventDefault();
    onResize(next);
  };

  return (
    <div
      role="separator"
      aria-label="Resize page editor"
      aria-orientation="vertical"
      aria-controls="intro-page-settings"
      aria-valuemin={min}
      aria-valuemax={max}
      aria-valuenow={Math.round(width)}
      aria-valuetext={`${Math.round(width)} pixels wide`}
      aria-disabled={disabled}
      tabIndex={disabled ? -1 : 0}
      title="Drag to resize. Double-click or press Enter to reset. Arrow keys resize."
      className="group hidden touch-none select-none items-center justify-center rounded-lg outline-none hover:bg-content/5 focus-visible:bg-content/10 focus-visible:ring-2 focus-visible:ring-brand-hover xl:flex cursor-col-resize"
      onKeyDown={onKeyDown}
      onDoubleClick={() => {
        if (!disabled) onResize(416);
      }}
      onPointerDown={(event) => {
        if (disabled || event.button !== 0) return;
        event.preventDefault();
        event.currentTarget.focus();
        drag.current = { x: event.clientX, width };
        event.currentTarget.setPointerCapture(event.pointerId);
      }}
      onPointerMove={(event) => {
        if (
          disabled ||
          !drag.current ||
          !event.currentTarget.hasPointerCapture(event.pointerId)
        )
          return;
        onResize(drag.current.width + drag.current.x - event.clientX);
      }}
      onPointerUp={(event) => {
        drag.current = null;
        if (event.currentTarget.hasPointerCapture(event.pointerId))
          event.currentTarget.releasePointerCapture(event.pointerId);
      }}
      onPointerCancel={() => {
        drag.current = null;
      }}
      onLostPointerCapture={() => {
        drag.current = null;
      }}
    >
      <span
        aria-hidden="true"
        className="h-12 w-1 rounded-full bg-content-muted/40 group-hover:bg-brand-hover group-focus-visible:bg-brand-hover"
      />
    </div>
  );
}
