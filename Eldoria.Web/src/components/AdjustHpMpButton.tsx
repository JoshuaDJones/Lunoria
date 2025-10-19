import clsx from "clsx";

interface AdjustHpMpButtonProps {
  title: string;
  type: "mp" | "hp";
  onClick: () => void;
}

const AdjustHpMpButton = ({ title, type, onClick }: AdjustHpMpButtonProps) => {
  return (
    <button
      className={clsx(
        "flex flex-1 rounded items-center justify-center border font-cinzel text-white hover:opacity-60",
        {
          "bg-red-400/50": type === "hp",
          "bg-green-400/50": type === "mp",
        },
      )}
      onClick={onClick}
    >
      {title}
    </button>
  );
};

export default AdjustHpMpButton;
