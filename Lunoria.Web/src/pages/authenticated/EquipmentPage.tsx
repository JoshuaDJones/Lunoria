import { CollectionPage } from "@/components/layout/CollectionPage";
import { EquipmentGrid, listEquipment } from "@/features/equipment";

export function EquipmentPage() {
  return (
    <CollectionPage
      title="Equipment"
      itemName="equipment"
      loadItems={listEquipment}
      renderItems={(equipment) => <EquipmentGrid equipment={equipment} />}
    />
  );
}
