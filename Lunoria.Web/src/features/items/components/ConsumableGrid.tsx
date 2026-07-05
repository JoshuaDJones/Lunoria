import { CardGrid } from "@/components/ui/CardGrid";
import { ConsumableCard } from "@/features/items/components/ConsumableCard";
import type { Item } from "@/features/items/types";

interface ConsumableGridProps {
  items: Item[];
  onSelect?: (item: Item) => void;
}

export function ConsumableGrid({ items, onSelect }: ConsumableGridProps) {
  return (
    <CardGrid>
      {items.map((item) => (
        <ConsumableCard key={item.id} item={item} onSelect={onSelect} />
      ))}
    </CardGrid>
  );
}
