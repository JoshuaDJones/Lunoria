import { SceneCharacterDto } from "../../types/scene";
import SceneCharacterAdjustCard from "../cards/SceneCharacterAdjustCard";

interface EnemiesAdjustmentsListProps {
  enemies?: SceneCharacterDto[];
  onRefreshRequest: () => void;
}

const EnemiesAdjustmentsList = ({
  enemies,
  onRefreshRequest,
}: EnemiesAdjustmentsListProps) => {
  return (
    <div className="flex-1 flex flex-col px-5 gap-2">
      {enemies?.map((e) => (
        <SceneCharacterAdjustCard
          sceneCharacter={e}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

export default EnemiesAdjustmentsList;
