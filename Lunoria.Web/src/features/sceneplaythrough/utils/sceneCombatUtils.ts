import {
  ParticipantType,
  SceneAttackType,
  type SceneAttackResult,
  type ScenePlaythroughParticipant,
  type ScenePlaythroughSpell,
} from "@/features/journeys";

export function getAttackBaseDamage(
  attacker: ScenePlaythroughParticipant,
  attackType: SceneAttackType,
  spell?: ScenePlaythroughSpell,
) {
  switch (attackType) {
    case SceneAttackType.Melee:
      return attacker.meleeAttackDamage;
    case SceneAttackType.Range:
      return attacker.bowAttackDamage;
    case SceneAttackType.Spell:
      return spell?.damageEffect ?? null;
  }
}

export function getAttackDamageReduction(
  target: ScenePlaythroughParticipant,
  attackType: SceneAttackType,
) {
  switch (attackType) {
    case SceneAttackType.Melee:
      return target.meleeDamageReduction;
    case SceneAttackType.Range:
      return target.bowDamageReduction;
    case SceneAttackType.Spell:
      return target.spellDamageReduction;
  }
}

export function getEligibleAttackTargets(
  attacker: ScenePlaythroughParticipant,
  participants: ScenePlaythroughParticipant[],
  isSupport = false,
) {
  return participants.filter((target) => {
    if (isSupport) {
      return (
        target.isActive &&
        !target.isDead &&
        (attacker.participantType === ParticipantType.Enemy
          ? target.participantType === ParticipantType.Enemy
          : target.participantType === ParticipantType.Player ||
            target.participantType === ParticipantType.NPC)
      );
    }
    if (
      target.id === attacker.id ||
      !target.isActive ||
      target.isDown ||
      target.isDead
    ) {
      return false;
    }

    return attacker.participantType === ParticipantType.Enemy
      ? target.participantType === ParticipantType.Player ||
          target.participantType === ParticipantType.NPC
      : target.participantType === ParticipantType.Enemy;
  });
}

export function getAttackTypeLabel(attackType: SceneAttackType) {
  switch (attackType) {
    case SceneAttackType.Melee:
      return "Melee Attack";
    case SceneAttackType.Range:
      return "Range Attack";
    default:
      return "Cast Spell";
  }
}

export function getAttackResultMessage(result: SceneAttackResult) {
  if (result.isUtility)
    return "Utility spell cast. MP spent and one action used.";
  if (result.isSupport)
    return `Restored ${result.healthRestored} HP and ${result.magicRestored} MP.`;
  const reward = result.rewardStat
    ? result.rewardAmount > 0
      ? ` You gained ${result.rewardAmount} ${result.rewardStat}.`
      : ` Your ${result.rewardStat} was already at maximum.`
    : "";
  const outcome = result.targetDefeated
    ? " The target was defeated."
    : ` The target has ${result.targetCurrentHp} HP remaining.`;

  return `Dealt ${result.damage} damage.${outcome}${reward}`;
}
