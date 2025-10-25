import { faBinoculars, faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface ViewIconButtonProps {
  onViewClick: () => void;
}

const ViewIconButton = ({ onViewClick }: ViewIconButtonProps) => {
  return (
    <button
      className="h-10 w-10 rounded-lg bg-green-900 hover:opacity-70"
      onClick={(e) => {
        e.stopPropagation();
        onViewClick();
      }}
    >
      <FontAwesomeIcon size="xl" icon={faBinoculars} className="text-red-100" />
    </button>
  );
};

export default ViewIconButton;
