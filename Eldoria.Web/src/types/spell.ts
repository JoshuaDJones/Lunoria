export interface SpellDto {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  range: number;
  isRadius: boolean;
  mpCost: number;
  damageEffect: number | null;
  healthEffect: number | null;
  magicEffect: number | null;
  createDate: string;
}
