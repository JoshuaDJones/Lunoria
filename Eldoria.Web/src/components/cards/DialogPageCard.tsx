import { useEffect, useState } from "react";
import { DialogPageDto } from "../../types/scene";
import DialogActions from "../dialogs/DialogActions";
import DialogCardContainer from "../dialogs/DialogCardContainer";
import DialogItemButton from "../dialogs/DialogItemButton";
import Text, { TextColor, TextSize } from "../typography/Text";
import EditDialogPage from "../dialogs/EditDialogPage";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import ViewIconButton from "../buttons/ViewIconButton";

interface DialogPageCardProps {
  dialogPage: DialogPageDto;
  selectedDialogPageId?: number;
  onSelect: (dialogPageId: number) => void;
  onRefreshRequest: () => void;
}

const DialogPageCard = ({
  dialogPage,
  selectedDialogPageId,
  onSelect,
  onRefreshRequest,
}: DialogPageCardProps) => {
  const { del } = useApi();
  const { showSuccess, showError } = useToast();

  const isSelected = dialogPage.id === selectedDialogPageId;
  const [showEdit, setShowEdit] = useState(false);

  useEffect(() => {
    if (!isSelected) setShowEdit(false);
  }, [isSelected]);

  const deleteDialogPage = async () => {
    try {
      await del(`${BASE_URL}/DialogPage/${dialogPage.id}`);

      showSuccess("Dialog page deleted.");
      setShowEdit(false);
      onRefreshRequest();
    } catch (err) {
      console.error(err);
      showError("Dialog page not deleted.");
    }
  };

  return (
    <DialogCardContainer isActive={isSelected}>
      <DialogItemButton onClick={() => onSelect(dialogPage.id)}>
        <img src={dialogPage.photoUrl} className="h-[100px] rounded-xl" />
        <div className="justify-center items-end flex flex-col flex-1">
          <Text className="" size={TextSize.lg} textColor={TextColor.white}>
            Order: {dialogPage.orderNum}
          </Text>
          <Text className="" size={TextSize.lg} textColor={TextColor.white}>
            Sections: {dialogPage.dialogPageSections.length}
          </Text>
        </div>
      </DialogItemButton>
      <DialogActions
        isOpen={isSelected}
        onEditClick={() => setShowEdit(true)}
        onDeleteClick={async () => deleteDialogPage()}
      />
      <EditDialogPage
        isOpen={showEdit}
        dialogPage={dialogPage}
        onRefreshRequest={onRefreshRequest}
        onCloseClick={() => setShowEdit(false)}
      />
    </DialogCardContainer>
  );
};

export default DialogPageCard;
