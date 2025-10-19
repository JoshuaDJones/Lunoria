import { SpellDto } from "../../types/spell";
import Text, { TextColor, TextSize } from "../typography/Text";

interface DashboardSpellCardProps {
  spell: SpellDto;
}

const DashboardSpellCard = ({ spell }: DashboardSpellCardProps) => {
  return (
    <div key={spell.id} className="flex items-start gap-2">
      <img className="w-[15%] rounded-xl" src={spell.photoUrl} alt="" />
      <div className="flex flex-col">
        <Text className="mb-1 font-bold" textColor={TextColor.white}>
          {spell.name}
        </Text>
        <Text textColor={TextColor.white} size={TextSize.sm}>
          Cost: {spell.mpCost} Mp
        </Text>
        <Text textColor={TextColor.white} size={TextSize.sm}>
          Range: {spell.range} Tiles
        </Text>
        {!!spell.damageEffect && (
          <Text textColor={TextColor.white} size={TextSize.sm}>
            Damage: {spell.damageEffect}
          </Text>
        )}

        {!!spell.healthEffect && (
          <Text textColor={TextColor.white} size={TextSize.sm}>
            Health: {spell.healthEffect}
          </Text>
        )}

        {!!spell.magicEffect && (
          <Text textColor={TextColor.white} size={TextSize.sm}>
            Magic: {spell.magicEffect}
          </Text>
        )}
      </div>
    </div>
  );
};

export default DashboardSpellCard;
