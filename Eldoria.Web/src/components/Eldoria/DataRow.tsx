import clsx from "clsx";
import React from "react";
import EasyText from "../EasyText";

interface DataRowProps {
  title: string;
  value: string;
  className?: string;
}

export default function DataRow({ title, value, className }: DataRowProps) {
  return (
    <div className={clsx(className, "flex flex-row gap-2")}>
      <EasyText className="text-2xl">{title}</EasyText>
      <EasyText className="text-2xl">{value}</EasyText>
    </div>
  );
}
