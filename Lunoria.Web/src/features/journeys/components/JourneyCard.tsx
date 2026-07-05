import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Journey } from "@/features/journeys/types";

interface JourneyCardProps {
  journey: Journey;
  onSelect?: (journey: Journey) => void;
  onViewScenes?: (journey: Journey) => void;
}

export function JourneyCard({
  journey,
  onSelect,
  onViewScenes,
}: JourneyCardProps) {
  return (
    <MediaCard
      title={journey.name}
      description={journey.description}
      imageUrl={journey.photoUrl}
      onClick={onSelect ? () => onSelect(journey) : undefined}
    >
      <StatGrid className="mt-4 px-4 pb-4">
        <Stat
          label="Created"
          value={new Intl.DateTimeFormat().format(new Date(journey.createDate))}
        />
      </StatGrid>
      {onViewScenes && (
        <div className="mt-auto border-t border-border px-4 py-3">
          <button
            type="button"
            onClick={(event) => {
              event.stopPropagation();
              onViewScenes(journey);
            }}
            className="w-full rounded-lg border border-brand-subtle/50 px-4 py-2 text-sm font-semibold text-brand-hover transition hover:bg-brand/10"
          >
            View scenes
          </button>
        </div>
      )}
    </MediaCard>
  );
}
