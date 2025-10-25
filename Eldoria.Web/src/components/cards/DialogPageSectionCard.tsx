import Text, { TextColor, TextSize } from "../typography/Text";
import { DialogPageSectionDto } from "../../types/scene";
import DialogCardContainer from "../dialogs/DialogCardContainer";
import DialogItemButton from "../dialogs/DialogItemButton";
import DialogActions from "../dialogs/DialogActions";
import EditPageSection from "../dialogs/EditPageSection";
import { useEffect, useState } from "react";
import { useToast } from "../../providers/ToastProvider";
import { BASE_URL, useApi } from "../../hooks/useApi";

interface DialogPageSectionCardProps {
  dialogPageSection: DialogPageSectionDto;
  selectedPageSectionId?: number;
  onSelect: (pageSectionId: number) => void;
  onRefreshRequest: () => void;
}

const DialogPageSectionCard = ({
  dialogPageSection,
  selectedPageSectionId,
  onSelect,
  onRefreshRequest
}: DialogPageSectionCardProps) => {
  const {del} = useApi();
  const {showError, showSuccess} = useToast();
  const isSelected = dialogPageSection.id === selectedPageSectionId;
  const [editOpen, setEditOpen] = useState(false);

  useEffect(() => {
    if(!isSelected)
      setEditOpen(false);
  }, [isSelected])

  const handleDelete = async () => {
    try {
      await del(`${BASE_URL}/DialogPageSection/${dialogPageSection.id}`);
      showSuccess('Section deleted.');
      onRefreshRequest();
    } catch(err) {
      console.error(err);
      showError('Section not deleted.')
    }
  }

  return (
    <DialogCardContainer isActive={isSelected}>
      <DialogItemButton
        onClick={() => onSelect(dialogPageSection.id)}
        className="flex-col"
      >
        <Text
          className="self-start"
          textColor={TextColor.white}
          size={TextSize.xl}
        >
          {dialogPageSection.character?.name ?? "Narrator"}
        </Text>

        <div className="flex items-center justify gap-2 mt-2">
          <Text textColor={TextColor.white} size={TextSize.lg}>
            Order:{" "}
          </Text>
          <Text textColor={TextColor.white}>{dialogPageSection.orderNum}</Text>
        </div>

        <Text
          className="self-start mt-2"
          textColor={TextColor.white}
          size={TextSize.lg}
        >
          Reading Prompt
        </Text>
        <div className="bg-stone-600 p-2 rounded-lg flex">
          <Text className="text-start" textColor={TextColor.white}>
            {dialogPageSection.readingText}
          </Text>
        </div>
      </DialogItemButton>
      <DialogActions
        isOpen={isSelected}
        onEditClick={() => setEditOpen(true)}
        onDeleteClick={async () => await handleDelete()}
      />
      <EditPageSection isOpen={editOpen} dialogPageSection={dialogPageSection} onRefreshRequest={onRefreshRequest} onCloseClick={() => setEditOpen(false)} />
    </DialogCardContainer>
  );
};

export default DialogPageSectionCard;
