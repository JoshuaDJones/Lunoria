import { type FormEvent, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { AuthField } from "@/features/auth/components/AuthField";
import { AuthPage } from "@/features/auth/components/AuthPage";
import { Button } from "@/components/ui";
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
        <Button
          type="submit"
          disabled={isSubmitting}
          variant="primary"
          size="lg"
          className="w-full"
        >
          {isSubmitting ? "Signing in..." : "Sign in"}
        </Button>
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
