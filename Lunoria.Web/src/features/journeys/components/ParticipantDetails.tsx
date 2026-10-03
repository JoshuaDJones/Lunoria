import { useState, type ReactNode } from "react";
import {
  ParticipantType,
  type ScenePlaythroughParticipant,
  type ScenePlaythroughSpell,
} from "@/features/journeys/types";

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
      <DetailHeading
        name={p.name}
        description={p.description}
        imageUrl={p.portraitUrl?.trim() || p.photoUrl}
        character
      />
      <Section title="Stats">
        <p className="mb-3 text-sm text-content-muted">
          Current values include equipment effects.
        </p>
        <DataList values={stats} />
      </Section>
      <Section title="Transformation character">
        {p.alternateForm ? (
          <>
            <DetailHeading
              name={`${p.alternateForm.name}${p.isInAlternateForm ? " (current form)" : ""}`}
              description={p.alternateForm.description}
              imageUrl={
                p.alternateForm.portraitUrl?.trim() || p.alternateForm.photoUrl
              }
              character
            />
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
              Base spells
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
      <Section title="Spells">
        {p.spells.length ? (
          p.spells.map((spell) => <SpellData key={spell.id} spell={spell} />)
        ) : (
          <Empty>No spells.</Empty>
        )}
      </Section>
      <Section title="Equipment">
        {p.equippableItems.length ? (
          p.equippableItems.map((link) => {
            const item = link.item;
            return (
              <article
                key={link.inventoryItemId}
                className="rounded-lg border border-border p-3"
              >
                <DetailHeading name={item.name} imageUrl={item.photoUrl} />
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
      <Section title="Consumables">
        {p.consumableItems.length ? (
          p.consumableItems.map((link) => (
            <article
              key={link.inventoryItemId}
              className="rounded-lg border border-border p-3"
            >
              <DetailHeading
                name={link.item.name}
                imageUrl={link.item.photoUrl}
              />
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
      <DetailHeading name={spell.name} imageUrl={spell.photoUrl} />
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

function DetailHeading({
  name,
  description,
  imageUrl,
  character = false,
}: {
  name: string;
  description?: string;
  imageUrl?: string | null;
  character?: boolean;
}) {
  const [failedUrl, setFailedUrl] = useState<string>();
  const src = imageUrl?.trim();
  return (
    <div
      className={
        character
          ? "mb-8 flex flex-col items-center gap-4 text-center"
          : "mb-3 flex items-start gap-4"
      }
    >
      <div
        className={
          character
            ? "flex w-full justify-center"
            : "flex h-20 w-20 shrink-0 items-center justify-center overflow-hidden rounded-xl border border-border bg-canvas"
        }
      >
        {src && src !== failedUrl ? (
          <img
            src={src}
            alt={name}
            loading="lazy"
            className={
              character
                ? "h-auto w-auto max-h-64 max-w-full rounded-xl sm:max-h-80"
                : "h-full w-full object-contain"
            }
            onError={() => setFailedUrl(src)}
          />
        ) : (
          <span className="p-2 text-center text-xs text-content-muted">
            No image
          </span>
        )}
      </div>
      <div
        className={character ? "w-full min-w-0 text-center" : "min-w-0 flex-1"}
      >
        <h4
          className={
            character
              ? "break-words text-xl font-semibold"
              : "break-words font-semibold"
          }
        >
          {name}
        </h4>
        {description && (
          <p className="mt-1 whitespace-pre-wrap break-words text-sm text-content-secondary">
            {description}
          </p>
        )}
      </div>
    </div>
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
