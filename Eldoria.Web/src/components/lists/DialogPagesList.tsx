import { SceneDialogDto } from "../../types/scene";
import CreateDialogPage from "../dialogs/CreateDialogPage";
import DialogPageCard from "../cards/DialogPageCard";
import ListContainer from "../dialogs/ListContainer";

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
  const dialogPages = selectedDialog?.dialogPages ?? [];

  return (
    <ListContainer title="Pages">
      {selectedDialog && (
        <CreateDialogPage
          sceneDialogId={selectedDialog.id}
          onRefreshRequest={onRefreshRequest}
        />
      )}
      {dialogPages.map((p) => (
        <DialogPageCard
          key={p.id}
          dialogPage={p}
          selectedDialogPageId={selectedDialogPageId}
          onSelect={(dialogPageId) => onDialogPageSelect(dialogPageId)}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </ListContainer>
  );
};

export default DialogPagesList;
