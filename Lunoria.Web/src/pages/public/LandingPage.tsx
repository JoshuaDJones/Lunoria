import AppLayout from "@/app/layouts";
import { Link } from "react-router-dom";

export function LandingPage() {
  return (
    <AppLayout
      sidebar={false}
      background={
        <div className="absolute right-0 left-0 w-full h-full top-0 bottom-0 z-0 valley-village-image" />
      }
    >
      <main className="flex h-full w-full items-center justify-center">
        <section className="max-w-xl rounded-2xl border border-brand-subtle/20 bg-surface/80 px-8 py-14 text-center shadow-2xl backdrop-blur-sm sm:px-14">
          <p className="mb-3 text-sm uppercase tracking-[0.35em] text-brand-hover">
            A new journey begins
          </p>
          <h1 className="text-5xl font-bold sm:text-7xl">
            Lunoria
          </h1>
          <p className="mx-auto mt-5 max-w-md text-content-secondary">
            Create your party, guide your heroes, and keep every adventure in
            one place.
          </p>
          <Link
            to="/login"
            className="mt-9 inline-flex min-w-40 justify-center rounded-lg bg-brand px-7 py-3 font-semibold text-on-brand transition hover:bg-brand-hover"
          >
            Enter
          </Link>
        </section>
      </main>
    </AppLayout>
  );
}
