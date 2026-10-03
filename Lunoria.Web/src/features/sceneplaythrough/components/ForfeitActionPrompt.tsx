import { useState } from "react";
import { Button } from "@/components/ui";

function ForfeitActionPrompt({
  participantName,
  onConfirm,
}: {
  participantName: string;
  onConfirm: () => Promise<void>;
}) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  return (
    <div>
      <p className="text-content-secondary">
        End {participantName}&apos;s turn without taking an action?
      </p>
      <div className="mt-6 flex justify-end">
        <Button
          size="lg"
          variant="danger"
          inverted
          disabled={isSubmitting}
          onClick={() => {
            setIsSubmitting(true);
            void onConfirm().finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Forfeiting..." : "Forfeit Action"}
        </Button>
      </div>
    </div>
  );
}

export default ForfeitActionPrompt;
