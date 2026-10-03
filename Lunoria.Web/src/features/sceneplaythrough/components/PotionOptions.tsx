import { useState } from "react";
import { Button } from "@/components/ui";
import {
  type ScenePlaythroughInventoryItem,
  type ScenePlaythroughParticipant,
} from "@/features/journeys";
import { groupTradeInventoryItems } from "@/features/sceneplaythrough/utils/sceneInventoryUtils";

function PotionOptions({
  participant,
  onUse,
}: {
  participant: ScenePlaythroughParticipant;
  onUse: (item: ScenePlaythroughInventoryItem) => Promise<void>;
}) {
  const potionGroups = groupTradeInventoryItems(
    participant.consumableItems,
    false,
  );
  const [selectedInventoryItemId, setSelectedInventoryItemId] =
    useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const selectedGroup = potionGroups.find(
    ({ inventoryItem }) =>
      inventoryItem.inventoryItemId === selectedInventoryItemId,
  );
  const selectedPotion = selectedGroup?.inventoryItem;
  const hpEffect = selectedPotion?.item.hpEffect ?? 0;
  const mpEffect = selectedPotion?.item.mpEffect ?? 0;
  const hpRestored = Math.min(
    hpEffect,
    Math.max(0, participant.maxHp - participant.currentHp),
  );
  const mpRestored = Math.min(
    mpEffect,
    Math.max(0, participant.maxMp - participant.currentMp),
  );

  if (potionGroups.length === 0) {
    return (
      <p className="text-content-muted">
        This participant has no available consumable items.
      </p>
    );
  }

  return (
    <div>
      <div className="grid gap-4 sm:grid-cols-2">
        {potionGroups.map(({ inventoryItem, quantity }) => {
          const isSelected =
            inventoryItem.inventoryItemId === selectedInventoryItemId;
          const item = inventoryItem.item;

          return (
            <button
              key={inventoryItem.inventoryItemId}
              type="button"
              aria-pressed={isSelected}
              className={`overflow-hidden rounded-xl border-2 bg-surface text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 ${
                isSelected
                  ? "border-utility ring-2 ring-utility/30"
                  : "border-border"
              }`}
              disabled={isSubmitting}
              onClick={() =>
                setSelectedInventoryItemId(inventoryItem.inventoryItemId)
              }
            >
              <div className="relative aspect-[3/2] bg-canvas">
                {item.photoUrl ? (
                  <img
                    src={item.photoUrl}
                    alt=""
                    className="h-full w-full object-contain"
                  />
                ) : (
                  <div className="grid h-full place-items-center text-content-muted">
                    No image
                  </div>
                )}
                {quantity > 1 && (
                  <span className="absolute right-2 top-2 rounded-full bg-utility px-2 py-1 text-xs font-bold text-on-utility shadow-lg">
                    ×{quantity}
                  </span>
                )}
              </div>
              <div className="p-3">
                <p className="font-semibold text-content">{item.name}</p>
                {item.description && (
                  <p className="mt-1 line-clamp-2 text-sm text-content-muted">
                    {item.description}
                  </p>
                )}
                <div className="mt-3 flex flex-wrap gap-2 text-xs">
                  <span className="rounded-full bg-danger/10 px-2 py-1 font-semibold text-danger">
                    HP +{item.hpEffect ?? 0}
                  </span>
                  <span className="rounded-full bg-magic/10 px-2 py-1 font-semibold text-magic-hover">
                    MP +{item.mpEffect ?? 0}
                  </span>
                </div>
              </div>
            </button>
          );
        })}
      </div>

      {selectedPotion && (
        <div className="mt-5 grid grid-cols-2 gap-3 rounded-xl border border-border bg-surface p-4 text-center">
          <div>
            <p className="text-xs text-content-muted">HP after potion</p>
            <p className="mt-1 text-xl font-semibold text-content">
              {participant.currentHp + hpRestored} / {participant.maxHp}
            </p>
          </div>
          <div>
            <p className="text-xs text-content-muted">MP after potion</p>
            <p className="mt-1 text-xl font-semibold text-content">
              {participant.currentMp + mpRestored} / {participant.maxMp}
            </p>
          </div>
        </div>
      )}

      <div className="mt-6 flex justify-end">
        <Button
          variant="magic"
          size="lg"
          disabled={!selectedPotion || isSubmitting}
          onClick={() => {
            if (!selectedPotion) return;
            setIsSubmitting(true);
            void onUse(selectedPotion).finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Using Potion..." : "Use Potion"}
        </Button>
      </div>
    </div>
  );
}

export default PotionOptions;
