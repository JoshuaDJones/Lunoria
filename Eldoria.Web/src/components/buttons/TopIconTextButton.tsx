import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Text, { TextColor, TextSize } from "../typography/Text";
import clsx from "clsx";

interface TopIconTextButtonProps {
    title?: string;
    onClick: () => void;
    className?: string;
}

const TopIconTextButton = ({
    title,
    onClick,
    className
}: TopIconTextButtonProps) => (
    <button className={clsx("flex flex-col items-center justify-center border gap-2 rounded-xl w-[80px] h-[75px]", className)}>
        <FontAwesomeIcon icon={faPlus} color="white" size="xl" />
        <Text textColor={TextColor.white} size={TextSize.xs}>{title}</Text>
    </button>
)

export default TopIconTextButton