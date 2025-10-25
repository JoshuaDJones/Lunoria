import { SceneDialogDto } from "../../types/scene";
import CreateSceneDialog from "../dialogs/CreateSceneDialog";
import SceneDialogCard from "../cards/SceneDialogCard";
import ListContainer from "../dialogs/ListContainer";

interface DialogListProps {
  sceneId: number;
  sceneDialogs: SceneDialogDto[];
  selectedDialogId?: number;
  onSceneDialogSelect: (sceneDialogId: number) => void;
  onRefreshRequest: () => void;
}

const DialogsList = ({
  sceneId,
  sceneDialogs,
  selectedDialogId,
  onSceneDialogSelect,
  onRefreshRequest,
}: DialogListProps) => {
  return (
    <ListContainer title="Dialogs">
      <CreateSceneDialog onDialogCreated={onRefreshRequest} sceneId={sceneId} />
      {sceneDialogs.map((s) => (
        <SceneDialogCard
          sceneDialog={s}
          isSelected={s.id === selectedDialogId}
          onSelect={(id) => onSceneDialogSelect(id)}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </ListContainer>
  );
};

export default DialogsList;
