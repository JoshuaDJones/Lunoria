import { useState } from "react";
import { QRCodeSVG } from "qrcode.react";
import { Button } from "@/components/ui";

export function PlaythroughJoinDialog({
  joinUrl,
  expiresAt,
  onRevoke,
}: {
  joinUrl: string;
  expiresAt: string;
  onRevoke: () => Promise<void>;
}) {
  const [hasCopied, setHasCopied] = useState(false);
  const [isRevoking, setIsRevoking] = useState(false);

  const copyLink = async () => {
    try {
      await navigator.clipboard.writeText(joinUrl);
    } catch {
      const input = document.createElement("textarea");
      input.value = joinUrl;
      input.style.position = "fixed";
      input.style.opacity = "0";
      document.body.appendChild(input);
      input.select();
      document.execCommand("copy");
      input.remove();
    }
    setHasCopied(true);
    window.setTimeout(() => setHasCopied(false), 2_000);
  };

  return (
    <div className="text-center">
      <p className="text-content-secondary">
        Scan this code on a phone to view the live playthrough anonymously.
      </p>

      <div className="mx-auto mt-6 w-full max-w-72 rounded-2xl bg-white p-4">
        <QRCodeSVG
          value={joinUrl}
          size={288}
          level="M"
          marginSize={1}
          className="h-auto w-full"
        />
      </div>

      <p className="mt-5 break-all rounded-xl border border-border bg-surface p-3 text-left text-sm text-content-secondary">
        {joinUrl}
      </p>
      <p className="mt-2 text-xs text-content-muted">
        Expires {formatDate(expiresAt)}. Opening Join again creates a new link.
      </p>

      <div className="mt-6 flex flex-wrap justify-center gap-3">
        <Button variant="primary" onClick={() => void copyLink()}>
          {hasCopied ? "Copied" : "Copy Link"}
        </Button>
        <Button
          variant="danger"
          inverted
          disabled={isRevoking}
          onClick={() => {
            setIsRevoking(true);
            void onRevoke().finally(() => setIsRevoking(false));
          }}
        >
          {isRevoking ? "Closing..." : "Close Guest Session"}
        </Button>
      </div>
    </div>
  );
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}
