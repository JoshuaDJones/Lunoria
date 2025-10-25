import { useState } from "react";
import { DialogPageSectionDto } from "../../types/scene";
import AppInput from "../inputs/AppInput";
import EditDialogItemContainer from "./EditDialogItemContainer";
import DialogCharacterSelect, { DialogCharacter } from "./DialogCharacterSelect";
import DialogInputError from "../inputs/DialogInputError";
import AppTextArea from "../inputs/AppTextArea";
import CheckInput from "../inputs/CheckInput";
import { canBeNumber } from "../../utils/numberUtils";
import { useToast } from "../../providers/ToastProvider";
import { BASE_URL, useApi } from "../../hooks/useApi";

interface InputValues {
    dialogCharacter?: DialogCharacter;
    orderNum?: string;
    readingText?: string;
    isNarrator: boolean;
}

interface InputErrors {
    characterId?: string;
    orderNum?: string;
    readingText?: string;
}

interface EditPageSectionProps {
    isOpen: boolean;
    dialogPageSection: DialogPageSectionDto;
    onRefreshRequest: () => void;
    onCloseClick: () => void;
}

const EditPageSection = ({
    isOpen,
    dialogPageSection,
    onRefreshRequest,
    onCloseClick
}: EditPageSectionProps) => {
    const {showError, showSuccess} = useToast();
    const {patch} = useApi();
    
    const [inputValues, setInputValues] = useState<InputValues>({
        dialogCharacter: {
            characterId: dialogPageSection.character?.id,
            characterPhoto: dialogPageSection.character?.photoUrl
        },
        orderNum: dialogPageSection.orderNum.toString(),
        readingText: dialogPageSection.readingText,
        isNarrator: dialogPageSection.isNarrator
    });

    const [inputErrors, setInputErrors] = useState<InputErrors>();

    const save = async () => {
        const isValid = validate();

        if(!isValid) 
            return;

        try {
            await patch(`${BASE_URL}/DialogPageSection/${dialogPageSection.id}`, {
                CharacterId: inputValues.dialogCharacter?.characterId ?? null,
                OrderNum: inputValues.orderNum,
                ReadingText: inputValues.readingText,
                IsNarrator: inputValues.isNarrator
            });

            onCloseClick();
            showSuccess("Page section saved.");
            onRefreshRequest();
        } catch (err) {
            showError("Page section not saved.");
            console.error(err);
        }        
    }

const validate = (): boolean => {
  const newInputErrors: InputErrors = {};

  if (
    (!inputValues.dialogCharacter?.characterId && !inputValues.isNarrator) ||
    (inputValues.dialogCharacter?.characterId && inputValues.isNarrator)
  ) {
    newInputErrors.characterId = "Please select a character OR mark section as narrator.";
  }

  const canParse = canBeNumber(inputValues.orderNum ?? "");

  if (!canParse || !inputValues.orderNum) {
    newInputErrors.orderNum = "Order num must be a valid number.";
  }

  if (!inputValues.readingText) {
    newInputErrors.readingText = "Reading prompt must have a value.";
  }

  setInputErrors(newInputErrors);

  const hasErrors = Object.values(newInputErrors).some((msg) => !!msg);
  return !hasErrors;
};

    const reset = () => {
        setInputValues({
        dialogCharacter: {
            characterId: dialogPageSection.character?.id,
            characterPhoto: dialogPageSection.character?.photoUrl
        },
        orderNum: dialogPageSection.orderNum.toString(),
        readingText: dialogPageSection.readingText,
        isNarrator: dialogPageSection.isNarrator
        });

        setInputErrors({});
    }

    if(!isOpen) 
        return null;

  return (
    <EditDialogItemContainer title={"Edit Page Section"} onCloseClick={() => {
        reset();
        onCloseClick();
    }} onSaveClick={async () => save()} className="gap-3">
        <DialogCharacterSelect selectedCharacter={inputValues.dialogCharacter} onCharacterSelect={(character) => setInputValues((prev) => ({
            ...prev,
            dialogCharacter: character
        }))} 
        className="mt-6"/>
        <DialogInputError message={inputErrors?.characterId}/>

            <AppInput
              theme="dark"
              title="Order No."
              value={inputValues.orderNum?.toString()}
              onChange={(e) => {
                setInputValues((prev) => ({
                  ...prev,
                  orderNum: e.target.value,
                }));
              }}
            />
            <DialogInputError message={inputErrors?.orderNum} />

            <AppTextArea
              theme="dark"
              title="Reading Prompt"
              value={inputValues.readingText}
              onChange={(e) => {
                setInputValues((prev) => ({
                  ...prev,
                  readingText: e.target.value,
                }));
              }}
            />
            <DialogInputError message={inputErrors?.readingText} />


            <CheckInput
              title={"Is Narrator"}
              isSelected={inputValues.isNarrator ?? false}
              onChange={(isSelected: boolean) =>
                setInputValues((prev) => ({
                  ...prev,
                  isNarrator: isSelected,
                }))
              }
            />
    </EditDialogItemContainer>
  )
}

export default EditPageSection