import React from "react";
import EasyText from "./EasyText";
import clsx from "clsx";

interface InputAreaProps
  extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  title?: string;
  valid?: boolean;
  containerClassName?: string;
}

const InputArea = ({
  title,
  valid,
  className,
  containerClassName,
  ...props
}: InputAreaProps) => {
  return (
    <div className={clsx("relative w-full", containerClassName)}>
      <EasyText className="absolute -top-3 left-3 bg-gray-50 dark:bg-gray-800 px-1">
        {title}
      </EasyText>
      <textarea
        {...props}
        className={clsx(
          "w-full p-2 py-3 bg-transparent border border-gray-600 rounded-lg text-black dark:text-white focus:border-2 focus:border-blue-500 focus:outline-none",
          {
            "border-red-500 focus:border-red-500 dark:border-red-300 focus:border-red-300":
              valid === false,
          },
          className,
        )}
      ></textarea>
    </div>
  );
};

export default InputArea;

{
  /* <textarea className="w-full bg-transparent border mt-5">

</textarea> */
}
