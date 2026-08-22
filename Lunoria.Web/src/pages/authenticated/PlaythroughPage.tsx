import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import {
  getPlaythrough,
  type PlaythroughDetails,
} from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";

export function PlaythroughPage() {
  const { playthroughId: playthroughIdParam } = useParams<{
    playthroughId: string;
  }>();
  const playthroughId = Number(playthroughIdParam);
  const [playthrough, setPlaythrough] = useState<PlaythroughDetails>();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!Number.isInteger(playthroughId) || playthroughId <= 0) {
      setError("The playthrough ID is invalid.");
      setIsLoading(false);
      return;
    }

    let isCurrent = true;
    setIsLoading(true);
    setError("");

    void getPlaythrough(playthroughId)
      .then((loadedPlaythrough) => {
        if (isCurrent) setPlaythrough(loadedPlaythrough);
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setError(getApiError(requestError).message);
      })
      .finally(() => {
        if (isCurrent) setIsLoading(false);
      });

    return () => {
      isCurrent = false;
    };
  }, [playthroughId]);

  return (
    <AppLayout
      sidebar={<></>}
      scrolling
      background={<div className="stone-image absolute inset-0 z-0 h-full w-full" />}
    >
      <main className="w-full p-6 sm:p-10">
        {isLoading && <p className="text-content">Loading playthrough...</p>}

        {!isLoading && error && (
          <p className="text-danger" role="alert">
            {error}
          </p>
        )}

        {!isLoading && !error && playthrough && (
          <pre className="overflow-auto whitespace-pre-wrap rounded-xl bg-surface/90 p-5 text-sm text-content">
            {JSON.stringify(playthrough, null, 2)}
          </pre>
        )}
      </main>
    </AppLayout>
  );
}
