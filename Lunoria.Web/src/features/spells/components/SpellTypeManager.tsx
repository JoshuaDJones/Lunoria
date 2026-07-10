import { useCallback, useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPen, faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";
import { useConfirmDialog, useToast } from "@/app/providers";
import { requiredPhoto, textValue } from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Button } from "@/components/ui";
import { getApiError } from "@/lib/apiClient";
import {
  createSpellType,
  deleteSpellType,
  listSpellTypes,
  updateSpellType,
} from "../api/spellsApi";
import type { SpellType } from "../types";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
];

interface SpellTypeManagerProps {
  onChanged: () => void;
}

export function SpellTypeManager({ onChanged }: SpellTypeManagerProps) {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [spellTypes, setSpellTypes] = useState<SpellType[]>([]);
  const [editing, setEditing] = useState<SpellType | null | undefined>();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const load = useCallback(async () => {
    setIsLoading(true);

    try {
      setSpellTypes(await listSpellTypes());
      setError("");
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    let isCurrent = true;

    void listSpellTypes()
      .then((items) => {
        if (isCurrent) {
          setSpellTypes(items);
          setError("");
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setError(getApiError(requestError).message);
      })
      .finally(() => {
        if (isCurrent) setIsLoading(false);
      });

    return () => {
      isCurrent = false;
    };
  }, []);

  const remove = async (spellType: SpellType) => {
    const confirmed = await confirm({
      title: `Delete spell type "${spellType.name}"?`,
      message:
        "This action cannot be undone. Spell types in use may not be deleted.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteSpellType(spellType.id);
      await load();
      onChanged();
      toast.success(`Spell type "${spellType.name}" was deleted.`);
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to delete spell type",
      );
    }
  };

  if (editing !== undefined) {
    return (
      <div>
        <Button className="mb-5" onClick={() => setEditing(undefined)}>
          Back to spell types
        </Button>
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
              await updateSpellType(editing.id, { ...input, photo });
              toast.success(`Spell type "${input.name}" was updated.`);
            } else {
              await createSpellType({
                ...input,
                photo: requiredPhoto(photo),
              });
              toast.success(`Spell type "${input.name}" was created.`);
            }

            setEditing(undefined);
            await load();
            onChanged();
          }}
        />
      </div>
    );
  }

  return (
    <div>
      <div className="mb-5 flex items-center justify-between gap-4">
        <p className="text-sm text-content-secondary">
          Create and maintain the categories available to spells.
        </p>
        <Button
          variant="add"
          leftIcon={<FontAwesomeIcon icon={faPlus} />}
          onClick={() => setEditing(null)}
        >
          Add type
        </Button>
      </div>

      {isLoading && <p role="status">Loading spell types...</p>}

      {!isLoading && error && (
        <div role="alert" className="rounded-lg border border-danger/40 p-4">
          <p className="text-danger">{error}</p>
          <Button className="mt-3" onClick={() => void load()}>
            Try again
          </Button>
        </div>
      )}

      {!isLoading && !error && spellTypes.length === 0 && (
        <p className="rounded-lg border border-border p-5 text-content-muted">
          No spell types yet.
        </p>
      )}

      {!isLoading && !error && spellTypes.length > 0 && (
        <ul className="space-y-3">
          {spellTypes.map((spellType) => (
            <li
              key={spellType.id}
              className="flex items-center gap-4 rounded-lg border border-border bg-surface p-4"
            >
              <img
                src={spellType.photoUrl}
                alt=""
                className="size-14 rounded-md object-cover"
              />
              <div className="min-w-0 flex-1">
                <h3 className="font-semibold text-content">{spellType.name}</h3>
                <p className="line-clamp-2 text-sm text-content-secondary">
                  {spellType.description}
                </p>
              </div>
              <div className="flex gap-2">
                <Button
                  aria-label={`Edit ${spellType.name}`}
                  onClick={() => setEditing(spellType)}
                  leftIcon={<FontAwesomeIcon icon={faPen} />}
                >
                  Edit
                </Button>
                <Button
                  aria-label={`Delete ${spellType.name}`}
                  variant="danger"
                  inverted
                  onClick={() => void remove(spellType)}
                  leftIcon={<FontAwesomeIcon icon={faTrash} />}
                >
                  Delete
                </Button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
