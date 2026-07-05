import { useEffect, useState } from "react";
import AppLayout from "@/app/layouts/AppLayout";
import { JourneyGrid, listJourneys, type Journey } from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";

export function HomePage() {
  const [journeys, setJourneys] = useState<Journey[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let isCurrent = true;

    void listJourneys()
      .then((loadedJourneys) => {
        if (isCurrent) {
          setJourneys(loadedJourneys);
          setError("");
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) {
          setError(getApiError(requestError).message);
        }
      })
      .finally(() => {
        if (isCurrent) {
          setIsLoading(false);
        }
      });

    return () => {
      isCurrent = false;
    };
  }, []);

  const retryLoadJourneys = async () => {
    setIsLoading(true);
    setError("");

    try {
      setJourneys(await listJourneys());
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout
      scrolling
      background={
        <div className="absolute right-0 left-0 w-full h-full top-0 bottom-0 z-0 stone-image" />
      }
    >
      <main className="w-full p-6 sm:p-10">
        <div className="mx-auto w-full max-w-7xl">
          <header className="mb-6 flex items-center justify-between gap-4">
            <h1 className="text-4xl font-semibold text-content sm:text-5xl">
              Journeys
            </h1>
            <button
              type="button"
              className="rounded-lg bg-brand px-5 py-3 font-semibold text-on-brand transition hover:bg-brand-hover"
            >
              Add journey
            </button>
          </header>

          {isLoading && (
            <p className="text-content-secondary" role="status">
              Loading journeys...
            </p>
          )}

          {!isLoading && error && (
            <div
              className="rounded-xl border border-danger/40 bg-surface/90 p-5"
              role="alert"
            >
              <p className="text-danger">{error}</p>
              <button
                type="button"
                onClick={() => void retryLoadJourneys()}
                className="mt-4 rounded-lg border border-border px-4 py-2 text-content transition hover:border-brand-hover hover:text-brand-hover"
              >
                Try again
              </button>
            </div>
          )}

          {!isLoading && !error && journeys.length === 0 && (
            <div className="rounded-xl border border-border bg-surface/80 p-8 text-center">
              <h2 className="text-2xl font-semibold text-content">
                No journeys yet
              </h2>
              <p className="mt-2 text-content-muted">
                Add your first journey to begin building an adventure.
              </p>
            </div>
          )}

          {!isLoading && !error && journeys.length > 0 && (
            <JourneyGrid journeys={journeys} />
          )}
        </div>
      </main>
    </AppLayout>
  );
}
