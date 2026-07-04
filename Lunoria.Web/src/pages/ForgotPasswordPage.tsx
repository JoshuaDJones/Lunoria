import { Link } from "react-router-dom";
import { AuthPage } from "@/features/auth/components/AuthPage";

export function ForgotPasswordPage() {
  return (
    <AuthPage
      title="Forgot password"
      subtitle="Password recovery is not available yet. Contact the game administrator for help accessing your account."
    >
      <Link
        to="/login"
        className="inline-flex w-full justify-center rounded-lg bg-amber-500 px-5 py-3 font-semibold text-slate-950 transition hover:bg-amber-400"
      >
        Back to sign in
      </Link>
    </AuthPage>
  );
}
