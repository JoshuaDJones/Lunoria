import { useState } from "react";
import { DialogPageDto } from "../../types/scene";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import EditDialogItemContainer from "./EditDialogItemContainer";
import AppInput from "../inputs/AppInput";
import DialogInputError from "../inputs/DialogInputError";
import FileInput from "../inputs/FileInput";
import { canBeNumber } from "../../utils/numberUtils";
import Text, { TextColor } from "../typography/Text";
import clsx from "clsx";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowRight } from "@fortawesome/free-solid-svg-icons";

interface InputValues {
  orderNum?: string;
  photo?: File;
}

interface InputErrors {
  orderNum?: string;
  photo?: string;
}

interface EditDialogPageProps {
  isOpen: boolean;
  dialogPage: DialogPageDto;
  onRefreshRequest: () => void;
  onCloseClick: () => void;
}

const EditDialogPage = ({
  isOpen,
  dialogPage,
  onRefreshRequest,
  onCloseClick,
}: EditDialogPageProps) => {
  const { putForm } = useApi();
  const { showToast } = useToast();

  const [inputValues, setInputValues] = useState<InputValues>({
    orderNum: dialogPage.orderNum.toString(),
  });
  const [inputErrors, setInputErrors] = useState<InputErrors>();

  const save = async () => {
    const isValid = validate();

    if (!isValid) return;

    const formData = new FormData();

    formData.append("OrderNum", inputValues?.orderNum ?? "");

    if (inputValues.photo) formData.append("Photo", inputValues?.photo ?? "");

    try {
      await putForm(`${BASE_URL}/DialogPage/${dialogPage.id}`, formData);

      setInputValues({
        orderNum: undefined,
        photo: undefined,
      });

      setInputErrors({
        orderNum: undefined,
        photo: undefined,
      });

      showToast("Success", "Dialog page saved.", ToastType.success, 3000);
      onRefreshRequest();
      close();
    } catch (err) {
      showToast("Error", "Dialog page not saved.", ToastType.error, 3000);
      console.error(err);
    }
  };

  const validate = (): boolean => {
    const canParse = canBeNumber(inputValues?.orderNum ?? "");

    if (!canParse) {
      setInputErrors((prev) => ({
        ...prev,
        orderNum: "Order must be a valid number.",
      }));
      return false;
    }

    setInputErrors({
      orderNum: undefined,
      photo: undefined,
    });

    return true;
  };

  const close = () => {
    setInputValues({
      orderNum: dialogPage.orderNum.toString(),
    });
    setInputErrors({});
    onCloseClick();
  };

  if (!isOpen) return null;

  return (
    <EditDialogItemContainer
      title="Edit Dialog Page"
      onCloseClick={close}
      onSaveClick={async () => await save()}
    >
      <AppInput
        valid={!inputErrors?.orderNum}
        theme="dark"
        title="Order Num"
        value={inputValues?.orderNum}
        onChange={(e) =>
          setInputValues((prev) => ({
            ...prev,
            orderNum: e.target.value,
          }))
        }
      />
      <DialogInputError message={inputErrors?.orderNum} />

      <FileInput
        className="mt-4"
        title={"Photo"}
        onFileSelect={(file) =>
          setInputValues((prev) => ({
            ...prev,
            photo: file,
          }))
        }
      />

      <div className="flex items-stretch mt-4 self-center bg-stone-800 p-4 rounded-xl">
        <div className="flex flex-col items-center gap-2">
          <Text textColor={TextColor.white}>Original</Text>
          <img
            src={dialogPage.photoUrl}
            className={clsx("w-[80px] rounded-lg", {
              "opacity-20": !!inputValues?.photo,
            })}
          />
        </div>
        {inputValues?.photo && (
          <div className="flex items-center justify-center px-3">
            <FontAwesomeIcon icon={faArrowRight} size="xl" color="white" />
          </div>
        )}

        <div className="flex flex-col items-center gap-2">
          {inputValues?.photo && (
            <>
              <Text textColor={TextColor.white}>Replacement</Text>
              <img
                src={URL.createObjectURL(inputValues.photo)}
                className="w-[80px] rounded-lg"
              />
            </>
          )}
        </div>
      </div>
    </EditDialogItemContainer>
  );
};

export default EditDialogPage;
