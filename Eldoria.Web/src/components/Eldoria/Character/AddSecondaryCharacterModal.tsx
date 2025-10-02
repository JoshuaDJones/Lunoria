import React, { useEffect, useState } from "react";
import { useAuth } from "../../../providers/AuthProvider";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import Title from "../../typography/Title";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import EasyText, { TextColor } from "../../EasyText";
import { BASE_URL, get, post, put } from "../../../api/requests";

interface AddSecondaryCharacterModalProps {
  visible: boolean;
  onClose: () => void;
  onSave: () => void;
  sceneCharacterId: number;
}

interface CharacterDto {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  hp: number;
  mp: number;
}

const list_characters_url = `${BASE_URL}characters`;
const attach_secondary_url = (sceneCharacterId: number) =>
  `${BASE_URL}scene-characters/${sceneCharacterId}/secondary`;

const AddSecondaryCharacterModal = ({
  visible,
  onClose,
  onSave,
  sceneCharacterId,
}: AddSecondaryCharacterModalProps) => {
  const { token } = useAuth();
  const [characters, setCharacters] = useState<CharacterDto[]>([]);
  const [loading, setLoading] = useState(false);

  const loadCharacters = async () => {
    setLoading(true);
    try {
      const res = await get(list_characters_url, undefined, token);
      setCharacters(Array.isArray(res) ? res : (res?.items ?? []));
    } catch (err) {
      console.error("Failed to load characters", err);
      setCharacters([]);
    } finally {
      setLoading(false);
    }
  };

  const attachSecondary = async (characterId: number) => {
    try {
      await post(
        attach_secondary_url(sceneCharacterId),
        { sceneCharacterId, secondaryCharacterId: characterId },
        undefined,
        token,
      );
      onSave();
      onClose();
    } catch (err) {
      console.error("Failed to attach secondary character", err);
    }
  };

  useEffect(() => {
    if (visible) loadCharacters();
  }, [visible]);

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-6">
          <Title>Add Secondary Character</Title>

          {loading && <EasyText>Loading…</EasyText>}

          <div className="flex flex-1 flex-col gap-4 overflow-y-auto">
            {characters.map((char) => (
              <div
                key={char.id}
                className="flex flex-row gap-4 p-3 border rounded-lg items-center"
              >
                <img
                  src={char.photoUrl}
                  alt={char.name}
                  className="w-16 h-16 object-cover rounded"
                />
                <div className="flex-1">
                  <EasyText className="text-lg font-bold">{char.name}</EasyText>
                  <EasyText className="text-sm">{char.description}</EasyText>
                </div>
                <EasyButton
                  title="Add"
                  variant={EasyButtonVariant.Primary}
                  onClick={() => attachSecondary(char.id)}
                />
              </div>
            ))}
            {characters.length === 0 && !loading && (
              <EasyText
                className="italic text-gray-500"
                textColor={TextColor.white}
              >
                No characters available
              </EasyText>
            )}
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default AddSecondaryCharacterModal;
