import { faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface DeleteIconButtonProps {
  onDeleteClick: () => void;
}

const DeleteIconButton = ({ onDeleteClick }: DeleteIconButtonProps) => {
  return (
    <button
      className="h-10 w-10 rounded-lg bg-red-900 hover:opacity-70"
      onClick={(e) => {
        e.stopPropagation();
        onDeleteClick();
      }}
    >
      <FontAwesomeIcon size="xl" icon={faTrash} className="text-red-100" />
    </button>
  );
};

export default DeleteIconButton;
