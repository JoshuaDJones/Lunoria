import { CardGrid } from "@/components/ui/CardGrid";
import { SceneCard } from "@/features/scenes/components/SceneCard";
import type { Scene } from "@/features/scenes/types";
import type { JourneyCharacterSyncSummary } from "@/features/characterSync/summaryTypes";

interface SceneGridProps {
  characterSync?: {
    summary?: JourneyCharacterSyncSummary;
    loading: boolean;
    error: string;
  };
  scenes: Scene[];
  className?: string;
  onViewEvents: (scene: Scene) => void;
  onViewObjectives: (scene: Scene) => void;
  onViewChests: (scene: Scene) => void;
  onViewCharacters: (scene: Scene) => void;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
  onDelete: (scene: Scene) => void;
}

export function SceneGrid({
  characterSync,
  scenes,
  className,
  onViewEvents,
  onViewObjectives,
  onViewChests,
  onViewCharacters,
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
          characterSync={
            characterSync
              ? {
                  entries:
                    characterSync.summary?.scenes.find(
                      (item) => item.sceneId === scene.id,
                    )?.characters ?? [],
                  checking: characterSync.loading,
                  checkError: characterSync.error,
                }
              : undefined
          }
          onViewEvents={onViewEvents}
          onViewObjectives={onViewObjectives}
          onViewChests={onViewChests}
          onViewCharacters={onViewCharacters}
          onViewDialogs={onViewDialogs}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </CardGrid>
  );
}
