export const canBeNumber = (value: string): boolean => {
  if (value.trim() === "") return false;
  return !isNaN(Number(value));
};
