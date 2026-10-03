import { useState } from "react";
import { CharacterType } from "@/features/characters";

interface CharacterThumbnailProps {
  name: string;
  characterType: CharacterType;
  imageUrl?: string | null;
  selected: boolean;
  onSelect: () => void;
}

export function CharacterThumbnail({
  name,
  characterType,
  imageUrl,
  selected,
  onSelect,
}: CharacterThumbnailProps) {
  const [failedUrl, setFailedUrl] = useState<string>();
  const borderColor =
    characterType === CharacterType.Enemy
      ? "border-red-400"
      : characterType === CharacterType.NPC
        ? "border-green-400"
        : "border-blue-400";
  return (
    <button
      type="button"
      aria-label={`Show ${name}`}
      aria-current={selected ? "true" : undefined}
      title={name}
      onClick={onSelect}
      className={`flex h-11 w-11 shrink-0 items-center justify-center overflow-hidden rounded-lg border-2 bg-surface/80 transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility ${borderColor} ${selected ? "ring-2 ring-white/80 ring-offset-2 ring-offset-surface" : "opacity-75 hover:opacity-100"}`}
    >
      {imageUrl && imageUrl !== failedUrl ? (
        <img
          src={imageUrl}
          alt=""
          draggable={false}
          className="h-9 w-9 rounded-md object-cover object-top"
          onError={() => setFailedUrl(imageUrl)}
        />
      ) : (
        <span aria-hidden="true" className="text-sm font-semibold text-content">
          {name.trim().slice(0, 2).toUpperCase() || "?"}
        </span>
      )}
    </button>
  );
}
