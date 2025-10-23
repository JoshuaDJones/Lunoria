import DeleteIconButton from "../buttons/DeleteIconButton";
import EditIconButton from "../buttons/EditIconButton";

interface DialogActionsProps {
  isOpen: boolean;
  onEditClick: () => void;
  onDeleteClick: () => void;
}

const DialogActions = ({
  isOpen,
  onEditClick,
  onDeleteClick,
}: DialogActionsProps) => {
  if (!isOpen) return null;

  return (
    <div className="border-t-2 border-stone-500 justify-end flex p-2 gap-2">
      <EditIconButton onEditClick={onEditClick} />
      <DeleteIconButton onDeleteClick={onDeleteClick} />
    </div>
  );
};

export default DialogActions;
