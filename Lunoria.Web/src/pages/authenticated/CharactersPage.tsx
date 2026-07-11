import { useCallback, useState } from "react";
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
import { Drawer, Select } from "@/components/ui";
import { useConfirmDialog, useToast } from "@/app/providers";
import { getApiError } from "@/lib/apiClient";
import {
  CharacterGrid,
  CharacterType,
  createCharacter,
  deleteCharacter,
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
  {
    name: "characterType",
    label: "Character type",
    type: "radio",
    required: true,
    options: [
      { label: "Playable character", value: "player" },
      { label: "NPC", value: "npc" },
      { label: "Enemy", value: "enemy" },
    ],
  },
];

export function CharactersPage() {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
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

  const openConfirmDelete = async (character: Character) => {
    const confirmed = await confirm({
      title: `Delete character "${character.name}"?`,
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteCharacter(character.id);
      setReloadKey((value) => value + 1);
      toast.success(`Character "${character.name}" was deleted.`);
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to delete character",
      );
    }
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
            <Select
              id="character-type-filter"
              value={typeFilter}
              onChange={(event) =>
                setTypeFilter(Number(event.target.value) as CharacterType)
              }
              className="w-auto px-3 py-2"
            >
              <option value={CharacterType.Any}>All characters</option>
              <option value={CharacterType.Player}>Playable characters</option>
              <option value={CharacterType.NPC}>NPCs</option>
              <option value={CharacterType.Enemy}>Enemies</option>
            </Select>
          </div>
        }
        renderItems={(characters) => (
          <CharacterGrid
            characters={characters}
            onSelect={setEditing}
            onDelete={openConfirmDelete}
          />
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
              characterType: editing?.isPlayer
                ? "player"
                : editing?.isNPC
                  ? "npc"
                  : editing?.isEnemy
                    ? "enemy"
                    : editing
                      ? ""
                      : "player",
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
                alternateFormId: nullableNumberValue(values, "alternateFormId"),
                isPlayer: values.characterType === "player",
                isNPC: values.characterType === "npc",
                isEnemy: values.characterType === "enemy",
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
