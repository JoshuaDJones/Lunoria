import type { ReactNode } from "react";
import { Link } from "react-router-dom";

interface AuthPageProps {
  title: string;
  subtitle: string;
  children: ReactNode;
}

export function AuthPage({ title, subtitle, children }: AuthPageProps) {
  return (
    <main className="grid min-h-screen place-items-center px-5 py-10 valley-village-image">
      <section className="w-full max-w-md rounded-2xl border border-brand-subtle/20 bg-surface/90 p-7 shadow-2xl backdrop-blur-sm sm:p-9">
        <Link to="/" className="text-xl font-bold text-brand-hover">
          Lunoria
        </Link>
        <h1 className="mt-7 text-3xl font-bold">{title}</h1>
        <p className="mt-2 text-sm text-content-muted">{subtitle}</p>
        <div className="mt-7">{children}</div>
      </section>
    </main>
  );
}
