import { CardGrid } from "@/components/ui/CardGrid";
import { SceneCard } from "@/features/scenes/components/SceneCard";
import type { Scene } from "@/features/scenes/types";

interface SceneGridProps {
  scenes: Scene[];
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
}

export function SceneGrid({
  scenes,
  onViewDialogs,
  onEdit,
}: SceneGridProps) {
  return (
    <CardGrid>
      {scenes.map((scene) => (
        <SceneCard
          key={scene.id}
          scene={scene}
          onViewDialogs={onViewDialogs}
          onEdit={onEdit}
        />
      ))}
    </CardGrid>
  );
}
