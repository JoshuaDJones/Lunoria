import type { ReactNode } from "react";
import {
  ParticipantType,
  type ScenePlaythroughParticipant,
  type ScenePlaythroughSpell,
} from "../types";

export function ParticipantDetails({
  participant: p,
}: {
  participant: ScenePlaythroughParticipant;
}) {
  const stats: [string, ReactNode][] = [
    [
      "Type",
      p.participantType === ParticipantType.Player
        ? "Player"
        : p.participantType === ParticipantType.NPC
          ? "NPC"
          : "Enemy",
    ],
    [
      "Status",
      p.isDead
        ? "Defeated"
        : p.isDown
          ? "Downed"
          : p.isActive
            ? "Active"
            : "Inactive",
    ],
    ["Current turn", p.isCurrentParticipant ? "Yes" : "No"],
    ["Form", p.isInAlternateForm ? "Alternate" : "Normal"],
    ["Can transform", p.canTransform ? "Yes" : "No"],
    ["HP", `${p.currentHp} / ${p.maxHp}`],
    ["MP", `${p.currentMp} / ${p.maxMp}`],
    ["Movement", p.movement],
    ["Melee damage", p.meleeAttackDamage ?? "Unavailable"],
    ["Bow damage", p.bowAttackDamage ?? "Unavailable"],
    [
      "Attacks remaining / per turn",
      `${p.attacksRemaining} / ${p.attacksPerTurn}`,
    ],
    ["Melee damage reduction", p.meleeDamageReduction],
    ["Bow damage reduction", p.bowDamageReduction],
    ["Spell damage reduction", p.spellDamageReduction],
    [
      "Consumable slots",
      `${p.consumableItems.length} / ${p.maxConsumableInventory}`,
    ],
    [
      "Equipment slots",
      `${p.equippableItems.length} / ${p.maxEquippableInventory}`,
    ],
    ["Downed turns remaining", p.downedTurnsRemaining ?? "Not downed"],
  ];
  return (
    <div className="space-y-6 text-content">
      {p.description && (
        <p className="whitespace-pre-wrap text-content-secondary">
          {p.description}
        </p>
      )}
      <Section title="Stats">
        <p className="mb-3 text-sm text-content-muted">
          Current values include equipment effects.
        </p>
        <DataList values={stats} />
      </Section>
      <Section title="Transformation character">
        {p.alternateForm ? (
          <>
            <h4 className="font-semibold">
              {p.alternateForm.name}
              {p.isInAlternateForm ? " (current form)" : ""}
            </h4>
            {p.alternateForm.description && (
              <p className="whitespace-pre-wrap text-sm text-content-secondary">
                {p.alternateForm.description}
              </p>
            )}
            <p className="text-sm text-content-muted">
              Saved base character data, not the participant’s live stats.
              Transformation uses this form’s movement, melee, bow, and spells.
              The participant keeps their HP, MP, and inventory; equipment
              effects still apply.
            </p>
            <DataList
              values={[
                ["Base max HP", p.alternateForm.maxHp],
                ["Base max MP", p.alternateForm.maxMp],
                ["Base movement", p.alternateForm.movement],
                [
                  "Base melee",
                  p.alternateForm.meleeAttackDamage ?? "Unavailable",
                ],
                ["Base bow", p.alternateForm.bowAttackDamage ?? "Unavailable"],
                [
                  "Base consumable capacity",
                  p.alternateForm.maxConsumableInventory,
                ],
                [
                  "Base equipment capacity",
                  p.alternateForm.maxEquippableInventory,
                ],
              ]}
            />
            <h5 className="font-semibold">
              Base spells ({p.alternateForm.spells.length})
            </h5>
            {p.alternateForm.spells.length ? (
              p.alternateForm.spells.map((spell) => (
                <SpellData key={spell.id} spell={spell} />
              ))
            ) : (
              <Empty>No base spells.</Empty>
            )}
          </>
        ) : (
          <Empty>No alternate form assigned.</Empty>
        )}
      </Section>
      <Section title={`Spells (${p.spells.length})`}>
        {p.spells.length ? (
          p.spells.map((spell) => <SpellData key={spell.id} spell={spell} />)
        ) : (
          <Empty>No spells.</Empty>
        )}
      </Section>
      <Section title={`Equipment (${p.equippableItems.length})`}>
        {p.equippableItems.length ? (
          p.equippableItems.map((link) => {
            const item = link.item;
            return (
              <article
                key={link.inventoryItemId}
                className="rounded-lg border border-border p-3"
              >
                <h4 className="font-semibold">{item.name}</h4>
                {item.description && (
                  <p className="my-2 whitespace-pre-wrap text-sm text-content-secondary">
                    {item.description}
                  </p>
                )}
                <DataList
                  values={[
                    ["Equipped", link.isEquipped ? "Yes" : "No"],
                    [
                      "Melee damage modifier",
                      item.meleeAttackDamageModifier ?? 0,
                    ],
                    ["Bow damage modifier", item.bowAttackDamageModifier ?? 0],
                    ["Movement modifier", item.movementModifier ?? 0],
                    ["Max HP modifier", item.maxHpModifier ?? 0],
                    ["Max MP modifier", item.maxMpModifier ?? 0],
                    [
                      "Consumable capacity modifier",
                      item.maxConsumableInventoryModifier ?? 0,
                    ],
                    [
                      "Equipment capacity modifier",
                      item.maxEquippableInventoryModifier ?? 0,
                    ],
                    ["Additional attacks", item.additionalAttacksPerTurn ?? 0],
                    ["Melee reduction", item.meleeDamageReduction ?? 0],
                    ["Bow reduction", item.bowDamageReduction ?? 0],
                    ["Spell reduction", item.spellDamageReduction ?? 0],
                    ["Spell damage modifier", item.spellDamageModifier ?? 0],
                    [
                      "Affected spell type",
                      item.affectedSpellType ?? "All spell types",
                    ],
                  ]}
                />
                {item.addedSpells.length > 0 && (
                  <div className="mt-3 space-y-2">
                    <h5 className="font-semibold">Granted spells</h5>
                    {item.addedSpells.map((spell) => (
                      <SpellData key={spell.id} spell={spell} />
                    ))}
                  </div>
                )}
              </article>
            );
          })
        ) : (
          <Empty>No equipment.</Empty>
        )}
      </Section>
      <Section title={`Consumables (${p.consumableItems.length})`}>
        {p.consumableItems.length ? (
          p.consumableItems.map((link) => (
            <article
              key={link.inventoryItemId}
              className="rounded-lg border border-border p-3"
            >
              <h4 className="font-semibold">{link.item.name}</h4>
              {link.item.description && (
                <p className="my-2 whitespace-pre-wrap text-sm text-content-secondary">
                  {link.item.description}
                </p>
              )}
              <DataList
                values={[
                  ["HP effect", link.item.hpEffect ?? 0],
                  ["MP effect", link.item.mpEffect ?? 0],
                ]}
              />
            </article>
          ))
        ) : (
          <Empty>No consumables.</Empty>
        )}
      </Section>
    </div>
  );
}

function SpellData({ spell }: { spell: ScenePlaythroughSpell }) {
  return (
    <article className="rounded-lg border border-border p-3">
      <h4 className="font-semibold">{spell.name}</h4>
      {spell.description && (
        <p className="my-2 whitespace-pre-wrap text-sm text-content-secondary">
          {spell.description}
        </p>
      )}
      <DataList
        values={[
          [
            "Casting type",
            spell.isUtility
              ? "Utility"
              : spell.isSupport
                ? "Restoration"
                : "Damage",
          ],
          ["MP cost", spell.mpCost],
          ["Damage", spell.damageEffect ?? 0],
          ["Healing", spell.healthEffect ?? 0],
          ["MP restoration", spell.magicEffect ?? 0],
          ["Range", spell.range],
          ["Radius", spell.isRadius ? "Yes" : "No"],
        ]}
      />
    </article>
  );
}

function DataList({ values }: { values: [string, ReactNode][] }) {
  return (
    <dl className="grid grid-cols-1 gap-2 text-sm sm:grid-cols-2">
      {values.map(([label, value]) => (
        <div key={label} className="min-w-0">
          <dt className="text-content-muted">{label}</dt>
          <dd className="break-words font-medium">{value}</dd>
        </div>
      ))}
    </dl>
  );
}
function Section({ title, children }: { title: string; children: ReactNode }) {
  return (
    <section className="space-y-3">
      <h3 className="text-xl font-semibold">{title}</h3>
      {children}
    </section>
  );
}
function Empty({ children }: { children: ReactNode }) {
  return <p className="text-sm text-content-muted">{children}</p>;
}
