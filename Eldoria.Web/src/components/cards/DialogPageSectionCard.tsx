import Text, { TextColor, TextSize } from "../typography/Text";
import { DialogPageSectionDto } from "../../types/scene";
import clsx from "clsx";

interface DialogPageSectionCardProps {
  dialogPageSection: DialogPageSectionDto;
  selectedPageSectionId?: number;
  onSelect: (pageSectionId: number) => void;
}

const DialogPageSectionCard = ({
  dialogPageSection,
  selectedPageSectionId,
  onSelect,
}: DialogPageSectionCardProps) => {
  const isSelected = dialogPageSection.id === selectedPageSectionId;

  return (
    <button
      onClick={() => onSelect(dialogPageSection.id)}
      className={clsx(
        "flex w-full flex-col rounded-lg px-5 py-3 bg-stone-900/75 mt-3",
        {
          "outline outline-blue-400": isSelected,
        },
      )}
    >
      <Text
        className="self-start"
        textColor={TextColor.white}
        size={TextSize.xl}
      >
        {dialogPageSection.character?.name ?? "Narrator"}
      </Text>

      <div className="flex items-center justify gap-2 mt-2">
        <Text textColor={TextColor.white} size={TextSize.lg}>
          Order:{" "}
        </Text>
        <Text textColor={TextColor.white}>{dialogPageSection.orderNum}</Text>
      </div>

      <Text
        className="self-start mt-2"
        textColor={TextColor.white}
        size={TextSize.lg}
      >
        Reading Prompt
      </Text>
      <div className="bg-stone-600 p-2 rounded-lg flex">
        <Text className="text-start" textColor={TextColor.white}>
          {dialogPageSection.readingText}
        </Text>
      </div>
    </button>
  );
};

export default DialogPageSectionCard;
