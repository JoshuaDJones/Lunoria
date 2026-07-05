import { CardGrid } from "@/components/ui/CardGrid";
import { ConsumableCard } from "@/features/items/components/ConsumableCard";
import type { Item } from "@/features/items/types";

interface ConsumableGridProps {
  items: Item[];
}

export function ConsumableGrid({ items }: ConsumableGridProps) {
  return (
    <CardGrid>
      {items.map((item) => (
        <ConsumableCard key={item.id} item={item} />
      ))}
    </CardGrid>
  );
}
