import { type AttackAnimationState } from "@/features/sceneplaythrough/types";
import { createPortal } from "react-dom";

const AttackAnimationOverlay = ({
  animation,
  inline = false,
}: {
  animation: AttackAnimationState;
  inline?: boolean;
}) => {
  const overlay = (
    <div
      className="attack-animation-overlay fixed inset-0 z-[9999] flex items-center justify-center bg-canvas/90 p-6 backdrop-blur-sm"
      role="status"
      aria-live="assertive"
      aria-label={`${animation.targetName} was attacked`}
    >
      <div className="attack-animation-target relative aspect-square w-full max-w-md overflow-hidden rounded-3xl border-2 border-danger/70 bg-canvas shadow-2xl shadow-danger/25">
        {animation.imageUrl ? (
          <img
            src={animation.imageUrl}
            alt=""
            className="h-full w-full object-contain"
          />
        ) : (
          <div className="flex h-full items-center justify-center p-8 text-center text-3xl font-semibold text-content">
            {animation.targetName}
          </div>
        )}
        <span className="attack-animation-slash attack-animation-slash-first" />
        <span className="attack-animation-slash attack-animation-slash-second" />
      </div>
    </div>
  );

  return inline ? overlay : createPortal(overlay, document.body);
};

export default AttackAnimationOverlay;
