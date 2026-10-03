import { useState } from "react";

function SceneBackground({ photoUrl }: { photoUrl?: string }) {
  const [imageFailed, setImageFailed] = useState(false);

  return (
    <div
      aria-hidden="true"
      className="pointer-events-none absolute inset-0 z-0"
    >
      <div className="valley-village-image absolute inset-0 h-full w-full" />
      {photoUrl && !imageFailed && (
        <>
          <img
            src={photoUrl}
            alt=""
            className="absolute inset-0 h-full w-full object-cover object-center"
            onError={() => setImageFailed(true)}
          />
          <div className="absolute inset-0 bg-black/40" />
        </>
      )}
    </div>
  );
}

export default SceneBackground;
