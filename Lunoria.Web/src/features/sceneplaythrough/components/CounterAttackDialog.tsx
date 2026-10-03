import { useEffect, useRef, useState } from "react";
import { Button } from "@/components/ui";
import {
  SceneAttackType,
  type ScenePlaythroughDetails,
} from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";
import AttackAnimationOverlay from "@/features/sceneplaythrough/components/AttackAnimationOverlay";
import AttackTypeOptions from "@/features/sceneplaythrough/components/AttackTypeOptions";
import AttackResolutionOptions from "@/features/sceneplaythrough/components/AttackResolutionOptions";
import { type AttackAnimationState } from "@/features/sceneplaythrough/types";

const CounterAttackDialog = ({
  scene,
  attackAnimation,
  onResolve,
  onReload,
}: {
  scene: ScenePlaythroughDetails;
  attackAnimation?: AttackAnimationState;
  onResolve: (input?: {
    attackType: SceneAttackType;
    roll: number;
    playthroughSpellId: number | null;
  }) => Promise<void>;
  onReload: () => Promise<void>;
}) => {
  const dialog = useRef<HTMLDialogElement>(null);
  const [attackType, setAttackType] = useState<SceneAttackType>();
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const pending = useRef(false);

  const defender = scene.participants.find(
    (p) => p.id === scene.counterattackerId,
  );

  const target = scene.participants.find(
    (p) => p.id === scene.counterattackTargetId,
  );

  const offensiveDefender = defender
    ? {
        ...defender,
        spells: defender.spells.filter(
          (s) => !s.isSupport && !s.isUtility && s.damageEffect !== null,
        ),
      }
    : undefined;

  useEffect(() => {
    const element = dialog.current;
    element?.showModal();
    return () => element?.close();
  }, []);

  const resolve = async (input?: {
    attackType: SceneAttackType;
    roll: number;
    playthroughSpellId: number | null;
  }) => {
    if (pending.current) return;
    pending.current = true;
    setBusy(true);
    setError("");
    try {
      await onResolve(input);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      pending.current = false;
      setBusy(false);
    }
  };

  return (
    <dialog
      ref={dialog}
      onCancel={(event) => event.preventDefault()}
      aria-labelledby="counterattack-title"
      className="m-auto max-h-[90dvh] w-[min(95vw,48rem)] overflow-y-auto rounded-xl border border-border bg-surface-raised p-6 text-content backdrop:bg-black/70"
    >
      <h2 id="counterattack-title" className="text-2xl font-semibold">
        {defender?.name ?? "Defender"}: counterattack
      </h2>
      <p className="my-4">
        You may attack {target?.name ?? "the original attacker"} once. Normal MP
        costs apply. This does not use your regular turn and cannot trigger
        another counterattack.
      </p>
      {error && (
        <p role="alert" className="my-3 text-danger">
          {error}
        </p>
      )}
      <fieldset disabled={busy}>
        {offensiveDefender &&
          target &&
          (attackType === undefined ? (
            <AttackTypeOptions
              participant={offensiveDefender}
              onSelect={setAttackType}
            />
          ) : (
            <AttackResolutionOptions
              attacker={offensiveDefender}
              targets={[target]}
              attackType={attackType}
              onAttack={(_target, roll, spellId) =>
                resolve({ attackType, roll, playthroughSpellId: spellId })
              }
            />
          ))}
        <div className="mt-5 flex flex-wrap gap-3">
          {attackType !== undefined && (
            <Button onClick={() => setAttackType(undefined)}>Back</Button>
          )}
          <Button onClick={() => void resolve()}>Pass counterattack</Button>
          {error && (
            <Button
              onClick={() =>
                void onReload().catch((e) => setError(getApiError(e).message))
              }
            >
              Reload scene
            </Button>
          )}
        </div>
      </fieldset>
      {busy && <p role="status">Resolving counterattack…</p>}
      {attackAnimation && (
        <AttackAnimationOverlay animation={attackAnimation} inline />
      )}
    </dialog>
  );
};

export default CounterAttackDialog;
