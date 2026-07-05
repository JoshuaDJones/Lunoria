import { Card } from "@/components/ui/Card";
import type { ReactNode } from "react";

interface MediaCardProps {
  title: string;
  description: string;
  imageUrl?: string | null;
  children?: ReactNode;
  onClick?: () => void;
}

export function MediaCard({
  title,
  description,
  imageUrl,
  children,
  onClick,
}: MediaCardProps) {
  return (
    <Card
      onClick={onClick}
      onKeyDown={(event) => {
        if (onClick && (event.key === "Enter" || event.key === " ")) {
          event.preventDefault();
          onClick();
        }
      }}
      role={onClick ? "button" : undefined}
      tabIndex={onClick ? 0 : undefined}
      className="flex min-h-52 flex-col transition focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-hover"
    >
      <div className="flex flex-1 gap-4 p-4">
        <div className="min-w-0 flex-1">
          <h2 className="wrap-break-word text-2xl font-semibold text-content">
            {title}
          </h2>
          <p className="mt-2 line-clamp-4 wrap-break-word text-sm text-content-secondary">
            {description}
          </p>
        </div>

        {imageUrl && (
          <img
            src={imageUrl}
            alt=""
            className="h-auto w-[39%] shrink-0 self-start rounded-lg object-contain"
          />
        )}
      </div>
      {children}
    </Card>
  );
}
