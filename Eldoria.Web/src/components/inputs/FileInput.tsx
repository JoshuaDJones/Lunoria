import React, { ChangeEvent } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";
import clsx from "clsx";

enum FileInputTheme {
  light,
  dark,
}

interface FileInputProps {
  title: string;
  theme?: FileInputTheme;
  onFileSelect: (file: File | undefined) => void;
  className?: string;
}

const FileInput = ({
  title,
  theme = FileInputTheme.light,
  onFileSelect,
  className
}: FileInputProps) => {
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
      <input
        name="photo"
        type="file"
        accept="image/png, image/jpeg"
        className={clsx("font-cinzel", {
          "text-white": theme === FileInputTheme.light,
          "text-black": theme === FileInputTheme.dark,
        })}
        onChange={(event: ChangeEvent<HTMLInputElement>) => {
          const file = event.currentTarget.files?.[0];
          onFileSelect(file);
        }}
      />
    </div>
  );
};

export default FileInput;
