export function TreasureChestIcon({ className }: { className?: string }) {
  return (
    <svg
      aria-hidden="true"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.5"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
    >
      <path
        d="M3 11V9a6 6 0 0 1 6-6h6a6 6 0 0 1 6 6v10a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-8Z"
        fill="currentColor"
        fillOpacity="0.12"
      />
      <path d="M3 11h7m4 0h7M7 4v17M17 4v17" />
      <rect x="10" y="9" width="4" height="6" rx="1" />
      <path d="M12 11.5v1" />
    </svg>
  );
}
