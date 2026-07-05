import { type FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AuthField } from "@/features/auth/components/AuthField";
import { AuthPage } from "@/features/auth/components/AuthPage";
import { Button } from "@/components/ui";
import { getApiError } from "@/lib/apiClient";
import { register, useAuth } from "@/features/auth";

export function RegisterPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { setAuthToken } = useAuth();
  const navigate = useNavigate();

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    setIsSubmitting(true);
    try {
      const result = await register({ email, password });
      setAuthToken(result.accessToken);
      navigate("/home", { replace: true });
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthPage title="Create an account" subtitle="Begin building your world.">
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
          autoComplete="new-password"
          onChange={setPassword}
        />
        <AuthField
          id="confirm-password"
          label="Confirm password"
          type="password"
          value={confirmPassword}
          autoComplete="new-password"
          onChange={setConfirmPassword}
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
          {isSubmitting ? "Creating account..." : "Create account"}
        </Button>
      </form>
      <p className="mt-6 text-center text-sm text-content-muted">
        Already registered?{" "}
        <Link to="/login" className="text-brand-hover hover:text-brand-subtle">
          Sign in
        </Link>
      </p>
    </AuthPage>
  );
}
