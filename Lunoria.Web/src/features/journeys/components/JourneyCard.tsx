import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import { Button } from "@/components/ui";
import type { Journey } from "@/features/journeys/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faImages, faPen, faTrash } from "@fortawesome/free-solid-svg-icons";

interface JourneyCardProps {
  journey: Journey;
  onSelect?: (journey: Journey) => void;
  onDelete?: (journey: Journey) => void;
  onViewScenes?: (journey: Journey) => void;
}

export function JourneyCard({
  journey,
  onSelect,
  onDelete,
  onViewScenes,
}: JourneyCardProps) {
  return (
    <MediaCard
      title={journey.name}
      description={journey.description}
      imageUrl={journey.photoUrl}
    >
      <StatGrid className="mt-4 px-4 pb-4">
        <Stat
          label="Created"
          value={new Intl.DateTimeFormat().format(new Date(journey.createDate))}
        />
      </StatGrid>
      {onViewScenes && (
        <div className="mt-auto border-t border-border px-4 py-3 flex gap-2 justify-end">
          <Button
            onClick={(event) => {
              event.stopPropagation();
              onViewScenes(journey);
            }}
            variant="primary"
            inverted
            leftIcon={<FontAwesomeIcon icon={faImages} />}
          >
            Scenes
          </Button>
          <Button
            onClick={onDelete ? () => onDelete(journey) : undefined}
            variant="danger"
            inverted
            size="md"
            leftIcon={<FontAwesomeIcon icon={faTrash} />}
          >
            Delete
          </Button>
          <Button
            variant="secondary"
            size="md"
            inverted
            onClick={onSelect ? () => onSelect(journey) : undefined}
            leftIcon={<FontAwesomeIcon icon={faPen} />}
          >
            Edit
          </Button>
        </div>
      )}
    </MediaCard>
  );
}
