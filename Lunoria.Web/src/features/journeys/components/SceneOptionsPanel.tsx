import { useState, type FormEvent, type ReactNode } from "react";
import { Button, FormField, Input, Select } from "@/components/ui";
import type {
  ScenePlaythroughDetails,
  ScenePlaythroughDialog,
  ScenePlaythroughParticipant,
} from "@/features/journeys/types";
import type { UpdateSceneParticipantStatsInput } from "@/features/journeys/api/journeysApi";

interface SceneOptionsPanelProps {
  scene: ScenePlaythroughDetails;
  busyAction?: string;
  onActivateJourneyCharacter: (id: number) => void;
  onAddPlaythroughCharacter: (id: number) => void;
  onUpdateParticipant: (
    id: number,
    input: UpdateSceneParticipantStatsInput,
  ) => void;
  onViewDialog: (dialog: ScenePlaythroughDialog) => void;
  onEndScene: () => void;
}

export function SceneOptionsPanel({
  scene,
  busyAction,
  onActivateJourneyCharacter,
  onAddPlaythroughCharacter,
  onUpdateParticipant,
  onViewDialog,
  onEndScene,
}: SceneOptionsPanelProps) {
  const journeyCharacters = scene.journeyCharacters ?? [];
  const playthroughCharacters = scene.playthroughCharacters ?? [];
  const participants = scene.participants ?? [];
  const dialogs = scene.dialogs ?? [];
  const availableJourneyCharacters = journeyCharacters.filter(
    (character) => !character.isParticipant,
  );
  const [participantId, setParticipantId] = useState(
    participants[0]?.id ?? 0,
  );
  const participant = participants.find(
    (item) => item.id === participantId,
  );

  return (
    <div className="space-y-8">
      <OptionSection title="Activate Journey Characters">
        {availableJourneyCharacters.length === 0 ? (
          <EmptyMessage>All journey characters are participating.</EmptyMessage>
        ) : (
          <div className="space-y-3">
            {availableJourneyCharacters.map((character) => (
              <OptionRow
                key={character.id}
                name={character.name}
                imageUrl={character.portraitUrl || character.photoUrl}
                actionLabel={character.isActive ? "Add" : "Activate"}
                disabled={busyAction !== undefined}
                busy={busyAction === `activate-${character.id}`}
                onAction={() => onActivateJourneyCharacter(character.id)}
              />
            ))}
          </div>
        )}
      </OptionSection>

      <OptionSection title="Add Scene Character">
        {playthroughCharacters.length === 0 ? (
          <EmptyMessage>No NPC or enemy characters are available.</EmptyMessage>
        ) : (
          <div className="space-y-3">
            {playthroughCharacters.map((character) => (
              <OptionRow
                key={character.id}
                name={character.name}
                imageUrl={character.portraitUrl || character.photoUrl}
                actionLabel="Add"
                disabled={busyAction !== undefined}
                busy={busyAction === `add-${character.id}`}
                onAction={() => onAddPlaythroughCharacter(character.id)}
              />
            ))}
          </div>
        )}
      </OptionSection>

      <OptionSection title="Correct Participant Stats">
        {participants.length === 0 ? (
          <EmptyMessage>No participants are available.</EmptyMessage>
        ) : (
          <>
            <FormField htmlFor="participant-option" label="Participant">
              <Select
                id="participant-option"
                value={participantId}
                onChange={(event) => setParticipantId(Number(event.target.value))}
              >
                {participants.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.name} (#{item.id})
                  </option>
                ))}
              </Select>
            </FormField>
            {participant && (
              <ParticipantStatsEditor
                key={participant.id}
                participant={participant}
                disabled={busyAction !== undefined}
                busy={busyAction === `stats-${participant.id}`}
                onSubmit={(input) => onUpdateParticipant(participant.id, input)}
              />
            )}
          </>
        )}
      </OptionSection>

      <OptionSection title="Dialogs">
        {dialogs.length === 0 ? (
          <EmptyMessage>This scene has no dialogs.</EmptyMessage>
        ) : (
          <div className="space-y-3">
            {dialogs.map((dialog) => (
              <div
                key={dialog.id}
                className="flex items-center justify-between gap-3 rounded-xl border border-border bg-surface p-3"
              >
                <div>
                  <p className="font-semibold text-content">{dialog.title}</p>
                  <p className="text-sm text-content-muted">
                    {dialog.dialogPages.length} pages
                  </p>
                </div>
                <Button onClick={() => onViewDialog(dialog)}>View</Button>
              </div>
            ))}
          </div>
        )}
      </OptionSection>

      <OptionSection title="End Scene">
        <div className="rounded-xl border border-danger/40 bg-danger/5 p-4">
          <p className="text-sm text-content-secondary">
            Complete this scene and return to the journey playthrough.
          </p>
          <Button
            variant="danger"
            inverted
            className="mt-4 w-full"
            disabled={busyAction !== undefined}
            onClick={onEndScene}
          >
            End Scene
          </Button>
        </div>
      </OptionSection>
    </div>
  );
}

function ParticipantStatsEditor({
  participant,
  disabled,
  busy,
  onSubmit,
}: {
  participant: ScenePlaythroughParticipant;
  disabled: boolean;
  busy: boolean;
  onSubmit: (input: UpdateSceneParticipantStatsInput) => void;
}) {
  const [stats, setStats] = useState<UpdateSceneParticipantStatsInput>({
    currentHp: participant.currentHp,
    maxHp: participant.maxHp,
    currentMp: participant.currentMp,
    maxMp: participant.maxMp,
    movement: participant.movement,
    meleeAttackDamage: participant.meleeAttackDamage,
    bowAttackDamage: participant.bowAttackDamage,
  });

  const setNumber = (
    field: keyof UpdateSceneParticipantStatsInput,
    value: string,
    nullable = false,
  ) => {
    setStats((current) => ({
      ...current,
      [field]: nullable && value === "" ? null : Number(value),
    }));
  };

  const submit = (event: FormEvent) => {
    event.preventDefault();
    onSubmit(stats);
  };

  return (
    <form className="mt-4 grid grid-cols-2 gap-3" onSubmit={submit}>
      <StatInput label="Current HP" value={stats.currentHp} onChange={(value) => setNumber("currentHp", value)} />
      <StatInput label="Max HP" value={stats.maxHp} onChange={(value) => setNumber("maxHp", value)} min={1} />
      <StatInput label="Current MP" value={stats.currentMp} onChange={(value) => setNumber("currentMp", value)} />
      <StatInput label="Max MP" value={stats.maxMp} onChange={(value) => setNumber("maxMp", value)} />
      <StatInput label="Movement" value={stats.movement} onChange={(value) => setNumber("movement", value)} />
      <StatInput label="Melee" value={stats.meleeAttackDamage} onChange={(value) => setNumber("meleeAttackDamage", value, true)} />
      <StatInput label="Bow" value={stats.bowAttackDamage} onChange={(value) => setNumber("bowAttackDamage", value, true)} />
      <Button type="submit" variant="primary" className="self-end" disabled={disabled}>
        {busy ? "Saving..." : "Save Stats"}
      </Button>
    </form>
  );
}

function StatInput({ label, value, onChange, min = 0 }: { label: string; value: number | null; onChange: (value: string) => void; min?: number }) {
  const id = `participant-stat-${label.toLowerCase().replace(" ", "-")}`;
  return (
    <FormField htmlFor={id} label={label}>
      <Input id={id} type="number" min={min} value={value ?? ""} onChange={(event) => onChange(event.target.value)} />
    </FormField>
  );
}

function OptionSection({ title, children }: { title: string; children: ReactNode }) {
  return (
    <section>
      <h3 className="mb-3 text-xl font-semibold text-content">{title}</h3>
      {children}
    </section>
  );
}

function OptionRow({ name, imageUrl, actionLabel, disabled, busy, onAction }: { name: string; imageUrl: string | null; actionLabel: string; disabled: boolean; busy: boolean; onAction: () => void }) {
  return (
    <div className="flex items-center gap-3 rounded-xl border border-border bg-surface p-3">
      {imageUrl ? <img src={imageUrl} alt="" className="h-12 w-12 rounded-lg object-contain" /> : <div className="h-12 w-12 rounded-lg bg-surface-raised" />}
      <p className="min-w-0 flex-1 truncate font-semibold text-content">{name}</p>
      <Button disabled={disabled} onClick={onAction}>{busy ? "Working..." : actionLabel}</Button>
    </div>
  );
}

function EmptyMessage({ children }: { children: ReactNode }) {
  return <p className="text-sm text-content-muted">{children}</p>;
}
