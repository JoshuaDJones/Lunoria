import React, { PropsWithChildren } from "react";

const ModalContent = ({ children }: PropsWithChildren) => {
  const handleContentClick = (e: React.MouseEvent) => {
    e.stopPropagation();
  };

  return (
    <div
      className="flex flex-col bg-gray-50 dark:bg-gray-800 p-5 rounded-3xl w-full h-full md:h-[95%] md:w-3/4 lg:w-1/2 overflow-y-auto"
      onClick={handleContentClick}
    >
      {children}
    </div>
  );
};

export default ModalContent;
