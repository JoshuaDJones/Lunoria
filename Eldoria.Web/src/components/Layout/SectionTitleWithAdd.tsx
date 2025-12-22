import Title, { TitleColor, TitleSize } from "../typography/Title";
import CreateIconButton from "../buttons/CreateIconButton";
import clsx from "clsx";

interface SectionTitleWithAddProps {
  title: string;
  onAddClick: () => void;
  className?: string;
}

const SectionTitleWithAdd = ({
  title,
  onAddClick,
  className,
}: SectionTitleWithAddProps) => {
  return (
    <div className={clsx("flex items-center gap-2 self-center", className)}>
      <Title
        className="self-center"
        color={TitleColor.default}
        size={TitleSize.large}
      >
        {title}
      </Title>
      <CreateIconButton onClick={onAddClick} />
    </div>
  );
};

export default SectionTitleWithAdd;
