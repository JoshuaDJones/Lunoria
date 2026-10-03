import { useEffect, useState } from "react";

type Phase =
  "loading" | "background" | "title" | "titleExit" | "content" | "ready";

export function usePlaythroughEntrance(
  key: string,
  imageUrl: string | undefined,
  enabled: boolean,
  cinematic: boolean,
) {
  const [state, setState] = useState<{ key: string; phase: Phase }>();
  useEffect(() => {
    if (!enabled) return;
    let cancelled = false;
    let started = false;
    const timers: ReturnType<typeof setTimeout>[] = [];
    const image = new Image();
    const motion = window.matchMedia("(prefers-reduced-motion: reduce)");
    const show = (phase: Phase) => {
      if (!cancelled) setState({ key, phase });
    };
    const start = () => {
      if (cancelled || started) return;
      started = true;
      timers.forEach(clearTimeout);
      if (!cinematic || motion.matches) {
        show("ready");
        return;
      }
      show("background");
      timers.push(setTimeout(() => show("title"), 800));
      // Fade in for 600ms, hold fully visible for two seconds, then fade out.
      timers.push(setTimeout(() => show("titleExit"), 3400));
      timers.push(setTimeout(() => show("content"), 4000));
      timers.push(setTimeout(() => show("ready"), 4800));
    };
    const reduceMotion = () => {
      if (!motion.matches) return;
      started = true;
      timers.forEach(clearTimeout);
      show("ready");
    };
    motion.addEventListener("change", reduceMotion);
    void Promise.resolve().then(() => {
      if (cancelled) return;
      if (!cinematic || motion.matches) {
        start();
        return;
      }
      image.onload = start;
      image.onerror = start;
      image.src = imageUrl || "/Valley_Village_Background.png";
      if (image.complete) start();
      if (!started) timers.push(setTimeout(start, 5000));
    });
    return () => {
      cancelled = true;
      timers.forEach(clearTimeout);
      image.onload = null;
      image.onerror = null;
      motion.removeEventListener("change", reduceMotion);
    };
  }, [key, imageUrl, enabled, cinematic]);
  const phase = enabled && state?.key === key ? state.phase : "loading";
  return {
    backgroundVisible: phase !== "loading",
    titleVisible: phase === "title",
    contentVisible: phase === "content" || phase === "ready",
    ready: phase === "ready",
  };
}
