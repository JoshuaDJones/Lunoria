import { useCallback, useState } from "react";
import { Link, Navigate, useNavigate, useParams } from "react-router-dom";
import { CollectionPage } from "@/components/layout/CollectionPage";
import {
  requiredPhoto,
  textValue,
} from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { Drawer } from "@/components/ui/Drawer";
import {
  createScene,
  listScenes,
  SceneGrid,
  updateScene,
  type Scene,
} from "@/features/scenes";

const fields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  { name: "gridUrl", label: "Grid URL", required: true },
];

export function JourneyScenesPage() {
  const { journeyId: journeyIdParam } = useParams();
  const journeyId = Number(journeyIdParam);
  const navigate = useNavigate();
  const [editing, setEditing] = useState<Scene | null | undefined>();
  const [reloadKey, setReloadKey] = useState(0);

  const loadJourneyScenes = useCallback(
    () => listScenes({ journeyId }),
    [journeyId],
  );

  if (!Number.isInteger(journeyId) || journeyId <= 0) {
    return <Navigate to="/home" replace />;
  }

  const handleSaved = () => {
    setEditing(undefined);
    setReloadKey((value) => value + 1);
  };

  return (
    <>
      <CollectionPage
        title="Scenes"
        itemName="scene"
        loadItems={loadJourneyScenes}
        reloadKey={reloadKey}
        onAdd={() => setEditing(null)}
        toolbar={            
                      <Link
              to="/home"
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to journeys
            </Link>
        }
        renderItems={(scenes) => (
          <SceneGrid
            scenes={scenes}
            onEdit={setEditing}
            onViewDialogs={(scene) =>
              navigate(`/journeys/${journeyId}/scenes/${scene.id}/dialogs`)
            }
          />
        )}
      />

      {editing !== undefined && (
        <Drawer
          title={editing ? "Edit scene" : "Create scene"}
          onClose={() => setEditing(undefined)}
        >
          <ResourceForm
            fields={fields}
            initialValues={{
              name: editing?.name ?? "",
              description: editing?.description ?? "",
              gridUrl: editing?.gridUrl ?? "",
            }}
            existingPhotoUrl={editing?.photoUrl}
            requirePhoto={!editing}
            onSubmit={async (values, photo) => {
              const input = {
                journeyId,
                name: textValue(values, "name"),
                description: textValue(values, "description"),
                gridUrl: textValue(values, "gridUrl"),
              };

              if (editing) {
                await updateScene(editing.id, { ...input, photo });
              } else {
                await createScene({ ...input, photo: requiredPhoto(photo) });
              }

              handleSaved();
            }}
          />
        </Drawer>
      )}
    </>
  );
}
