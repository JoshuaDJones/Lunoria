import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Scene } from "@/features/scenes/types";

interface SceneCardProps {
  scene: Scene;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
}

export function SceneCard({
  scene,
  onViewDialogs,
  onEdit,
}: SceneCardProps) {
  return (
    <MediaCard
      title={scene.name}
      description={scene.description}
      imageUrl={scene.photoUrl}
      onClick={() => onViewDialogs(scene)}
    >
      <StatGrid className="mt-4 px-4">
        <Stat label="Grid URL" value={scene.gridUrl || "None"} />
        <Stat
          label="Created"
          value={new Intl.DateTimeFormat().format(new Date(scene.createDate))}
        />
      </StatGrid>
      <div className="mt-4 flex gap-2 border-t border-border px-4 py-3">
        <button
          type="button"
          onClick={(event) => {
            event.stopPropagation();
            onViewDialogs(scene);
          }}
          className="flex-1 rounded-lg border border-brand-subtle/50 px-4 py-2 text-sm font-semibold text-brand-hover transition hover:bg-brand/10"
        >
          View dialogs
        </button>
        <button
          type="button"
          onClick={(event) => {
            event.stopPropagation();
            onEdit(scene);
          }}
          className="rounded-lg border border-border px-4 py-2 text-sm text-content-secondary transition hover:border-brand-hover hover:text-brand-hover"
        >
          Edit
        </button>
      </div>
    </MediaCard>
  );
}
