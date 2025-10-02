import React, { PropsWithChildren, useEffect, useState } from "react";
import Title, { TitleColor, TitleSize } from "../typography/Title";

interface RightModalContentProps {
  title: string;
}

const RightModalContent = ({
  title,
  children,
}: PropsWithChildren<RightModalContentProps>) => {
  const handleContentClick = (e: React.MouseEvent) => {
    e.stopPropagation();
  };

  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(true);
  }, []);

  return (
    <div
      className={`flex flex-col bg-stone-800 p-5 rounded-tl-3xl rounded-bl-3xl w-[40%] h-full overflow-y-auto absolute right-0 transform transition-transform duration-500 ease-in-out 
        ${open ? "translate-x-0" : "translate-x-full"}`}
      onClick={handleContentClick}
    >
      <Title color={TitleColor.white} size={TitleSize.medium}>
        {title}
      </Title>
      <div className="flex-1 flex flex-col px-5">{children}</div>
    </div>
  );
};

export default RightModalContent;
