import Title, { TitleColor, TitleSize } from "../typography/Title";
import { SceneDialogDto } from "../../types/scene";
import CreateDialogPage from "../dialogs/CreateDialogPage";
import DialogPageCard from "../cards/DialogPageCard";
import DialogListTitle from "../dialogs/DialogListTitle";

interface DialogPagesListProps {
  selectedDialogPageId?: number;
  selectedDialog?: SceneDialogDto;
  onDialogPageSelect: (dialogPageId: number) => void;
  onRefreshRequest: () => void;
}

const DialogPagesList = ({
  selectedDialogPageId,
  selectedDialog,
  onDialogPageSelect,
  onRefreshRequest,
}: DialogPagesListProps) => {
  const dialogPages = selectedDialog?.dialogPages;

  return (
    <div className="flex flex-col flex-1">
      <DialogListTitle title="Pages" />
      <div className="flex-1 flex-col mt-5 p-5">
        {selectedDialog && (
          <CreateDialogPage
            sceneDialogId={selectedDialog.id}
            onRefreshRequest={onRefreshRequest}
          />
        )}
        {dialogPages &&
          dialogPages.map((p) => (
            <DialogPageCard
              dialogPage={p}
              selectedDialogPageId={selectedDialogPageId}
              onSelect={(dialogPageId) => onDialogPageSelect(dialogPageId)}
            />
          ))}
      </div>
    </div>
  );
};

export default DialogPagesList;
