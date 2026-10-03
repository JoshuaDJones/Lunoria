import { type ScenePlaythroughSpell } from "@/features/journeys";

const SpellAttackSelector = ({
  spells,
  currentMp,
  selectedSpellId,
  onSelect,
}: {
  spells: ScenePlaythroughSpell[];
  currentMp: number;
  selectedSpellId?: number;
  onSelect: (spellId: number) => void;
}) => {
  return (
    <section className="mt-6">
      <h3 className="text-lg font-semibold text-content">Select a spell</h3>
      {spells.length === 0 ? (
        <p className="mt-3 rounded-xl border border-border bg-surface p-4 text-content-muted">
          This participant has no usable spells.
        </p>
      ) : (
        <div className="mt-3 grid gap-3 sm:grid-cols-2">
          {spells.map((spell) => {
            const canAfford = spell.mpCost <= currentMp;
            const isSelected = selectedSpellId === spell.id;

            return (
              <button
                key={spell.id}
                type="button"
                disabled={!canAfford}
                aria-pressed={isSelected}
                className={`cursor-pointer rounded-xl border-2 bg-surface p-3 text-left transition hover:border-magic focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-magic/60 disabled:cursor-not-allowed disabled:opacity-40 ${
                  isSelected ? "border-magic" : "border-border"
                }`}
                onClick={() => onSelect(spell.id)}
              >
                <span className="block font-semibold text-content">
                  {spell.name}
                </span>
                <span className="mt-1 block text-sm text-content-muted">
                  {spell.isUtility
                    ? "Utility spell"
                    : spell.isSupport
                      ? `Restore ${spell.healthEffect ?? 0} HP / ${spell.magicEffect ?? 0} MP`
                      : `Damage ${spell.damageEffect}`}{" "}
                  · MP cost {spell.mpCost}
                </span>
                {spell.description && (
                  <span className="mt-2 block text-sm text-content-secondary">
                    {spell.description}
                  </span>
                )}
              </button>
            );
          })}
        </div>
      )}
    </section>
  );
};

export default SpellAttackSelector;
