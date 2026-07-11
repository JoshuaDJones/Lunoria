import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { CollectionPage } from "@/components/layout/CollectionPage";
import { requiredPhoto, textValue } from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Drawer } from "@/components/ui/Drawer";
import {
  createJourney,
  deleteJourney,
  JourneyGrid,
  listJourneys,
  updateJourney,
  type Journey,
} from "@/features/journeys";
import { useConfirmDialog, useToast } from "@/app/providers";
import { getApiError } from "@/lib/apiClient";

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
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [editing, setEditing] = useState<Journey | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  const openConfirmDelete = async (journey: Journey) => {
    const confirmed = await confirm({
      title: `Delete journey "${journey.name}"?`,
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteJourney(journey.id);
      setReloadKey((value) => value + 1);
      toast.success(`Journey "${journey.name}" was deleted.`);
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to delete journey",
      );
    }
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
            onDelete={openConfirmDelete}
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
                toast.success(`Journey "${input.name}" was updated.`);
              } else {
                await createJourney({ ...input, photo: requiredPhoto(photo) });
                toast.success(`Journey "${input.name}" was created.`);
              }

              handleSaved();
            }}
          />
        </Drawer>
      )}
    </>
  );
}
