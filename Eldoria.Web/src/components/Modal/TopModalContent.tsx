import React, { PropsWithChildren, useEffect, useState } from "react";
import Title, { TitleColor, TitleSize } from "../typography/Title";

interface TopModalContentProps {
  title: string;
}

const TopModalContent = ({
  title,
  children,
}: PropsWithChildren<TopModalContentProps>) => {
  const handleContentClick = (e: React.MouseEvent) => {
    e.stopPropagation();
  };

  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(true);
  }, []);

  return (
    <div
      className={`flex flex-col bg-stone-800 p-5 rounded-bl-3xl rounded-br-3xl w-full h-[40%] overflow-y-auto absolute top-0 transform transition-transform duration-500 ease-in-out 
        ${open ? "translate-y-0" : "-translate-y-full"}`}
      onClick={handleContentClick}
    >
      <Title color={TitleColor.white} size={TitleSize.medium}>
        {title}
      </Title>
      <div className="flex-1 flex flex-col px-5">{children}</div>
    </div>
  );
};

export default TopModalContent;
