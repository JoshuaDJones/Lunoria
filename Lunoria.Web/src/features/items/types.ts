export interface Item {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  hpEffect: number;
  mpEffect: number;
  createDate: string;
}

export interface ItemInput {
  name: string;
  description: string;
  photo?: File;
  hpEffect: number;
  mpEffect: number;
}

export type CreateItemInput = ItemInput & { photo: File };
