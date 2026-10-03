import { type ScenePlaythroughParticipant } from "@/features/journeys";
import { type TradeSelection } from "@/features/sceneplaythrough/types";
import TradeInventorySlots from "@/features/sceneplaythrough/components/TradeInventorySlots";

function TradeCharacterInventory({
  participant,
  selection,
  disabled,
  onSelect,
  onReceive,
}: {
  participant: ScenePlaythroughParticipant;
  selection: TradeSelection | undefined;
  disabled: boolean;
  onSelect: (selection: TradeSelection | undefined) => void;
  onReceive: (isEquippable: boolean) => void;
}) {
  const imageUrl =
    participant.portraitUrl?.trim() || participant.photoUrl?.trim();

  return (
    <section className="rounded-2xl border border-border bg-surface p-4">
      <header className="mb-4 flex items-center gap-3">
        {imageUrl ? (
          <img
            src={imageUrl}
            alt=""
            className="h-14 w-14 rounded-xl object-cover"
          />
        ) : (
          <div className="grid h-14 w-14 place-items-center rounded-xl bg-surface-raised text-xl font-semibold text-content-muted">
            {participant.name.charAt(0)}
          </div>
        )}
        <div>
          <h3 className="text-xl font-semibold text-content">
            {participant.name}
          </h3>
          <p className="text-xs text-content-muted">Player inventory</p>
        </div>
      </header>

      <div className="space-y-5">
        <TradeInventorySlots
          title="Consumables"
          participantId={participant.id}
          items={participant.consumableItems ?? []}
          limit={participant.maxConsumableInventory}
          isEquippable={false}
          selection={selection}
          disabled={disabled}
          onSelect={onSelect}
          onReceive={onReceive}
        />
        <TradeInventorySlots
          title="Equippable Items"
          participantId={participant.id}
          items={participant.equippableItems ?? []}
          limit={participant.maxEquippableInventory}
          isEquippable
          selection={selection}
          disabled={disabled}
          onSelect={onSelect}
          onReceive={onReceive}
        />
      </div>
    </section>
  );
}

export default TradeCharacterInventory;
