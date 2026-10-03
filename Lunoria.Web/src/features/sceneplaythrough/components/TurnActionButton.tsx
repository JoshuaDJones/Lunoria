const TurnActionButton = ({
  label,
  imageSrc,
  onClick,
}: {
  label: string;
  imageSrc: string;
  onClick: () => void;
}) => {
  return (
    <button
      type="button"
      aria-label={label}
      className="group relative aspect-square cursor-pointer overflow-hidden rounded-xl border-2 border-border bg-canvas shadow-lg transition hover:-translate-y-0.5 hover:border-utility hover:shadow-xl focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60"
      onClick={onClick}
    >
      <img
        src={imageSrc}
        alt=""
        className="h-full w-full object-cover transition duration-200 group-hover:scale-[1.02]"
      />
      <span className="absolute left-0 top-0 rounded-br-xl bg-canvas/90 px-4 py-2 text-left text-lg font-semibold text-content shadow-lg backdrop-blur-sm">
        {label}
      </span>
    </button>
  );
};

export default TurnActionButton;
