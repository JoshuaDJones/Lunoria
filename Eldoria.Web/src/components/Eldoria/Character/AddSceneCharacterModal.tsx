import React, { ChangeEvent, useEffect, useState } from "react";
import { useAuth } from "../../../providers/AuthProvider";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import Title from "../../typography/Title";
import EasyInput from "../../EasyInput";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import {
  BASE_URL,
  create_character_url,
  get,
  get_all_characters_url,
  post,
  postForm,
} from "../../../api/requests";
import { Character } from "../../../models.eldoria/character";
import BorderBox from "../BorderBox";
import EasyText from "../../EasyText";
import DataRow from "../DataRow";

interface AddSceneCharacterModalProps {
  sceneId: number;
  type: "enemy" | "player";
  visible: boolean;
  onClose: () => void;
  onRequestRefresh: () => void;
}

const AddSceneCharacterModal = ({
  sceneId,
  type,
  visible,
  onClose,
  onRequestRefresh,
}: AddSceneCharacterModalProps) => {
  const { token } = useAuth();
  const [characters, setCharacters] = useState<Character[]>([]);

  const filterCharacters =
    type === "enemy"
      ? characters.filter((c) => c.isActive === false)
      : characters.filter((c) => c.isActive === true);

  const getCharacters = async () => {
    const response = await get(get_all_characters_url, undefined, token);
    setCharacters(response);
  };

  useEffect(() => {
    getCharacters();
  }, []);

  const addCharacter = async (
    characterId: number,
    maxHp: number,
    maxMp: number,
  ) => {
    const isEnemy = type === "enemy";
    const isDown = false;
    const inTurnOrder = type === "player";
    const currentHp = maxHp;
    const currentMp = maxMp;

    const response = await post(
      `${BASE_URL}scene-characters/`,
      {
        sceneId: sceneId,
        mainCharacterId: characterId,
        currentHp: currentHp,
        currentMp: currentMp,
        isEnemy: isEnemy,
        inTurnOrder: inTurnOrder,
        isDown: isDown,
      },
      undefined,
      token,
    );

    onClose();
    onRequestRefresh();
  };

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-12">
          <Title>Add Character</Title>

          <div className="flex-1">
            {filterCharacters.map((c) => (
              <BorderBox className="mb-5">
                <div className="flex flex-row gap-5">
                  <img className="w-[25%]" src={c.photoUrl} />
                  <div className="flex-1 flex-col">
                    <EasyText className="text-4xl">{c.name}</EasyText>
                    <DataRow title={"Description:"} value={c.descripton} />
                    <DataRow
                      title={"MaxHp:"}
                      value={c.maxHp.toString()}
                      className="mt-10"
                    />
                    <DataRow title={"MaxMp:"} value={c.maxMp.toString()} />
                    <DataRow
                      title={"Melee:"}
                      value={c.meleeAttackDamage?.toString() ?? ""}
                    />
                    <DataRow
                      title={"Bow:"}
                      value={c.bowAttackDamage?.toString() ?? ""}
                    />
                    <DataRow
                      title={"Movement:"}
                      value={c.movement?.toString() ?? ""}
                    />
                    <DataRow
                      title={"Max Inventory:"}
                      value={c.maxInventory?.toString() ?? ""}
                    />
                  </div>
                  <EasyButton
                    title={"Add"}
                    variant={EasyButtonVariant.Primary}
                    className="self-end"
                    onClick={() => addCharacter(c.id, c.maxHp, c.maxMp)}
                  />
                </div>
              </BorderBox>
            ))}
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default AddSceneCharacterModal;
