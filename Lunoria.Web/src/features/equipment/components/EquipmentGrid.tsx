import { CardGrid } from "@/components/ui/CardGrid";
import { EquipmentCard } from "@/features/equipment/components/EquipmentCard";
import type { EquippableItem } from "@/features/equipment/types";

interface EquipmentGridProps {
  equipment: EquippableItem[];
  onSelect?: (item: EquippableItem) => void;
  onDelete?: (item: EquippableItem) => void;
}

export function EquipmentGrid({
  equipment,
  onSelect,
  onDelete,
}: EquipmentGridProps) {
  return (
    <CardGrid>
      {equipment.map((item) => (
        <EquipmentCard
          key={item.id}
          item={item}
          onSelect={onSelect}
          onDelete={onDelete}
        />
      ))}
    </CardGrid>
  );
}
