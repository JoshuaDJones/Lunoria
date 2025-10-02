import clsx from "clsx";
import { InputHTMLAttributes, useState } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";

export interface AppInputProps extends InputHTMLAttributes<HTMLInputElement> {
  title?: string;
  valid?: boolean;
  theme?: "light" | "dark";
  className?: string;
  containerClassName?: string;
}

const AppInput = ({
  title,
  valid,
  theme = "light",
  className,
  containerClassName,
  type = "text",
  ...props
}: AppInputProps) => {
  const [showPassword, setShowPassword] = useState(false);

  const isPassword = type === "password";
  const inputType = isPassword && showPassword ? "text" : type;

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
      <div className="relative">
        <input
          {...props}
          type={inputType}
          className={clsx(
            "w-full p-2 py-3 bg-transparent border border-gray-600 rounded-lg focus:border-2 focus:border-blue-500 focus:outline-none pr-10",
            {
              "border-red-500 focus:border-red-500 dark:border-red-300 focus:border-red-300":
                valid === false,
              "text-black": theme === "light",
              "text-white": theme === "dark",
            },
            className,
          )}
        />
        {isPassword && (
          <button
            type="button"
            className="absolute inset-y-0 right-3 flex items-center text-gray-600 dark:text-gray-300 hover:opacity-80"
            onClick={() => setShowPassword(!showPassword)}
          >
            <FontAwesomeIcon
              size="lg"
              icon={showPassword ? faEyeSlash : faEye}
            />
          </button>
        )}
      </div>
    </div>
  );
};

export default AppInput;
