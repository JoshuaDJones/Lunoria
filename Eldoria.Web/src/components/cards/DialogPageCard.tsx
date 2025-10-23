import { DialogPageDto } from "../../types/scene";
import Text, { TextColor, TextSize } from "../typography/Text";
import clsx from "clsx";

interface DialogPageCardProps {
  dialogPage: DialogPageDto;
  selectedDialogPageId?: number;
  onSelect: (dialogPageId: number) => void;
}

const DialogPageCard = ({
  dialogPage,
  selectedDialogPageId,
  onSelect,
}: DialogPageCardProps) => {
  const isSelected = dialogPage.id === selectedDialogPageId;

  return (
    <button
      onClick={() => onSelect(dialogPage.id)}
      className={clsx("flex w-full rounded-lg px-5 py-3 bg-stone-900/75 mt-3", {
        "outline outline-blue-400": isSelected,
      })}
    >
      <img src={dialogPage.photoUrl} className="h-[100px]" />
      <div className="justify-center items-end flex flex-col flex-1">
        <Text className="" size={TextSize.lg} textColor={TextColor.white}>
          Order: {dialogPage.orderNum}
        </Text>
        <Text className="" size={TextSize.lg} textColor={TextColor.white}>
          Sections: {dialogPage.dialogPageSections.length}
        </Text>
      </div>
    </button>
  );
};

export default DialogPageCard;
