import { ChangeEvent, useState } from "react";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import { useAuth } from "../../../providers/AuthProvider";
import { create_items_url, postForm } from "../../../api/requests";
import Title from "../../typography/Title";
import EasyInput from "../../EasyInput";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";

interface CreateItemModalProps {
  visible: boolean;
  onClose: () => void;
  onSave: () => void;
}

const CreateItemModal = ({
  visible,
  onClose,
  onSave,
}: CreateItemModalProps) => {
  const { token } = useAuth();
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [hpEffect, setHpEffect] = useState("");
  const [mpEffect, setMpEffect] = useState("");
  const [file, setFile] = useState<File>();

  const save = async () => {
    if (!name || !description || !hpEffect || !mpEffect || !file || !token)
      return;

    const formData = new FormData();

    formData.append("Name", name);
    formData.append("Description", description);
    formData.append("HpEffect", hpEffect);
    formData.append("MpEffect", mpEffect);

    if (!file || !(file instanceof File)) return;

    formData.append("Photo", file, file.name);

    await postForm(create_items_url, formData, token);

    setName("");
    setDescription("");
    setHpEffect("");
    setMpEffect("");

    onSave();
    onClose();
  };

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-20">
          <Title>Create Item</Title>

          <EasyInput
            title="Name"
            value={name}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setName(event.target.value);
            }}
          />

          <EasyInput
            title="Description"
            value={description}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setDescription(event.target.value);
            }}
          />

          <EasyInput
            title="Hp Effect"
            value={hpEffect}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setHpEffect(event.target.value);
            }}
          />

          <EasyInput
            title="Mp Effect"
            value={mpEffect}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setMpEffect(event.target.value);
            }}
          />

          <input
            id="photo"
            name="photo"
            type="file"
            className="text-black dark:text-white"
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              const selectedFile = event.currentTarget.files
                ? event.currentTarget.files[0]
                : null;

              if (selectedFile) setFile(selectedFile);
            }}
          />

          <div className="flex justify-center">
            <EasyButton
              title={"Save Item"}
              variant={EasyButtonVariant.Primary}
              onClick={async () => await save()}
            />
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default CreateItemModal;
