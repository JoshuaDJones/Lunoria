import { faCaretUp } from "@fortawesome/free-solid-svg-icons";
import { faCaretDown } from "@fortawesome/free-solid-svg-icons";
import { faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import EasyText from "../EasyText";
import clsx from "clsx";
import { PropsWithChildren } from "react";

interface ContentListItemProps {
  index: number;
  content: string;
  onMoveUp: (index: number) => void;
  onMoveDown: (index: number) => void;
  onDelete: (index: number) => void;
}

const ContentListItem = ({
  index,
  content,
  onMoveUp,
  onMoveDown,
  onDelete,
}: ContentListItemProps) => {
  return (
    <div className="flex flex-row bg-blue-300 dark:bg-gray-800 mb-2 p-3 items-center gap-2">
      <div className="flex items-start h-full ">
        <EasyText className="text-2xl font-bold">{index + 1}</EasyText>
      </div>

      <EasyText className="flex-1 text-lg break-all">{content}</EasyText>

      <ActionButton className={"bg-gray-500"} onPress={() => onMoveUp(index)}>
        <FontAwesomeIcon icon={faCaretUp} size="2xl" color="white" />
      </ActionButton>

      <ActionButton className={"bg-blue-500"} onPress={() => onMoveDown(index)}>
        <FontAwesomeIcon icon={faCaretDown} size="2xl" color="white" />
      </ActionButton>

      <ActionButton className={"bg-red-500"} onPress={() => onDelete(index)}>
        <FontAwesomeIcon icon={faTrash} size="lg" color="white" />
      </ActionButton>
    </div>
  );
};

interface ActionButtonProps {
  className: string;
  onPress: () => void;
}

const ActionButton = ({
  children,
  className,
  onPress,
}: PropsWithChildren<ActionButtonProps>) => {
  return (
    <div
      onClick={onPress}
      className={clsx(
        "flex items-center justify-center h-10 w-10 rounded-lg cursor-pointer hover:opacity-80",
        className,
      )}
    >
      {children}
    </div>
  );
};

export default ContentListItem;
