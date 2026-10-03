import { useState } from "react";
import { Button } from "@/components/ui";
import {
  SceneAttackType,
  type ScenePlaythroughParticipant,
} from "@/features/journeys";
import DieFaceButton from "@/features/sceneplaythrough/components/DieFaceButton";
import {
  getAttackBaseDamage,
  getAttackDamageReduction,
  getEligibleAttackTargets,
} from "@/features/sceneplaythrough/utils/sceneCombatUtils";
import SpellAttackSelector from "@/features/sceneplaythrough/components/SpellAttackSelector";

const AttackResolutionOptions = ({
  attacker,
  targets: participants,
  attackType,
  onAttack,
}: {
  attacker: ScenePlaythroughParticipant;
  targets: ScenePlaythroughParticipant[];
  attackType: SceneAttackType;
  onAttack: (
    targetParticipantId: number | null,
    roll: number,
    playthroughSpellId: number | null,
  ) => Promise<void>;
}) => {
  const availableSpells = attacker.spells.filter(
    (spell) =>
      spell.damageEffect !== null || spell.isSupport || spell.isUtility,
  );
  const [selectedTargetId, setSelectedTargetId] = useState<number>();
  const [selectedRoll, setSelectedRoll] = useState<number>();
  const [selectedSpellId, setSelectedSpellId] = useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const selectedSpell = availableSpells.find(
    (spell) => spell.id === selectedSpellId,
  );
  const isSupport =
    attackType === SceneAttackType.Spell && selectedSpell?.isSupport === true;
  const isUtility =
    attackType === SceneAttackType.Spell && selectedSpell?.isUtility === true;
  const targets =
    attackType === SceneAttackType.Spell && !selectedSpell
      ? []
      : getEligibleAttackTargets(attacker, participants, isSupport);
  const selectedTarget = targets.find(
    (target) => target.id === selectedTargetId,
  );
  const baseDamage = getAttackBaseDamage(attacker, attackType, selectedSpell);

  const damageReduction = selectedTarget
    ? getAttackDamageReduction(selectedTarget, attackType)
    : 0;

  const totalDamage =
    baseDamage === null || selectedRoll === undefined
      ? null
      : Math.max(0, baseDamage + selectedRoll - damageReduction);

  const hasRequiredSpell =
    attackType !== SceneAttackType.Spell || selectedSpell !== undefined;
  const canAttack =
    (isUtility || selectedTarget !== undefined) &&
    (isUtility ||
      (selectedRoll !== undefined && (isSupport || totalDamage !== null))) &&
    (attackType !== SceneAttackType.Spell ||
      (selectedSpell !== undefined &&
        selectedSpell.mpCost <= attacker.currentMp)) &&
    hasRequiredSpell &&
    !isSubmitting;

  return (
    <div>
      {isUtility ? (
        <p className="rounded-xl border border-border bg-surface p-4 text-content">
          Utility spell: uses {selectedSpell?.mpCost} MP and one action. No
          target or die roll is required. No other stats are changed.
        </p>
      ) : isSupport ? (
        <p className="rounded-xl border border-border bg-surface p-4 text-content">
          Base restoration: {selectedSpell?.healthEffect ?? 0} HP and{" "}
          {selectedSpell?.magicEffect ?? 0} MP. Add the selected roll to each
          positive restoration effect, capped at the target’s maximum. Uses{" "}
          {selectedSpell?.mpCost} MP and one action.
          {selectedRoll !== undefined && (
            <span className="mt-2 block">
              With roll {selectedRoll}: up to{" "}
              {(selectedSpell?.healthEffect ?? 0) > 0
                ? selectedSpell!.healthEffect! + selectedRoll
                : 0}{" "}
              HP and{" "}
              {(selectedSpell?.magicEffect ?? 0) > 0
                ? selectedSpell!.magicEffect! + selectedRoll
                : 0}{" "}
              MP.
            </span>
          )}
        </p>
      ) : (
        <div className="grid grid-cols-3 gap-4 rounded-xl border border-border bg-surface p-4 text-center">
          <div>
            <p className="text-sm text-content-muted">Attack damage</p>
            <p className="mt-1 text-3xl font-semibold text-content">
              {baseDamage ?? "—"}
            </p>
          </div>
          <div>
            <p className="text-sm text-content-muted">Target reduction</p>
            <p className="mt-1 text-3xl font-semibold text-content">
              {selectedTarget ? damageReduction : "—"}
            </p>
          </div>
          <div>
            <p className="text-sm text-content-muted">Total attack damage</p>
            <p className="mt-1 text-3xl font-semibold text-content">
              {totalDamage ?? "—"}
            </p>
          </div>
        </div>
      )}
      {attackType === SceneAttackType.Spell && (
        <SpellAttackSelector
          spells={availableSpells}
          currentMp={attacker.currentMp}
          selectedSpellId={selectedSpellId}
          onSelect={(id) => {
            setSelectedSpellId(id);
            setSelectedTargetId(undefined);
          }}
        />
      )}

      {!isUtility && (
        <section className="mt-6">
          <h3 className="text-lg font-semibold text-content">
            Select a target
          </h3>
          {targets.length === 0 ? (
            <p className="mt-3 rounded-xl border border-border bg-surface p-4 text-content-muted">
              {attackType === SceneAttackType.Spell && !selectedSpell
                ? "Choose a spell to see eligible targets."
                : "There are no eligible targets for this action."}
            </p>
          ) : (
            <div className="mt-3 grid gap-3 sm:grid-cols-2">
              {targets.map((target) => {
                const imageUrl =
                  target.portraitUrl?.trim() || target.photoUrl?.trim();
                const isSelected = selectedTargetId === target.id;

                return (
                  <button
                    key={target.id}
                    type="button"
                    aria-pressed={isSelected}
                    className={`flex cursor-pointer items-center gap-3 overflow-hidden rounded-xl border-2 bg-surface p-3 text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 ${
                      isSelected ? "border-utility" : "border-border"
                    }`}
                    onClick={() => setSelectedTargetId(target.id)}
                  >
                    <span className="flex h-20 w-20 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-canvas">
                      {imageUrl ? (
                        <img
                          src={imageUrl}
                          alt=""
                          className="h-full w-full object-contain"
                        />
                      ) : (
                        <span className="text-xs text-content-muted">
                          No image
                        </span>
                      )}
                    </span>
                    <span className="min-w-0">
                      <span className="block font-semibold text-content">
                        {target.name}
                      </span>
                      <span className="mt-1 block text-sm text-content-muted">
                        HP {target.currentHp} / {target.maxHp}
                      </span>
                    </span>
                  </button>
                );
              })}
            </div>
          )}
        </section>
      )}
      {!isUtility && (
        <section className="mt-6">
          <h3 className="text-lg font-semibold text-content">
            Select a die face
          </h3>
          <div className="mt-3 grid grid-cols-3 gap-3 sm:grid-cols-6">
            {[1, 2, 3, 4, 5, 6].map((value) => (
              <DieFaceButton
                key={value}
                value={value}
                selected={selectedRoll === value}
                onClick={() => setSelectedRoll(value)}
                compact
              />
            ))}
          </div>
        </section>
      )}
      <div className="mt-6 flex justify-end">
        <Button
          variant="danger"
          size="lg"
          disabled={!canAttack}
          onClick={() => {
            if (
              (!isUtility &&
                (selectedTargetId === undefined || !selectedTarget)) ||
              (!isUtility && selectedRoll === undefined) ||
              !hasRequiredSpell
            ) {
              return;
            }

            setIsSubmitting(true);
            void onAttack(
              isUtility ? null : selectedTargetId!,
              isUtility ? 1 : selectedRoll!,
              selectedSpell?.id ?? null,
            ).finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting
            ? "Resolving..."
            : attackType === SceneAttackType.Spell
              ? "Cast spell"
              : "Attack"}
        </Button>
      </div>
    </div>
  );
};

export default AttackResolutionOptions;
