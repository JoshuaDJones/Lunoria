function LootStat({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-border bg-surface p-3">
      <p className="text-xs text-content-muted">{label}</p>
      <p className="mt-1 font-semibold text-content">{value}</p>
    </div>
  );
}

export default LootStat;
