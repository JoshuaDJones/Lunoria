import { faEdit } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";

interface EditIconButtonProps {
  onEditClick: () => void;
}

const EditIconButton = ({ onEditClick }: EditIconButtonProps) => {
  return (
    <button
      className="h-10 w-10 rounded-lg bg-blue-900 hover:opacity-70"
      onClick={(e) => {
        e.stopPropagation();
        onEditClick();
      }}
    >
      <FontAwesomeIcon size="xl" icon={faEdit} className="text-blue-100" />
    </button>
  );
};

export default EditIconButton;
