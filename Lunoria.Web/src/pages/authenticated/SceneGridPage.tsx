import { useEffect, useState, type CSSProperties } from "react";
import { Navigate, useParams } from "react-router-dom";
import { ApiLoadError } from "@/components/ui";
import { getSceneGrid, type SceneGridConfiguration } from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";

export function SceneGridPage() {
  const { sceneId: sceneIdParam } = useParams<{ sceneId: string }>();
  const sceneId = Number(sceneIdParam);
  const [grid, setGrid] = useState<SceneGridConfiguration>();
  const [error, setError] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    if (!Number.isInteger(sceneId) || sceneId <= 0) return;
    let current = true;

    void getSceneGrid(sceneId)
      .then((loadedGrid) => {
        if (current) {
          setGrid(loadedGrid);
          setError("");
        }
      })
      .catch((requestError: unknown) => {
        if (current) setError(getApiError(requestError).message);
      });

    return () => {
      current = false;
    };
  }, [sceneId, reloadKey]);

  if (!Number.isInteger(sceneId) || sceneId <= 0) {
    return <Navigate to="/home" replace />;
  }

  if (error) {
    return (
      <main className="stone-image flex min-h-screen items-center justify-center p-6">
        <div className="w-full max-w-lg rounded-2xl bg-surface/95 p-6">
          <ApiLoadError
            error={error}
            onRetry={() => setReloadKey((key) => key + 1)}
          />
        </div>
      </main>
    );
  }

  if (!grid) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-canvas text-content-muted">
        Loading grid…
      </main>
    );
  }

  const gridStyle: CSSProperties = {
    backgroundImage: `linear-gradient(to right, ${grid.gridColor} 1px, transparent 1px), linear-gradient(to bottom, ${grid.gridColor} 1px, transparent 1px)`,
    backgroundSize: `${100 / grid.columns}% ${100 / grid.rows}%`,
  };

  return (
    <main className="relative h-screen w-screen overflow-hidden bg-black">
      {grid.backgroundImageUrl && (
        <img
          src={grid.backgroundImageUrl}
          alt=""
          className="pointer-events-none absolute inset-0 size-full object-cover"
          draggable={false}
        />
      )}
      <div className="pointer-events-none absolute inset-0" style={gridStyle} />
    </main>
  );
}
