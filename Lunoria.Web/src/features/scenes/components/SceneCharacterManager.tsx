import { useEffect, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft, faBookOpen, faPen, faPlus, faTrash, faUser } from "@fortawesome/free-solid-svg-icons";
import { useConfirmDialog, useToast } from "@/app/providers";
import { Button, FormField, Input, Select } from "@/components/ui";
import { CharacterType, listCharacters, type Character } from "@/features/characters";
import {
  addSceneCharacter,
  deleteSceneCharacter,
  listSceneCharacters,
  replaceSceneCharacterSpells,
  updateSceneCharacter,
} from "@/features/scenes/api/scenesApi";
import type { Scene, SceneCharacter, SceneCharacterInput } from "@/features/scenes/types";
import { listSpells, type Spell } from "@/features/spells";
import { getApiError } from "@/lib/apiClient";

interface Props { scene: Scene }
type View = "characters" | "attach" | "edit" | "spells";

export function SceneCharacterManager({ scene }: Props) {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [assignments, setAssignments] = useState<SceneCharacter[]>([]);
  const [selectedId, setSelectedId] = useState<number>();
  const [catalog, setCatalog] = useState<Character[]>([]);
  const [spells, setSpells] = useState<Spell[]>([]);
  const [view, setView] = useState<View>("characters");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const selected = assignments.find((item) => item.id === selectedId);

  const load = async () => {
    setIsLoading(true); setError("");
    try { setAssignments(await listSceneCharacters(scene.id)); }
    catch (requestError) { setError(getApiError(requestError).message); }
    finally { setIsLoading(false); }
  };

  useEffect(() => {
    let current = true;
    void listSceneCharacters(scene.id)
      .then((loaded) => { if (current) { setAssignments(loaded); setError(""); } })
      .catch((requestError: unknown) => { if (current) setError(getApiError(requestError).message); })
      .finally(() => { if (current) setIsLoading(false); });
    return () => { current = false; };
  }, [scene.id]);

  const openAttach = async () => {
    try {
      if (catalog.length === 0) setCatalog(await listCharacters({ typeFilter: CharacterType.Any, take: 500 }));
      setView("attach");
    } catch (requestError) { toast.error(getApiError(requestError).message, "Unable to load characters"); }
  };

  const openSpells = async (assignment: SceneCharacter) => {
    try {
      if (spells.length === 0) setSpells(await listSpells({ take: 500 }));
      setSelectedId(assignment.id); setView("spells");
    } catch (requestError) { toast.error(getApiError(requestError).message, "Unable to load spells"); }
  };

  const remove = async (assignment: SceneCharacter) => {
    const confirmed = await confirm({
      title: `Remove "${assignment.character.name}" from this scene?`,
      message: "Its scene-specific settings and spells will be deleted.",
      confirmLabel: "Remove", variant: "danger",
    });
    if (!confirmed) return;
    try {
      await deleteSceneCharacter(assignment.id);
      setAssignments((current) => current.filter((item) => item.id !== assignment.id));
      toast.success(`${assignment.character.name} was removed from the scene.`);
    } catch (requestError) { toast.error(getApiError(requestError).message, "Unable to remove character"); }
  };

  if (isLoading) return <p className="text-content-secondary">Loading scene characters...</p>;
  if (error) return <div className="space-y-4"><p className="rounded-lg border border-danger/40 p-3 text-danger" role="alert">{error}</p><Button onClick={() => void load()} variant="primary">Try again</Button></div>;

  if (view === "attach") {
    const assignedIds = new Set(assignments.map((item) => item.characterId));
    const available = catalog.filter((character) =>
      character.characterType !== CharacterType.Player &&
      !assignedIds.has(character.id),
    );
    return <AttachForm characters={available} onCancel={() => setView("characters")} onSave={async (characterId) => {
      const saved = await addSceneCharacter(scene.id, characterId);
      setAssignments((current) => [...current, saved]); setView("characters");
      toast.success(`${saved.character.name} was attached to the scene.`);
    }} />;
  }

  if (view === "edit" && selected) {
    return <EditForm assignment={selected} characters={catalog} onCancel={() => setView("characters")} onSave={async (input) => {
      const saved = await updateSceneCharacter(selected.id, input);
      setAssignments((current) => current.map((item) => item.id === saved.id ? saved : item));
      setView("characters"); toast.success(`${saved.character.name} was updated.`);
    }} />;
  }

  if (view === "spells" && selected) {
    return <SpellManager assignment={selected} spells={spells} onBack={() => setView("characters")} onSave={async (spellIds) => {
      const saved = await replaceSceneCharacterSpells(selected.id, spellIds);
      setAssignments((current) => current.map((item) => item.id === saved.id ? saved : item));
      toast.success("Scene character spells were updated.");
    }} />;
  }

  return <div>
    <div className="flex items-center justify-between gap-3">
      <p className="text-sm text-content-secondary">Characters participating in {scene.name}.</p>
      <Button onClick={() => void openAttach()} variant="add" size="sm" leftIcon={<FontAwesomeIcon icon={faPlus} />}>Attach character</Button>
    </div>
    {assignments.length === 0 ? <EmptyState /> : <div className="mt-5 space-y-3">
      {assignments.map((assignment) => <article key={assignment.id} className="rounded-xl border border-border bg-surface p-4">
        <div className="flex items-center gap-3">
          {assignment.character.photoUrl ? <img src={assignment.character.photoUrl} alt="" className="size-12 rounded-lg object-cover" /> : <span className="flex size-12 items-center justify-center rounded-lg bg-utility/15 text-utility-hover"><FontAwesomeIcon icon={faUser} /></span>}
          <div className="min-w-0 flex-1"><h3 className="font-semibold text-content">{assignment.character.name}</h3><p className="text-sm text-content-secondary">HP {assignment.maxHp} · MP {assignment.maxMp} · {assignment.isInitiallyActive ? "Initially active" : "Initially inactive"}</p></div>
        </div>
        <div className="mt-3 flex flex-wrap justify-end gap-2 border-t border-border pt-3">
          <Button onClick={() => void remove(assignment)} variant="danger" size="sm" leftIcon={<FontAwesomeIcon icon={faTrash} />}>Remove</Button>
          <Button onClick={() => void openSpells(assignment)} variant="magic" size="sm" leftIcon={<FontAwesomeIcon icon={faBookOpen} />}>Spells</Button>
          <Button onClick={async () => { if (catalog.length === 0) { try { setCatalog(await listCharacters({ typeFilter: CharacterType.Any, take: 500 })); } catch (requestError) { toast.error(getApiError(requestError).message); return; } } setSelectedId(assignment.id); setView("edit"); }} variant="primary" size="sm" leftIcon={<FontAwesomeIcon icon={faPen} />}>Edit</Button>
        </div>
      </article>)}
    </div>}
  </div>;
}

function AttachForm({ characters, onSave, onCancel }: { characters: Character[]; onSave: (id: number) => Promise<void>; onCancel: () => void }) {
  const [id, setId] = useState(""); const [saving, setSaving] = useState(false); const [error, setError] = useState("");
  const submit = async (event: FormEvent) => { event.preventDefault(); setSaving(true); setError(""); try { await onSave(Number(id)); } catch (requestError) { setError(getApiError(requestError).message); setSaving(false); } };
  return <form onSubmit={(event) => void submit(event)} className="space-y-5"><h3 className="text-2xl font-semibold text-content">Attach character</h3>{characters.length === 0 ? <p className="text-content-secondary">Every available NPC and enemy is already attached.</p> : <FormField htmlFor="scene-character" label="NPC or enemy"><Select id="scene-character" value={id} onChange={(e) => setId(e.target.value)} required><option value="" disabled>Select an NPC or enemy</option>{characters.map((character) => <option key={character.id} value={character.id}>{character.name}</option>)}</Select></FormField>}<FormActions saving={saving} error={error} onCancel={onCancel} disableSave={characters.length === 0} /></form>;
}

function EditForm({ assignment, characters, onSave, onCancel }: { assignment: SceneCharacter; characters: Character[]; onSave: (input: SceneCharacterInput) => Promise<void>; onCancel: () => void }) {
  const [values, setValues] = useState({ melee: optionalNumber(assignment.meleeAttackDamage), bow: optionalNumber(assignment.bowAttackDamage), movement: String(assignment.movement), consumables: String(assignment.maxConsumableInventory), equipment: String(assignment.maxEquippableInventory), hp: String(assignment.maxHp), mp: String(assignment.maxMp), active: assignment.isInitiallyActive, alternate: String(assignment.alternateForm?.id ?? "") });
  const [saving, setSaving] = useState(false); const [error, setError] = useState("");
  const set = (key: keyof typeof values, value: string | boolean) => setValues((current) => ({ ...current, [key]: value }));
  const submit = async (event: FormEvent) => { event.preventDefault(); setSaving(true); setError(""); try { await onSave({ meleeAttackDamage: nullableNumber(values.melee), bowAttackDamage: nullableNumber(values.bow), movement: Number(values.movement), maxConsumableInventory: Number(values.consumables), maxEquippableInventory: Number(values.equipment), maxHp: Number(values.hp), maxMp: Number(values.mp), isInitiallyActive: values.active, alternateFormId: nullableNumber(values.alternate) }); } catch (requestError) { setError(getApiError(requestError).message); setSaving(false); } };
  const alternates = characters.filter((character) => character.id !== assignment.characterId);
  return <form onSubmit={(event) => void submit(event)} className="space-y-5"><h3 className="text-2xl font-semibold text-content">Edit {assignment.character.name}</h3><div className="grid gap-4 sm:grid-cols-2"><NumberField id="sc-hp" label="Max HP" value={values.hp} setValue={(v) => set("hp", v)} min={1} /><NumberField id="sc-mp" label="Max MP" value={values.mp} setValue={(v) => set("mp", v)} /><NumberField id="sc-melee" label="Melee damage" value={values.melee} setValue={(v) => set("melee", v)} optional /><NumberField id="sc-bow" label="Bow damage" value={values.bow} setValue={(v) => set("bow", v)} optional /><NumberField id="sc-movement" label="Movement" value={values.movement} setValue={(v) => set("movement", v)} /><NumberField id="sc-consumables" label="Consumable slots" value={values.consumables} setValue={(v) => set("consumables", v)} /><NumberField id="sc-equipment" label="Equipment slots" value={values.equipment} setValue={(v) => set("equipment", v)} /></div><FormField htmlFor="sc-alternate" label="Alternate form"><Select id="sc-alternate" value={values.alternate} onChange={(e) => set("alternate", e.target.value)}><option value="">None</option>{alternates.map((character) => <option key={character.id} value={character.id}>{character.name}</option>)}</Select></FormField><label className="flex items-center gap-3 text-content"><input type="checkbox" checked={values.active} onChange={(e) => set("active", e.target.checked)} className="size-4" />Initially active</label><FormActions saving={saving} error={error} onCancel={onCancel} /></form>;
}

function SpellManager({ assignment, spells, onBack, onSave }: { assignment: SceneCharacter; spells: Spell[]; onBack: () => void; onSave: (ids: number[]) => Promise<void> }) {
  const [selection, setSelection] = useState(() => new Set(assignment.sceneCharacterSpells.map((item) => item.spell.id))); const [saving, setSaving] = useState(false); const [error, setError] = useState("");
  const toggle = (id: number) => setSelection((current) => { const next = new Set(current); if (next.has(id)) next.delete(id); else next.add(id); return next; });
  const save = async () => { setSaving(true); setError(""); try { await onSave([...selection]); setSaving(false); } catch (requestError) { setError(getApiError(requestError).message); setSaving(false); } };
  return <div><Button onClick={onBack} size="sm" leftIcon={<FontAwesomeIcon icon={faArrowLeft} />}>All characters</Button><h3 className="mt-5 text-2xl font-semibold text-content">{assignment.character.name} spells</h3>{spells.length === 0 ? <p className="mt-4 text-content-secondary">No spells are available.</p> : <div className="mt-5 space-y-2">{spells.map((spell) => <label key={spell.id} className="flex cursor-pointer items-center gap-3 rounded-xl border border-border bg-surface p-4"><input type="checkbox" checked={selection.has(spell.id)} onChange={() => toggle(spell.id)} className="size-4" /><span><span className="block font-semibold text-content">{spell.name}</span><span className="text-sm text-content-secondary">{spell.spellType?.name ?? "Spell"} · MP {spell.mpCost}</span></span></label>)}</div>}<div className="mt-5"><FormActions saving={saving} error={error} onCancel={onBack} onSave={() => void save()} /></div></div>;
}

function NumberField({ id, label, value, setValue, optional, min = 0 }: { id: string; label: string; value: string; setValue: (v: string) => void; optional?: boolean; min?: number }) { return <FormField htmlFor={id} label={label}><Input id={id} type="number" min={min} value={value} onChange={(e) => setValue(e.target.value)} required={!optional} /></FormField>; }
function FormActions({ saving, error, onCancel, onSave, disableSave }: { saving: boolean; error: string; onCancel: () => void; onSave?: () => void; disableSave?: boolean }) { return <div className="border-t border-border pt-4">{error && <p className="mb-3 text-sm text-danger" role="alert">{error}</p>}<div className="flex justify-end gap-3"><Button onClick={onCancel} disabled={saving}>Cancel</Button><Button type={onSave ? "button" : "submit"} onClick={onSave} disabled={saving || disableSave} variant="primary">{saving ? "Saving..." : "Save"}</Button></div></div>; }
function EmptyState() { return <div className="mt-5 rounded-xl border border-border bg-surface/60 p-8 text-center"><h3 className="text-xl font-semibold text-content">No scene characters yet</h3><p className="mt-2 text-sm text-content-secondary">Attach the first character to this scene.</p></div>; }
function nullableNumber(value: string): number | null { return value === "" ? null : Number(value); }
function optionalNumber(value: number | null): string { return value === null ? "" : String(value); }
