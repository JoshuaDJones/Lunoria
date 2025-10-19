import { faCaretUp } from "@fortawesome/free-solid-svg-icons";
import { faCaretDown } from "@fortawesome/free-solid-svg-icons";
import { faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Modal from "./Modal";
import EasyText from "../EasyText";
import EasyButton, { EasyButtonVariant } from "../EasyButton";
import { useState } from "react";
import InputArea from "../InputArea";
import EasyError from "../EasyError";
import ModalContent from "./ModalContent";
import ModalHeader from "./ModalHeader";
import ContentListItem from "./ContentListItem";

export interface CreateDirectionModel {
  content: string;
}

interface AttachDirectionsModalProps {
  visible: boolean;
  onClose: (directions: CreateDirectionModel[]) => void;
}

const AttachDirectionsModal = ({
  visible,
  onClose,
}: AttachDirectionsModalProps) => {
  const [directions, setDirections] = useState<CreateDirectionModel[]>([]);
  const [currentDirection, setCurrentDirection] = useState("");
  const [directionError, setDirectionError] = useState("");

  const handleAttachDirection = () => {
    if (!currentDirection) {
      setDirectionError("There is no direction to be saved.");
      return;
    }

    setDirectionError("");
    setDirections((prev) => [
      ...prev,
      {
        content: currentDirection,
      },
    ]);
    setCurrentDirection("");
  };

  const moveDirectionUp = (index: number) => {
    if (index === 0) return;
    swapArrayDirections(index, index - 1);
  };

  const moveDirectionDown = (index: number) => {
    if (index === directions.length - 1) return;
    swapArrayDirections(index, index + 1);
  };

  const swapArrayDirections = (index1: number, index2: number) => {
    const newDirectionsArray = [...directions];

    const item1 = newDirectionsArray[index1];
    const item2 = newDirectionsArray[index2];

    newDirectionsArray[index1] = item2;
    newDirectionsArray[index2] = item1;

    setDirections(newDirectionsArray);
  };

  const deleteDirection = (index: number) => {
    setDirections(directions.filter((_, i) => i !== index));
  };

  return (
    <Modal visible={visible} onBackgroundClose={() => onClose(directions)}>
      <ModalContent>
        <ModalHeader
          title={"Directions"}
          onCloseClicked={() => onClose(directions)}
        />
        <div className="pt-10 flex flex-col mb-4">
          <InputArea
            title="Direction"
            value={currentDirection}
            onChange={(e) => setCurrentDirection(e.target.value)}
            valid={!directionError}
          />
          <EasyError>{directionError}</EasyError>
          <EasyButton
            title="Save"
            type="button"
            variant={EasyButtonVariant.Primary}
            className="self-end"
            onClick={handleAttachDirection}
          />
        </div>

        <div className="flex-1 mt-2 bg-blue-100 dark:bg-gray-900 rounded-lg flex flex-col p-2">
          {directions.map((direction, index) => (
            <ContentListItem
              index={index}
              content={direction.content}
              onMoveUp={(index) => moveDirectionUp(index)}
              onMoveDown={(index) => moveDirectionDown(index)}
              onDelete={(index) => deleteDirection(index)}
            />
          ))}
        </div>
      </ModalContent>
    </Modal>
  );
};

export default AttachDirectionsModal;
