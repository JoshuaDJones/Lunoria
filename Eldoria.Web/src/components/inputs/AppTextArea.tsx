import clsx from "clsx";
import { TextareaHTMLAttributes } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";

export interface AppTextAreaProps
  extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  title?: string;
  valid?: boolean;
  theme?: "light" | "dark";
  className?: string;
  containerClassName?: string;
}

const AppTextArea = ({
  title,
  valid,
  theme = "light",
  className,
  containerClassName,
  rows = 4,
  ...props
}: AppTextAreaProps) => {
  return (
    <div className={clsx("relative w-full", containerClassName)}>
      {title && (
        <Text
          size={TextSize.xl}
          textColor={theme === "light" ? TextColor.black : TextColor.white}
        >
          {title}
        </Text>
      )}
      <textarea
        {...props}
        rows={rows}
        className={clsx(
          "w-full p-2 py-3 bg-transparent border border-gray-600 rounded-lg focus:border-2 focus:border-blue-500 focus:outline-none resize-none",
          {
            "border-red-500 focus:border-red-500 dark:border-red-300 focus:border-red-300":
              valid === false,
            "text-black": theme === "light",
            "text-white": theme === "dark",
          },
          className,
        )}
      />
    </div>
  );
};

export default AppTextArea;
