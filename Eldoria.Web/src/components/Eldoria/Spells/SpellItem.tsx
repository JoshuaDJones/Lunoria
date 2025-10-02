import React from "react";
import { Spell } from "../../../models.eldoria/spell";
import BorderBox from "../BorderBox";
import DataRow from "../DataRow";
import { DateTime } from "luxon";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faTrash } from "@fortawesome/free-solid-svg-icons";

interface SpellItemProps {
  spell: Spell;
}

const SpellItem = ({ spell }: SpellItemProps) => {
  return (
    <div>
      <BorderBox>
        <img src={spell.photoUrl} className="w-[150px]" />
        <DataRow title={"Name:"} value={spell.name} />
        <DataRow title={"Description:"} value={spell.name} />
        <DataRow title={"Cost:"} value={spell.cost.toString()} />
        <DataRow title={"Damage:"} value={spell.damage.toString()} />
        <DataRow title={"Health:"} value={spell.health.toString()} />
        <DataRow
          title={"Created:"}
          value={DateTime.fromISO(spell.createDate).toFormat("MM-dd-yyyy")}
        />

        <div className="flex-row flex gap-2 mt-5">
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
        </div>
      </BorderBox>
    </div>
  );
};

export default SpellItem;
