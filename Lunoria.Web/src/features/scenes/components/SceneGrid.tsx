import { CardGrid } from "@/components/ui/CardGrid";
import { SceneCard } from "@/features/scenes/components/SceneCard";
import type { Scene } from "@/features/scenes/types";

interface SceneGridProps {
  scenes: Scene[];
  className?: string;
  onViewEvents: (scene: Scene) => void;
  onViewChests: (scene: Scene) => void;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
  onDelete: (scene: Scene) => void;
}

export function SceneGrid({
  scenes,
  className,
  onViewEvents,
  onViewChests,
  onViewDialogs,
  onEdit,
  onDelete,
}: SceneGridProps) {
  return (
    <CardGrid className={className}>
      {scenes.map((scene) => (
        <SceneCard
          key={scene.id}
          scene={scene}
          onViewEvents={onViewEvents}
          onViewChests={onViewChests}
          onViewDialogs={onViewDialogs}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </CardGrid>
  );
}
