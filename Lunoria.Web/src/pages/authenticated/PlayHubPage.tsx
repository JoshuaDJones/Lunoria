import { Link, Navigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";

export function PlayHubPage() {
  const { seriesId, journeyId } = useParams<{
    seriesId: string;
    journeyId: string;
  }>();

  if (!seriesId || !journeyId) {
    return <Navigate to="/journeys" replace />;
  }

  return (
    <AppLayout
      scrolling
      background={
        <div className="stone-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full p-6 sm:p-10">
        <header className="mb-6">
          <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
            Play Hub
          </h1>
          <Link
            to={`/series/${seriesId}/journeys/${journeyId}`}
            className="text-sm text-content-secondary hover:text-brand-hover"
          >
            ← Back to Journey
          </Link>
        </header>
      </main>
    </AppLayout>
  );
}
