import { useState } from "react";
import { Button } from "@/components/ui";
import { dieDotPositions } from "@/features/sceneplaythrough/types";

function MovementRollOptions({
  defaultMovement,
  onContinue,
}: {
  defaultMovement: number;
  onContinue: (roll: number) => Promise<void>;
}) {
  const [selectedRoll, setSelectedRoll] = useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const totalMovement = Math.max(0, defaultMovement + (selectedRoll ?? 0));

  return (
    <div>
      <div className="grid grid-cols-2 gap-4 rounded-xl border border-border bg-surface p-4 text-center">
        <div>
          <p className="text-sm text-content-muted">Default movement</p>
          <p className="mt-1 text-3xl font-semibold text-content">
            {defaultMovement}
          </p>
        </div>
        <div>
          <p className="text-sm text-content-muted">Total movement</p>
          <p className="mt-1 text-3xl font-semibold text-content">
            {totalMovement}
          </p>
        </div>
      </div>

      <p className="mt-6 text-content-secondary">
        Select a die face to add it to the default movement.
      </p>
      <div className="mt-6 grid grid-cols-2 gap-4 sm:grid-cols-3">
        {[1, 2, 3, 4, 5, 6].map((value) => (
          <button
            key={value}
            type="button"
            aria-label={`Select movement roll ${value}`}
            aria-pressed={selectedRoll === value}
            className={`aspect-square cursor-pointer rounded-xl border-2 bg-surface p-5 transition hover:border-utility hover:bg-utility/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/50 ${
              selectedRoll === value
                ? "border-utility bg-utility/10"
                : "border-border"
            }`}
            onClick={() => setSelectedRoll(value)}
          >
            <span className="grid h-full w-full grid-cols-3 grid-rows-3 gap-2">
              {Array.from({ length: 9 }, (_, index) => (
                <span
                  key={index}
                  className={
                    dieDotPositions[value].includes(index)
                      ? "m-auto block h-4 w-4 rounded-full bg-content sm:h-5 sm:w-5"
                      : undefined
                  }
                />
              ))}
            </span>
          </button>
        ))}
      </div>

      <div className="mt-6 flex justify-end">
        <Button
          variant="primary"
          disabled={selectedRoll === undefined || isSubmitting}
          onClick={() => {
            if (selectedRoll === undefined) return;
            setIsSubmitting(true);
            void onContinue(selectedRoll).finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Continuing..." : "Continue"}
        </Button>
      </div>
    </div>
  );
}

export default MovementRollOptions;
