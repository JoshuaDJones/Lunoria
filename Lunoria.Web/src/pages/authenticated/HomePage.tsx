import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { CollectionPage } from "@/components/layout/CollectionPage";
import { requiredPhoto, textValue } from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Drawer } from "@/components/ui/Drawer";
import { useConfirmDialog, useToast } from "@/app/providers";
import { getApiError } from "@/lib/apiClient";
import { Series } from "@/features/series/types";
import { createSeries, deleteSeries, listSeries, updateSeries } from "@/features/series/api/seriesApi";
import { SeriesGrid } from "@/features/series/components/SeriesGrid";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: false,
  },
];

export function HomePage() {
  const navigate = useNavigate();
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [editing, setEditing] = useState<Series | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  const openConfirmDelete = async (series: Series) => {
    const confirmed = await confirm({
      title: `Delete series "${series.name}"?`,
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteSeries(series.id);
      setReloadKey((value) => value + 1);
      toast.success(`Series "${series.name}" was deleted.`);
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to delete series",
      );
    }
  };

  return (
    <>
      <CollectionPage
        title="Series"
        itemName="series"
        loadItems={listSeries}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        renderItems={(series) => (
          <SeriesGrid 
            series={series}
            onDelete={openConfirmDelete}
            onSelect={setEditing}
            onViewJourneys={(series) =>
              navigate(`/series/${series.id}/journeys`)
            }
          />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? `Edit series` : "Create new series"}
          onClose={() => setEditing(undefined)}>
            <ResourceForm fields={fields}
            initialValues={{
              name: editing?.name ?? "",
              description: editing?.description ?? ""
            }}
            existingPhotoUrl={editing?.photoUrl ?? undefined}
            requirePhoto={!editing}
            onSubmit={async (values, photoFile) => {
              const input = {
                name: textValue(values, "name"),
                description: textValue(values, "description"),
                photo: photoFile ?? undefined,
              };

              if (editing) {
                await updateSeries(editing.id, input);
                toast.success(`Series "${editing.name}" was updated.`);
              } else {
                await createSeries(input);
                toast.success(`Series "${input.name}" was created.`);
              }

              handleSaved();
            }}
            />
          </Drawer>
      )}
    </>
  );
}
