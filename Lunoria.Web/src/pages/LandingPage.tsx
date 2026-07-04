import { Link } from "react-router-dom";

export function LandingPage() {
  return (
    <main className="grid min-h-screen place-items-center px-6 stone-image">
      <section className="max-w-xl rounded-2xl border border-amber-300/20 bg-slate-950/80 px-8 py-14 text-center shadow-2xl backdrop-blur-sm sm:px-14">
        <p className="mb-3 text-sm uppercase tracking-[0.35em] text-amber-400">
          A new journey begins
        </p>
        <h1 className="font-cinzel text-5xl font-bold sm:text-7xl">Lunoria</h1>
        <p className="mx-auto mt-5 max-w-md text-slate-300">
          Create your party, guide your heroes, and keep every adventure in one
          place.
        </p>
        <Link
          to="/login"
          className="mt-9 inline-flex min-w-40 justify-center rounded-lg bg-amber-500 px-7 py-3 font-semibold text-slate-950 transition hover:bg-amber-400"
        >
          Enter
        </Link>
      </section>
    </main>
  );
}
