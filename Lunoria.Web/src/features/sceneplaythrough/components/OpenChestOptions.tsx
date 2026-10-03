import { useEffect, useRef, useState } from "react";
import {
  type SceneOpenChestResult,
  type ScenePlaythroughChest,
} from "@/features/journeys";
import DieFaceButton from "@/features/sceneplaythrough/components/DieFaceButton";
import ChestLootResult from "@/features/sceneplaythrough/components/ChestLootResult";

function OpenChestOptions({
  chests,
  onOpen,
  onComplete,
}: {
  chests: ScenePlaythroughChest[];
  onOpen: (chestId: number, roll: number) => Promise<SceneOpenChestResult>;
  onComplete: (result: SceneOpenChestResult) => void;
}) {
  const [selectedChestId, setSelectedChestId] = useState<number | undefined>(
    chests.length === 1 ? chests[0].id : undefined,
  );
  const [selectedRoll, setSelectedRoll] = useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [resolvedLoot, setResolvedLoot] = useState<SceneOpenChestResult>();
  const chestSoundRef = useRef<HTMLAudioElement | null>(null);
  const selectedChest = chests.find((chest) => chest.id === selectedChestId);

  useEffect(() => {
    const sound = new Audio(
      "/sounds/Treasure_Chest_Magical_Glittering_Gold.wav",
    );
    sound.volume = 0.75;
    sound.loop = true;
    chestSoundRef.current = sound;
    void sound.play().catch(() => {
      // Browsers may block audio if playback permission has not been granted.
    });

    return () => {
      sound.pause();
      sound.currentTime = 0;
      chestSoundRef.current = null;
    };
  }, []);

  const selectRoll = async (roll: number) => {
    if (
      selectedChest === undefined ||
      roll > selectedChest.dieSides ||
      isSubmitting
    ) {
      return;
    }

    setSelectedRoll(roll);
    setIsSubmitting(true);
    chestSoundRef.current?.pause();
    if (chestSoundRef.current) chestSoundRef.current.currentTime = 0;

    try {
      const result = await onOpen(selectedChest.id, roll);
      setResolvedLoot(result);
      setIsSubmitting(false);
    } catch {
      setSelectedRoll(undefined);
      setIsSubmitting(false);
    }
  };

  if (resolvedLoot) {
    return (
      <ChestLootResult
        result={resolvedLoot}
        onContinue={() => onComplete(resolvedLoot)}
      />
    );
  }

  return (
    <div>
      <div className="mx-auto aspect-square w-full max-w-sm overflow-hidden rounded-2xl border-2 border-utility/60 bg-canvas shadow-xl shadow-utility/10">
        <img
          src="/Opening_Chest.png"
          alt="An open treasure chest"
          className="h-full w-full object-contain"
          onError={(event) => {
            event.currentTarget.onerror = null;
            event.currentTarget.src = "/Open_Chest_Action.png";
          }}
        />
      </div>

      <section className="mt-6">
        <h3 className="text-lg font-semibold text-content">
          {chests.length === 1 ? "Chest" : "Select a chest"}
        </h3>
        <div className="mt-3 grid gap-3 sm:grid-cols-2">
          {chests.map((chest) => {
            const isSelected = selectedChestId === chest.id;

            return (
              <button
                key={chest.id}
                type="button"
                disabled={isSubmitting}
                aria-pressed={isSelected}
                className={`cursor-pointer rounded-xl border-2 bg-surface p-4 text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 disabled:cursor-not-allowed disabled:opacity-50 ${
                  isSelected ? "border-utility" : "border-border"
                }`}
                onClick={() => {
                  setSelectedChestId(chest.id);
                  setSelectedRoll(undefined);
                }}
              >
                <span className="block font-semibold text-content">
                  {chest.name}
                </span>
                <span className="mt-1 block text-sm text-content-muted">
                  Roll a d{chest.dieSides} for loot
                </span>
              </button>
            );
          })}
        </div>
      </section>

      <section className="mt-6">
        <h3 className="text-lg font-semibold text-content">
          Select a die face
        </h3>
        <div className="mt-3 grid grid-cols-3 gap-3 sm:grid-cols-6">
          {[1, 2, 3, 4, 5, 6].map((value) => {
            const isUnavailable =
              selectedChest !== undefined && value > selectedChest.dieSides;

            return (
              <DieFaceButton
                key={value}
                value={value}
                selected={selectedRoll === value}
                disabled={
                  selectedChest === undefined || isUnavailable || isSubmitting
                }
                ariaLabel={`Select chest roll ${value}`}
                onClick={() => void selectRoll(value)}
                compact
              />
            );
          })}
        </div>
      </section>
      {isSubmitting && !resolvedLoot && (
        <p className="mt-5 text-center text-sm text-content-muted">
          Opening chest...
        </p>
      )}
    </div>
  );
}

export default OpenChestOptions;
