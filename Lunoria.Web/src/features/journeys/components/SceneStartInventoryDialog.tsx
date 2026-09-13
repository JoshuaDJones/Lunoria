import { useEffect, useRef, useState } from "react";
import { Button } from "@/components/ui";
import type {
  SceneInventoryResolutionInput,
  SceneStartInventory,
} from "../types";

interface Props {
  pending: SceneStartInventory;
  busy: boolean;
  error: string;
  onResolve: (input: SceneInventoryResolutionInput) => void;
  onReload: () => void;
}

export function SceneStartInventoryDialog({
  pending,
  busy,
  error,
  onResolve,
  onReload,
}: Props) {
  const dialog = useRef<HTMLDialogElement>(null);
  const [selection, setSelection] = useState("reward");
  const [mode, setMode] = useState<"select" | "discard" | "transfer">("select");
  useEffect(() => {
    const element = dialog.current;
    element?.showModal();
    return () => element?.close();
  }, []);
  const selected = pending.items.find((item) => String(item.id) === selection);
  const name = selected?.name ?? pending.rewardName;
  const recipientIds = selected?.recipientIds ?? pending.rewardRecipientIds;
  const resolve = (targetJourneyCharacterId: number | null) =>
    onResolve({
      resolutionToken: pending.resolutionToken,
      inventoryItemId: selected?.id ?? null,
      targetJourneyCharacterId,
    });

  return (
    <dialog
      ref={dialog}
      onCancel={(event) => event.preventDefault()}
      aria-labelledby="inventory-resolution-title"
      className="m-auto w-[min(95vw,36rem)] max-h-[90dvh] overflow-y-auto rounded-xl border border-border bg-surface p-6 text-content shadow-xl backdrop:bg-black/70"
    >
      <h2 id="inventory-resolution-title" className="text-2xl font-semibold">
        Inventory full
      </h2>
      <p className="mt-3">
        {pending.characterName} cannot receive {pending.rewardName} yet.
      </p>
      <p className="mt-2 text-sm">
        {pending.isEquippable ? "Equipment" : "Consumables"}:{" "}
        {pending.inventoryCount}/{pending.inventoryCapacity}. Rewards remaining:{" "}
        {pending.remainingQuantity}.
      </p>
      <p className="mt-2 text-sm">
        Discard an item or give it to another player to continue the scene. You
        can also discard or give away the incoming reward.
      </p>
      {error && (
        <p role="alert" className="mt-4 text-red-400">
          {error}
        </p>
      )}
      <fieldset disabled={busy} className="mt-5 space-y-4">
        {mode === "select" ? (
          <>
            <label className="block" htmlFor="inventory-resolution-item">
              Choose an item
            </label>
            <select
              id="inventory-resolution-item"
              className="w-full rounded border border-border bg-surface p-3 text-content"
              value={selection}
              onChange={(event) => setSelection(event.target.value)}
            >
              <option value="reward">
                Incoming reward: {pending.rewardName} (one)
              </option>
              {pending.items.map((item, index) => (
                <option key={item.id} value={String(item.id)}>
                  {index + 1}. {item.name}
                </option>
              ))}
            </select>
            <div className="flex flex-wrap gap-3">
              <Button onClick={() => setMode("discard")}>Discard…</Button>
              <Button onClick={() => setMode("transfer")}>
                Give to another player…
              </Button>
            </div>
          </>
        ) : mode === "discard" ? (
          <>
            <p>
              Discard one {name}? This removes it permanently from this
              playthrough.
            </p>
            <Button onClick={() => resolve(null)}>Confirm discard</Button>
          </>
        ) : (
          <>
            <h3 className="font-semibold">Give one {name} to:</h3>
            <div className="flex flex-col gap-2">
              {pending.recipients.map((recipient) => (
                <Button
                  key={recipient.id}
                  disabled={!recipientIds.includes(recipient.id)}
                  onClick={() => resolve(recipient.id)}
                >
                  {recipient.name}
                  {!recipientIds.includes(recipient.id)
                    ? " — no capacity for this item"
                    : ""}
                </Button>
              ))}
            </div>
            {!recipientIds.length && (
              <p>
                No other active player has room for this item. Go back and
                choose another item or discard it.
              </p>
            )}
          </>
        )}
        {mode !== "select" && (
          <Button variant="utility" onClick={() => setMode("select")}>
            Back
          </Button>
        )}
        {error && (
          <Button variant="utility" onClick={onReload}>
            Reload inventory
          </Button>
        )}
      </fieldset>
      {busy && (
        <p role="status" className="mt-4">
          Updating inventory…
        </p>
      )}
    </dialog>
  );
}
