import { useState } from "react";
import { CollectionPage } from "@/components/layout/CollectionPage";
import {
  booleanValue,
  nullableNumberValue,
  numberValue,
  requiredPhoto,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Drawer } from "@/components/ui/Drawer";
import {
  createSpell,
  listSpells,
  SpellGrid,
  updateSpell,
  type Spell,
} from "@/features/spells";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  { name: "spellTypeId", label: "Spell type ID", type: "number", required: true },
  { name: "range", label: "Range", type: "number", required: true },
  { name: "mpCost", label: "MP cost", type: "number", required: true },
  { name: "isRadius", label: "Area effect", type: "checkbox" },
  { name: "damageEffect", label: "Damage effect", type: "number" },
  { name: "healthEffect", label: "Health effect", type: "number" },
  { name: "magicEffect", label: "Magic effect", type: "number" },
];

export function SpellsPage() {
  const [editing, setEditing] = useState<Spell | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  return (
    <>
      <CollectionPage
        title="Spells"
        itemName="spell"
        loadItems={listSpells}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        renderItems={(spells) => (
          <SpellGrid spells={spells} onSelect={setEditing} />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? "Edit spell" : "Create spell"}
          onClose={() => setEditing(undefined)}
        >
          <ResourceForm
            fields={fields}
            initialValues={{
              name: editing?.name ?? "",
              description: editing?.description ?? "",
              spellTypeId: String(editing?.spellTypeId ?? ""),
              range: String(editing?.range ?? 0),
              mpCost: String(editing?.mpCost ?? 0),
              isRadius: editing?.isRadius ?? false,
              damageEffect: String(editing?.damageEffect ?? ""),
              healthEffect: String(editing?.healthEffect ?? ""),
              magicEffect: String(editing?.magicEffect ?? ""),
            }}
            existingPhotoUrl={editing?.photoUrl}
            requirePhoto={!editing}
            onSubmit={async (values, photo) => {
              const input = {
                name: textValue(values, "name"),
                description: textValue(values, "description"),
                spellTypeId: numberValue(values, "spellTypeId"),
                range: numberValue(values, "range"),
                mpCost: numberValue(values, "mpCost"),
                isRadius: booleanValue(values, "isRadius"),
                damageEffect: nullableNumberValue(values, "damageEffect"),
                healthEffect: nullableNumberValue(values, "healthEffect"),
                magicEffect: nullableNumberValue(values, "magicEffect"),
              };

              if (editing) {
                await updateSpell(editing.id, { ...input, photo });
              } else {
                await createSpell({ ...input, photo: requiredPhoto(photo) });
              }

              handleSaved();
            }}
          />
        </Drawer>
      )}
    </>
  );
}
