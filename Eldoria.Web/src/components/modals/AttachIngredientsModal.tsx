import { useState } from "react";
import Modal from "./Modal";
import ModalContent from "./ModalContent";
import ModalHeader from "./ModalHeader";
import EasyInput from "../EasyInput";
import EasyError from "../EasyError";
import EasyButton, { EasyButtonVariant } from "../EasyButton";
import ContentListItem from "./ContentListItem";

export interface CreateIngredientModel {
  content: string;
}

interface AttachIngredientsModalProps {
  visible: boolean;
  onClose: (ingredients: CreateIngredientModel[]) => void;
}

const AttachIngredientsModal = ({
  visible,
  onClose,
}: AttachIngredientsModalProps) => {
  const [ingredients, setIngredients] = useState<CreateIngredientModel[]>([]);
  const [currentIngredient, setCurrentIngredient] = useState("");
  const [ingredientError, setIngredientError] = useState("");

  const handleAttachIngredient = () => {
    if (!currentIngredient) {
      setIngredientError("There is no ingredient to be saved.");
      return;
    }

    setIngredientError("");
    setIngredients((prev) => [
      ...prev,
      {
        content: currentIngredient,
      },
    ]);
    setCurrentIngredient("");
  };

  const moveIngredientUp = (index: number) => {
    if (index === 0) return;
    swapArrayIngredients(index, index - 1);
  };

  const moveIngredientDown = (index: number) => {
    if (index === ingredients.length - 1) return;
    swapArrayIngredients(index, index + 1);
  };

  const deleteIngredient = (index: number) => {
    setIngredients(ingredients.filter((_, i) => i !== index));
  };

  const swapArrayIngredients = (index1: number, index2: number) => {
    const newIngredientArray = [...ingredients];

    const item1 = newIngredientArray[index1];
    const item2 = newIngredientArray[index2];

    newIngredientArray[index1] = item2;
    newIngredientArray[index2] = item1;

    setIngredients(newIngredientArray);
  };

  return (
    <Modal visible={visible} onBackgroundClose={() => onClose(ingredients)}>
      <ModalContent>
        <ModalHeader
          title="Ingredients"
          onCloseClicked={() => onClose(ingredients)}
        />
        <div className="pt-10 flex flex-col mb-4">
          <EasyInput
            title="Ingredient"
            value={currentIngredient}
            onChange={(e) => setCurrentIngredient(e.target.value)}
            valid={!ingredientError}
          />
          <EasyError>{ingredientError}</EasyError>
          <EasyButton
            title="Save"
            type="button"
            variant={EasyButtonVariant.Primary}
            className="self-end mt-1"
            onClick={handleAttachIngredient}
          />
        </div>
        <div className="flex-1 mt-2 bg-blue-100 dark:bg-gray-900 rounded-lg flex flex-col p-2">
          {ingredients.map((ingredient, index) => (
            <ContentListItem
              index={index}
              content={ingredient.content}
              onMoveUp={(index) => moveIngredientUp(index)}
              onMoveDown={(index) => moveIngredientDown(index)}
              onDelete={(index) => deleteIngredient(index)}
            />
          ))}
        </div>{" "}
      </ModalContent>
    </Modal>
  );
};

export default AttachIngredientsModal;
