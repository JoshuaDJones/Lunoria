import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Text, { TextColor } from "../typography/Text";
import clsx from "clsx";
import { faCheck, faClose } from "@fortawesome/free-solid-svg-icons";

interface BoolListItemRowProps {
  title: string;
  titleColor?: TextColor;
  isSelected: boolean;
  className?: string;
}

const BoolListItemRow = ({
  title,
  titleColor,
  isSelected,
  className,
}: BoolListItemRowProps) => {
  return (
    <div className={clsx(className, "flex flex-row gap-2 items-center")}>
      <Text
        className="text-2xl font-bold tracking-wider"
        textColor={titleColor ?? TextColor.white}
      >
        {title}:
      </Text>
      {isSelected && (
        <FontAwesomeIcon size="lg" icon={faCheck} className="text-blue-400" />
      )}
      {!isSelected && (
        <FontAwesomeIcon size="lg" icon={faClose} className="text-red-400" />
      )}
    </div>
  );
};

export default BoolListItemRow;
