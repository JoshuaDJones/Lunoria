import Title, { TitleColor, TitleSize } from "../typography/Title";
import { SceneDialogDto } from "../../types/scene";
import CreateSceneDialog from "../CreateSceneDialog";
import SceneDialogCard from "../cards/SceneDialogCard";

interface DialogListProps {
  sceneId: number;
  sceneDialogs: SceneDialogDto[];
  selectedDialogId?: number;
  onSceneDialogSelect: (sceneDialogId: number) => void;
  onDialogCreated: () => void;
}

const DialogsList = ({
  sceneId,
  sceneDialogs,
  selectedDialogId,
  onSceneDialogSelect,
  onDialogCreated,
}: DialogListProps) => {
  console.log(sceneDialogs);

  return (
    <div className="flex flex-col flex-1">
      <Title
        className="self-center border-b-2 text-3xl"
        color={TitleColor.white}
        size={TitleSize.custom}
      >
        Dialogs
      </Title>
      <div className="flex-1 flex-col mt-5 p-5">
                <CreateSceneDialog
          onDialogCreated={onDialogCreated}
          sceneId={sceneId}
        />
        {sceneDialogs.map((s) => (
          <SceneDialogCard
            sceneDialog={s}
            isSelected={s.id === selectedDialogId}
            onSelect={(id) => onSceneDialogSelect(id)}
          />
        ))}
      </div>
    </div>
  );
};

export default DialogsList;
