import { useCallback, useEffect, useState } from "react";
import { CollectionPage } from "@/components/layout/CollectionPage";
import {
  booleanValue,
  nullableNumberValue,
  numberValue,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Button, Drawer, Select } from "@/components/ui";
import { useConfirmDialog, useToast } from "@/app/providers";
import { getApiError } from "@/lib/apiClient";
import {
  createSpell,
  deleteSpell,
  listSpells,
  listSpellTypes,
  SpellGrid,
  SpellTypeManager,
  updateSpell,
  type Spell,
  type SpellType,
} from "@/features/spells";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  {
    name: "spellTypeId",
    label: "Spell type ID",
    type: "select",
    required: true,
  },
  { name: "range", label: "Range", type: "number", required: true },
  { name: "mpCost", label: "MP cost", type: "number", required: true },
  { name: "isRadius", label: "Area effect", type: "checkbox" },
  { name: "damageEffect", label: "Damage effect", type: "number" },
  { name: "healthEffect", label: "Health effect", type: "number" },
  { name: "magicEffect", label: "Magic effect", type: "number" },
];

export function SpellsPage() {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [editing, setEditing] = useState<Spell | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);
  const [spellTypes, setSpellTypes] = useState<SpellType[]>([]);
  const [managingSpellTypes, setManagingSpellTypes] = useState(false);
  const [spellTypeFilter, setSpellTypeFilter] = useState(0);

  const loadFilteredSpells = useCallback(
    () =>
      listSpells(
        spellTypeFilter === 0 ? {} : { spellTypeId: spellTypeFilter },
      ),
    [spellTypeFilter],
  );

  const loadAvailableSpellTypes = useCallback(async () => {
    try {
      setSpellTypes(await listSpellTypes());
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to load spell types",
      );
    }
  }, [toast]);

  useEffect(() => {
    let isCurrent = true;

    void listSpellTypes()
      .then((items) => {
        if (isCurrent) setSpellTypes(items);
      })
      .catch((requestError: unknown) => {
        if (isCurrent) {
          toast.error(
            getApiError(requestError).message,
            "Unable to load spell types",
          );
        }
      });

    return () => {
      isCurrent = false;
    };
  }, [toast]);

  const spellFields = fields.map((field) =>
    field.name === "spellTypeId"
      ? {
          ...field,
          options: spellTypes.map((spellType) => ({
            label: spellType.name,
            value: String(spellType.id),
          })),
        }
      : field,
  );

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  const openConfirmDelete = async (spell: Spell) => {
    const confirmed = await confirm({
      title: `Delete spell "${spell.name}"?`,
      message:
        "This will remove the spell from all characters, journeys, and equipment. This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteSpell(spell.id);
      setReloadKey((value) => value + 1);
      toast.success(`Spell "${spell.name}" was deleted.`);
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to delete spell");
    }
  };

  return (
    <>
      <CollectionPage
        key={spellTypeFilter}
        title="Spells"
        itemName="spell"
        loadItems={loadFilteredSpells}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        toolbar={
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-3">
              <label
                htmlFor="spell-type-filter"
                className="text-sm font-semibold text-content-secondary"
              >
                Spell type
              </label>
              <Select
                id="spell-type-filter"
                value={spellTypeFilter}
                onChange={(event) =>
                  setSpellTypeFilter(Number(event.target.value))
                }
                className="w-auto px-3 py-2"
              >
                <option value={0}>All spell types</option>
                {spellTypes.map((spellType) => (
                  <option key={spellType.id} value={spellType.id}>
                    {spellType.name}
                  </option>
                ))}
              </Select>
            </div>
            <Button
              variant="magic"
              onClick={() => setManagingSpellTypes(true)}
            >
              Manage spell types
            </Button>
          </div>
        }
        renderItems={(spells) => (
          <SpellGrid
            spells={spells}
            onSelect={setEditing}
            onDelete={openConfirmDelete}
          />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? "Edit spell" : "Create spell"}
          onClose={() => setEditing(undefined)}
        >
          <ResourceForm
            fields={spellFields}
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
            existingPhotoUrl={editing?.photoUrl ?? undefined}
            fallbackPhotoUrl={editing?.spellType?.photoUrl}
            allowRemoveExistingPhoto
            onSubmit={async (values, photo, removeExistingPhoto) => {
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
                await updateSpell(editing.id, {
                  ...input,
                  photo,
                  removePhoto: removeExistingPhoto,
                });
                toast.success(`Spell "${input.name}" was updated.`);
              } else {
                await createSpell({ ...input, photo });
                toast.success(`Spell "${input.name}" was created.`);
              }

              handleSaved();
            }}
          />
        </Drawer>
      )}

      {managingSpellTypes && (
        <Drawer
          title="Manage spell types"
          onClose={() => setManagingSpellTypes(false)}
        >
          <SpellTypeManager onChanged={() => void loadAvailableSpellTypes()} />
        </Drawer>
      )}
    </>
  );
}
