import Text, { TextColor, TextSize } from "../typography/Text";
import { SceneDialogDto } from "../../types/scene";
import DialogActions from "../dialogs/DialogActions";
import DialogCardContainer from "../dialogs/DialogCardContainer";
import DialogItemButton from "../dialogs/DialogItemButton";

interface SceneDialogCardProps {
  sceneDialog: SceneDialogDto;
  isSelected: boolean;
  onSelect: (id: number) => void;
}

const SceneDialogCard = ({
  sceneDialog,
  isSelected,
  onSelect,
}: SceneDialogCardProps) => {
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
        isOpen={isSelected}
        onEditClick={() => {}}
        onDeleteClick={() => {}}
      />
    </DialogCardContainer>
  );
};

export default SceneDialogCard;
