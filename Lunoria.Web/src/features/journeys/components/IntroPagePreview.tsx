import clsx from "clsx";
import { IntroPageType } from "@/features/journeys/types";
import type { IntroPageConfig } from "@/features/journeys/introPageConfig";
import { IntroPageContent } from "./IntroPageContent";

interface IntroPagePreviewProps {
  type: IntroPageType;
  config: IntroPageConfig;
  imageUrl?: string | null;
  className?: string;
  fullScreen?: boolean;
}

export function IntroPagePreview({
  type,
  config,
  imageUrl,
  className,
  fullScreen = false,
}: IntroPagePreviewProps) {
  const image = imageUrl ? (
    <img
      src={imageUrl}
      alt={config.imageAlt}
      className={clsx(
        "h-full min-h-0 w-full bg-canvas",
        type === IntroPageType.ImageCenterOverlayCenterText
          ? "object-cover"
          : "object-contain",
      )}
    />
  ) : (
    <div className="flex h-full min-h-36 items-center justify-center bg-surface-raised text-content-muted">
      Image preview
    </div>
  );

  const content = (
    <div className="h-full min-h-0 min-w-0 overflow-y-auto p-5">
      <IntroPageContent content={config.content} />
    </div>
  );

  return (
    <div
      className={clsx(
        "relative overflow-hidden bg-surface text-content shadow-lg",
        fullScreen
          ? "h-full w-full"
          : "aspect-video rounded-xl border border-border",
        className,
      )}
    >
      {type === IntroPageType.ImageTopContentBottom && (
        <div className="grid h-full min-h-0 grid-rows-2">
          {image}
          {content}
        </div>
      )}
      {type === IntroPageType.ImageLeftContentRight && (
        <div className="grid h-full min-h-0 grid-cols-2">
          {image}
          {content}
        </div>
      )}
      {type === IntroPageType.ImageRightContentLeft && (
        <div className="grid h-full min-h-0 grid-cols-2">
          {content}
          {image}
        </div>
      )}
      {type === IntroPageType.ImageCenterOverlayCenterText && (
        <>
          <div className="absolute inset-0">{image}</div>
          <div className="absolute inset-0 min-h-0 overflow-y-auto bg-canvas/55 text-center">
            <div className="flex min-h-full items-center justify-center">
              {content}
            </div>
          </div>
        </>
      )}
      {type === IntroPageType.CharacterShowcase && (
        <div className="grid h-full min-h-0 grid-cols-[2fr_3fr] bg-canvas/80">
          <div className="min-h-0 p-4">{image}</div>
          <div className="min-h-0">{content}</div>
        </div>
      )}
    </div>
  );
}
