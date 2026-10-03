import { ParticipantType } from "@/features/journeys";

export function getParticipantTypeLabel(type: ParticipantType) {
  switch (type) {
    case ParticipantType.NPC:
      return "NPC";
    case ParticipantType.Enemy:
      return "Enemy";
    default:
      return "Journey character";
  }
}

export function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function formatLootModifier(value: number) {
  return value > 0 ? `+${value}` : String(value);
}
