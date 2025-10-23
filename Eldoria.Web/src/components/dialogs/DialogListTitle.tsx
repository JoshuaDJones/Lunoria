import Title, { TitleColor, TitleSize } from "../typography/Title";

interface DialogListTitleProps {
  title: string;
}

const DialogListTitle = ({ title }: DialogListTitleProps) => {
  return (
    <Title
      className="self-center py-2 px-10 bg-stone-800/90 rounded-xl text-3xl"
      color={TitleColor.white}
      size={TitleSize.custom}
    >
      {title}
    </Title>
  );
};

export default DialogListTitle;
