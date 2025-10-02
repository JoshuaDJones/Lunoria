import React, { useState } from "react";
import { Character } from "../../../models.eldoria/character";
import BorderBox from "../BorderBox";
import DataRow from "../DataRow";
import { DateTime } from "luxon";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";
import CharacterSpells from "./CharacterSpells";

interface CharacterItemProps {
  character: Character;
}

const CharacterItem = ({ character }: CharacterItemProps) => {
  const [showSpellsModal, setShowSpellModal] = useState(false);

  return (
    <div>
      <BorderBox>
        <img src={character.photoUrl} className="w-[150px]" />

        <DataRow title={"Name:"} value={character.name} />
        {/* mirroring your SpellItem's pattern, including the second row label */}
        <DataRow title={"Description:"} value={character.descripton} />

        <DataRow title={"Max HP:"} value={character.maxHp.toString()} />
        <DataRow title={"Max MP:"} value={character.maxMp.toString()} />
        <DataRow title={"Movement:"} value={character.movement.toString()} />
        <DataRow
          title={"Melee Attack Damage:"}
          value={
            character.meleeAttackDamage !== null
              ? character.meleeAttackDamage.toString()
              : ""
          }
        />
        <DataRow
          title={"Bow Attack Damage:"}
          value={
            character.bowAttackDamage !== null
              ? character.bowAttackDamage.toString()
              : ""
          }
        />
        <DataRow
          title={"Is Active:"}
          value={character.isActive ? "Yes" : "No"}
        />

        <DataRow
          title={"Created:"}
          value={DateTime.fromISO(character.createDate).toFormat("MM-dd-yyyy")}
        />

        <div className="flex-row flex flex-wrap gap-2 mt-5">
          <EasyButton
            className="gap-2"
            title={"Edit"}
            rightIcon={<FontAwesomeIcon icon={faEdit} color={"white"} />}
            variant={EasyButtonVariant.Primary}
          ></EasyButton>

          <EasyButton
            className="gap-2"
            title={"Delete"}
            rightIcon={<FontAwesomeIcon icon={faTrash} color={"white"} />}
            variant={EasyButtonVariant.Secondary}
          ></EasyButton>

          <EasyButton
            className="gap-2"
            title={"Spells"}
            rightIcon={<FontAwesomeIcon icon={faPlus} color={"white"} />}
            variant={EasyButtonVariant.Teal}
            onClick={() => setShowSpellModal(true)}
          ></EasyButton>

          <CharacterSpells
            characterId={character.id}
            visible={showSpellsModal}
            onClose={() => setShowSpellModal(false)}
            onSave={() => {}}
          />
        </div>
      </BorderBox>
    </div>
  );
};

export default CharacterItem;
