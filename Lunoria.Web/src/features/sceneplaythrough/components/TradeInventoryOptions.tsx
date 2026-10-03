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
  onSubmittingChange,
}: {
  participant: ScenePlaythroughParticipant;
  target: ScenePlaythroughParticipant;
  onTrade: (item: ScenePlaythroughInventoryItem) => Promise<void>;
  onSubmittingChange: (isSubmitting: boolean) => void;
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
      isSubmitting ||
      pendingTrade
    ) {
      return;
    }

    const destination =
      destinationParticipantId === participant.id ? participant : target;
    const items = isEquippable
      ? destination.equippableItems
      : destination.consumableItems;
    const limit = isEquippable
      ? destination.maxEquippableInventory
      : destination.maxConsumableInventory;
    if (items.length >= limit) return;

    setPendingTrade({ selection, destinationId: destinationParticipantId });
  };

  const previewInventory = (character: ScenePlaythroughParticipant) => {
    if (!pendingTrade) return character;
    const { selection: pendingSelection, destinationId } = pendingTrade;
    const { inventoryItem, ownerParticipantId } = pendingSelection;
    const inventoryKey = inventoryItem.isEquippable
      ? "equippableItems"
      : "consumableItems";
    const items = character[inventoryKey];
    return {
      ...character,
      [inventoryKey]:
        character.id === ownerParticipantId
          ? items.filter(
              (item) => item.inventoryItemId !== inventoryItem.inventoryItemId,
            )
          : character.id === destinationId
            ? [...items, inventoryItem]
            : items,
    };
  };

  const confirmTrade = async () => {
    if (!pendingTrade || submitPending.current) return;
    submitPending.current = true;
    setIsSubmitting(true);
    onSubmittingChange(true);
    try {
      await onTrade(pendingTrade.selection.inventoryItem);
    } catch {
      setPendingTrade(undefined);
      setSelection(undefined);
      setIsSubmitting(false);
    } finally {
      submitPending.current = false;
      onSubmittingChange(false);
    }
  };

  return (
    <div className="w-full max-w-5xl">
      <p className="mb-5 text-sm text-content-secondary">
        Drag one item into the matching inventory on the other side. A
        successful trade completes the current turn. Dropping an item previews
        the transfer; press Confirm trade to save it, or Cancel to restore the
        inventories.
      </p>
      <div className="grid gap-5 lg:grid-cols-2">
        <TradeCharacterInventory
          participant={previewInventory(participant)}
          selection={selection}
          disabled={isSubmitting || pendingTrade !== undefined}
          onSelect={setSelection}
          onReceive={(isEquippable) =>
            void transferTo(participant.id, isEquippable)
          }
        />
        <TradeCharacterInventory
          participant={previewInventory(target)}
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
              onClick={() => {
                setPendingTrade(undefined);
                setSelection(undefined);
              }}
            >
              Cancel
            </Button>
            <Button
              variant="primary"
              disabled={isSubmitting}
              aria-busy={isSubmitting}
              leftIcon={
                isSubmitting ? (
                  <span
                    aria-hidden="true"
                    className="block h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent motion-reduce:animate-none"
                  />
                ) : undefined
              }
              onClick={() => void confirmTrade()}
            >
              Confirm trade
            </Button>
          </div>
        </div>
      )}
      <span role="status" className="sr-only">
        {isSubmitting ? "Saving trade…" : ""}
      </span>
    </div>
  );
}

export default TradeInventoryOptions;
