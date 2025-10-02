import { PropsWithChildren } from "react";

const RoundedSection = ({ children }: PropsWithChildren) => {
  return (
    <div className="w-full flex flex-col bg-gray-300 dark:bg-gray-700 p-4 rounded-lg">
      {children}
    </div>
  );
};

export default RoundedSection;
