import { useEffect, useRef } from "react";
import { DialogPageType } from "@/features/scenes/types";
import type { DialogViewerDialog } from "@/features/scenes/components/DialogViewer";

export function DialogViewerPage({
  page,
  activeSectionIndex,
  outgoing,
}: {
  page: NonNullable<DialogViewerDialog["dialogPages"]>[number];
  activeSectionIndex: number;
  outgoing: boolean;
}) {
  const sections = [...(page.dialogPageSections ?? [])].sort(
    (left, right) => left.orderNum - right.orderNum,
  );
  const scroller = useRef<HTMLDivElement>(null);
  const activeSection = useRef<HTMLElement>(null);
  useEffect(() => {
    if (outgoing || !scroller.current || !activeSection.current) return;
    const container = scroller.current;
    const target = activeSection.current;
    const top =
      activeSectionIndex === 0
        ? 0
        : container.scrollTop +
          target.getBoundingClientRect().top -
          container.getBoundingClientRect().top -
          24;
    container.scrollTo({
      top: Math.max(0, top),
      behavior:
        activeSectionIndex === 0 ||
        window.matchMedia("(prefers-reduced-motion: reduce)").matches
          ? "instant"
          : "smooth",
    });
  }, [activeSectionIndex, outgoing, page.id]);
  return (
    <div className="relative flex h-full min-h-0 items-center justify-center overflow-hidden bg-canvas">
      {page?.pageType === DialogPageType.Video && page.mediaUrl ? (
        <video
          src={page.mediaUrl}
          controls
          playsInline
          preload="metadata"
          className="h-full max-h-full w-full max-w-full object-contain"
        >
          Your browser does not support video playback.
        </video>
      ) : page?.mediaUrl ? (
        <img
          src={page.mediaUrl}
          alt=""
          className="h-full max-h-full w-full max-w-full object-contain"
        />
      ) : (
        <p className="text-content-muted">This page has no media.</p>
      )}

      {page?.pageType === DialogPageType.Image && sections.length > 0 && (
        <div
          ref={scroller}
          className="scrollbar-hide absolute inset-4 overflow-y-auto py-10 sm:inset-x-[10%]"
        >
          <div className="mx-auto flex w-full flex-col items-center gap-5 sm:w-[80%] lg:w-[60%]">
            {sections.map((section, index) => {
              const speaker = section.isNarrator
                ? "Narrator"
                : (section.character?.name ?? "Unknown character");

              return (
                <article
                  key={section.id}
                  ref={index === activeSectionIndex ? activeSection : undefined}
                  aria-current={
                    index === activeSectionIndex ? "step" : undefined
                  }
                  style={{
                    borderColor:
                      section.character?.dialogActiveColor ||
                      section.character?.characterDialogSettings
                        ?.dialogActiveColor ||
                      undefined,
                  }}
                  className={`rounded-xl border-4 border-border bg-canvas/85 p-4 shadow-xl backdrop-blur-sm w-full transition-opacity motion-reduce:transition-none ${index === activeSectionIndex ? "opacity-100" : "opacity-50"}`}
                >
                  <div className="flex items-center gap-3">
                    {!section.isNarrator && section.character?.photoUrl && (
                      <img
                        src={section.character.photoUrl}
                        alt=""
                        className="h-10 w-10 rounded-md object-cover"
                      />
                    )}
                    <h3 className="font-semibold text-4xl text-content">
                      {speaker}
                    </h3>
                  </div>
                  <p className="mt-2 whitespace-pre-wrap text-2xl text-content-secondary">
                    {section.readingText}
                  </p>
                </article>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
