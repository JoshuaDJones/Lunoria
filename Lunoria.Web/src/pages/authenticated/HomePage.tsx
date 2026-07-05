import { CollectionPage } from "@/components/layout/CollectionPage";
import { JourneyGrid, listJourneys } from "@/features/journeys";

export function HomePage() {
  return (
    <CollectionPage
      title="Journeys"
      itemName="journey"
      loadItems={listJourneys}
      renderItems={(journeys) => <JourneyGrid journeys={journeys} />}
    />
  );
}
