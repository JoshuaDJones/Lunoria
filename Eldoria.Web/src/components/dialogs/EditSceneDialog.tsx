import { useState } from "react";
import { SceneDialogDto } from "../../types/scene";
import AppInput from "../inputs/AppInput";
import DialogInputError from "../inputs/DialogInputError";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import EditDialogItemContainer from "./EditDialogItemContainer";

interface InputValues {
  title?: string;
}

interface InputErrors {
  title?: string;
}

interface EditSceneDialogProps {
  isOpen: boolean;
  sceneDialog: SceneDialogDto;
  onRefreshRequest: () => void;
  onCloseClick: () => void;
}

const EditSceneDialog = ({
  isOpen,
  sceneDialog,
  onRefreshRequest,
  onCloseClick,
}: EditSceneDialogProps) => {
  const { patch } = useApi();
  const { showToast } = useToast();

  const [inputValues, setInputValues] = useState<InputValues>({
    title: sceneDialog.title,
  });

  const [inputErrors, setInputErrors] = useState<InputErrors>();

  const save = async () => {
    const isValid = validate();

    if (!isValid) return;

    try {
      await patch(`${BASE_URL}/SceneDialog/${sceneDialog.id}`, {
        Title: inputValues.title,
      });

      showToast("Success", "Scene dialog saved.", ToastType.success, 3000);
      onRefreshRequest();
      onCloseClick();
    } catch (err) {
      showToast("Error", "Scene dialog not saved.", ToastType.error, 3000);
      console.error(err);
    }
  };

  const validate = (): boolean => {
    if (!inputValues.title) {
      setInputErrors({ title: "Title must have a value." });
      return false;
    }

    setInputErrors({});
    return true;
  };

  const close = () => {
    setInputValues({
      title: sceneDialog.title,
    });
    setInputErrors({});
    onCloseClick();
  };

  if (!isOpen) return null;

  return (
    <EditDialogItemContainer
      title="Edit Scene Dialog"
      onCloseClick={close}
      onSaveClick={async () => await save()}
    >
      <AppInput
        valid={!inputErrors?.title}
        theme="dark"
        title="Title"
        value={inputValues.title}
        onChange={(e) => setInputValues({ title: e.target.value })}
      />
      <DialogInputError message={inputErrors?.title} />
    </EditDialogItemContainer>
  );
};

export default EditSceneDialog;
