import { useEffect, useState } from "react";
import { Link, Navigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { getJourney } from "@/features/journeys";

export function PlayHubPage() {
  const { seriesId, journeyId } = useParams<{
    seriesId: string;
    journeyId: string;
  }>();
  const [journeyName, setJourneyName] = useState("");

  useEffect(() => {
    const id = Number(journeyId);
    if (!Number.isInteger(id) || id <= 0) return;

    let isCurrent = true;
    void getJourney(id)
      .then((journey) => {
        if (isCurrent) setJourneyName(journey.name);
      })
      .catch(() => {
        if (isCurrent) setJourneyName("");
      });

    return () => {
      isCurrent = false;
    };
  }, [journeyId]);

  if (!seriesId || !journeyId) {
    return <Navigate to="/journeys" replace />;
  }

  return (
    <AppLayout
      scrolling
      background={<div className="stone-image absolute inset-0 z-0 h-full w-full" />}
    >
      <main className="w-full p-6 sm:p-10">
        <header className="mb-6">
          <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
            Play Hub{journeyName ? ` - ${journeyName}` : ""}
          </h1>
          <Link
            to={`/series/${seriesId}/journeys/${journeyId}`}
            className="text-sm text-content-secondary hover:text-brand-hover"
          >
            ← Back to Journey
          </Link>
        </header>

        <section className="min-h-[30rem] rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px] sm:p-6">
          <h2 className="text-3xl font-semibold text-content">Playthrough Redesign</h2>
          <div className="mt-3 h-px w-full bg-border" />
          <p className="mt-5 text-content-secondary">
            Playthrough controls will return when the replacement API is connected.
          </p>
        </section>
      </main>
    </AppLayout>
  );
}
