import { type FormEvent, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { AuthField } from "@/features/auth/components/AuthField";
import { AuthPage } from "@/features/auth/components/AuthPage";
import { getApiError } from "@/lib/apiClient";
import { login, useAuth } from "@/features/auth";

export function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { setAuthToken } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    setIsSubmitting(true);

    try {
      const result = await login({ email, password });
      setAuthToken(result.accessToken);
      const destination =
        (location.state as { from?: string } | null)?.from ?? "/home";
      navigate(destination, { replace: true });
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthPage title="Welcome back" subtitle="Sign in to continue your journey.">
      <form onSubmit={handleSubmit} className="space-y-5">
        <AuthField
          id="email"
          label="Email"
          type="email"
          value={email}
          autoComplete="email"
          onChange={setEmail}
        />
        <AuthField
          id="password"
          label="Password"
          type="password"
          value={password}
          autoComplete="current-password"
          onChange={setPassword}
        />
        {error && (
          <p className="text-sm text-danger" role="alert">
            {error}
          </p>
        )}
        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full rounded-lg bg-brand px-5 py-3 font-semibold text-on-brand transition hover:bg-brand-hover disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isSubmitting ? "Signing in..." : "Sign in"}
        </button>
      </form>
      <div className="mt-6 flex items-center justify-between gap-4 text-sm">
        <Link
          to="/register"
          className="text-brand-hover hover:text-brand-subtle"
        >
          Register
        </Link>
        <Link
          to="/forgot-password"
          className="text-content-secondary hover:text-content"
        >
          Forgot password?
        </Link>
      </div>
    </AuthPage>
  );
}
