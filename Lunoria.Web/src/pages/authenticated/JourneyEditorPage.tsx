import AppLayout from "@/app/layouts";
import { ApiLoadError } from "@/components/ui";
import { getJourney, type Journey } from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";
import clsx from "clsx";
import { useEffect, useState } from "react";
import { Link, Navigate, useParams } from "react-router-dom";

export function JourneyEditorPage() {
  const { seriesId, journeyId: journeyIdParam } = useParams<{
    seriesId: string;
    journeyId: string;
  }>();
  const journeyId = Number(journeyIdParam);
  const [journey, setJourney] = useState<Journey>();
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  const loadJourney = async () => {
    setIsLoading(true);
    setError("");

    try {
      setJourney(await getJourney(journeyId));
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (!Number.isInteger(journeyId) || journeyId <= 0) return;

    let isCurrent = true;

    void getJourney(journeyId)
      .then((loadedJourney) => {
        if (isCurrent) {
          setJourney(loadedJourney);
          setError("");
        }
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
  }, [journeyId]);

  if (!Number.isInteger(journeyId) || journeyId <= 0) {
    return <Navigate to="/home" replace />;
  }

  return (
    <AppLayout
      scrolling
      background={
        <div className="stone-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full p-6 sm:p-10">
        <header className={clsx("mb-6 flex items-center justify-between")}>
          <div>
            <h1 className="text-6xl text-content">
              {journey?.name ?? (isLoading ? "Loading journey..." : "Journey")}
            </h1>
            <Link
              to={`/series/${seriesId}/journeys`}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to Journeys
            </Link>
          </div>

          {/* <Button
            onClick={onAdd}
            disabled={!onAdd}
            title={
              onAdd ? `Add ${itemName}` : `${itemName} creation coming soon`
            }
            variant="add"
            size="lg"
            leftIcon={<FontAwesomeIcon icon={faPlus} />}
          >
            Add {itemName}
          </Button> */}
        </header>

        {!isLoading && error && (
          <ApiLoadError error={error} onRetry={loadJourney} />
        )}
      </main>
    </AppLayout>
  );
}
