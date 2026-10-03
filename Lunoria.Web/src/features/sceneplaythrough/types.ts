import { type ScenePlaythroughInventoryItem } from "@/features/journeys";

export interface AttackAnimationState {
  targetName: string;
  imageUrl: string | null;
}

export const dieDotPositions: Record<number, number[]> = {
  1: [4],
  2: [0, 8],
  3: [0, 4, 8],
  4: [0, 2, 6, 8],
  5: [0, 2, 4, 6, 8],
  6: [0, 2, 3, 5, 6, 8],
};

export interface TradeSelection {
  ownerParticipantId: number;
  inventoryItem: ScenePlaythroughInventoryItem;
}
