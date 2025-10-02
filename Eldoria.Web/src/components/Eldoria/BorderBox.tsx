import clsx from "clsx";
import React, { PropsWithChildren } from "react";

interface BorderBoxProps {
  className?: string;
}

const BorderBox = ({
  children,
  className,
}: PropsWithChildren<BorderBoxProps>) => {
  return (
    <div className={clsx("p-5 flex flex-col rounded-2xl border-2", className)}>
      {children}
    </div>
  );
};

export default BorderBox;
