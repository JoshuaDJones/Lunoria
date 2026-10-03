import {
  ParticipantType,
  type ScenePlaythroughInventoryItem,
  type ScenePlaythroughParticipant,
} from "@/features/journeys";

export function groupTradeInventoryItems(
  items: ScenePlaythroughInventoryItem[],
  isEquippable: boolean,
) {
  if (isEquippable) {
    return items.map((inventoryItem) => ({ inventoryItem, quantity: 1 }));
  }

  const groups = new Map<
    number,
    { inventoryItem: ScenePlaythroughInventoryItem; quantity: number }
  >();

  for (const inventoryItem of items) {
    const existing = groups.get(inventoryItem.item.id);
    if (existing) {
      existing.quantity += 1;
    } else {
      groups.set(inventoryItem.item.id, { inventoryItem, quantity: 1 });
    }
  }

  return Array.from(groups.values());
}

export function getEligibleTradePartners(
  participant: ScenePlaythroughParticipant,
  participants: ScenePlaythroughParticipant[],
) {
  return participants.filter(
    (candidate) =>
      candidate.id !== participant.id &&
      candidate.participantType === ParticipantType.Player &&
      candidate.isActive &&
      !candidate.isDead,
  );
}
