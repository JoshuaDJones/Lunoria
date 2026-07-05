import { CollectionPage } from "@/components/layout/CollectionPage";
import { ConsumableGrid, listItems } from "@/features/items";

export function ConsumablesPage() {
  return (
    <CollectionPage
      title="Consumables"
      itemName="consumable"
      loadItems={listItems}
      renderItems={(items) => <ConsumableGrid items={items} />}
    />
  );
}
