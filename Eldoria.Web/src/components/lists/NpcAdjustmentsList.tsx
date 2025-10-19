import React from "react";
import { SceneCharacterDto } from "../../types/scene";
import SceneCharacterAdjustCard from "../cards/SceneCharacterAdjustCard";

interface NpcAdjustmentsListProps {
  npcs?: SceneCharacterDto[];
  onRefreshRequest: () => void;
}

const NpcAdjustmentsList = ({
  npcs,
  onRefreshRequest,
}: NpcAdjustmentsListProps) => {
  return (
    <div className="flex-1 flex flex-col px-5 gap-2">
      {npcs?.map((e) => (
        <SceneCharacterAdjustCard
          sceneCharacter={e}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

export default NpcAdjustmentsList;
