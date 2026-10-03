import { Button } from "@/components/ui";
import { type SceneOpenChestResult } from "@/features/journeys";
import { formatLootModifier } from "@/features/sceneplaythrough/utils/sceneDisplayUtils";
import LootStat from "@/features/sceneplaythrough/components/LootStat";

function ChestLootResult({
  result,
  onContinue,
}: {
  result: SceneOpenChestResult;
  onContinue: () => void;
}) {
  const item = result.item;
  const equippableStats = [
    ["Melee attack", item.meleeAttackDamageModifier],
    ["Bow attack", item.bowAttackDamageModifier],
    ["Movement", item.movementModifier],
    ["Maximum HP", item.maxHpModifier],
    ["Maximum MP", item.maxMpModifier],
    ["Consumable slots", item.maxConsumableInventoryModifier],
    ["Equipment slots", item.maxEquippableInventoryModifier],
    ["Additional attacks", item.additionalAttacksPerTurn],
    ["Melee reduction", item.meleeDamageReduction],
    ["Bow reduction", item.bowDamageReduction],
    ["Spell reduction", item.spellDamageReduction],
  ] as const;
  const consumableStats = [
    ["HP effect", item.hpEffect],
    ["MP effect", item.mpEffect],
  ] as const;
  const stats = result.isEquippable ? equippableStats : consumableStats;

  return (
    <div className="chest-loot-reveal">
      <div className="grid gap-6 md:grid-cols-2 md:items-start">
        <div className="flex aspect-square items-center justify-center overflow-hidden rounded-2xl border-2 border-utility/60 bg-canvas p-4 shadow-xl shadow-utility/10">
          {item.photoUrl?.trim() ? (
            <img
              src={item.photoUrl}
              alt={item.name}
              className="max-h-full max-w-full object-contain drop-shadow-[0_0_1.25rem_rgba(250,204,21,0.65)]"
            />
          ) : (
            <span className="p-6 text-center text-2xl font-semibold text-content">
              {item.name}
            </span>
          )}
        </div>

        <div className="min-w-0">
          <p
            className={`text-sm font-semibold uppercase tracking-wide ${
              result.awarded ? "text-utility" : "text-danger"
            }`}
          >
            {result.awarded ? "Loot received" : "Inventory full"}
          </p>
          <h3 className="mt-1 text-2xl font-semibold text-content">
            {result.quantity} × {item.name}
          </h3>
          {item.description && (
            <p className="mt-3 text-sm leading-6 text-content-secondary">
              {item.description}
            </p>
          )}

          {!result.awarded && (
            <p className="mt-4 rounded-xl border border-danger/50 bg-danger/10 p-3 text-sm text-content-secondary">
              This item did not fit in the player&apos;s inventory. The chest
              remains unopened and the turn was forfeited.
            </p>
          )}

          <div className="mt-5 grid grid-cols-2 gap-2">
            {stats.map(([label, value]) => (
              <LootStat
                key={label}
                label={label}
                value={formatLootModifier(value ?? 0)}
              />
            ))}
            {result.isEquippable && item.affectedSpellType && (
              <LootStat label="Spell type" value={item.affectedSpellType} />
            )}
            {result.isEquippable && item.spellDamageModifier !== null && (
              <LootStat
                label="Spell damage"
                value={formatLootModifier(item.spellDamageModifier)}
              />
            )}
          </div>

          {result.isEquippable && item.addedSpells.length > 0 && (
            <div className="mt-4 rounded-xl border border-border bg-surface p-3">
              <p className="text-xs font-semibold uppercase tracking-wide text-content-muted">
                Added spells
              </p>
              <ul className="mt-2 space-y-2">
                {item.addedSpells.map((spell) => (
                  <li key={spell.id} className="text-sm text-content-secondary">
                    <span className="font-semibold text-content">
                      {spell.name}
                    </span>
                    {` · ${spell.mpCost} MP`}
                    {spell.damageEffect !== null &&
                      ` · ${spell.damageEffect} damage`}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      </div>

      <div className="mt-6 flex justify-end">
        <Button variant="primary" size="lg" onClick={onContinue}>
          Continue
        </Button>
      </div>
    </div>
  );
}

export default ChestLootResult;
