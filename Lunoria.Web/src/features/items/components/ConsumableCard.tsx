import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Item } from "@/features/items/types";

interface ConsumableCardProps {
  item: Item;
  onSelect?: (item: Item) => void;
}

export function ConsumableCard({ item, onSelect }: ConsumableCardProps) {
  return (
    <MediaCard
      title={item.name}
      description={item.description}
      imageUrl={item.photoUrl}
      onClick={onSelect ? () => onSelect(item) : undefined}
    >
      <StatGrid className="mt-4 px-4 pb-4">
        <Stat label="HP effect" value={item.hpEffect} />
        <Stat label="MP effect" value={item.mpEffect} />
      </StatGrid>
    </MediaCard>
  );
}
