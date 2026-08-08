import { CardGrid } from "@/components/ui/CardGrid";
import { SceneCard } from "@/features/scenes/components/SceneCard";
import type { Scene } from "@/features/scenes/types";

interface SceneGridProps {
  scenes: Scene[];
  className?: string;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
  onDelete: (scene: Scene) => void;
}

export function SceneGrid({
  scenes,
  className,
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
          onViewDialogs={onViewDialogs}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </CardGrid>
  );
}
