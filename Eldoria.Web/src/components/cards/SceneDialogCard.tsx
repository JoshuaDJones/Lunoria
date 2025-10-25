import Text, { TextColor, TextSize } from "../typography/Text";
import { SceneDialogDto } from "../../types/scene";
import DialogActions from "../dialogs/DialogActions";
import DialogCardContainer from "../dialogs/DialogCardContainer";
import DialogItemButton from "../dialogs/DialogItemButton";
import { useEffect, useState } from "react";
import EditSceneDialog from "../dialogs/EditSceneDialog";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import ViewIconButton from "../buttons/ViewIconButton";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import ViewSceneDialogModal from "../modals/ViewSceneDialogModal";

interface SceneDialogCardProps {
  sceneDialog: SceneDialogDto;
  isSelected: boolean;
  onSelect: (id: number) => void;
  onRefreshRequest: () => void;
}

const SceneDialogCard = ({
  sceneDialog,
  isSelected,
  onSelect,
  onRefreshRequest,
}: SceneDialogCardProps) => {
  const { del } = useApi();
  const { showToast } = useToast();
  const modalRouter = useModalRouter(); 

  const [isEditOpen, setIsEditOpen] = useState(false);

  useEffect(() => {
    if (!isSelected && isEditOpen) setIsEditOpen(false);
  }, [isSelected]);

  const handleDelete = async () => {
    try {
      await del(`${BASE_URL}/SceneDialog/${sceneDialog.id}`);
      showToast("Success", "Scene dialog deleted.", ToastType.success, 3000);
      onRefreshRequest();
      setIsEditOpen(false);
    } catch (err) {
      showToast("Error", "Scene dialog not deleted.", ToastType.error, 3000);
      console.error(err);
    }
  };

  return (
    <DialogCardContainer isActive={isSelected}>
      <DialogItemButton onClick={() => onSelect(sceneDialog.id)}>
        <Text
          size={TextSize.lg}
          textColor={TextColor.white}
          className="flex-1 flex"
        >
          {sceneDialog.title}
        </Text>
        <Text size={TextSize.lg} textColor={TextColor.white}>
          Pages: {sceneDialog.dialogPages.length}
        </Text>
      </DialogItemButton>
      <DialogActions
        isOpen={isSelected && !isEditOpen}
        onEditClick={() => setIsEditOpen(true)}
        onDeleteClick={async () => await handleDelete()}
        additionalActions={<ViewIconButton onViewClick={() => modalRouter.push(<ViewSceneDialogModal sceneDialog={sceneDialog} />)} />}
      />
      <EditSceneDialog
        isOpen={isEditOpen && isSelected}
        sceneDialog={sceneDialog}
        onRefreshRequest={onRefreshRequest}
        onCloseClick={() => setIsEditOpen(false)}
      />
    </DialogCardContainer>
  );
};

export default SceneDialogCard;
