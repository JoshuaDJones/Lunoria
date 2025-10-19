import { SceneCharacterDto } from "../../types/scene";
import DashboardSceneCharacterCard from "../cards/DashboardSceneCharacterCard";

interface DashboardNpcListProps {
  characters?: SceneCharacterDto[];
  onRefreshRequest: () => void;
}

const DashboardNpcList = ({
  characters,
  onRefreshRequest,
}: DashboardNpcListProps) => {
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

export default DashboardNpcList;
