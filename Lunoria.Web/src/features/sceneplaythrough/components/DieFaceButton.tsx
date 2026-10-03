import { dieDotPositions } from "@/features/sceneplaythrough/types";

const DieFaceButton = ({
  value,
  selected,
  onClick,
  compact = false,
  disabled = false,
  ariaLabel,
}: {
  value: number;
  selected: boolean;
  onClick: () => void;
  compact?: boolean;
  disabled?: boolean;
  ariaLabel?: string;
}) => {
  return (
    <button
      type="button"
      aria-label={ariaLabel ?? `Select die face ${value}`}
      aria-pressed={selected}
      disabled={disabled}
      className={`aspect-square cursor-pointer rounded-xl border-2 bg-surface transition hover:border-utility hover:bg-utility/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/50 disabled:cursor-not-allowed disabled:opacity-30 ${
        compact ? "p-2" : "p-5"
      } ${selected ? "border-utility bg-utility/10" : "border-border"}`}
      onClick={onClick}
    >
      <span className="grid h-full w-full grid-cols-3 grid-rows-3 gap-1">
        {Array.from({ length: 9 }, (_, index) => (
          <span
            key={index}
            className={
              dieDotPositions[value].includes(index)
                ? `m-auto block rounded-full bg-content ${
                    compact ? "h-2.5 w-2.5" : "h-4 w-4 sm:h-5 sm:w-5"
                  }`
                : undefined
            }
          />
        ))}
      </span>
    </button>
  );
};

export default DieFaceButton;
