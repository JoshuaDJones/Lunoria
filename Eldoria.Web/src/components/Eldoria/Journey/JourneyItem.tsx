import { Journey } from "../../../models.eldoria/journey";
import BorderBox from "../BorderBox";
import DataRow from "../DataRow";
import { DateTime } from "luxon";
import { faEdit } from "@fortawesome/free-solid-svg-icons";
import { faRunning } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrash } from "@fortawesome/free-solid-svg-icons";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";

interface JourneyItemProps {
  journey: Journey;
  onClick: (id: number) => void;
}

export const JourneyItem = ({ journey, onClick }: JourneyItemProps) => {
  return (
    <div>
      <BorderBox>
        <DataRow title={"Name:"} value={journey.name} />
        <DataRow title={"Description:"} value={journey.description} />
        <DataRow
          title={"Created:"}
          value={DateTime.fromISO(journey.createDate).toFormat("MM-dd-yyyy")}
        />

        <div className="flex-row flex gap-2 mt-5">
          <EasyButton
            className="gap-2"
            title={"Edit"}
            rightIcon={<FontAwesomeIcon icon={faEdit} color={"white"} />}
            variant={EasyButtonVariant.Primary}
          ></EasyButton>

          <EasyButton
            className="gap-2"
            title={"Delete"}
            rightIcon={<FontAwesomeIcon icon={faTrash} color={"white"} />}
            variant={EasyButtonVariant.Secondary}
          ></EasyButton>

          <EasyButton
            className="gap-2"
            title={"Go To Scenes"}
            rightIcon={<FontAwesomeIcon icon={faRunning} color={"white"} />}
            variant={EasyButtonVariant.Teal}
            onClick={() => onClick(journey.id)}
          ></EasyButton>
        </div>
      </BorderBox>
    </div>
  );
};
