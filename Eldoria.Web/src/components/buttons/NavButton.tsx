import clsx from "clsx";
import AppButton, { AppButtonSize, AppButtonVariant } from "./AppButton";

interface NavButtonProps {
  title: string;
  isSelected: boolean;
  onClick: () => void;
}

const NavButton = ({ title, isSelected, onClick }: NavButtonProps) => {
  return (
    <AppButton
      title={title}
      variant={AppButtonVariant.custom}
      size={AppButtonSize.custom}
      className={clsx("text-3xl", {
        "text-blue-300": isSelected,
        "text-gray-300": !isSelected,
      })}
      onClick={onClick}
    />
  );
};

export default NavButton;
