import { ReactElement } from "react";
import DeleteIconButton from "../buttons/DeleteIconButton";
import EditIconButton from "../buttons/EditIconButton";

interface DialogActionsProps {
  isOpen: boolean;
  onEditClick: () => void;
  onDeleteClick: () => void;
  additionalActions?: ReactElement;
}

const DialogActions = ({
  isOpen,
  onEditClick,
  onDeleteClick,
  additionalActions,
}: DialogActionsProps) => {
  if (!isOpen) return null;

  return (
    <div className="justify-end flex p-2 gap-2">
      <EditIconButton onEditClick={onEditClick} />
      <DeleteIconButton onDeleteClick={onDeleteClick} />
      {additionalActions}
    </div>
  );
};

export default DialogActions;
