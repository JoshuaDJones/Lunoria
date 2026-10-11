export type CharacterSyncSummaryReason =
  | "stats"
  | "spellAssignments"
  | "alternateForm"
  | "sharedSpells"
  | "baseCharacter";

export interface CharacterSyncSummaryEntry {
  assignmentId: number;
  characterId: number;
  name: string;
  status: "updatesAvailable" | "reviewNeeded" | "baseUnavailable";
  reasons: CharacterSyncSummaryReason[];
  reviewReasons: CharacterSyncSummaryReason[];
}

export interface JourneyCharacterSyncSummary {
  journeyId: number;
  players: CharacterSyncSummaryEntry[];
  scenes: { sceneId: number; characters: CharacterSyncSummaryEntry[] }[];
}
