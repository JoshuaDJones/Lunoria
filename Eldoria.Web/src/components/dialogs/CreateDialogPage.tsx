import React, { useState } from "react";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import AppInput from "../inputs/AppInput";
import DialogInputError from "../inputs/DialogInputError";
import FileInput from "../inputs/FileInput";
import CancelIconButton from "../buttons/CancelIconButton";
import SaveIconButton from "../buttons/SaveIconButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { canBeNumber } from "../../utils/numberUtils";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { useLoading } from "../../providers/LoadingProvider";

interface InputValues {
  orderNum?: string;
  photo?: File;
}

interface InputErrors {
  orderNum?: string;
  photo?: string;
}

interface CreateDialogPageProps {
  sceneDialogId: number;
  onRefreshRequest: () => void;
}

const CreateDialogPage = ({
  sceneDialogId,
  onRefreshRequest,
}: CreateDialogPageProps) => {
  const { postForm } = useApi();
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();

  const [isCreateInputOpen, setIsCreateInputOpen] = useState(false);

  const [inputValues, setInputValues] = useState<InputValues>({
    orderNum: undefined,
    photo: undefined,
  });

  const [inputErrors, setInputErrors] = useState<InputErrors>({
    orderNum: undefined,
    photo: undefined,
  });

  const save = async () => {

    const isValid = validate();

    if (!isValid) return;    

    const formData = new FormData();

    formData.append("OrderNum", inputValues.orderNum ?? "");
    formData.append("Photo", inputValues.photo ?? "");

    try {
      showLoading();
      
      await postForm(`${BASE_URL}/DialogPage/${sceneDialogId}`, formData);

      setInputValues({
        orderNum: undefined,
        photo: undefined,
      });

      setInputErrors({
        orderNum: undefined,
        photo: undefined,
      });

      setIsCreateInputOpen(false);
      showToast("Success", "Dialog page created.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      showToast("Error", "Dialog page not created.", ToastType.error, 3000);
      console.error(err);
    } finally {
      closeLoading();
    }
  };

  const validate = (): boolean => {
    const canParse = canBeNumber(inputValues.orderNum ?? "");

    if (!canParse) {
      setInputErrors((prev) => ({
        ...prev,
        orderNum: "Order must be a valid number.",
      }));
      return false;
    }

    if (!inputValues.photo) {
      setInputErrors((prev) => ({
        ...prev,
        photo: "Photo can not be empty.",
      }));
      return false;
    }

    setInputErrors({
      orderNum: undefined,
      photo: undefined,
    });

    return true;
  };

  return (
    <div className="flex mt-3">
      {!isCreateInputOpen && (
        <AppButton
          className="border-2 hover:opacity-50"
          title="Add"
          variant={AppButtonVariant.ghost}
          size={AppButtonSize.sm}
          rightIcon={
            <FontAwesomeIcon icon={faPlus} className="text-white ml-1" />
          }
          onClick={() => setIsCreateInputOpen(true)}
        />
      )}

      {isCreateInputOpen && (
        <div className="flex flex-1 flex-col border-2 rounded-lg p-5 bg-stone-400/75">
          <AppInput
            valid={!inputErrors.orderNum}
            value={inputValues.orderNum}
            theme="dark"
            title="Order"
            onChange={(e) => {
              setInputValues((prev) => ({
                ...prev,
                orderNum: e.target.value,
              }));
            }}
          />
          <DialogInputError message={inputErrors.orderNum} />

          <FileInput
            title={"Photo"}
            onFileSelect={(file) =>
              setInputValues((prev) => ({
                ...prev,
                photo: file,
              }))
            }
            useClear={false}
            className="mt-4"
          />
          <DialogInputError message={inputErrors.photo} />

          <div className="flex items-end justify-end mt-2 gap-2">
            <CancelIconButton
              onCancelClick={() => setIsCreateInputOpen(false)}
            />
            <SaveIconButton onSaveClick={async () => await save()} />
          </div>
        </div>
      )}
    </div>
  );
};

export default CreateDialogPage;
