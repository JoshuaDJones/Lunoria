import {
  faCancel,
  faCircle,
  faCircleXmark,
  faClose,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface CloseIconButtonProps {
  onCloseClick: () => void;
}

const CloseIconButton = ({ onCloseClick }: CloseIconButtonProps) => {
  return (
    <button
      className="h-10 w-10 rounded-lg bg-red-900 hover:opacity-70"
      onClick={(e) => {
        e.stopPropagation();
        onCloseClick();
      }}
    >
      <FontAwesomeIcon size="xl" icon={faClose} className="text-red-100" />
    </button>
  );
};

export default CloseIconButton;
