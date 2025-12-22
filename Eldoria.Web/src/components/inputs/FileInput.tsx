import { ChangeEvent, useRef } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";
import clsx from "clsx";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";

enum FileInputTheme {
  light,
  dark,
}

interface FileInputProps {
  title: string;
  theme?: FileInputTheme;
  onFileSelect: (file: File | undefined) => void;
  useClear?: boolean;
  className?: string;
}

const FileInput = ({
  title,
  theme = FileInputTheme.light,
  onFileSelect,
  useClear = true,
  className,
}: FileInputProps) => {
  const inputRef = useRef<HTMLInputElement | null>(null);

  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.currentTarget.files?.[0];
    onFileSelect(file);
  };

  const clearInput = () => {
    if (inputRef.current) {
      inputRef.current.value = "";
      onFileSelect(undefined);
    }
  };

  return (
    <div className={clsx("flex flex-col", className)}>
      <Text
        size={TextSize.xl}
        textColor={
          theme === FileInputTheme.light ? TextColor.white : TextColor.black
        }
      >
        {title}
      </Text>
      <div className="flex">
        <input
          ref={inputRef}
          name="photo"
          type="file"
          accept="image/png, image/jpeg"
          className={clsx("font-cinzel flex-1", {
            "text-white": theme === FileInputTheme.light,
            "text-black": theme === FileInputTheme.dark,
          })}
          onChange={handleChange}
        />
        {useClear && (
          <AppButton
            title={"Clear"}
            variant={AppButtonVariant.warning}
            size={AppButtonSize.xs}
            onClick={clearInput}
          />
        )}
      </div>
    </div>
  );
};

export default FileInput;
