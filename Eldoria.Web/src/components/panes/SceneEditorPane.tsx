import clsx from "clsx";
import { useEffect, useState } from "react";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import { JourneyCharacterDto } from "../../types/journey";
import { SceneCharacterDto, SceneDialogDto } from "../../types/scene";
import PlayersAdjustmentsList from "../lists/PlayersAdjustmentsList";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowCircleRight } from "@fortawesome/free-solid-svg-icons";
import EnemiesAdjustmentsList from "../lists/EnemiesAdjustmentsList";
import EnemyNPCActivationList from "../lists/EnemyNPCActivationList";
import NpcAdjustmentsList from "../lists/NpcAdjustmentsList";
import DashboardDialogList from "../lists/DashboardDialogList";

enum AdjustmentPaneState {
  players,
  npcs,
  enemies,
  activation,
  dialog,
}

interface SceneEditorPaneProps {
  sceneId: number;
  sceneDialogs: SceneDialogDto[]
  players?: JourneyCharacterDto[];
  npcs?: SceneCharacterDto[];
  enemies?: SceneCharacterDto[];
  onRefreshRequest: () => void;
}

const SceneEditorPane = ({
  sceneId,
  sceneDialogs,
  players,
  npcs,
  enemies,
  onRefreshRequest,
}: SceneEditorPaneProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [paneState, setPaneState] = useState(AdjustmentPaneState.players);

  useEffect(() => {
    if (isOpen) onRefreshRequest();
  }, [isOpen]);

  let title = "";
  let characterList;

  if (paneState === AdjustmentPaneState.players) {
    title = "Players";
    characterList = (
      <PlayersAdjustmentsList
        onRefreshRequest={onRefreshRequest}
        players={players}
      />
    );
  } else if (paneState === AdjustmentPaneState.npcs) {
    title = "NPCs";
    characterList = (
      <NpcAdjustmentsList onRefreshRequest={onRefreshRequest} npcs={npcs} />
    );
  } else if (paneState === AdjustmentPaneState.enemies) {
    title = "Enemies";
    characterList = (
      <EnemiesAdjustmentsList
        onRefreshRequest={onRefreshRequest}
        enemies={enemies}
      />
    );
  } else if (paneState === AdjustmentPaneState.dialog) {
    title = "Dialog";
    characterList = (<DashboardDialogList sceneDialogs={sceneDialogs} />)
  } else {
    title = "Activation";
    characterList = (
      <EnemyNPCActivationList
        onRefreshRequest={onRefreshRequest}
        sceneId={sceneId}
      />
    );
  }

  return (
    <div
      className={clsx(
        "flex absolute top-0 bottom-0 right-0 h-full w-[400px] z-20 transform transition-transform duration-500 ease-in-out",
        {
          "-translate-x-0": isOpen,
          "translate-x-[350px]": !isOpen,
        },
      )}
    >
      <div className="w-[50px] h-full flex flex-col justify-end font-cinzel gap-1 pb-20">
        {isOpen && (
          <button
            className="hover:opacity-50 mb-3"
            onClick={() => setIsOpen(false)}
          >
            <FontAwesomeIcon
              icon={faArrowCircleRight}
              size={"2xl"}
              className="text-slate-200"
            />
          </button>
        )}
        <PaneOptionOption
          title={"Players"}
          isPaneOpen={isOpen}
          isSelected={paneState === AdjustmentPaneState.players}
          type={OptionType.players}
          onClick={() => {
            setPaneState(AdjustmentPaneState.players);
            setIsOpen(true);
          }}
        />
        <PaneOptionOption
          title={"NPCs"}
          isPaneOpen={isOpen}
          isSelected={paneState === AdjustmentPaneState.npcs}
          type={OptionType.npcs}
          onClick={() => {
            setPaneState(AdjustmentPaneState.npcs);
            setIsOpen(true);
          }}
        />
        <PaneOptionOption
          title={"Enemies"}
          isPaneOpen={isOpen}
          isSelected={paneState === AdjustmentPaneState.enemies}
          type={OptionType.enemies}
          onClick={() => {
            setPaneState(AdjustmentPaneState.enemies);
            setIsOpen(true);
          }}
        />
        <PaneOptionOption
          title={"Activate"}
          isPaneOpen={isOpen}
          isSelected={paneState === AdjustmentPaneState.activation}
          type={OptionType.activation}
          onClick={() => {
            setPaneState(AdjustmentPaneState.activation);
            setIsOpen(true);
          }}
        />
        <PaneOptionOption
          title={"Dialog"}
          isPaneOpen={isOpen}
          isSelected={paneState === AdjustmentPaneState.dialog}
          type={OptionType.dialog}
          onClick={() => {
            setPaneState(AdjustmentPaneState.dialog);
            setIsOpen(true);
          }}
        />
      </div>
      <div className="bg-stone-800/90 rounded-l-xl flex flex-col flex-1 h-full w-full py-5">
        <Title
          className="self-center text-4xl"
          size={TitleSize.custom}
          color={TitleColor.white}
        >
          {title}
        </Title>

        {paneState !== AdjustmentPaneState.activation && (
          <div className="flex flex-1 py-2 overflow-y-auto scrollbar-hide">
            {characterList}
          </div>
        )}

        {paneState === AdjustmentPaneState.activation && characterList}
      </div>
    </div>
  );
};

enum OptionType {
  players,
  enemies,
  npcs,
  activation,
  dialog,
}

interface PaneOptionProps {
  title: string;
  isPaneOpen: boolean;
  isSelected: boolean;
  type: OptionType;
  onClick: () => void;
}

const PaneOptionOption = ({
  title,
  isPaneOpen,
  isSelected,
  type,
  onClick,
}: PaneOptionProps) => {
  return (
    <button
      className={clsx(
        "relative flex items-center justify-center rounded-l-2xl text-white w-[50px] h-[125px] text-xl",
        {
          "border border-white font-bold": isPaneOpen && isSelected,
          "bg-orange-900/90": type === OptionType.activation,
          "bg-blue-900/90": type === OptionType.players,
          "bg-green-900/90": type === OptionType.npcs,
          "bg-red-900/90": type === OptionType.enemies,
          "bg-purple-900/90": type === OptionType.dialog,
        },
      )}
      onClick={onClick}
    >
      <span className="rotate-90">{title}</span>
    </button>
  );
};

export default SceneEditorPane;
