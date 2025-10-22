import Title, { TitleColor, TitleSize } from "../typography/Title";
import { SceneDialogDto } from "../../types/scene";
import CreateDialogPage from "../CreateDialogPage";
import DialogPageCard from "../cards/DialogPageCard";

interface DialogPagesListProps {
  selectedDialogPageId?: number;
  selectedDialog?: SceneDialogDto;
  onDialogPageSelect: (dialogPageId: number) => void;
  onRefreshRequest: () => void;
}

const DialogPagesList = ({ selectedDialogPageId, selectedDialog, onDialogPageSelect, onRefreshRequest }: DialogPagesListProps) => {
  const dialogPages = selectedDialog?.dialogPages;

  return (
    <div className="flex flex-col flex-1">
      <Title
        className="self-center border-b-2 text-3xl"
        color={TitleColor.white}
        size={TitleSize.custom}
      >
        Pages
      </Title>
      <div className="flex-1 flex-col mt-5 p-5">
                {selectedDialog && (
          <CreateDialogPage sceneDialogId={selectedDialog.id} onRefreshRequest={onRefreshRequest}/>
        )}
        {dialogPages &&
          dialogPages.map((p) => (
            <DialogPageCard 
            dialogPage={p} 
            selectedDialogPageId={selectedDialogPageId} 
            onSelect={(dialogPageId) => onDialogPageSelect(dialogPageId)} />
          ))}
      </div>
    </div>
  );
};

export default DialogPagesList;
