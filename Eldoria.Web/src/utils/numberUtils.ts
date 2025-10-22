export const canBeNumber = (value: string): boolean => {
  return !isNaN(Number(value));
};