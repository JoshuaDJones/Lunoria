import { CardGrid } from "@/components/ui/CardGrid";
import { JourneyCard } from "@/features/journeys/components/JourneyCard";
import type { Journey } from "@/features/journeys/types";

interface JourneyGridProps {
  journeys: Journey[];
  onSelect?: (journey: Journey) => void;
  onDelete?: (journey: Journey) => void;
  onViewScenes?: (journey: Journey) => void;
}

export function JourneyGrid({
  journeys,
  onSelect,
  onDelete,
  onViewScenes,
}: JourneyGridProps) {
  return (
    <CardGrid>
      {journeys.map((journey) => (
        <JourneyCard
          key={journey.id}
          journey={journey}
          onSelect={onSelect}
          onDelete={onDelete}
          onViewScenes={onViewScenes}
        />
      ))}
    </CardGrid>
  );
}
