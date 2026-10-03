import {
  SceneAttackType,
  type ScenePlaythroughParticipant,
} from "@/features/journeys";
import TurnActionButton from "@/features/sceneplaythrough/components/TurnActionButton";

const AttackTypeOptions = ({
  participant,
  onSelect,
}: {
  participant: ScenePlaythroughParticipant;
  onSelect: (attackType: SceneAttackType) => void;
}) => {
  return (
    <div className="grid gap-4 sm:grid-cols-3">
      {participant.meleeAttackDamage !== null && (
        <TurnActionButton
          label="Melee Attack"
          imageSrc="/Melee_Attack.png"
          onClick={() => onSelect(SceneAttackType.Melee)}
        />
      )}
      {participant.bowAttackDamage !== null && (
        <TurnActionButton
          label="Range Attack"
          imageSrc="/Bow_Attack.png"
          onClick={() => onSelect(SceneAttackType.Range)}
        />
      )}
      {participant.spells.some(
        (spell) =>
          spell.damageEffect !== null || spell.isSupport || spell.isUtility,
      ) && (
        <TurnActionButton
          label="Cast Spell"
          imageSrc="/Spell_Attack.png"
          onClick={() => onSelect(SceneAttackType.Spell)}
        />
      )}
    </div>
  );
};

export default AttackTypeOptions;
