import { Card } from "@/components/ui/Card";
import type { Journey } from "@/features/journeys/types";

interface JourneyCardProps {
  journey: Journey;
}

export function JourneyCard({ journey }: JourneyCardProps) {
  return (
    <Card className="flex min-h-52 flex-col transition hover:border-brand-subtle/50 hover:bg-surface-raised/90">
      <div className="flex flex-1 gap-4 p-4">
        <div className="min-w-0 flex-1">
          <h2 className="wrap-break-word text-2xl font-semibold text-content">
            {journey.name}
          </h2>
          <p className="mt-2 line-clamp-4 wrap-break-word text-sm text-content-secondary">
            {journey.description}
          </p>
        </div>

        {journey.photoUrl && (
          <img
            src={journey.photoUrl}
            alt=""
            className="h-36 w-2/5 shrink-0 rounded-lg object-cover"
          />
        )}
      </div>
    </Card>
  );
}
