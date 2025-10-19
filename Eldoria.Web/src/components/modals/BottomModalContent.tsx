import React, { PropsWithChildren, useEffect, useState } from "react";
import Title, { TitleColor, TitleSize } from "../typography/Title";

interface BottomModalContentProps {
  title: string;
}

const BottomModalContent = ({
  title,
  children,
}: PropsWithChildren<BottomModalContentProps>) => {
  const handleContentClick = (e: React.MouseEvent) => {
    e.stopPropagation();
  };

  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(true);
  }, []);

  return (
    <div
      className={`flex flex-col bg-stone-800 p-5 rounded-tl-3xl rounded-tr-3xl w-full h-[40%] overflow-y-auto absolute bottom-0 transform transition-transform duration-500 ease-in-out 
        ${open ? "translate-y-0" : "translate-y-full"}`}
      onClick={handleContentClick}
    >
      <Title color={TitleColor.white} size={TitleSize.medium}>
        {title}
      </Title>
      <div className="flex-1 flex flex-col px-5">{children}</div>
    </div>
  );
};

export default BottomModalContent;
