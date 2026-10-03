import { ParticipantType } from "@/features/journeys";
import TurnActionButton from "@/features/sceneplaythrough/components/TurnActionButton";

function TurnActionOptions({
  participantType,
  canTransform,
  isInAlternateForm,
  hasUnopenedChests,
  hasConsumables,
  onSelect,
  onForfeit,
}: {
  participantType: ParticipantType;
  canTransform: boolean;
  isInAlternateForm: boolean;
  hasUnopenedChests: boolean;
  hasConsumables: boolean;
  onSelect: (
    title: "Attack" | "Open Chest" | "Use Potion" | "Trade Item" | "Transform",
  ) => void;
  onForfeit: () => void;
}) {
  const canManageItems = participantType === ParticipantType.Player;

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <TurnActionButton
        label="Attack"
        imageSrc="/Attack_Action.png"
        onClick={() => onSelect("Attack")}
      />
      {canTransform && (
        <TurnActionButton
          label={isInAlternateForm ? "Revert" : "Transform"}
          imageSrc="/Transform_Action.png"
          onClick={() => onSelect("Transform")}
        />
      )}
      {hasConsumables && (
        <TurnActionButton
          label="Use Potion"
          imageSrc="/Use_Potion_Action.png"
          onClick={() => onSelect("Use Potion")}
        />
      )}
      {canManageItems && (
        <>
          {hasUnopenedChests && (
            <TurnActionButton
              label="Open Chest"
              imageSrc="/Open_Chest_Action.png"
              onClick={() => onSelect("Open Chest")}
            />
          )}
          <TurnActionButton
            label="Trade Item"
            imageSrc="/Trade_Action.png"
            onClick={() => onSelect("Trade Item")}
          />
        </>
      )}
      <TurnActionButton
        label="Forfeit Action"
        imageSrc="/Forfeit_Action.png"
        onClick={onForfeit}
      />
    </div>
  );
}

export default TurnActionOptions;
