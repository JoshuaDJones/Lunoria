import { Button, Card } from "@/components/ui";
import { type ScenePlaythroughParticipant } from "@/features/journeys";
import { getParticipantTypeLabel } from "@/features/sceneplaythrough/utils/sceneDisplayUtils";

const ParticipantCard = ({
  participant,
  onShowDetails,
  turnPromptLabel,
  onTurnPrompt,
}: {
  participant: ScenePlaythroughParticipant;
  onShowDetails: () => void;
  turnPromptLabel?: "Begin Turn" | "Select Action";
  onTurnPrompt: () => void;
}) => {
  const imageUrl =
    participant.portraitUrl?.trim() || participant.photoUrl?.trim();
  const isWaitingForTurn =
    participant.isCurrentParticipant && turnPromptLabel !== undefined;

  return (
    <Card
      className={
        participant.isCurrentParticipant
          ? "relative border-utility"
          : "relative"
      }
    >
      <div
        className={`flex min-h-48 transition ${
          isWaitingForTurn ? "pointer-events-none select-none blur-[1.5px]" : ""
        }`}
      >
        <div className="flex w-2/5 shrink-0 items-start justify-center p-2">
          {imageUrl ? (
            <img
              src={imageUrl}
              alt=""
              className="h-auto max-h-56 w-auto rounded-xl object-contain object-top"
            />
          ) : (
            <span className="text-content-muted">No image</span>
          )}
        </div>

        <div className="flex min-w-0 flex-1 flex-col p-4">
          <div>
            <h3 className="pr-6 text-xl font-semibold text-content">
              {participant.name}
            </h3>
            <p className="text-sm text-content-muted">
              {getParticipantTypeLabel(participant.participantType)}
            </p>
            {participant.description && (
              <p className="mt-2 text-sm text-content-secondary">
                {participant.description}
              </p>
            )}
          </div>

          <dl className="mt-4 flex min-w-0 flex-col gap-2 text-sm">
            <div className="flex flex-wrap items-baseline justify-between gap-x-3 gap-y-1 rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">HP</dt>
              <dd className="font-semibold text-content">
                {participant.currentHp} / {participant.maxHp}
              </dd>
            </div>
            <div className="flex flex-wrap items-baseline justify-between gap-x-3 gap-y-1 rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">MP</dt>
              <dd className="font-semibold text-content">
                {participant.currentMp} / {participant.maxMp}
              </dd>
            </div>
            <div className="flex flex-wrap items-baseline justify-between gap-x-3 gap-y-1 rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">Move</dt>
              <dd className="font-semibold text-content">
                {participant.movement}
              </dd>
            </div>
            <div className="flex flex-wrap items-baseline justify-between gap-x-3 gap-y-1 rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">Melee</dt>
              <dd className="font-semibold text-content">
                {participant.meleeAttackDamage ?? "Unavailable"}
              </dd>
            </div>
            {participant.bowAttackDamage !== null && (
              <div className="flex flex-wrap items-baseline justify-between gap-x-3 gap-y-1 rounded-lg bg-surface/75 p-2">
                <dt className="text-content-muted">Bow</dt>
                <dd className="font-semibold text-content">
                  {participant.bowAttackDamage}
                </dd>
              </div>
            )}
          </dl>

          {(participant.isDown ||
            participant.isDead ||
            participant.isInAlternateForm) && (
            <div className="mt-3 flex flex-wrap gap-2 text-xs font-semibold">
              {participant.isDown && (
                <span className="rounded-full border border-border px-3 py-1 text-content-secondary">
                  Down
                  {participant.downedTurnsRemaining !== null
                    ? ` · ${participant.downedTurnsRemaining} scheduled turns`
                    : ""}
                </span>
              )}
              {participant.isDead && (
                <span className="rounded-full border border-danger px-3 py-1 text-danger">
                  Dead
                </span>
              )}
              {participant.isInAlternateForm && (
                <span className="rounded-full border border-border px-3 py-1 text-content-secondary">
                  Alternate form
                </span>
              )}
            </div>
          )}
        </div>
      </div>

      {isWaitingForTurn && (
        <div className="absolute inset-0 flex items-center justify-center bg-canvas/30">
          <Button
            variant="utility"
            size="lg"
            className="animate-[scene-turn-attention_1.5s_ease-in-out_infinite] motion-reduce:animate-none"
            onClick={onTurnPrompt}
          >
            {turnPromptLabel}
          </Button>
        </div>
      )}
      <button
        type="button"
        aria-label={`Show details for ${participant.name}`}
        title="Show details"
        onClick={onShowDetails}
        className="absolute right-2 top-2 z-10 flex h-8 w-8 cursor-pointer items-center justify-center rounded-full border-0 bg-transparent p-0 text-content-muted transition-colors hover:text-content focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility"
      >
        <svg
          aria-hidden="true"
          viewBox="0 0 24 24"
          className="h-5 w-5"
          fill="none"
          stroke="currentColor"
          strokeWidth="1.75"
          strokeLinecap="round"
          strokeLinejoin="round"
        >
          <circle cx="12" cy="12" r="9" />
          <path d="M12 11v6" />
          <path d="M12 7h.01" />
        </svg>
      </button>
    </Card>
  );
};

export default ParticipantCard;
