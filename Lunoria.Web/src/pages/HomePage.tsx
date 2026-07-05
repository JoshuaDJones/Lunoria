import AppLayout from "@/app/layouts/AppLayout";
import { useAuth } from "@/features/auth";

export function HomePage() {
  return (
    <AppLayout
      background={
        <div className="absolute right-0 left-0 w-full h-full top-0 bottom-0 z-0 stone-image" />
      }
    >
      {" "}

    </AppLayout>
  );
}

{
  /* </AppLayout>
    <main className="grid min-h-screen place-items-center px-6 stone-image">
      <section className="rounded-2xl border border-amber-300/20 bg-slate-950/80 px-10 py-12 text-center shadow-2xl backdrop-blur-sm">
        <p className="mb-3 text-sm uppercase tracking-[0.35em] text-amber-400">
          Welcome back
        </p>
        <h1 className="font-cinzel text-5xl font-bold sm:text-7xl">Lunoria</h1>

      </section>
    </main> */
}
