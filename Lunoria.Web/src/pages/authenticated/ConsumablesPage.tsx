import { useState } from "react";
import { CollectionPage } from "@/components/layout/CollectionPage";
import {
  numberValue,
  requiredPhoto,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Drawer } from "@/components/ui/Drawer";
import { useConfirmDialog, useToast } from "@/app/providers";
import { getApiError } from "@/lib/apiClient";
import {
  ConsumableGrid,
  createItem,
  deleteItem,
  listItems,
  updateItem,
  type Item,
} from "@/features/items";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  { name: "hpEffect", label: "HP effect", type: "number", required: true },
  { name: "mpEffect", label: "MP effect", type: "number", required: true },
];

export function ConsumablesPage() {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [editing, setEditing] = useState<Item | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  const openConfirmDelete = async (item: Item) => {
    const confirmed = await confirm({
      title: `Delete consumable "${item.name}"?`,
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteItem(item.id);
      setReloadKey((value) => value + 1);
      toast.success(`Consumable "${item.name}" was deleted.`);
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to delete consumable",
      );
    }
  };

  return (
    <>
      <CollectionPage
        title="Consumables"
        itemName="consumable"
        loadItems={listItems}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        renderItems={(items) => (
          <ConsumableGrid
            items={items}
            onSelect={setEditing}
            onDelete={openConfirmDelete}
          />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? "Edit consumable" : "Create consumable"}
          onClose={() => setEditing(undefined)}
        >
          <ResourceForm
            fields={fields}
            initialValues={{
              name: editing?.name ?? "",
              description: editing?.description ?? "",
              hpEffect: String(editing?.hpEffect ?? 0),
              mpEffect: String(editing?.mpEffect ?? 0),
            }}
            existingPhotoUrl={editing?.photoUrl}
            requirePhoto={!editing}
            onSubmit={async (values, photo) => {
              const input = {
                name: textValue(values, "name"),
                description: textValue(values, "description"),
                hpEffect: numberValue(values, "hpEffect"),
                mpEffect: numberValue(values, "mpEffect"),
              };

              if (editing) {
                await updateItem(editing.id, { ...input, photo });
              } else {
                await createItem({ ...input, photo: requiredPhoto(photo) });
              }

              handleSaved();
            }}
          />
        </Drawer>
      )}
    </>
  );
}
