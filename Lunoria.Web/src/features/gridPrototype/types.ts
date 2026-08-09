export interface GridPrototypeCharacter {
  id: number;
  name: string;
  imageUrl: string;
  characterType: number;
}

export interface GridPrototypeToken {
  id: string;
  characterId: number;
  name: string;
  imageUrl: string;
  row: number;
  column: number;
}

export interface GridPrototypeSession {
  code: string;
  rows: number;
  columns: number;
  gridColor: string;
  backgroundImage: string | null;
  tokens: GridPrototypeToken[];
}

export interface CreateGridPrototypeSessionResult {
  hostToken: string;
  session: GridPrototypeSession;
}
