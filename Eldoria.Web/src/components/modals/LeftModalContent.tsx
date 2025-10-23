import React, { PropsWithChildren, useEffect, useState } from "react";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import { cn } from "../../utils/cn";

interface LeftModalContentProps {
  title: string;
  useCustomWidth?: boolean;
  className?: string;
}

const LeftModalContent = ({
  title,
  useCustomWidth = false,
  className,
  children,
}: PropsWithChildren<LeftModalContentProps>) => {
  const handleContentClick = (e: React.MouseEvent) => {
    e.stopPropagation();
  };

  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(true);
  }, []);

  return (
    <div
      className={cn(
        "flex flex-col bg-stone-800 p-5 rounded-tr-3xl rounded-br-3xl",
        "absolute left-0 top-0 h-full overflow-y-auto",
        "transform transition-transform duration-500 ease-in-out",
        open ? "translate-x-0" : "-translate-x-full",
        className,
        !useCustomWidth ? "w-[40%]" : "",
      )}
      onClick={handleContentClick}
    >
      <Title color={TitleColor.white} size={TitleSize.medium}>
        {title}
      </Title>
      <div className="flex-1 flex flex-col px-5">{children}</div>
    </div>
  );
};

export default LeftModalContent;
