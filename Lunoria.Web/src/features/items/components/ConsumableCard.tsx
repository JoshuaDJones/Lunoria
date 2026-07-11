import { Button } from "@/components/ui/Button";
import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Item } from "@/features/items/types";
import { faPen, faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface ConsumableCardProps {
  item: Item;
  onSelect?: (item: Item) => void;
  onDelete?: (item: Item) => void;
}

export function ConsumableCard({
  item,
  onSelect,
  onDelete,
}: ConsumableCardProps) {
  return (
    <MediaCard
      title={item.name}
      description={item.description}
      imageUrl={item.photoUrl}
    >
      <StatGrid className="mt-4 px-4 pb-4">
        <Stat label="HP effect" value={item.hpEffect} />
        <Stat label="MP effect" value={item.mpEffect} />
      </StatGrid>
      <div className="mt-auto border-t border-border px-4 py-3 flex gap-2 justify-end">
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onDelete?.(item);
          }}
          variant="danger"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faTrash} />}
        >
          Delete
        </Button>
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onSelect?.(item);
          }}
          variant="primary"
          size="md"
          inverted
          leftIcon={<FontAwesomeIcon icon={faPen} />}
        >
          Edit
        </Button>
      </div>
    </MediaCard>
  );
}
