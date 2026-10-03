import { useState } from "react";
import { InfoTooltip } from "@/components/ui/InfoTooltip";
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
  onSelect: (selection: TradeSelection | undefined) => void;
  onReceive: (isEquippable: boolean) => void;
}) {
  const [hoveredSelection, setHoveredSelection] = useState<TradeSelection>();
  const itemGroups = groupTradeInventoryItems(items, isEquippable);
  const freeSlotCount = Math.max(0, limit - items.length);
  const displayedSlotCount = itemGroups.length + freeSlotCount;
  const canReceive = Boolean(
    selection &&
    selection.ownerParticipantId !== participantId &&
    selection.inventoryItem.isEquippable === isEquippable &&
    !disabled &&
    items.length < limit,
  );
  const isDropTargetHovered = canReceive && hoveredSelection === selection;

  return (
    <section
      className={`rounded-xl border p-3 transition ${
        isDropTargetHovered
          ? "border-utility bg-utility/5"
          : "border-border bg-canvas/35"
      }`}
      onDragOver={(event) => {
        if (canReceive) {
          event.preventDefault();
          event.dataTransfer.dropEffect = "move";
          setHoveredSelection(selection);
        }
      }}
      onDragLeave={(event) => {
        if (
          event.relatedTarget instanceof Node &&
          event.currentTarget.contains(event.relatedTarget)
        )
          return;
        setHoveredSelection(undefined);
      }}
      onDrop={(event) => {
        event.preventDefault();
        setHoveredSelection(undefined);
        if (canReceive) onReceive(isEquippable);
        onSelect(undefined);
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
                    isDropTargetHovered
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
              <div
                key={`${inventoryItem.isEquippable ? "equipment" : "consumable"}-${inventoryItem.inventoryItemId}`}
                className="relative aspect-square min-w-0"
              >
                <button
                  type="button"
                  draggable={!disabled}
                  aria-pressed={isSelected}
                  aria-label={`Trade ${inventoryItem.item.name}`}
                  aria-disabled={disabled}
                  className={`relative h-full w-full overflow-hidden rounded-lg border text-left transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 ${
                    isSelected
                      ? "border-utility ring-2 ring-utility/40"
                      : "border-border"
                  } cursor-grab hover:border-utility active:cursor-grabbing`}
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
                  onDragEnd={() => onSelect(undefined)}
                >
                  {inventoryItem.item.photoUrl ? (
                    <img
                      src={inventoryItem.item.photoUrl}
                      draggable={false}
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
                </button>
                <div className="absolute bottom-1.5 right-1.5">
                  <InfoTooltip text={inventoryItem.item.name} />
                </div>
              </div>
            );
          })}
        </div>
      )}
    </section>
  );
}

export default TradeInventorySlots;
