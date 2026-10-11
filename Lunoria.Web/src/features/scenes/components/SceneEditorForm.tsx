import { useRef, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { Button, FormField, Input, Textarea } from "@/components/ui";
import {
  createScene,
  createSceneGrid,
  deleteSceneGrid,
  updateScene,
  updateSceneGrid,
} from "@/features/scenes/api/scenesApi";
import { SceneImageField } from "@/features/scenes/components/SceneImageField";
import { SceneGridPreview } from "@/features/scenes/components/SceneGridPreview";
import type { Scene } from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

type GridMode = "internal" | "external" | "none";

interface SceneEditorFormProps {
  journeyId: number;
  scene?: Scene | null;
  onSaved: (sceneName: string, editing: boolean) => void;
  onBusyChange?: (busy: boolean) => void;
}

export function SceneEditorForm({
  journeyId,
  scene,
  onSaved,
  onBusyChange,
}: SceneEditorFormProps) {
  const [name, setName] = useState(scene?.name ?? "");
  const [description, setDescription] = useState(scene?.description ?? "");
  const [gridMode, setGridMode] = useState<GridMode>(
    scene?.grid ? "internal" : scene?.gridUrl ? "external" : "none",
  );
  const [gridUrl, setGridUrl] = useState(scene?.gridUrl ?? "");
  const [rows, setRows] = useState(String(scene?.grid?.rows ?? 20));
  const [columns, setColumns] = useState(String(scene?.grid?.columns ?? 36));
  const [gridColor, setGridColor] = useState(
    scene?.grid?.gridColor ?? "#ffffff",
  );
  const [photo, setPhoto] = useState<File>();
  const [background, setBackground] = useState<File>();
  const [removeBackground, setRemoveBackground] = useState(false);
  const [savedScene, setSavedScene] = useState<Scene | undefined>(
    scene ?? undefined,
  );
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);
  const savingRef = useRef(false);
  const uploadedPhoto = useRef<File | undefined>(undefined);

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (savingRef.current) return;
    setError("");
    if (!savedScene && !photo) {
      setError("A scene background photo is required.");
      return;
    }
    if (!name.trim() || !description.trim()) {
      setError("Name and description are required.");
      return;
    }
    if (gridMode === "external" && !gridUrl.trim()) {
      setError("An external grid URL is required.");
      return;
    }
    if (
      gridMode === "internal" &&
      (!Number.isInteger(Number(rows)) ||
        !Number.isInteger(Number(columns)) ||
        Number(rows) < 1 ||
        Number(rows) > 100 ||
        Number(columns) < 1 ||
        Number(columns) > 100)
    ) {
      setError("Grid rows and columns must each be between 1 and 100.");
      return;
    }
    savingRef.current = true;
    setSaving(true);
    onBusyChange?.(true);
    let sceneWasSaved = false;
    try {
      const input = {
        journeyId,
        name: name.trim(),
        description: description.trim(),
        gridUrl: gridMode === "external" ? gridUrl.trim() : "",
      };
      const result = savedScene
        ? await updateScene(savedScene.id, {
            ...input,
            photo: photo !== uploadedPhoto.current ? photo : undefined,
          })
        : await createScene({ ...input, photo: photo! });
      sceneWasSaved = true;
      uploadedPhoto.current = photo;
      // Retain the ID on partial failure so retry cannot create a duplicate scene.
      setSavedScene(result);
      if (gridMode === "internal") {
        const gridInput = {
          rows: Number(rows),
          columns: Number(columns),
          gridColor,
          background,
        };
        const grid = result.grid
          ? await updateSceneGrid(result.id, { ...gridInput, removeBackground })
          : await createSceneGrid(result.id, gridInput);
        setSavedScene({ ...result, grid });
      } else if (gridMode === "none" && result.grid) {
        await deleteSceneGrid(result.id);
        setSavedScene({ ...result, grid: null });
      }
      // Scene update removes an internal grid when an external URL is supplied.
      onSaved(input.name, Boolean(scene));
    } catch (reason) {
      setError(
        `${sceneWasSaved ? "Scene details were saved, but the grid change could not finish. You can retry without creating another scene. " : ""}${getApiError(reason).message}`,
      );
    } finally {
      savingRef.current = false;
      setSaving(false);
      onBusyChange?.(false);
    }
  };

  return (
    <form onSubmit={(event) => void submit(event)} aria-busy={saving}>
      <fieldset disabled={saving} className="min-w-0 space-y-5">
        <FormField htmlFor="scene-name" label="Name">
          <Input
            id="scene-name"
            required
            value={name}
            onChange={(event) => setName(event.target.value)}
            className="bg-surface"
          />
        </FormField>
        <FormField htmlFor="scene-description" label="Description">
          <Textarea
            id="scene-description"
            required
            rows={4}
            value={description}
            onChange={(event) => setDescription(event.target.value)}
            className="bg-surface"
          />
        </FormField>
        <SceneImageField
          label="Scene background photo"
          tooltip="This photo is used as the background when playing this scene during a playthrough."
          file={photo}
          existingUrl={savedScene?.photoUrl}
          disabled={saving}
          onChange={setPhoto}
        />

        <fieldset className="space-y-3">
          <legend className="mb-3 text-sm font-medium text-content-secondary">
            Grid
          </legend>
          {(
            [
              ["none", "No grid"],
              [
                "internal",
                scene?.grid ? "Use a Lunoria grid" : "Create a Lunoria grid",
              ],
              ["external", "Use an external grid URL"],
            ] as const
          ).map(([mode, label]) => (
            <label
              key={mode}
              className="flex cursor-pointer items-center gap-3 text-sm text-content-secondary"
            >
              <input
                type="radio"
                name="scene-grid-mode"
                value={mode}
                checked={gridMode === mode}
                onChange={() => setGridMode(mode)}
                aria-controls={
                  mode === "internal"
                    ? "scene-internal-grid"
                    : mode === "external"
                      ? "scene-external-grid"
                      : undefined
                }
                className="size-4 accent-brand"
              />
              {label}
            </label>
          ))}
        </fieldset>

        {/* Keep controls mounted to preserve drafts and files; disable hidden required fields. */}
        <div
          id="scene-internal-grid"
          inert={gridMode !== "internal"}
          aria-hidden={gridMode !== "internal"}
          className={`grid transition-[grid-template-rows,opacity] duration-250 motion-reduce:transition-none ${gridMode === "internal" ? "grid-rows-[1fr] opacity-100" : "grid-rows-[0fr] opacity-0"}`}
        >
          <div className="min-h-0 overflow-hidden">
            <fieldset
              disabled={saving || gridMode !== "internal"}
              className="min-w-0 space-y-4 rounded-xl border border-border bg-surface/50 p-4"
            >
              <legend className="px-1 text-sm font-semibold text-content">
                Lunoria grid settings
              </legend>
              <div className="grid grid-cols-2 gap-4">
                <FormField htmlFor="scene-grid-rows" label="Rows">
                  <Input
                    id="scene-grid-rows"
                    type="number"
                    required
                    min={1}
                    max={100}
                    step={1}
                    value={rows}
                    onChange={(event) => setRows(event.target.value)}
                  />
                </FormField>
                <FormField htmlFor="scene-grid-columns" label="Columns">
                  <Input
                    id="scene-grid-columns"
                    type="number"
                    required
                    min={1}
                    max={100}
                    step={1}
                    value={columns}
                    onChange={(event) => setColumns(event.target.value)}
                  />
                </FormField>
              </div>
              <div>
                <span
                  id="scene-grid-color-label"
                  className="mb-2 block text-sm font-medium text-content-secondary"
                >
                  Grid color
                </span>
                <Input
                  id="scene-grid-color"
                  aria-labelledby="scene-grid-color-label"
                  type="color"
                  value={gridColor}
                  onChange={(event) => setGridColor(event.target.value)}
                  className="h-11 w-full cursor-pointer p-1"
                />
              </div>
              <SceneImageField
                label="Grid background image (optional)"
                tooltip="This image appears behind the grid tiles. It does not replace the scene background photo."
                file={background}
                existingUrl={scene?.grid?.backgroundImageUrl}
                disabled={saving || gridMode !== "internal"}
                allowRemove
                removeExisting={removeBackground}
                onRemoveExisting={setRemoveBackground}
                onChange={setBackground}
              />
              <SceneGridPreview
                rows={Number(rows)}
                columns={Number(columns)}
                gridColor={gridColor}
                background={background}
                backgroundImageUrl={
                  removeBackground ? undefined : scene?.grid?.backgroundImageUrl
                }
              />
            </fieldset>
          </div>
        </div>
        <div
          id="scene-external-grid"
          inert={gridMode !== "external"}
          aria-hidden={gridMode !== "external"}
          className={`grid transition-[grid-template-rows,opacity] duration-250 motion-reduce:transition-none ${gridMode === "external" ? "grid-rows-[1fr] opacity-100" : "grid-rows-[0fr] opacity-0"}`}
        >
          <div className="min-h-0 overflow-hidden">
            <fieldset
              disabled={saving || gridMode !== "external"}
              className="rounded-xl border border-border bg-surface/50 p-4"
            >
              <FormField htmlFor="scene-grid-url" label="External grid URL">
                <Input
                  id="scene-grid-url"
                  required
                  value={gridUrl}
                  onChange={(event) => setGridUrl(event.target.value)}
                  placeholder="https://…"
                />
              </FormField>
            </fieldset>
          </div>
        </div>
        {scene?.grid && gridMode !== "internal" && (
          <p className="text-sm text-amber-200">
            Saving will remove this scene’s Lunoria grid and its background
            image.
          </p>
        )}
        {error && (
          <p role="alert" className="text-sm text-danger">
            {error}
          </p>
        )}
        <Button
          type="submit"
          variant="primary"
          size="lg"
          className="w-full"
          disabled={saving}
          aria-busy={saving}
          leftIcon={
            saving ? (
              <FontAwesomeIcon icon={faSpinner} spin aria-hidden="true" />
            ) : undefined
          }
        >
          {scene ? "Save Changes" : "Create Scene"}
        </Button>
      </fieldset>
    </form>
  );
}
