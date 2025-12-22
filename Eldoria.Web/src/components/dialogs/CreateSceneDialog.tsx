import { useState } from "react";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import CancelIconButton from "../buttons/CancelIconButton";
import SaveIconButton from "../buttons/SaveIconButton";
import AppInput from "../inputs/AppInput";
import DialogInputError from "../inputs/DialogInputError";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface CreateSceneDialogProps {
  sceneId: number;
  onDialogCreated: () => void;
}

const CreateSceneDialog = ({
  sceneId,
  onDialogCreated,
}: CreateSceneDialogProps) => {
  const { post } = useApi();
  const { showToast } = useToast();

  const [inputOpen, setInputOpen] = useState(false);
  const [inputError, setInputError] = useState("");
  const [inputValue, setInputValue] = useState("");

  const handleSave = async () => {
    if (inputValue === "") {
      setInputError("Please enter a name.");
      return;
    }

    try {
      await post(`${BASE_URL}/SceneDialog/${sceneId}`, {
        title: inputValue,
      });

      showToast("Success", "Scene dialog saved.", ToastType.success, 3000);

      setInputOpen(false);
      setInputError("");
      setInputValue("");

      onDialogCreated();
    } catch (err) {
      showToast("Error", "Could not save scene dialog.", ToastType.error, 3000);
      console.error(err);
    }
  };

  const handleCancel = () => {
    setInputOpen(false);
    setInputError("");
    setInputValue("");
  };

  return (
    <div className="flex mt-3">
      {!inputOpen && (
        <AppButton
          onClick={() => setInputOpen(true)}
          className="border-2 hover:opacity-50"
          title={"Add"}
          variant={AppButtonVariant.ghost}
          size={AppButtonSize.sm}
          rightIcon={
            <FontAwesomeIcon icon={faPlus} className="text-white ml-1" />
          }
        />
      )}

      {inputOpen && (
        <div className="flex flex-1 flex-col border-2 rounded-lg p-5 bg-stone-400/75">
          <AppInput
            valid={!inputError}
            value={inputValue}
            theme="dark"
            title="New Scene Dialog"
            onChange={(e) => setInputValue(e.target.value)}
          />
          <DialogInputError message={inputError} />

          <div className="flex items-end justify-end mt-2 gap-2">
            <CancelIconButton onCancelClick={handleCancel} />
            <SaveIconButton onSaveClick={handleSave} />
          </div>
        </div>
      )}
    </div>
  );
};

export default CreateSceneDialog;
