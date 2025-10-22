import { faCancel } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface CancelIconButtonProps {
  onCancelClick: () => void;
}

const DeleteIconButton = ({ onCancelClick }: CancelIconButtonProps) => {
  return (
    <button
      className="h-10 w-10 rounded-lg bg-red-900 hover:opacity-70"
      onClick={(e) => {
        e.stopPropagation();
        onCancelClick();
      }}
    >
      <FontAwesomeIcon size="xl" icon={faCancel} className="text-red-100" />
    </button>
  );
};

export default DeleteIconButton;
