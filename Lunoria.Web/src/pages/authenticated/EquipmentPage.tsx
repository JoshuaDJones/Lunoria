import { useState } from "react";
import { CollectionPage } from "@/components/layout/CollectionPage";
import {
  nullableNumberValue,
  numberValue,
  requiredPhoto,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Button, Drawer } from "@/components/ui";
import { useModalStack } from "@/app/providers";
import {
  createEquipment,
  EquipmentGrid,
  listEquipment,
  SpellPickerDialog,
  updateEquipment,
  type EquippableItem,
} from "@/features/equipment";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  {
    name: "meleeAttackDamageModifier",
    label: "Melee damage modifier",
    type: "number",
  },
  {
    name: "bowAttackDamageModifier",
    label: "Bow damage modifier",
    type: "number",
  },
  { name: "movementModifier", label: "Movement modifier", type: "number" },
  { name: "maxHpModifier", label: "Max HP modifier", type: "number" },
  { name: "maxMpModifier", label: "Max MP modifier", type: "number" },
  {
    name: "maxConsumableInventoryModifier",
    label: "Consumable capacity modifier",
    type: "number",
  },
  {
    name: "maxEquippableInventoryModifier",
    label: "Equipment capacity modifier",
    type: "number",
  },
  {
    name: "meleeDamageReduction",
    label: "Melee damage reduction",
    type: "number",
  },
  { name: "bowDamageReduction", label: "Bow damage reduction", type: "number" },
  {
    name: "spellDamageReduction",
    label: "Spell damage reduction",
    type: "number",
  },
  {
    name: "affectedSpellTypeId",
    label: "Affected spell type ID",
    type: "number",
  },
  {
    name: "spellDamageModifier",
    label: "Spell damage modifier",
    type: "number",
  },
];

export function EquipmentPage() {
  const modalStack = useModalStack();
  const [editing, setEditing] = useState<EquippableItem | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);
  const [selectedSpellIds, setSelectedSpellIds] = useState<number[]>([]);

  const openCreate = () => {
    setSelectedSpellIds([]);
    setEditing(null);
  };

  const openEdit = (item: EquippableItem) => {
    setSelectedSpellIds(item.addedSpells.map((spell) => spell.id));
    setEditing(item);
  };

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  const openSpellPicker = () => {
    modalStack.push({
      title: "Select spells",
      placement: "center",
      content: (
        <SpellPickerDialog
          selectedIds={selectedSpellIds}
          onClose={modalStack.pop}
          onApply={(spellIds) => {
            setSelectedSpellIds(spellIds);
            modalStack.pop();
          }}
        />
      ),
    });
  };

  return (
    <>
      <CollectionPage
        title="Equipment"
        itemName="equipment"
        loadItems={listEquipment}
        reloadKey={reloadKey}
        onAdd={openCreate}
        renderItems={(equipment) => (
          <EquipmentGrid equipment={equipment} onSelect={openEdit} />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? "Edit equipment" : "Create equipment"}
          onClose={() => setEditing(undefined)}
        >
          <ResourceForm
            fields={fields}
            initialValues={{
              name: editing?.name ?? "",
              description: editing?.description ?? "",
              meleeAttackDamageModifier: String(
                editing?.meleeAttackDamageModifier ?? 0,
              ),
              bowAttackDamageModifier: String(
                editing?.bowAttackDamageModifier ?? 0,
              ),
              movementModifier: String(editing?.movementModifier ?? 0),
              maxHpModifier: String(editing?.maxHpModifier ?? 0),
              maxMpModifier: String(editing?.maxMpModifier ?? 0),
              maxConsumableInventoryModifier: String(
                editing?.maxConsumableInventoryModifier ?? 0,
              ),
              maxEquippableInventoryModifier: String(
                editing?.maxEquippableInventoryModifier ?? 0,
              ),
              meleeDamageReduction: String(editing?.meleeDamageReduction ?? 0),
              bowDamageReduction: String(editing?.bowDamageReduction ?? 0),
              spellDamageReduction: String(editing?.spellDamageReduction ?? 0),
              affectedSpellTypeId: String(editing?.affectedSpellTypeId ?? ""),
              spellDamageModifier: String(editing?.spellDamageModifier ?? 0),
            }}
            existingPhotoUrl={editing?.photoUrl}
            requirePhoto={!editing}
            onSubmit={async (values, photo) => {
              const input = {
                name: textValue(values, "name"),
                description: textValue(values, "description"),
                meleeAttackDamageModifier: numberValue(
                  values,
                  "meleeAttackDamageModifier",
                ),
                bowAttackDamageModifier: numberValue(
                  values,
                  "bowAttackDamageModifier",
                ),
                movementModifier: numberValue(values, "movementModifier"),
                maxHpModifier: numberValue(values, "maxHpModifier"),
                maxMpModifier: numberValue(values, "maxMpModifier"),
                maxConsumableInventoryModifier: numberValue(
                  values,
                  "maxConsumableInventoryModifier",
                ),
                maxEquippableInventoryModifier: numberValue(
                  values,
                  "maxEquippableInventoryModifier",
                ),
                meleeDamageReduction: numberValue(
                  values,
                  "meleeDamageReduction",
                ),
                bowDamageReduction: numberValue(values, "bowDamageReduction"),
                spellDamageReduction: numberValue(
                  values,
                  "spellDamageReduction",
                ),
                affectedSpellTypeId: nullableNumberValue(
                  values,
                  "affectedSpellTypeId",
                ),
                spellDamageModifier: numberValue(values, "spellDamageModifier"),
                addedSpellIds: selectedSpellIds,
              };

              if (editing) {
                await updateEquipment(editing.id, { ...input, photo });
              } else {
                await createEquipment({
                  ...input,
                  photo: requiredPhoto(photo),
                });
              }

              handleSaved();
            }}
          >
            <div className="rounded-xl border border-border bg-surface p-4">
              <div className="flex items-center justify-between gap-4">
                <div>
                  <h3 className="font-semibold text-content">Added spells</h3>
                  <p className="mt-1 text-sm text-content-muted">
                    {selectedSpellIds.length === 0
                      ? "No spells selected"
                      : `${selectedSpellIds.length} spell${selectedSpellIds.length === 1 ? "" : "s"} selected`}
                  </p>
                </div>
                <Button onClick={openSpellPicker} variant="accent">
                  Select spells
                </Button>
              </div>
            </div>
          </ResourceForm>
        </Drawer>
      )}
    </>
  );
}
