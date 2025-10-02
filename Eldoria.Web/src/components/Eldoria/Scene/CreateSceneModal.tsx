import { ChangeEvent, useState } from "react";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import { useAuth } from "../../../providers/AuthProvider";
import { create_scene_url, postForm } from "../../../api/requests";
import Title from "../../typography/Title";
import EasyInput from "../../EasyInput";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";

interface CreateSceneModalProps {
  visible: boolean;
  journeyId: number;
  onClose: () => void;
  onSave: () => void;
}

const CreateSceneModal = ({
  journeyId,
  visible,
  onClose,
  onSave,
}: CreateSceneModalProps) => {
  const { token } = useAuth();
  const [name, setName] = useState("");
  const [grid, setGrid] = useState("");
  const [file, setFile] = useState<File>();

  const save = async () => {
    if (!name || !grid || !file || !token) return;

    const formData = new FormData();

    formData.append("JourneyId", journeyId.toString());
    formData.append("Name", name);
    formData.append("GridUrl", grid);

    if (!file || !(file instanceof File)) return;

    formData.append("Photo", file, file.name);

    await postForm(create_scene_url, formData, token);

    setGrid("");
    setName("");

    onSave();
    onClose();
  };

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-20">
          <Title>Create Scene</Title>
          <EasyInput
            title="Name"
            value={name}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setName(event.target.value);
            }}
          />

          <EasyInput
            title="Grid"
            value={grid}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              setGrid(event.target.value);
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

          <div className="flex justify-center">
            <EasyButton
              title={"Save Scene"}
              variant={EasyButtonVariant.Primary}
              onClick={async () => await save()}
            />
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default CreateSceneModal;
