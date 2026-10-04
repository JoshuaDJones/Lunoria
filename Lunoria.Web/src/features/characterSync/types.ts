import type { Spell } from "@/features/spells/types";

export type CharacterSyncKind = "journey" | "scene";

export interface CharacterRevisionStatus {
  currentRevision: number;
  syncedRevision: number | null;
  acknowledgedRevision: number | null;
  requiresReview: boolean;
  updateAvailable: boolean;
}

export interface CharacterSpellUpdate {
  spellId: number;
  name: string;
  currentRevision: number;
  acknowledgedRevision: number | null;
  isArchived: boolean;
  requiresReview: boolean;
  updateAvailable: boolean;
}

export interface CharacterSyncStatus {
  baseAvailable: boolean;
  stats: CharacterRevisionStatus;
  spellAssignments: CharacterRevisionStatus;
  alternateForm: CharacterRevisionStatus;
  sharedSpells: CharacterSpellUpdate[];
  requiresReview: boolean;
  hasUpdates: boolean;
}

export interface CharacterSyncStats {
  maxHp: number;
  maxMp: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxConsumableInventory: number;
  maxEquippableInventory: number;
}

export interface CharacterSyncPreview {
  assignmentId: number;
  characterId: number;
  status: CharacterSyncStatus;
  assignedStats: CharacterSyncStats;
  baseStats: CharacterSyncStats;
  assignedAlternateFormId: number | null;
  baseAlternateFormId: number | null;
  assignedSpells: Spell[];
  baseSpells: Spell[];
  reviewToken: string;
  sharedSpellNotice: string;
}

export interface CharacterSyncSelection {
  stats: boolean;
  spellAssignments: boolean;
  alternateForm: boolean;
  sharedSpellUpdates: boolean;
}

export interface CharacterSyncRequest extends CharacterSyncSelection {
  expectedReviewToken: string;
}
