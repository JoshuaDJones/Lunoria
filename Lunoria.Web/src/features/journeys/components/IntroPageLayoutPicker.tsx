import { IntroPageType } from "@/features/journeys/types";
import { introPageTypeLabels } from "@/features/journeys/introPageConfig";

const types = Object.values(IntroPageType).filter(
  (value): value is IntroPageType => typeof value === "number",
);

export function IntroPageLayoutPicker({
  onSelect,
}: {
  onSelect: (type: IntroPageType) => void;
}) {
  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {types.map((type) => {
        const overlay = type === IntroPageType.ImageCenterOverlayCenterText;
        const top = type === IntroPageType.ImageTopContentBottom;
        const reversed = type === IntroPageType.ImageRightContentLeft;
        const showcase = type === IntroPageType.CharacterShowcase;
        return (
          <button
            key={type}
            type="button"
            onClick={() => onSelect(type)}
            className="rounded-xl border border-border bg-surface p-3 text-left transition hover:border-brand-hover focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-hover"
          >
            <div
              aria-hidden="true"
              className={`relative mb-3 flex aspect-video gap-2 overflow-hidden rounded-lg bg-canvas p-3 ${top ? "flex-col" : reversed ? "flex-row-reverse" : "flex-row"}`}
            >
              <div
                className={`flex items-center justify-center rounded bg-brand/20 text-2xl text-brand-hover ${overlay ? "absolute inset-2" : showcase ? "w-2/5" : "flex-1"}`}
              >
                <svg
                  viewBox="0 0 48 32"
                  className="h-10 w-14"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2"
                >
                  <path d="m3 29 13-17 10 12 7-9 12 14Z" />
                  <circle cx="33" cy="7" r="4" />
                </svg>
              </div>
              <div
                className={`flex flex-1 flex-col justify-center gap-2 ${overlay ? "relative z-10 m-auto max-w-[65%] rounded bg-canvas/80 p-3" : ""}`}
              >
                <div className="h-2 w-3/4 rounded bg-content/60" />
                <div className="h-1 rounded bg-content/25" />
                <div className="h-1 w-4/5 rounded bg-content/25" />
              </div>
            </div>
            <span className="text-sm font-semibold text-content">
              {introPageTypeLabels[type]}
            </span>
          </button>
        );
      })}
    </div>
  );
}
