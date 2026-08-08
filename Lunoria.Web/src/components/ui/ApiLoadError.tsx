import { Button } from "@/components/ui/Button";

interface ApiLoadErrorProps {
  error: string;
  onRetry: () => void | Promise<void>;
  isRetrying?: boolean;
}

export function ApiLoadError({
  error,
  onRetry,
  isRetrying = false,
}: ApiLoadErrorProps) {
  return (
    <div
      className="rounded-xl border border-danger/40 bg-surface/90 p-5"
      role="alert"
    >
      <p className="text-danger">{error}</p>
      <Button
        onClick={() => void onRetry()}
        disabled={isRetrying}
        className="mt-4 text-content"
      >
        {isRetrying ? "Trying again..." : "Try again"}
      </Button>
    </div>
  );
}
