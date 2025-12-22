import { JourneyCharacterDto } from "../../types/journey";
import DashboardPlayerCard from "../cards/DashboardPlayerCard";

interface DashboardPlayerListProps {
  players?: JourneyCharacterDto[];
  onRefreshRequest: () => void;
}

const DashboardPlayerList = ({
  players,
  onRefreshRequest,
}: DashboardPlayerListProps) => {
  return (
    <div className="flex flex-wrap gap-4 items-start mt-3">
      {players?.map((p) => (
        <DashboardPlayerCard player={p} onRefreshRequest={onRefreshRequest} />
      ))}
    </div>
  );
};

export default DashboardPlayerList;
