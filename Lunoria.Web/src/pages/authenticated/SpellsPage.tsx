import { useCallback, useEffect, useState } from "react";
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
import { Button, Drawer } from "@/components/ui";
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
      message: "This action cannot be undone.",
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
        title="Spells"
        itemName="spell"
        loadItems={listSpells}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        toolbar={
          <div className="flex justify-end">
            <Button
              variant="accent"
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
