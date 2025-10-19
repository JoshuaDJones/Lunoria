import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faClose } from "@fortawesome/free-solid-svg-icons";
import EasyText from "../EasyText";
import { useTheme } from "../../providers/ThemeProvider";

interface ModalHeaderProps {
  title: string;
  onCloseClicked: () => void;
}

const ModalHeader = ({ title, onCloseClicked }: ModalHeaderProps) => {
  const { theme } = useTheme();

  return (
    <div className="flex flex-row items-center">
      <EasyText className="flex-1 text-2xl font-semibold">{title}</EasyText>
      <div className="cursor-pointer hover:opacity-80">
        <FontAwesomeIcon
          icon={faClose}
          size="2xl"
          color={theme === "light" ? "black" : "white"}
          onClick={onCloseClicked}
        />
      </div>
    </div>
  );
};

export default ModalHeader;
