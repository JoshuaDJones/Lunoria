import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import { Button } from "@/components/ui";
import type { Scene } from "@/features/scenes/types";

interface SceneCardProps {
  scene: Scene;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
}

export function SceneCard({ scene, onViewDialogs, onEdit }: SceneCardProps) {
  return (
    <MediaCard
      title={scene.name}
      description={scene.description}
      imageUrl={scene.photoUrl}
    >
      <StatGrid className="mt-4 px-4">
        <Stat label="Grid URL" value={scene.gridUrl || "None"} />
        <Stat
          label="Created"
          value={new Intl.DateTimeFormat().format(new Date(scene.createDate))}
        />
      </StatGrid>
      <div className="mt-4 flex gap-2 border-t border-border px-4 py-3 justify-end">
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onViewDialogs(scene);
          }}
          variant="accent"
        >
          View dialogs
        </Button>
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onEdit(scene);
          }}
        >
          Edit
        </Button>
      </div>
    </MediaCard>
  );
}
