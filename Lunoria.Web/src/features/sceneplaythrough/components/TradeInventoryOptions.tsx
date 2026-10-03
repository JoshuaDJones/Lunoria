import { useRef, useState } from "react";
import { Button } from "@/components/ui";
import {
  type ScenePlaythroughInventoryItem,
  type ScenePlaythroughParticipant,
} from "@/features/journeys";
import { type TradeSelection } from "@/features/sceneplaythrough/types";
import TradeCharacterInventory from "@/features/sceneplaythrough/components/TradeCharacterInventory";

function TradeInventoryOptions({
  participant,
  target,
  onTrade,
}: {
  participant: ScenePlaythroughParticipant;
  target: ScenePlaythroughParticipant;
  onTrade: (item: ScenePlaythroughInventoryItem) => Promise<void>;
}) {
  const [selection, setSelection] = useState<TradeSelection>();
  const [pendingTrade, setPendingTrade] = useState<{
    selection: TradeSelection;
    destinationId: number;
  }>();
  const submitPending = useRef(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const transferTo = async (
    destinationParticipantId: number,
    isEquippable: boolean,
  ) => {
    if (
      !selection ||
      selection.ownerParticipantId === destinationParticipantId ||
      selection.inventoryItem.isEquippable !== isEquippable ||
      isSubmitting
    ) {
      return;
    }

    setPendingTrade({ selection, destinationId: destinationParticipantId });
  };

  const confirmTrade = async () => {
    if (!pendingTrade || submitPending.current) return;
    submitPending.current = true;
    setIsSubmitting(true);
    try {
      await onTrade(pendingTrade.selection.inventoryItem);
    } catch {
      setIsSubmitting(false);
    } finally {
      submitPending.current = false;
    }
  };

  return (
    <div className="w-full max-w-5xl">
      <p className="mb-5 text-sm text-content-secondary">
        Drag one item into the matching inventory on the other side. You can
        also select an item and use the Move Here button. A successful trade
        completes the current turn. Dropping an item only selects the trade;
        press Confirm trade to complete it.
      </p>
      <div className="grid gap-5 lg:grid-cols-2">
        <TradeCharacterInventory
          participant={participant}
          selection={selection}
          disabled={isSubmitting || pendingTrade !== undefined}
          onSelect={setSelection}
          onReceive={(isEquippable) =>
            void transferTo(participant.id, isEquippable)
          }
        />
        <TradeCharacterInventory
          participant={target}
          selection={selection}
          disabled={isSubmitting || pendingTrade !== undefined}
          onSelect={setSelection}
          onReceive={(isEquippable) => void transferTo(target.id, isEquippable)}
        />
      </div>
      {pendingTrade && (
        <div className="mt-5 space-y-3 rounded-xl border border-utility p-4">
          <p className="text-content">
            Trade one {pendingTrade.selection.inventoryItem.item.name} from{" "}
            {pendingTrade.selection.ownerParticipantId === participant.id
              ? participant.name
              : target.name}{" "}
            to{" "}
            {pendingTrade.destinationId === participant.id
              ? participant.name
              : target.name}
            ?
          </p>
          <div className="flex flex-wrap justify-end gap-3">
            <Button
              disabled={isSubmitting}
              onClick={() => setPendingTrade(undefined)}
            >
              Cancel
            </Button>
            <Button
              variant="primary"
              disabled={isSubmitting}
              onClick={() => void confirmTrade()}
            >
              Confirm trade
            </Button>
          </div>
        </div>
      )}
      {isSubmitting && (
        <p className="mt-4 text-center text-sm font-semibold text-content-secondary">
          Trading item...
        </p>
      )}
    </div>
  );
}

export default TradeInventoryOptions;
