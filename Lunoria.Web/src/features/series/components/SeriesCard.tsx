import { Button, MediaCard, Stat, StatGrid } from "@/components/ui";
import type { Series } from "@/features/series/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faImages, faPen, faTrash } from "@fortawesome/free-solid-svg-icons";

interface SeriesCardProps {
    series: Series;
    onSelect: (series: Series) => void;
    onDelete: (series: Series) => void;
    onViewJourneys: (series: Series) => void;
}

export function SeriesCard({
    series,
    onSelect,
    onDelete,
    onViewJourneys,
}: SeriesCardProps){
    return (
        <MediaCard
            title={series.name}
            description={series.description ?? ""}
            imageUrl={series.photoUrl}>
            <StatGrid className="mt-4 px-4 pb-4">
                <Stat label="Created" value={new Intl.DateTimeFormat().format(new Date(series.CreatedAt))}/>
            </StatGrid>
            <div className="mt-auto border-t border-border px-4 py-3 flex gap-2 justify-end">
          <Button
            onClick={(event) => {
              event.stopPropagation();
              onViewJourneys(series);
            }}
            variant="primary"
            inverted
            leftIcon={<FontAwesomeIcon icon={faImages} />}
          >
            Scenes
          </Button>
          <Button
            onClick={() => onDelete(series)}
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
            onClick={() => onSelect(series)}
            leftIcon={<FontAwesomeIcon icon={faPen} />}
          >
            Edit
          </Button>
        </div>
        </MediaCard>
    )
}

