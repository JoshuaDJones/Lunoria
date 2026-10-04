import type { ReactNode } from "react";
import type {
  CharacterRevisionStatus,
  CharacterSyncPreview,
  CharacterSyncSelection,
  CharacterSyncStats,
} from "@/features/characterSync/types";

const statLabels: [keyof CharacterSyncStats, string][] = [
  ["maxHp", "Max HP"],
  ["maxMp", "Max MP"],
  ["meleeAttackDamage", "Melee"],
  ["bowAttackDamage", "Bow"],
  ["movement", "Movement"],
  ["maxConsumableInventory", "Consumable slots"],
  ["maxEquippableInventory", "Equipment slots"],
];

function revisionLabel(status: CharacterRevisionStatus) {
  return status.requiresReview
    ? "Previous base version unknown — review recommended."
    : status.updateAvailable
      ? "The base character changed since your last review."
      : "No new base changes. You can still replace customized values.";
}

function Category({
  title,
  checked,
  onChange,
  children,
}: {
  title: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
  children: ReactNode;
}) {
  return (
    <section className="rounded-xl border border-border bg-surface p-4">
      <label className="mb-3 flex cursor-pointer items-center gap-3 font-semibold text-content">
        <input
          type="checkbox"
          className="size-4 accent-blue-500"
          checked={checked}
          onChange={(event) => onChange(event.target.checked)}
        />
        {title}
      </label>
      {children}
    </section>
  );
}

export function CharacterSyncDetails({
  preview,
  selection,
  onChange,
  alternateNames,
}: {
  preview: CharacterSyncPreview;
  selection: CharacterSyncSelection;
  onChange: (key: keyof CharacterSyncSelection, checked: boolean) => void;
  alternateNames: Record<number, string>;
}) {
  const added = preview.baseSpells.filter(
    (spell) =>
      !preview.assignedSpells.some((current) => current.id === spell.id),
  );
  const removed = preview.assignedSpells.filter(
    (spell) => !preview.baseSpells.some((base) => base.id === spell.id),
  );
  const changedSpells = preview.status.sharedSpells.filter(
    (spell) => spell.updateAvailable || spell.requiresReview,
  );
  const alternateName = (id: number | null) =>
    id === null
      ? "None"
      : (alternateNames[id] ?? `Unavailable character (#${id})`);
  return (
    <div className="space-y-4">
      <Category
        title="Stats"
        checked={selection.stats}
        onChange={(checked) => onChange("stats", checked)}
      >
        <p className="mb-3 text-sm text-content-muted">
          {revisionLabel(preview.status.stats)}
        </p>
        <table className="w-full text-left text-sm">
          <caption className="sr-only">
            Assigned statistics compared with the base character
          </caption>
          <thead>
            <tr className="text-content-muted">
              <th scope="col" className="pb-2">
                Stat
              </th>
              <th scope="col" className="pb-2 text-right">
                Current
              </th>
              <th scope="col" className="pb-2 text-right">
                Base
              </th>
            </tr>
          </thead>
          <tbody>
            {statLabels.map(([key, label]) => {
              const changed =
                preview.assignedStats[key] !== preview.baseStats[key];
              return (
                <tr
                  key={key}
                  className={
                    changed
                      ? "bg-amber-400/10 text-amber-200"
                      : "text-content-secondary"
                  }
                >
                  <th scope="row" className="py-2 font-medium">
                    {label}
                    {changed && <span className="sr-only"> (different)</span>}
                  </th>
                  <td className="py-2 text-right">
                    {preview.assignedStats[key] ?? "—"}
                  </td>
                  <td className="py-2 text-right">
                    {preview.baseStats[key] ?? "—"}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
        <p className="mt-3 text-xs text-content-muted">
          Apply replaces all stats above. Turn order and initially active
          settings stay unchanged.
        </p>
      </Category>

      <Category
        title="Spell assignments"
        checked={selection.spellAssignments}
        onChange={(checked) => onChange("spellAssignments", checked)}
      >
        <p className="mb-3 text-sm text-content-muted">
          {revisionLabel(preview.status.spellAssignments)}
        </p>
        {added.length === 0 && removed.length === 0 ? (
          <p className="text-sm text-content-secondary">
            The spell lists match.
          </p>
        ) : (
          <div className="grid gap-3 sm:grid-cols-2">
            <div>
              <h4 className="mb-2 font-medium text-green-300">Will be added</h4>
              {added.length ? (
                <ul className="space-y-1 text-sm text-content-secondary">
                  {added.map((spell) => (
                    <li key={spell.id}>{spell.name}</li>
                  ))}
                </ul>
              ) : (
                <p className="text-sm text-content-muted">None</p>
              )}
            </div>
            <div>
              <h4 className="mb-2 font-medium text-red-300">Will be removed</h4>
              {removed.length ? (
                <ul className="space-y-1 text-sm text-content-secondary">
                  {removed.map((spell) => (
                    <li key={spell.id}>{spell.name}</li>
                  ))}
                </ul>
              ) : (
                <p className="text-sm text-content-muted">None</p>
              )}
            </div>
          </div>
        )}
        <p className="mt-3 text-xs text-content-muted">
          Apply replaces the assigned list with the base list; it does not merge
          them.
        </p>
      </Category>

      <Category
        title="Alternate form"
        checked={selection.alternateForm}
        onChange={(checked) => onChange("alternateForm", checked)}
      >
        <p className="mb-3 text-sm text-content-muted">
          {revisionLabel(preview.status.alternateForm)}
        </p>
        <dl className="grid grid-cols-2 gap-2 text-sm">
          <dt className="text-content-muted">Current</dt>
          <dd className="text-content">
            {alternateName(preview.assignedAlternateFormId)}
          </dd>
          <dt className="text-content-muted">Base</dt>
          <dd className="text-content">
            {alternateName(preview.baseAlternateFormId)}
          </dd>
        </dl>
        <p className="mt-3 text-xs text-content-muted">
          Apply uses the base alternate assignment, including clearing it when
          the base has none.
        </p>
      </Category>

      {changedSpells.length > 0 ? (
        <Category
          title="Updated shared spells — mark reviewed"
          checked={selection.sharedSpellUpdates}
          onChange={(checked) => onChange("sharedSpellUpdates", checked)}
        >
          <p className="mb-3 text-sm text-content-secondary">
            {preview.sharedSpellNotice}
          </p>
          <p className="mb-3 text-xs text-content-muted">
            Previous definitions are not stored. These are the current values,
            not an old-versus-new comparison.
          </p>
          <div className="space-y-3">
            {changedSpells.map((update) => {
              const spell = preview.assignedSpells.find(
                (item) => item.id === update.spellId,
              );
              return (
                <article
                  key={update.spellId}
                  className="rounded-lg border border-border p-3"
                >
                  <h4 className="font-semibold text-content">
                    {update.name}
                    {update.isArchived && " (archived)"}
                  </h4>
                  <p className="text-xs text-content-muted">
                    {update.requiresReview
                      ? "Previous version unknown"
                      : "Definition updated"}
                  </p>
                  {spell && (
                    <>
                      <p className="mt-2 whitespace-pre-wrap text-sm text-content-secondary">
                        {spell.description}
                      </p>
                      <dl className="mt-3 grid grid-cols-2 gap-2 text-sm sm:grid-cols-3">
                        {[
                          ["MP cost", spell.mpCost],
                          ["Range", spell.range],
                          ["Damage", spell.damageEffect ?? "—"],
                          ["HP effect", spell.healthEffect ?? "—"],
                          ["MP effect", spell.magicEffect ?? "—"],
                          ["Area effect", spell.isRadius ? "Yes" : "No"],
                          ["Type", spell.spellType?.name ?? "None"],
                        ].map(([label, value]) => (
                          <div key={label}>
                            <dt className="text-content-muted">{label}</dt>
                            <dd className="text-content">{value}</dd>
                          </div>
                        ))}
                      </dl>
                    </>
                  )}
                </article>
              );
            })}
          </div>
        </Category>
      ) : (
        <p className="text-sm text-content-muted">
          No unreviewed shared spell changes.
        </p>
      )}
    </div>
  );
}
