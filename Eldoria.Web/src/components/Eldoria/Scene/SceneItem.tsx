import React from "react";
import { Scene } from "../../../models.eldoria/scene";
import BorderBox from "../BorderBox";
import DataRow from "../DataRow";
import { DateTime } from "luxon";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit } from "@fortawesome/free-solid-svg-icons";
import { faRunning } from "@fortawesome/free-solid-svg-icons";
import { faTrash } from "@fortawesome/free-solid-svg-icons";

interface SceneItemProps {
  scene: Scene;
  onClick: (sceneId: number) => void;
}

const SceneItem = ({ scene, onClick }: SceneItemProps) => {
  return (
    <div>
      <BorderBox>
        <DataRow title={"Name:"} value={scene.name} />
        <DataRow title={"GridUrl:"} value={scene.gridUrl} />
        <DataRow
          title={"Created:"}
          value={DateTime.fromISO(scene.createDate).toFormat("MM-dd-yyyy")}
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
            title={"Go To Scene"}
            rightIcon={<FontAwesomeIcon icon={faRunning} color={"white"} />}
            variant={EasyButtonVariant.Teal}
            onClick={() => onClick(scene.id)}
          ></EasyButton>
        </div>
      </BorderBox>
    </div>
  );
};

export default SceneItem;
