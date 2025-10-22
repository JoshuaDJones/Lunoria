import Text, { TextColor, TextSize } from "../typography/Text";
import { SceneDialogDto } from "../../types/scene";
import clsx from "clsx";

interface SceneDialogCardProps {
  sceneDialog: SceneDialogDto;
  isSelected: boolean;
  onSelect: (id: number) => void;
}

const SceneDialogCard = ({
  sceneDialog,
  isSelected,
  onSelect,
}: SceneDialogCardProps) => {
  return (
    <button
      onClick={() => onSelect(sceneDialog.id)}
      className={clsx("flex w-full rounded-lg px-5 py-3 bg-stone-900/75 mt-3", {
        "outline outline-blue-400": isSelected,
      })}
    >
      <Text
        size={TextSize.lg}
        textColor={TextColor.white}
        className="flex-1 flex"
      >
        {sceneDialog.title}
      </Text>
      <Text size={TextSize.lg} textColor={TextColor.white}>
        Pages: {sceneDialog.dialogPages.length}
      </Text>
    </button>
  );
};

export default SceneDialogCard;
