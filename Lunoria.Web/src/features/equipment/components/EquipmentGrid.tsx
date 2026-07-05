import { CardGrid } from "@/components/ui/CardGrid";
import { EquipmentCard } from "@/features/equipment/components/EquipmentCard";
import type { EquippableItem } from "@/features/equipment/types";

interface EquipmentGridProps {
  equipment: EquippableItem[];
}

export function EquipmentGrid({ equipment }: EquipmentGridProps) {
  return (
    <CardGrid>
      {equipment.map((item) => (
        <EquipmentCard key={item.id} item={item} />
      ))}
    </CardGrid>
  );
}
