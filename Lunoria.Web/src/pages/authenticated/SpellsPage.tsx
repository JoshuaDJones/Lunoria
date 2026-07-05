import { CollectionPage } from "@/components/layout/CollectionPage";
import { listSpells, SpellGrid } from "@/features/spells";

export function SpellsPage() {
  return (
    <CollectionPage
      title="Spells"
      itemName="spell"
      loadItems={listSpells}
      renderItems={(spells) => <SpellGrid spells={spells} />}
    />
  );
}
