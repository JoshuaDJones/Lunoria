import { faSave, faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface SaveIconButtonProps {
  onSaveClick: () => void;
}

const SaveIconButton = ({ onSaveClick }: SaveIconButtonProps) => {
  return (
    <button
      className="h-10 w-10 rounded-lg bg-blue-900 hover:opacity-70"
      onClick={(e) => {
        e.stopPropagation();
        onSaveClick();
      }}
    >
      <FontAwesomeIcon size="xl" icon={faSave} className="text-blue-100" />
    </button>
  );
};

export default SaveIconButton;
