import { useCallback, useState } from "react";
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
  CharacterGrid,
  CharacterType,
  createCharacter,
  listCharacters,
  updateCharacter,
  type Character,
} from "@/features/characters";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  { name: "maxHp", label: "Max HP", type: "number", required: true },
  { name: "maxMp", label: "Max MP", type: "number", required: true },
  { name: "meleeAttackDamage", label: "Melee damage", type: "number" },
  { name: "bowAttackDamage", label: "Bow damage", type: "number" },
  { name: "movement", label: "Movement", type: "number", required: true },
  {
    name: "baseMaxConsumableInventory",
    label: "Consumable capacity",
    type: "number",
    required: true,
  },
  {
    name: "baseMaxEquippableInventory",
    label: "Equipment capacity",
    type: "number",
    required: true,
  },
  { name: "alternateFormId", label: "Alternate form ID", type: "number" },
  { name: "isPlayer", label: "Playable character", type: "checkbox" },
  { name: "isNPC", label: "NPC", type: "checkbox" },
  { name: "isEnemy", label: "Enemy", type: "checkbox" },
];

export function CharactersPage() {
  const [typeFilter, setTypeFilter] = useState(CharacterType.Any);
  const [editing, setEditing] = useState<Character | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);

  const loadCharacters = useCallback(
    () => listCharacters({ typeFilter }),
    [typeFilter],
  );

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  return (
    <>
      <CollectionPage
        key={typeFilter}
        title="Characters"
        itemName="character"
        loadItems={loadCharacters}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        toolbar={
          <div className="flex items-center gap-3">
            <label
              htmlFor="character-type-filter"
              className="text-sm font-semibold text-content-secondary"
            >
              Character type
            </label>
            <select
              id="character-type-filter"
              value={typeFilter}
              onChange={(event) =>
                setTypeFilter(Number(event.target.value) as CharacterType)
              }
              className="rounded-lg border border-border bg-surface-raised px-3 py-2 text-content outline-none transition focus:border-brand-hover focus:ring-2 focus:ring-brand-hover/20"
            >
              <option value={CharacterType.Any}>All characters</option>
              <option value={CharacterType.Player}>Playable characters</option>
              <option value={CharacterType.NPC}>NPCs</option>
              <option value={CharacterType.Enemy}>Enemies</option>
            </select>
          </div>
        }
        renderItems={(characters) => (
          <CharacterGrid characters={characters} onSelect={setEditing} />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? "Edit character" : "Create character"}
          onClose={() => setEditing(undefined)}
        >
          <ResourceForm
            fields={fields}
            initialValues={{
              name: editing?.name ?? "",
              description: editing?.description ?? "",
              maxHp: String(editing?.maxHp ?? 0),
              maxMp: String(editing?.maxMp ?? 0),
              meleeAttackDamage: String(editing?.meleeAttackDamage ?? ""),
              bowAttackDamage: String(editing?.bowAttackDamage ?? ""),
              movement: String(editing?.movement ?? 0),
              baseMaxConsumableInventory: String(
                editing?.baseMaxConsumableInventory ?? 0,
              ),
              baseMaxEquippableInventory: String(
                editing?.baseMaxEquippableInventory ?? 0,
              ),
              alternateFormId: String(editing?.alternateFormId ?? ""),
              isPlayer: editing?.isPlayer ?? false,
              isNPC: editing?.isNPC ?? false,
              isEnemy: editing?.isEnemy ?? false,
            }}
            existingPhotoUrl={editing?.photoUrl}
            requirePhoto={!editing}
            onSubmit={async (values, photo) => {
              const input = {
                name: textValue(values, "name"),
                description: textValue(values, "description"),
                maxHp: numberValue(values, "maxHp"),
                maxMp: numberValue(values, "maxMp"),
                meleeAttackDamage: nullableNumberValue(
                  values,
                  "meleeAttackDamage",
                ),
                bowAttackDamage: nullableNumberValue(values, "bowAttackDamage"),
                movement: numberValue(values, "movement"),
                baseMaxConsumableInventory: numberValue(
                  values,
                  "baseMaxConsumableInventory",
                ),
                baseMaxEquippableInventory: numberValue(
                  values,
                  "baseMaxEquippableInventory",
                ),
                alternateFormId: nullableNumberValue(
                  values,
                  "alternateFormId",
                ),
                isPlayer: booleanValue(values, "isPlayer"),
                isNPC: booleanValue(values, "isNPC"),
                isEnemy: booleanValue(values, "isEnemy"),
              };

              if (editing) {
                await updateCharacter(editing.id, { ...input, photo });
              } else {
                await createCharacter({
                  ...input,
                  photo: requiredPhoto(photo),
                });
              }

              handleSaved();
            }}
          />
        </Drawer>
      )}
    </>
  );
}
