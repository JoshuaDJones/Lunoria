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
  const [skipping, setSkipping] = useState(false);
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
    setSkipping(!input);
    setError("");
    try {
      await onResolve(input);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      pending.current = false;
      setBusy(false);
      setSkipping(false);
    }
  };

  const footerActions = (
    <>
      {attackType !== undefined && (
        <Button onClick={() => setAttackType(undefined)}>Back</Button>
      )}
      {error && (
        <Button
          onClick={() =>
            void onReload().catch((e) => setError(getApiError(e).message))
          }
        >
          Reload scene
        </Button>
      )}
      <Button
        className="ml-auto min-w-24"
        disabled={busy}
        aria-busy={busy && skipping}
        leftIcon={
          busy && skipping ? (
            <span
              aria-hidden="true"
              className="block h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent motion-reduce:animate-none"
            />
          ) : undefined
        }
        onClick={() => void resolve()}
      >
        Skip
      </Button>
    </>
  );

  return (
    <dialog
      ref={dialog}
      onCancel={(event) => event.preventDefault()}
      onKeyDown={(event) => {
        if (event.key === "Escape") {
          event.preventDefault();
          event.stopPropagation();
        }
      }}
      aria-labelledby="counterattack-title"
      className="m-auto max-h-[90dvh] w-[min(95vw,48rem)] overflow-y-auto rounded-xl border border-border bg-surface-raised p-6 text-content backdrop:bg-black/70"
    >
      <h2 id="counterattack-title" className="mb-5 text-2xl font-semibold">
        {defender?.name ?? "Defender"} counterattacks{" "}
        {target?.name ?? "the original attacker"}
      </h2>
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
              fixedTargetId={target.id}
              attackType={attackType}
              footerActions={footerActions}
              onAttack={(_target, roll, spellId) =>
                resolve({ attackType, roll, playthroughSpellId: spellId })
              }
            />
          ))}
        {(attackType === undefined || !offensiveDefender || !target) && (
          <div className="mt-5 flex flex-wrap items-center gap-3">
            {footerActions}
          </div>
        )}
      </fieldset>
      <span role="status" className="sr-only">
        {busy ? "Processing counterattack" : ""}
      </span>
      {attackAnimation && (
        <AttackAnimationOverlay animation={attackAnimation} inline />
      )}
    </dialog>
  );
};

export default CounterAttackDialog;
