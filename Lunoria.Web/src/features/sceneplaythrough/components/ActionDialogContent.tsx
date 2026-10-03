import { Button } from "@/components/ui";
import { type ScenePlaythroughCharacterOption } from "@/features/journeys";

const ActionDialogContent = ({
  playthroughCharacters,
  onSkip,
}: {
  playthroughCharacters: ScenePlaythroughCharacterOption[];
  onSkip: () => void;
}) => {
  return (
    <div>
      <Button className="mb-4" onClick={onSkip}>
        Skip Activation {"\u2192"}
      </Button>
      {playthroughCharacters.map((character) => (
        <div
          className="flex items-center gap-3 rounded-xl border border-border bg-surface p-3 mb-2"
          key={character.id}
        >
          {character.photoUrl ? (
            <img
              src={character.photoUrl}
              alt=""
              className="h-12 w-12 rounded-lg object-contain"
            />
          ) : (
            <div className="h-12 w-12 rounded-lg bg-surface-raised" />
          )}
          <p className="min-w-0 flex-1 truncate font-semibold text-content">
            {character.name}
          </p>
          <Button>Activate</Button>
        </div>
      ))}
    </div>
  );
};

export default ActionDialogContent;
