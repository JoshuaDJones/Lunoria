import clsx from "clsx";
import Title, { TitleColor, TitleSize } from "../typography/Title";

interface CheckInputProps {
  title: string;
  isSelected: boolean;
  onChange: (isSelected: boolean) => void;
  className?: string;
}

const CheckInput = ({
  title,
  isSelected,
  onChange,
  className,
}: CheckInputProps) => {
  return (
    <div className={clsx("flex gap-2 items-center", className)}>
      <Title size={TitleSize.small} color={TitleColor.white}>
        {title}
      </Title>
      <input
        type="checkbox"
        checked={isSelected}
        className="h-6 w-6"
        onChange={() => onChange(!isSelected)}
      />
    </div>
  );
};

export default CheckInput;
