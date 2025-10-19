import { JourneyCharacterDto } from "../../types/journey";
import JourneyCharacterAdjustmentCard from "../cards/JourneyCharacterAdjustCard";

interface PlayersAdjustmentsListProps {
  players?: JourneyCharacterDto[];
  onRefreshRequest: () => void;
}

const PlayersAdjustmentsList = ({
  players,
  onRefreshRequest,
}: PlayersAdjustmentsListProps) => {
  return (
    <div className="flex-1 flex flex-col px-5 gap-2">
      {players?.map((p) => (
        <JourneyCharacterAdjustmentCard
          journeyCharacter={p}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

export default PlayersAdjustmentsList;
