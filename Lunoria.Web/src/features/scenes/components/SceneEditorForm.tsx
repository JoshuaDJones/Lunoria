import { useState } from "react";
import { Button } from "@/components/ui";
import { requiredPhoto, textValue } from "@/components/forms/formValues";
import {
  ResourceForm,
  type FormValues,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import {
  createScene,
  createSceneGrid,
  deleteSceneGrid,
  updateScene,
  updateSceneGrid,
} from "@/features/scenes/api/scenesApi";
import type { Scene } from "@/features/scenes/types";

type GridMode = "internal" | "external" | "none";

interface SceneDraft {
  values: FormValues;
  photo?: File;
}

interface SceneEditorFormProps {
  journeyId: number;
  scene?: Scene | null;
  onSaved: (sceneName: string, editing: boolean) => void;
}

const sceneFields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  {
    name: "gridMode",
    label: "Grid",
    type: "radio",
    required: true,
    options: [
      { label: "Create a Lunoria grid", value: "internal" },
      { label: "Use an external grid URL", value: "external" },
      { label: "No grid", value: "none" },
    ],
  },
  {
    name: "gridUrl",
    label: "External grid URL",
    required: true,
    visibleWhen: { field: "gridMode", value: "external" },
  },
];

const gridFields: ResourceFormField[] = [
  { name: "rows", label: "Rows", type: "number", required: true },
  { name: "columns", label: "Columns", type: "number", required: true },
  { name: "gridColor", label: "Grid color", type: "color", required: true },
];

function initialGridMode(scene?: Scene | null): GridMode {
  if (scene?.grid) return "internal";
  if (scene?.gridUrl) return "external";
  return "none";
}

export function SceneEditorForm({
  journeyId,
  scene,
  onSaved,
}: SceneEditorFormProps) {
  const [page, setPage] = useState<"scene" | "grid">("scene");
  const [draft, setDraft] = useState<SceneDraft>();
  const [createdScene, setCreatedScene] = useState<Scene>();

  const saveScene = async (
    sceneDraft: SceneDraft,
    grid?: {
      rows: number;
      columns: number;
      gridColor: string;
      background?: File;
      removeBackground: boolean;
    },
  ) => {
    const name = textValue(sceneDraft.values, "name");
    const gridMode = textValue(sceneDraft.values, "gridMode") as GridMode;
    const gridUrl =
      gridMode === "external"
        ? textValue(sceneDraft.values, "gridUrl")
        : "";
    const input = {
      journeyId,
      name,
      description: textValue(sceneDraft.values, "description"),
      gridUrl,
    };

    const existingScene = scene ?? createdScene;
    let savedScene: Scene;
    if (existingScene) {
      savedScene = await updateScene(existingScene.id, {
        ...input,
        photo: sceneDraft.photo,
      });
    } else {
      savedScene = await createScene({
        ...input,
        photo: requiredPhoto(sceneDraft.photo),
      });
      // Creating a scene and its grid requires two API requests. Keep the
      // created scene so a failed grid request retries against the same scene
      // instead of creating a duplicate.
      setCreatedScene(savedScene);
      setDraft((current) =>
        current ? { ...current, photo: undefined } : current,
      );
    }

    if (gridMode === "internal" && grid) {
      const gridInput = {
        rows: grid.rows,
        columns: grid.columns,
        gridColor: grid.gridColor.trim() || "#ffffff",
        background: grid.background,
      };
      if (scene?.grid) {
        await updateSceneGrid(savedScene.id, {
          ...gridInput,
          removeBackground: grid.removeBackground,
        });
      } else {
        await createSceneGrid(savedScene.id, gridInput);
      }
    } else if (gridMode === "none" && scene?.grid) {
      await deleteSceneGrid(savedScene.id);
    }

    onSaved(name, Boolean(scene));
  };

  if (page === "grid" && draft) {
    return (
      <ResourceForm
        key="grid"
        fields={gridFields}
        initialValues={{
          rows: String(scene?.grid?.rows ?? 20),
          columns: String(scene?.grid?.columns ?? 36),
          gridColor: scene?.grid?.gridColor ?? "#ffffff",
        }}
        existingPhotoUrl={scene?.grid?.backgroundImageUrl ?? undefined}
        allowRemoveExistingPhoto={Boolean(scene?.grid?.backgroundImageUrl)}
        onSubmit={async (values, background, removeBackground) => {
          await saveScene(draft, {
            rows: Number(textValue(values, "rows")),
            columns: Number(textValue(values, "columns")),
            gridColor: textValue(values, "gridColor"),
            background,
            removeBackground: Boolean(removeBackground),
          });
        }}
      >
        <Button
          type="button"
          className="w-full"
          onClick={() => setPage("scene")}
        >
          Back to scene
        </Button>
      </ResourceForm>
    );
  }

  const formScene = scene ?? createdScene;
  const mode = initialGridMode(formScene);
  return (
    <ResourceForm
      key="scene"
      fields={sceneFields}
      initialValues={draft?.values ?? {
        name: formScene?.name ?? "",
        description: formScene?.description ?? "",
        gridMode: mode,
        gridUrl: formScene?.gridUrl ?? "",
      }}
      initialPhoto={draft?.photo}
      existingPhotoUrl={formScene?.photoUrl}
      requirePhoto={!formScene}
      submitLabel={(values) =>
        values.gridMode === "internal" ? "Continue to grid" : "Save"
      }
      onSubmit={async (values, photo) => {
        const nextDraft = { values, photo };
        if (values.gridMode === "internal") {
          setDraft(nextDraft);
          setPage("grid");
          return;
        }
        await saveScene(nextDraft);
      }}
    />
  );
}
