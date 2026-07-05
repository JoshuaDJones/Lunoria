import { CardGrid } from "@/components/ui/CardGrid";
import { JourneyCard } from "@/features/journeys/components/JourneyCard";
import type { Journey } from "@/features/journeys/types";

interface JourneyGridProps {
  journeys: Journey[];
}

export function JourneyGrid({ journeys }: JourneyGridProps) {
  return (
    <CardGrid>
      {journeys.map((journey) => (
        <JourneyCard key={journey.id} journey={journey} />
      ))}
    </CardGrid>
  );
}
