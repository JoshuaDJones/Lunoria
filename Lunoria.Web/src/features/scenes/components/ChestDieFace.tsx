import { dieDotPositions } from "@/features/sceneplaythrough/types";

export function ChestDieFace({
  value,
  className = "size-7",
}: {
  value: number;
  className?: string;
}) {
  // Legacy chests can still contain ranges outside the current six-sided die.
  if (value < 1 || value > 6 || !Number.isInteger(value)) {
    return <span className="text-sm font-semibold">{value}</span>;
  }
  return (
    <svg
      viewBox="0 0 32 32"
      role="img"
      aria-label={`Die face ${value}`}
      className={`shrink-0 ${className}`}
    >
      <rect
        x="1"
        y="1"
        width="30"
        height="30"
        rx="6"
        fill="currentColor"
        fillOpacity="0.08"
        stroke="currentColor"
        strokeWidth="1.5"
      />
      {dieDotPositions[value].map((position) => (
        <circle
          key={position}
          cx={8 + (position % 3) * 8}
          cy={8 + Math.floor(position / 3) * 8}
          r="2.3"
          fill="currentColor"
        />
      ))}
    </svg>
  );
}
