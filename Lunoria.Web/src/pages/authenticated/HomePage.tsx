import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { CollectionPage } from "@/components/layout/CollectionPage";
import {
  requiredPhoto,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Drawer } from "@/components/ui/Drawer";
import {
  createJourney,
  JourneyGrid,
  listJourneys,
  updateJourney,
  type Journey,
} from "@/features/journeys";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
];

export function HomePage() {
  const navigate = useNavigate();
  const [editing, setEditing] = useState<Journey | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  return (
    <>
      <CollectionPage
        title="Journeys"
        itemName="journey"
        loadItems={listJourneys}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        renderItems={(journeys) => (
          <JourneyGrid
            journeys={journeys}
            onSelect={setEditing}
            onViewScenes={(journey) =>
              navigate(`/journeys/${journey.id}/scenes`)
            }
          />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? "Edit journey" : "Create journey"}
          onClose={() => setEditing(undefined)}
        >
          <ResourceForm
            fields={fields}
            initialValues={{
              name: editing?.name ?? "",
              description: editing?.description ?? "",
            }}
            existingPhotoUrl={editing?.photoUrl}
            requirePhoto={!editing}
            onSubmit={async (values, photo) => {
              const input = {
                name: textValue(values, "name"),
                description: textValue(values, "description"),
              };

              if (editing) {
                await updateJourney(editing.id, { ...input, photo });
              } else {
                await createJourney({ ...input, photo: requiredPhoto(photo) });
              }

              handleSaved();
            }}
          />
        </Drawer>
      )}
    </>
  );
}
