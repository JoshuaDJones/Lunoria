import { useState } from "react";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { faCircleXmark, faPlus } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Text, { TextColor, TextSize } from "../typography/Text";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import PageSectionCharacterModal from "../modals/PageSectionCharacterModal";
import AppInput from "../inputs/AppInput";
import AppTextArea from "../inputs/AppTextArea";
import CheckInput from "../inputs/CheckInput";
import CancelIconButton from "../buttons/CancelIconButton";
import SaveIconButton from "../buttons/SaveIconButton";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { canBeNumber } from "../../utils/numberUtils";
import DialogInputError from "../inputs/DialogInputError";
import { useLoading } from "../../providers/LoadingProvider";

interface InputValues {
  characterId?: number;
  orderNum?: string;
  readingText?: string;
  isNarrator?: boolean;
}

interface InputErrors {
  characterId?: string;
  orderNum?: string;
  readingText?: string;
  isNarrator?: string;
}

interface CreatePageSectionProps {
  pageDialogId: number;
  onRefreshRequest: () => void;
}

const CreatePageSection = ({
  pageDialogId,
  onRefreshRequest,
}: CreatePageSectionProps) => {
  const modalRouter = useModalRouter();
  const { post } = useApi();
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();

  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [selectedCharacterPhoto, setSelectedCharacterPhoto] = useState("");

  const [inputValues, setInputValue] = useState<InputValues>({
    characterId: undefined,
    orderNum: undefined,
    readingText: undefined,
    isNarrator: undefined,
  });

  const [inputErrors, setInputErrors] = useState<InputErrors>({
    characterId: undefined,
    orderNum: undefined,
    readingText: undefined,
    isNarrator: undefined,
  });

  const save = async () => {
    const isValid = validate();
    if (!isValid) return;

    try {
      showLoading();

      await post(`${BASE_URL}/DialogPageSection/${pageDialogId}`, {
        CharacterId: inputValues.characterId,
        OrderNum: inputValues.orderNum,
        ReadingText: inputValues.readingText,
        IsNarrator: inputValues.isNarrator,
      });

      showToast("Success", "Page section created.", ToastType.success, 3000);

      reset();
      onRefreshRequest();
    } catch (err) {
      showToast(
        "Error",
        "Unable to create page section.",
        ToastType.error,
        3000,
      );

      console.error(err);
    } finally {
      closeLoading();
    }
  };

  const validate = (): boolean => {
    if (!inputValues.characterId && !inputValues.isNarrator) {
      setInputErrors((prev) => ({
        ...prev,
        characterId: "Please select a character or mark section as narrator.",
      }));

      return false;
    }

    if (inputValues.characterId && inputValues.isNarrator) {
      setInputErrors((prev) => ({
        ...prev,
        characterId:
          "Section can only belong to the narrator or a character not both.",
      }));

      return false;
    }

    const canParse = canBeNumber(inputValues.orderNum ?? "");

    if (!canParse || !inputValues.orderNum) {
      setInputErrors((prev) => ({
        ...prev,
        orderNum: "Order num must be a valid number.",
      }));

      return false;
    }

    if (!inputValues.readingText) {
      setInputErrors((prev) => ({
        ...prev,
        readingText: "Reading prompt must have a value.",
      }));

      return false;
    }

    setInputErrors({});
    return true;
  };

  const reset = () => {
    setIsCreateOpen(false);
    setSelectedCharacterPhoto("");
    setInputValue({});
    setInputErrors({});
  };

  return (
    <div className="flex mt-3">
      {!isCreateOpen && (
        <AppButton
          className="border-2 hover:opacity-50"
          title="Add"
          variant={AppButtonVariant.ghost}
          size={AppButtonSize.sm}
          rightIcon={
            <FontAwesomeIcon icon={faPlus} className="text-white ml-1" />
          }
          onClick={() => setIsCreateOpen(true)}
        />
      )}
      {isCreateOpen && (
        <div className="flex-1 flex flex-col border-2 rounded-lg p-5 bg-stone-400/75">
          <div className="flex flex-col gap-2 items-start">
            <div className="flex items-center gap-4">
              <div>
                <Text size={TextSize.xl} textColor={TextColor.white}>
                  Character
                </Text>
                <AppButton
                  className=""
                  title={
                    !inputValues.characterId
                      ? "Add Character"
                      : "Change Character"
                  }
                  variant={AppButtonVariant.primary}
                  size={AppButtonSize.sm}
                  onClick={() =>
                    modalRouter.push(
                      <PageSectionCharacterModal
                        onSelect={(characterId, photoUrl) => {
                          setSelectedCharacterPhoto(photoUrl);
                          setInputValue((prev) => ({
                            ...prev,
                            characterId: characterId,
                          }));
                        }}
                      />,
                    )
                  }
                />
              </div>
              {selectedCharacterPhoto && (
                <div className="relative">
                  <img
                    src={selectedCharacterPhoto}
                    className="h-[80px] rounded-lg border-2"
                  />
                  <FontAwesomeIcon
                    icon={faCircleXmark}
                    size="xl"
                    className="absolute -top-2 -right-2 text-black hover:opacity-50 cursor-pointer"
                    onClick={() => {
                      setInputValue((prev) => ({
                        ...prev,
                        characterId: undefined,
                      }));
                      setSelectedCharacterPhoto("");
                    }}
                  />
                </div>
              )}
            </div>
            <DialogInputError message={inputErrors.characterId} />

            <AppInput
              theme="dark"
              title="Order No."
              onChange={(e) => {
                setInputValue((prev) => ({
                  ...prev,
                  orderNum: e.target.value,
                }));
              }}
            />
            <DialogInputError message={inputErrors.orderNum} />

            <AppTextArea
              theme="dark"
              title="Reading Prompt"
              onChange={(e) => {
                setInputValue((prev) => ({
                  ...prev,
                  readingText: e.target.value,
                }));
              }}
            />
            <DialogInputError message={inputErrors.readingText} />

            <CheckInput
              title={"Is Narrator"}
              isSelected={inputValues.isNarrator ?? false}
              onChange={(isSelected: boolean) =>
                setInputValue((prev) => ({
                  ...prev,
                  isNarrator: isSelected,
                }))
              }
            />

            <div className="flex items-end justify-end mt-2 gap-2 w-full">
              <CancelIconButton onCancelClick={reset} />
              <SaveIconButton onSaveClick={async () => await save()} />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CreatePageSection;
