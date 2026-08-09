import { useEffect, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft, faPen } from "@fortawesome/free-solid-svg-icons";
import { ApiLoadError, Button, FormField, Input, Select } from "@/components/ui";
import { CharacterType, listCharacters, type Character } from "@/features/characters";
import { updateJourneyCharacter } from "@/features/journeys/api/journeysApi";
import type { JourneyCharacter } from "@/features/journeys/types";
import { getApiError } from "@/lib/apiClient";

interface Props {
  journeyCharacters: JourneyCharacter[];
  selectedCharacterIds: number[];
  onSave: (characterIds: number[]) => Promise<void>;
  onCharacterUpdated: (character: JourneyCharacter) => void;
  onCancel: () => void;
}

export function JourneyCharacterPicker({ journeyCharacters, selectedCharacterIds, onSave, onCharacterUpdated, onCancel }: Props) {
  const [characters, setCharacters] = useState<Character[]>([]);
  const [selectedIds, setSelectedIds] = useState(() => new Set(selectedCharacterIds));
  const [editingId, setEditingId] = useState<number>();
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");
  const editing = journeyCharacters.find((item) => item.id === editingId);

  const load = async () => { setIsLoading(true); setError(""); try { setCharacters(await listCharacters({ typeFilter: CharacterType.Player })); } catch (requestError) { setError(getApiError(requestError).message); } finally { setIsLoading(false); } };
  useEffect(() => { let current = true; void listCharacters({ typeFilter: CharacterType.Player }).then((loaded) => { if (current) { setCharacters(loaded); setError(""); } }).catch((requestError: unknown) => { if (current) setError(getApiError(requestError).message); }).finally(() => { if (current) setIsLoading(false); }); return () => { current = false; }; }, []);

  if (editing) {
    return <JourneyCharacterForm assignment={editing} characters={characters} onCancel={() => setEditingId(undefined)} onSave={async (request) => {
      const saved = await updateJourneyCharacter(editing.id, request);
      onCharacterUpdated(saved); setEditingId(undefined);
    }} />;
  }

  const save = async () => { setIsSaving(true); setError(""); try { await onSave([...selectedIds]); } catch (requestError) { setError(getApiError(requestError).message); setIsSaving(false); } };
  const toggle = (id: number) => setSelectedIds((current) => { const next = new Set(current); if (next.has(id)) next.delete(id); else next.add(id); return next; });

  return <div className="flex min-h-full flex-col">
    <p className="mb-5 text-sm text-content-secondary">Select playable characters for this journey, or edit the stats used when a new playthrough starts.</p>
    {isLoading && <p className="text-content-secondary" role="status">Loading playable characters...</p>}
    {!isLoading && error && <ApiLoadError error={error} onRetry={load} />}
    {!isLoading && !error && characters.length === 0 && <p className="rounded-xl border border-border p-5 text-content-muted">No playable characters are available.</p>}
    {!isLoading && !error && <div className="flex-1 space-y-3">{characters.map((character) => {
      const isSelected = selectedIds.has(character.id);
      const assignment = journeyCharacters.find((item) => item.characterId === character.id);
      return <article key={character.id} className={`rounded-xl border p-4 ${isSelected ? "border-add bg-add/10" : "border-border bg-surface"}`}>
        <button type="button" aria-pressed={isSelected} onClick={() => toggle(character.id)} disabled={isSaving} className="flex w-full cursor-pointer items-center gap-4 text-left disabled:opacity-60">
          {character.photoUrl && <img src={character.photoUrl} alt="" className="size-16 shrink-0 rounded-lg object-cover" />}
          <span className="min-w-0 flex-1"><span className="block truncate text-lg font-semibold text-content">{character.name}</span><span className="mt-1 line-clamp-2 block text-sm text-content-secondary">{assignment ? `HP ${assignment.maxHp} · MP ${assignment.maxMp} · ${assignment.isInitiallyActive ? "Initially active" : "Initially inactive"}` : character.description}</span></span>
          <span className={`shrink-0 rounded-full px-3 py-1 text-xs font-semibold ${isSelected ? "bg-add text-on-add" : "bg-surface-raised text-content-muted"}`}>{isSelected ? "Selected" : "Not selected"}</span>
        </button>
        {assignment && isSelected && <div className="mt-3 flex justify-end border-t border-border pt-3"><Button onClick={() => setEditingId(assignment.id)} variant="primary" size="sm" leftIcon={<FontAwesomeIcon icon={faPen} />}>Edit stats</Button></div>}
      </article>;
    })}</div>}
    <div className="mt-6 flex justify-end gap-3 border-t border-border pt-4"><Button onClick={onCancel} disabled={isSaving} size="lg">Cancel</Button><Button onClick={() => void save()} disabled={isLoading || isSaving} variant="primary" size="lg">{isSaving ? "Saving..." : "Save Characters"}</Button></div>
  </div>;
}

function JourneyCharacterForm({ assignment, characters, onSave, onCancel }: { assignment: JourneyCharacter; characters: Character[]; onSave: (request: Parameters<typeof updateJourneyCharacter>[1]) => Promise<void>; onCancel: () => void }) {
  const [values, setValues] = useState({ melee: text(assignment.meleeAttackDamage), bow: text(assignment.bowAttackDamage), movement: String(assignment.movement), consumables: String(assignment.maxConsumableInventory), equipment: String(assignment.maxEquippableInventory), hp: String(assignment.maxHp), mp: String(assignment.maxMp), active: assignment.isInitiallyActive, alternate: String(assignment.alternateForm?.id ?? "") });
  const [saving, setSaving] = useState(false); const [error, setError] = useState("");
  const set = (key: keyof typeof values, value: string | boolean) => setValues((current) => ({ ...current, [key]: value }));
  const submit = async (event: FormEvent) => { event.preventDefault(); setSaving(true); setError(""); try { await onSave({ meleeAttackDamage: numberOrNull(values.melee), bowAttackDamage: numberOrNull(values.bow), movement: Number(values.movement), maxConsumableInventory: Number(values.consumables), maxEquippableInventory: Number(values.equipment), maxHp: Number(values.hp), maxMp: Number(values.mp), isInitiallyActive: values.active, alternateFormId: numberOrNull(values.alternate) }); } catch (requestError) { setError(getApiError(requestError).message); setSaving(false); } };
  const alternates = characters.filter((character) => character.id !== assignment.characterId);
  return <form onSubmit={(event) => void submit(event)} className="space-y-5"><Button onClick={onCancel} size="sm" leftIcon={<FontAwesomeIcon icon={faArrowLeft} />}>All journey characters</Button><h3 className="text-2xl font-semibold text-content">Edit {assignment.character.name}</h3><div className="grid gap-4 sm:grid-cols-2"><NumberField id="jc-hp" label="Max HP" value={values.hp} change={(v) => set("hp", v)} min={1} /><NumberField id="jc-mp" label="Max MP" value={values.mp} change={(v) => set("mp", v)} /><NumberField id="jc-melee" label="Melee damage" value={values.melee} change={(v) => set("melee", v)} optional /><NumberField id="jc-bow" label="Bow damage" value={values.bow} change={(v) => set("bow", v)} optional /><NumberField id="jc-movement" label="Movement" value={values.movement} change={(v) => set("movement", v)} /><NumberField id="jc-consumables" label="Consumable slots" value={values.consumables} change={(v) => set("consumables", v)} /><NumberField id="jc-equipment" label="Equipment slots" value={values.equipment} change={(v) => set("equipment", v)} /></div><FormField htmlFor="jc-alternate" label="Alternate form"><Select id="jc-alternate" value={values.alternate} onChange={(event) => set("alternate", event.target.value)}><option value="">None</option>{alternates.map((character) => <option key={character.id} value={character.id}>{character.name}</option>)}</Select></FormField><label className="flex items-center gap-3 text-content"><input type="checkbox" checked={values.active} onChange={(event) => set("active", event.target.checked)} className="size-4" />Initially active</label>{error && <p className="text-sm text-danger" role="alert">{error}</p>}<div className="flex justify-end gap-3 border-t border-border pt-4"><Button onClick={onCancel} disabled={saving}>Cancel</Button><Button type="submit" disabled={saving} variant="primary">{saving ? "Saving..." : "Save stats"}</Button></div></form>;
}

function NumberField({ id, label, value, change, optional, min = 0 }: { id: string; label: string; value: string; change: (value: string) => void; optional?: boolean; min?: number }) { return <FormField htmlFor={id} label={label}><Input id={id} type="number" min={min} value={value} onChange={(event) => change(event.target.value)} required={!optional} /></FormField>; }
function numberOrNull(value: string): number | null { return value === "" ? null : Number(value); }
function text(value: number | null): string { return value === null ? "" : String(value); }
