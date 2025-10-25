import { PropsWithChildren } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";
import CloseIconButton from "../buttons/CloseIconButton";
import SaveIconButton from "../buttons/SaveIconButton";
import clsx from "clsx";

interface EditDialogItemContainerProps {
  title: string;
  onCloseClick: () => void;
  onSaveClick: () => void;
  className?: string;
}

const EditDialogItemContainer = ({
  title,
  onCloseClick,
  onSaveClick,
  className,
  children,
}: PropsWithChildren<EditDialogItemContainerProps>) => {
  return (
    <div
      className={clsx(
        "flex flex-col p-5 border-t-4 border-stone-600",
        className,
      )}
    >
      <Text
        textColor={TextColor.white}
        size={TextSize.xl}
        className="self-center"
      >
        {title}
      </Text>
      {children}
      <div className="flex gap-2 mt-4 justify-end">
        <CloseIconButton onCloseClick={onCloseClick} />
        <SaveIconButton onSaveClick={onSaveClick} />
      </div>
    </div>
  );
};

export default EditDialogItemContainer;
