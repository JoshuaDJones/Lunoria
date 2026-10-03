import { type ScenePlaythroughParticipant } from "@/features/journeys";

function TradePartnerOptions({
  participants,
  onSelect,
}: {
  participants: ScenePlaythroughParticipant[];
  onSelect: (participant: ScenePlaythroughParticipant) => void;
}) {
  if (participants.length === 0) {
    return (
      <p className="text-content-muted">
        There are no other active player participants available to trade.
      </p>
    );
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {participants.map((participant) => {
        const imageUrl =
          participant.portraitUrl?.trim() || participant.photoUrl?.trim();

        return (
          <button
            key={participant.id}
            type="button"
            className="group overflow-hidden rounded-xl border-2 border-border bg-surface text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60"
            onClick={() => onSelect(participant)}
          >
            {imageUrl ? (
              <img
                src={imageUrl}
                alt=""
                className="aspect-square w-full object-cover"
              />
            ) : (
              <div className="grid aspect-square w-full place-items-center bg-surface-raised text-4xl font-semibold text-content-muted">
                {participant.name.charAt(0)}
              </div>
            )}
            <span className="block p-3 font-semibold text-content">
              {participant.name}
            </span>
          </button>
        );
      })}
    </div>
  );
}

export default TradePartnerOptions;
