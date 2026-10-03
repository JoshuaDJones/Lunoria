import { Button } from "@/components/ui";
import { type ScenePlaythroughInventoryItem } from "@/features/journeys";
import { type TradeSelection } from "@/features/sceneplaythrough/types";
import { groupTradeInventoryItems } from "@/features/sceneplaythrough/utils/sceneInventoryUtils";

function TradeInventorySlots({
  title,
  participantId,
  items,
  limit,
  isEquippable,
  selection,
  disabled,
  onSelect,
  onReceive,
}: {
  title: string;
  participantId: number;
  items: ScenePlaythroughInventoryItem[];
  limit: number;
  isEquippable: boolean;
  selection: TradeSelection | undefined;
  disabled: boolean;
  onSelect: (selection: TradeSelection) => void;
  onReceive: (isEquippable: boolean) => void;
}) {
  const itemGroups = groupTradeInventoryItems(items, isEquippable);
  const freeSlotCount = Math.max(0, limit - items.length);
  const displayedSlotCount = itemGroups.length + freeSlotCount;
  const canReceive = Boolean(
    selection &&
    selection.ownerParticipantId !== participantId &&
    selection.inventoryItem.isEquippable === isEquippable &&
    !disabled,
  );

  return (
    <section
      className={`rounded-xl border p-3 transition ${
        canReceive
          ? "border-utility bg-utility/5"
          : "border-border bg-canvas/35"
      }`}
      onDragOver={(event) => {
        if (canReceive) event.preventDefault();
      }}
      onDrop={(event) => {
        event.preventDefault();
        if (canReceive) onReceive(isEquippable);
      }}
    >
      <div className="mb-3 flex items-center justify-between gap-3">
        <h4 className="font-semibold text-content">{title}</h4>
        <span className="text-xs text-content-muted">
          {items.length} / {limit}
        </span>
      </div>

      {displayedSlotCount === 0 ? (
        <p className="rounded-lg border border-dashed border-border p-3 text-center text-xs text-content-muted">
          No inventory slots
        </p>
      ) : (
        <div className="grid grid-cols-2 gap-2 sm:grid-cols-3">
          {Array.from({ length: displayedSlotCount }, (_, index) => {
            const itemGroup = itemGroups[index];
            if (!itemGroup) {
              return (
                <div
                  key={`empty-${index}`}
                  className={`grid aspect-square place-items-center rounded-lg border border-dashed text-center text-xs ${
                    canReceive
                      ? "border-utility text-utility-hover"
                      : "border-border text-content-muted"
                  }`}
                >
                  Empty slot
                </div>
              );
            }

            const { inventoryItem, quantity } = itemGroup;

            const isSelected =
              selection?.ownerParticipantId === participantId &&
              selection.inventoryItem.inventoryItemId ===
                inventoryItem.inventoryItemId &&
              selection.inventoryItem.isEquippable ===
                inventoryItem.isEquippable;
            return (
              <button
                key={`${inventoryItem.isEquippable ? "equipment" : "consumable"}-${inventoryItem.inventoryItemId}`}
                type="button"
                draggable={!disabled}
                aria-pressed={isSelected}
                title={`Trade ${inventoryItem.item.name}`}
                className={`relative aspect-square overflow-hidden rounded-lg border text-left transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 ${
                  isSelected
                    ? "border-utility ring-2 ring-utility/40"
                    : "border-border"
                } cursor-grab hover:border-utility active:cursor-grabbing`}
                onClick={() => {
                  if (!disabled) {
                    onSelect({
                      ownerParticipantId: participantId,
                      inventoryItem,
                    });
                  }
                }}
                onDragStart={(event) => {
                  if (disabled) {
                    event.preventDefault();
                    return;
                  }
                  event.dataTransfer.effectAllowed = "move";
                  event.dataTransfer.setData(
                    "text/plain",
                    String(inventoryItem.inventoryItemId),
                  );
                  onSelect({
                    ownerParticipantId: participantId,
                    inventoryItem,
                  });
                }}
              >
                {inventoryItem.item.photoUrl ? (
                  <img
                    src={inventoryItem.item.photoUrl}
                    alt=""
                    className="h-full w-full object-cover"
                  />
                ) : (
                  <div className="h-full w-full bg-surface-raised" />
                )}
                {quantity > 1 && (
                  <span className="absolute right-1.5 top-1.5 rounded-full bg-utility px-2 py-0.5 text-xs font-bold text-on-utility shadow-lg">
                    ×{quantity}
                  </span>
                )}
                <span className="absolute inset-x-0 bottom-0 bg-canvas/90 px-2 py-1 text-xs font-semibold text-content backdrop-blur-sm">
                  {inventoryItem.item.name}
                </span>
              </button>
            );
          })}
        </div>
      )}

      {canReceive && (
        <Button
          size="sm"
          variant="utility"
          className="mt-3 w-full"
          disabled={items.length >= limit}
          onClick={() => onReceive(isEquippable)}
        >
          {items.length >= limit ? "Inventory Full" : "Move Here"}
        </Button>
      )}
    </section>
  );
}

export default TradeInventorySlots;
