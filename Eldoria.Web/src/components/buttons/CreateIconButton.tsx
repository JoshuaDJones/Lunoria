import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";

interface CreateIconButtonProps {
  onClick: () => void;
}

const CreateIconButton = ({ onClick }: CreateIconButtonProps) => {
  return (
    <button
      className="h-10 w-10 rounded-lg bg-green-900 hover:opacity-70"
      onClick={onClick}
    >
      <FontAwesomeIcon size="2x" icon={faPlus} className="text-green-100" />
    </button>
  );
};

export default CreateIconButton;
