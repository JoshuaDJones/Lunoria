import React, { ChangeEvent, useState } from "react";
import { useAuth } from "../../../providers/AuthProvider";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import Title from "../../typography/Title";
import EasyInput from "../../EasyInput";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import { create_spell_url, postForm } from "../../../api/requests";

interface CreateSpellModalProps {
  visible: boolean;
  onClose: () => void;
  onSave: () => void;
}

const CreateSpellModal = ({
  visible,
  onClose,
  onSave,
}: CreateSpellModalProps) => {
  const { token } = useAuth();

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [file, setFile] = useState<File>();
  const [cost, setCost] = useState("");
  const [damage, setDamage] = useState("");
  const [health, setHealth] = useState("");

  const save = async () => {
    if (!name || !description || !file || !cost || !damage || !health) return;

    const formData = new FormData();

    formData.append("Name", name);
    formData.append("Description", description);

    if (!(file instanceof File)) return;

    formData.append("Photo", file, file.name);
    formData.append("Cost", cost);
    formData.append("Damage", damage);
    formData.append("Health", health);

    await postForm(create_spell_url, formData, token);

    setName("");
    setDescription("");
    setCost("");
    setDamage("");
    setHealth("");

    onSave();
    onClose();
  };

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-12">
          <Title>Create Spell</Title>
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

          <input
            id="titlePhoto"
            name="titlePhoto"
            type="file"
            className="text-black dark:text-white"
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              const selectedFile = event.currentTarget.files
                ? event.currentTarget.files[0]
                : null;

              if (selectedFile) setFile(selectedFile);
            }}
          />

          <EasyInput
            title="Cost"
            value={cost}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setCost(event.target.value);
            }}
          />

          <EasyInput
            title="Damage"
            value={damage}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setDamage(event.target.value);
            }}
          />

          <EasyInput
            title="Health"
            value={health}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setHealth(event.target.value);
            }}
          />

          <div className="flex justify-center">
            <EasyButton
              title={"Save Spell"}
              variant={EasyButtonVariant.Primary}
              onClick={async () => await save()}
            />
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default CreateSpellModal;
