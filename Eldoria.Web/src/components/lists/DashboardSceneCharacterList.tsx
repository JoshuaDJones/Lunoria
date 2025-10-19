import { SceneCharacterDto } from "../../types/scene";
import DashboardSceneCharacterCard from "../cards/DashboardSceneCharacterCard";

interface DashboardSceneCharacterListProps {
  characters?: SceneCharacterDto[];
  onRefreshRequest: () => void;
}

const DashboardSceneCharacterList = ({
  characters,
  onRefreshRequest,
}: DashboardSceneCharacterListProps) => {
  return (
    <div className="flex flex-wrap gap-4 items-start mt-3">
      {characters?.map((c) => (
        <DashboardSceneCharacterCard
          sceneCharacter={c}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

export default DashboardSceneCharacterList;
