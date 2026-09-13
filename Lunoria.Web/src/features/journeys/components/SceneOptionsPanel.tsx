import { useState, type FormEvent, type ReactNode } from "react";
import { Button, FormField, Input, Select } from "@/components/ui";
import type {
  ScenePlaythroughDetails,
  ScenePlaythroughDialog,
  ScenePlaythroughParticipant,
} from "@/features/journeys/types";
import type {
  AddSceneChestInput,
  UpdateSceneParticipantStatsInput,
} from "@/features/journeys/api/journeysApi";

interface SceneOptionsPanelProps {
  scene: ScenePlaythroughDetails;
  busyAction?: string;
  onActivateJourneyCharacter: (id: number) => void;
  onAddPlaythroughCharacter: (id: number) => void;
  onRemoveParticipant: (id: number) => void;
  onAddChest: (input: AddSceneChestInput) => void;
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
  onRemoveParticipant,
  onAddChest,
  onUpdateParticipant,
  onViewDialog,
  onEndScene,
}: SceneOptionsPanelProps) {
  const journeyCharacters = scene.journeyCharacters ?? [];
  const playthroughCharacters = scene.playthroughCharacters ?? [];
  const participants = scene.participants ?? [];
  const dialogs = scene.dialogs ?? [];
  const gridUrl = scene.gridUrl?.trim();
  const openGrid = () => {
    const url = scene.grid
      ? `${window.location.origin}/playthroughs/${scene.playthroughId}/scene-grids/${scene.id}`
      : gridUrl
        ? /^https?:\/\//i.test(gridUrl)
          ? gridUrl
          : `https://${gridUrl}`
        : undefined;
    if (!url) return;
    window.open(
      url,
      "_blank",
      `popup=yes,width=${window.screen.availWidth},height=${window.screen.availHeight},left=0,top=0,noopener,noreferrer`,
    );
  };
  const availableJourneyCharacters = journeyCharacters.filter(
    (character) => !character.isParticipant,
  );
  const [participantId, setParticipantId] = useState(participants[0]?.id ?? 0);
  const participant = participants.find((item) => item.id === participantId);
  const removableParticipants = participants.filter(
    (item) =>
      item.journeyPlaythroughCharacterId === null &&
      item.scenePlaythroughCharacterId !== null,
  );
  const [removalId, setRemovalId] = useState<number>();
  const removal = removableParticipants.find((item) => item.id === removalId);
  const removalDisabled =
    busyAction !== undefined || !!scene.counterattackToken;

  return (
    <div className="space-y-8">
      <OptionSection title="Scene Grid">
        {scene.grid || gridUrl ? (
          <Button onClick={openGrid} variant="primary">
            Open grid
          </Button>
        ) : (
          <EmptyMessage>No grid is assigned to this scene.</EmptyMessage>
        )}
      </OptionSection>
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

      <OptionSection title="Remove Scene Character">
        <p className="mb-3 text-sm text-content-secondary">
          Remove a scene character from participation, without a defeat or loot
          reward. Journey characters cannot be removed here.
        </p>
        {scene.counterattackToken && (
          <p className="mb-3 text-sm text-content-muted">
            Resolve or pass the counterattack first.
          </p>
        )}
        {removableParticipants.length === 0 ? (
          <EmptyMessage>
            No scene characters are available to remove.
          </EmptyMessage>
        ) : (
          <div className="space-y-3">
            {removableParticipants.map((item) => (
              <OptionRow
                key={item.id}
                name={`${item.name} (#${item.id})`}
                imageUrl={item.portraitUrl || item.photoUrl}
                actionLabel="Remove"
                disabled={removalDisabled}
                busy={busyAction === `remove-${item.id}`}
                onAction={() => setRemovalId(item.id)}
              />
            ))}
          </div>
        )}
        {removal && (
          <div className="mt-3 rounded-xl border border-danger/40 p-4">
            <p>
              Remove {removal.name} (#{removal.id}) from this scene?
              {removal.isCurrentParticipant &&
                " Their turn will end and the rotation will advance."}
            </p>
            <div className="mt-3 flex gap-3">
              <Button
                variant="danger"
                disabled={removalDisabled}
                onClick={() => {
                  onRemoveParticipant(removal.id);
                  setRemovalId(undefined);
                }}
              >
                Confirm removal
              </Button>
              <Button
                disabled={busyAction !== undefined}
                onClick={() => setRemovalId(undefined)}
              >
                Cancel
              </Button>
            </div>
          </div>
        )}
      </OptionSection>

      <OptionSection title="Add Chest">
        <LiveChestForm
          scene={scene}
          disabled={busyAction !== undefined}
          busy={busyAction === "add-chest"}
          onSubmit={onAddChest}
        />
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
                onChange={(event) =>
                  setParticipantId(Number(event.target.value))
                }
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

interface ChestFaceDraft {
  itemKey: string;
  quantity: number;
}

function LiveChestForm({
  scene,
  disabled,
  busy,
  onSubmit,
}: {
  scene: ScenePlaythroughDetails;
  disabled: boolean;
  busy: boolean;
  onSubmit: (input: AddSceneChestInput) => void;
}) {
  const [name, setName] = useState("");
  const [faces, setFaces] = useState<ChestFaceDraft[]>(() =>
    Array.from({ length: 6 }, () => ({ itemKey: "", quantity: 1 })),
  );
  const hasItems =
    scene.availableConsumableItems.length > 0 ||
    scene.availableEquippableItems.length > 0;
  const canSubmit =
    name.trim().length > 0 && faces.every((face) => face.itemKey !== "");

  const updateFace = (index: number, update: Partial<ChestFaceDraft>) => {
    setFaces((current) =>
      current.map((face, faceIndex) =>
        faceIndex === index ? { ...face, ...update } : face,
      ),
    );
  };

  const submit = (event: FormEvent) => {
    event.preventDefault();
    if (!canSubmit) return;

    onSubmit({
      name: name.trim(),
      dieSides: 6,
      lootEntries: faces.map((face, index) => {
        const [itemType, itemId] = face.itemKey.split(":");
        return {
          rollMinimum: index + 1,
          rollMaximum: index + 1,
          quantity: Math.max(1, face.quantity),
          playthroughEquippableItemId:
            itemType === "equippable" ? Number(itemId) : null,
          playthroughConsumableItemId:
            itemType === "consumable" ? Number(itemId) : null,
        };
      }),
    });
  };

  return (
    <form className="space-y-4" onSubmit={submit}>
      <p className="text-sm text-content-secondary">
        {scene.chests.length === 0
          ? "No chests are currently in this scene."
          : `${scene.chests.length} chest${scene.chests.length === 1 ? " is" : "s are"} currently in this scene.`}
      </p>

      {!hasItems ? (
        <EmptyMessage>
          This playthrough has no consumable or equippable items available for
          chest loot.
        </EmptyMessage>
      ) : (
        <>
          <FormField htmlFor="live-chest-name" label="Chest name">
            <Input
              id="live-chest-name"
              maxLength={250}
              value={name}
              placeholder="Treasure chest"
              disabled={disabled}
              onChange={(event) => setName(event.target.value)}
            />
          </FormField>

          <div className="space-y-3">
            {faces.map((face, index) => (
              <div
                key={index + 1}
                className="grid grid-cols-[auto_minmax(0,1fr)_5rem] items-end gap-2 rounded-xl border border-border bg-surface p-3"
              >
                <span className="self-center text-sm font-semibold text-content">
                  {index + 1}
                </span>
                <FormField
                  htmlFor={`live-chest-face-${index + 1}`}
                  label="Item"
                >
                  <Select
                    id={`live-chest-face-${index + 1}`}
                    value={face.itemKey}
                    disabled={disabled}
                    onChange={(event) =>
                      updateFace(index, { itemKey: event.target.value })
                    }
                  >
                    <option value="">Choose item...</option>
                    {scene.availableConsumableItems.length > 0 && (
                      <optgroup label="Consumables">
                        {scene.availableConsumableItems.map((item) => (
                          <option key={item.id} value={`consumable:${item.id}`}>
                            {item.name}
                          </option>
                        ))}
                      </optgroup>
                    )}
                    {scene.availableEquippableItems.length > 0 && (
                      <optgroup label="Equipment">
                        {scene.availableEquippableItems.map((item) => (
                          <option key={item.id} value={`equippable:${item.id}`}>
                            {item.name}
                          </option>
                        ))}
                      </optgroup>
                    )}
                  </Select>
                </FormField>
                <FormField
                  htmlFor={`live-chest-quantity-${index + 1}`}
                  label="Qty"
                >
                  <Input
                    id={`live-chest-quantity-${index + 1}`}
                    type="number"
                    min={1}
                    value={face.quantity}
                    disabled={disabled}
                    onChange={(event) =>
                      updateFace(index, {
                        quantity: Math.max(1, Number(event.target.value)),
                      })
                    }
                  />
                </FormField>
              </div>
            ))}
          </div>

          <Button
            type="submit"
            variant="add"
            className="w-full"
            disabled={disabled || !canSubmit}
          >
            {busy ? "Adding Chest..." : "Add Chest to Scene"}
          </Button>
        </>
      )}
    </form>
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
      <StatInput
        label="Current HP"
        value={stats.currentHp}
        onChange={(value) => setNumber("currentHp", value)}
      />
      <StatInput
        label="Max HP"
        value={stats.maxHp}
        onChange={(value) => setNumber("maxHp", value)}
        min={1}
      />
      <StatInput
        label="Current MP"
        value={stats.currentMp}
        onChange={(value) => setNumber("currentMp", value)}
      />
      <StatInput
        label="Max MP"
        value={stats.maxMp}
        onChange={(value) => setNumber("maxMp", value)}
      />
      <StatInput
        label="Movement"
        value={stats.movement}
        min={-2147483648}
        onChange={(value) => setNumber("movement", value)}
      />
      <StatInput
        label="Melee"
        value={stats.meleeAttackDamage}
        min={-2147483648}
        onChange={(value) => setNumber("meleeAttackDamage", value, true)}
      />
      <StatInput
        label="Bow"
        value={stats.bowAttackDamage}
        min={-2147483648}
        onChange={(value) => setNumber("bowAttackDamage", value, true)}
      />
      <Button
        type="submit"
        variant="primary"
        className="self-end"
        disabled={disabled}
      >
        {busy ? "Saving..." : "Save Stats"}
      </Button>
    </form>
  );
}

function StatInput({
  label,
  value,
  onChange,
  min = 0,
}: {
  label: string;
  value: number | null;
  onChange: (value: string) => void;
  min?: number;
}) {
  const id = `participant-stat-${label.toLowerCase().replace(" ", "-")}`;
  return (
    <FormField htmlFor={id} label={label}>
      <Input
        id={id}
        type="number"
        min={min}
        value={value ?? ""}
        onChange={(event) => onChange(event.target.value)}
      />
    </FormField>
  );
}

function OptionSection({
  title,
  children,
}: {
  title: string;
  children: ReactNode;
}) {
  return (
    <section>
      <h3 className="mb-3 text-xl font-semibold text-content">{title}</h3>
      {children}
    </section>
  );
}

function OptionRow({
  name,
  imageUrl,
  actionLabel,
  disabled,
  busy,
  onAction,
}: {
  name: string;
  imageUrl: string | null;
  actionLabel: string;
  disabled: boolean;
  busy: boolean;
  onAction: () => void;
}) {
  return (
    <div className="flex items-center gap-3 rounded-xl border border-border bg-surface p-3">
      {imageUrl ? (
        <img
          src={imageUrl}
          alt=""
          className="h-12 w-12 rounded-lg object-contain"
        />
      ) : (
        <div className="h-12 w-12 rounded-lg bg-surface-raised" />
      )}
      <p className="min-w-0 flex-1 truncate font-semibold text-content">
        {name}
      </p>
      <Button disabled={disabled} onClick={onAction}>
        {busy ? "Working..." : actionLabel}
      </Button>
    </div>
  );
}

function EmptyMessage({ children }: { children: ReactNode }) {
  return <p className="text-sm text-content-muted">{children}</p>;
}
