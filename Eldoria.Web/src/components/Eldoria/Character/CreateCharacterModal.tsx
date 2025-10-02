import React, { ChangeEvent, useState } from "react";
import { useAuth } from "../../../providers/AuthProvider";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import Title from "../../typography/Title";
import EasyInput from "../../EasyInput";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import { create_character_url, postForm } from "../../../api/requests";

interface CreateCharacterModalProps {
  visible: boolean;
  onClose: () => void;
  onSave: () => void;
}

const CreateCharacterModal = ({
  visible,
  onClose,
  onSave,
}: CreateCharacterModalProps) => {
  const { token } = useAuth();

  const [name, setName] = useState("");
  const [descripton, setDescripton] = useState(""); // keeping original spelling
  const [file, setFile] = useState<File>();
  const [maxHp, setMaxHp] = useState("");
  const [maxMp, setMaxMp] = useState("");
  const [movement, setMovement] = useState("");
  const [meleeAttackDamage, setMeleeAttackDamage] = useState("");
  const [bowAttackDamage, setBowAttackDamage] = useState("");
  const [isActive, setIsActive] = useState(false);

  const save = async () => {
    if (!name || !descripton || !file || !maxHp || !maxMp || !movement) return;

    const formData = new FormData();
    formData.append("Name", name);
    // property name matches the C# model's property (with its spelling)
    formData.append("Descripton", descripton);

    if (!(file instanceof File)) return;
    formData.append("Photo", file, file.name);

    formData.append("MaxHp", maxHp);
    formData.append("MaxMp", maxMp);
    formData.append("Movement", movement);

    // optional/nullable ints
    if (meleeAttackDamage)
      formData.append("MeleeAttackDamage", meleeAttackDamage);
    if (bowAttackDamage) formData.append("BowAttackDamage", bowAttackDamage);

    formData.append("IsActive", isActive ? "true" : "false");

    await postForm(create_character_url, formData, token);

    setName("");
    setDescripton("");
    setMaxHp("");
    setMaxMp("");
    setMovement("");
    setMeleeAttackDamage("");
    setBowAttackDamage("");
    setIsActive(false);

    onSave();
    onClose();
  };

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-12">
          <Title>Create Character</Title>

          <EasyInput
            title="Name"
            value={name}
            onChange={(e: ChangeEvent<HTMLInputElement>) =>
              setName(e.target.value)
            }
          />

          <EasyInput
            title="Description"
            value={descripton}
            onChange={(e: ChangeEvent<HTMLInputElement>) =>
              setDescripton(e.target.value)
            }
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
            title="Max HP"
            value={maxHp}
            onChange={(e: ChangeEvent<HTMLInputElement>) =>
              setMaxHp(e.target.value)
            }
          />

          <EasyInput
            title="Max MP"
            value={maxMp}
            onChange={(e: ChangeEvent<HTMLInputElement>) =>
              setMaxMp(e.target.value)
            }
          />

          <EasyInput
            title="Movement"
            value={movement}
            onChange={(e: ChangeEvent<HTMLInputElement>) =>
              setMovement(e.target.value)
            }
          />

          <EasyInput
            title="Melee Attack Damage (optional)"
            value={meleeAttackDamage}
            onChange={(e: ChangeEvent<HTMLInputElement>) =>
              setMeleeAttackDamage(e.target.value)
            }
          />

          <EasyInput
            title="Bow Attack Damage (optional)"
            value={bowAttackDamage}
            onChange={(e: ChangeEvent<HTMLInputElement>) =>
              setBowAttackDamage(e.target.value)
            }
          />

          <label className="flex items-center gap-3">
            <input
              type="checkbox"
              checked={isActive}
              onChange={(e) => setIsActive(e.target.checked)}
            />
            <span>Is Active</span>
          </label>

          <div className="flex justify-center">
            <EasyButton
              title={"Save Character"}
              variant={EasyButtonVariant.Primary}
              onClick={async () => await save()}
            />
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default CreateCharacterModal;
