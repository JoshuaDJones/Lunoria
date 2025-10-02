import React, { ChangeEvent, useState } from "react";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import { useAuth } from "../../../providers/AuthProvider";
import { create_journey_url, post } from "../../../api/requests";
import Title from "../../typography/Title";
import EasyInput from "../../EasyInput";
import InputArea from "../../InputArea";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";

interface CreateJourneyModalProps {
  visible: boolean;
  onClose: () => void;
  onSave: () => void;
}

const CreateJourneyModal = ({
  visible,
  onClose,
  onSave,
}: CreateJourneyModalProps) => {
  const { token } = useAuth();
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const save = async () => {
    if (!name || !description) return;

    await post(
      create_journey_url,
      {
        name: name,
        description: description,
      },
      undefined,
      token,
    );

    onSave();
    onClose();
  };

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-20">
          <Title>Create Journey</Title>
          <EasyInput
            title="Name"
            value={name}
            onChange={(event: ChangeEvent<HTMLInputElement>) => {
              console.log(event.target.value);

              setName(event.target.value);
            }}
          />

          <InputArea
            title="Description"
            value={description}
            onChange={(event: ChangeEvent<HTMLTextAreaElement>) => {
              console.log(event.target.value);

              setDescription(event.target.value);
            }}
          />
          <div className="flex justify-center">
            <EasyButton
              title={"Save Journey"}
              variant={EasyButtonVariant.Primary}
              onClick={async () => await save()}
            />
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default CreateJourneyModal;
