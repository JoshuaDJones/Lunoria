import Text, { TextColor, TextSize } from "../typography/Text";
import { DialogPageSectionDto } from "../../types/scene";
import clsx from "clsx";

interface ViewDialogSectionProps {
  pageSection: DialogPageSectionDto;
}

const ViewDialogSection = ({ pageSection }: ViewDialogSectionProps) => {
  const isNarrator = !pageSection.character;  

  return (
    <div className={clsx("border-4 relative flex flex-col rounded-xl w-[850px] bg-black/60",
      {
        "border-sky-600": !isNarrator,
        "border-stone-500": isNarrator
      }
    )}>
      <div className="flex m-2 items-center gap-3">
        {!isNarrator && (
          <img className="h-[40px] rounded-lg" src={pageSection.character.photoUrl} />
        )}
        <Text textColor={TextColor.white}>{isNarrator ? "Narrator" : pageSection.character.name}</Text>
      </div>
      
      <div className="pb-5 px-5 pt-2">
        <Text size={TextSize.xl} textColor={TextColor.white}>{pageSection.readingText}</Text>
      </div>
    </div>
  );
};

export default ViewDialogSection;
