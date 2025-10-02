import clsx from "clsx";

interface VerticalDividerProps {
  className?: string;
}

const VerticalDivider = ({ className }: VerticalDividerProps) => {
  return <div className={clsx("border mx-3", className)} />;
};

export default VerticalDivider;
