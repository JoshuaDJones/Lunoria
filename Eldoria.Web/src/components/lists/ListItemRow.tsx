import Text, { TextColor } from "../typography/Text";
import clsx from "clsx";

export enum RowTextSize {
  sm,
  xxl,
}

interface ListItemRowProps {
  title: string;
  value: string;
  titleColor?: TextColor;
  valueColor?: TextColor;
  size?: RowTextSize;
  className?: string;
}

const ListItemRow = ({
  title,
  value,
  titleColor,
  valueColor,
  size = RowTextSize.xxl,
  className,
}: ListItemRowProps) => {
  return (
    <div className={clsx(className, "flex flex-row gap-2")}>
      <Text
        textColor={titleColor ? titleColor : TextColor.white}
        className={clsx("font-bold tracking-wider", {
          "text-2xl": size === RowTextSize.xxl,
          "text-sm": size === RowTextSize.sm,
        })}
      >
        {title}:
      </Text>
      <Text
        textColor={valueColor ? valueColor : TextColor.white}
        className={clsx("font-bold tracking-wider", {
          "text-2xl": size === RowTextSize.xxl,
          "text-sm": size === RowTextSize.sm,
        })}
      >
        {value}
      </Text>
    </div>
  );
};

export default ListItemRow;
