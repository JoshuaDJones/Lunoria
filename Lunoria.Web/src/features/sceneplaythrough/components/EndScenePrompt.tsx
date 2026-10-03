import { useState } from "react";
import { Button } from "@/components/ui";

function EndScenePrompt({
  sceneName,
  onConfirm,
}: {
  sceneName: string;
  onConfirm: () => Promise<void>;
}) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  return (
    <div>
      <p className="text-content-secondary">
        End {sceneName}? The scene will be marked completed and cannot be
        resumed.
      </p>
      <div className="mt-6 flex justify-end">
        <Button
          size="lg"
          variant="danger"
          inverted
          disabled={isSubmitting}
          onClick={() => {
            setIsSubmitting(true);
            void onConfirm().catch(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Ending..." : "End Scene"}
        </Button>
      </div>
    </div>
  );
}

export default EndScenePrompt;
