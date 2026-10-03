import { useState } from "react";
import { Button } from "@/components/ui";
import { type ScenePlaythroughParticipant } from "@/features/journeys";

function TransformConfirmation({
  participant,
  onConfirm,
  onCancel,
}: {
  participant: ScenePlaythroughParticipant;
  onConfirm: () => Promise<void>;
  onCancel: () => void;
}) {
  const [busy, setBusy] = useState(false);
  return (
    <div className="space-y-5">
      <p className="text-content-secondary">
        {participant.isInAlternateForm
          ? `Return ${participant.name} to normal form?`
          : `Transform ${participant.name} into ${participant.alternateForm?.name ?? "their alternate form"}?`}{" "}
        This ends the current turn. HP, MP, and inventory are preserved.
      </p>
      <div className="flex justify-end gap-3">
        <Button disabled={busy} onClick={onCancel}>
          Cancel
        </Button>
        <Button
          variant="primary"
          disabled={busy}
          onClick={() => {
            setBusy(true);
            void onConfirm().finally(() => setBusy(false));
          }}
        >
          {busy ? "Transforming..." : "Confirm"}
        </Button>
      </div>
    </div>
  );
}

export default TransformConfirmation;
