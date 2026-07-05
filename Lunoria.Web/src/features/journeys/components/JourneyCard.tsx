import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Journey } from "@/features/journeys/types";

interface JourneyCardProps {
  journey: Journey;
  onSelect?: (journey: Journey) => void;
}

export function JourneyCard({ journey, onSelect }: JourneyCardProps) {
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
    </MediaCard>
  );
}
