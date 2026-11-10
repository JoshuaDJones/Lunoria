import { useRef, useState } from "react";
import { Button } from "@/components/ui";
import { getApiError } from "@/lib/apiClient";

export function UndoTurnPrompt({ inProgress, onConfirm, onCancel, onBusyChange }: {
  inProgress: boolean;
  onConfirm: () => Promise<void>;
  onCancel: () => void;
  onBusyChange: (busy: boolean) => void;
}) {
  const pending = useRef(false);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const confirm = async () => {
    if (pending.current) return;
    pending.current = true;
    setBusy(true);
    setError("");
    onBusyChange(true);
    try { await onConfirm(); }
    catch (error) { setError(getApiError(error).message); }
    finally {
      pending.current = false;
      setBusy(false);
      onBusyChange(false);
    }
  };
  return (
    <div className="space-y-5">
      <p className="text-content-secondary">
        Undo {inProgress ? "the current" : "the last completed"} turn?
        HP, MP, inventory, chests, transformations, character activations and
        counterattacks will return to their state before the turn began.
        Move any physical pieces back on the board as needed.
      </p>
      <p className="text-sm text-content-muted">Only one turn can be undone. The event history will record the undo.</p>
      {error && <p role="alert" className="text-danger">{error}</p>}
      <div className="flex justify-end gap-3">
        <Button disabled={busy} onClick={onCancel}>Cancel</Button>
        <Button variant="danger" disabled={busy} onClick={() => void confirm()}>
          {busy && <span role="status" aria-label="Undoing turn" className="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent" />}
          Undo turn
        </Button>
      </div>
    </div>
  );
}
