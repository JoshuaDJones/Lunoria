import { useEffect, useState } from "react";

type EntrancePhase = "black" | "background" | "title" | "content" | "ready";

export function useSceneEntrance(
  sceneKey: string,
  imageUrl: string | undefined,
  enabled: boolean,
) {
  const [entrance, setEntrance] = useState<{
    key: string;
    phase: EntrancePhase;
  }>();

  useEffect(() => {
    if (!enabled) return;
    let cancelled = false;
    let started = false;
    const timers: ReturnType<typeof setTimeout>[] = [];
    const motion = window.matchMedia("(prefers-reduced-motion: reduce)");
    const image = new Image();
    const show = (phase: EntrancePhase) => {
      if (!cancelled) setEntrance({ key: sceneKey, phase });
    };
    const start = () => {
      if (cancelled || started) return;
      started = true;
      timers.forEach(clearTimeout);
      if (motion.matches) {
        show("ready");
        return;
      }
      show("black");
      // Leave a black first frame even when the background is already cached.
      timers.push(setTimeout(() => show("background"), 75));
      timers.push(setTimeout(() => show("title"), 875));
      timers.push(setTimeout(() => show("content"), 1625));
      timers.push(setTimeout(() => show("ready"), 2275));
    };
    const handleMotionChange = () => {
      if (motion.matches) {
        started = true;
        timers.forEach(clearTimeout);
        show("ready");
      }
    };
    motion.addEventListener("change", handleMotionChange);
    // Defer cached/missing-image completion so setup is safe in Strict Mode.
    void Promise.resolve().then(() => {
      if (cancelled) return;
      if (!imageUrl || motion.matches) {
        start();
        return;
      }
      image.onload = start;
      image.onerror = start;
      image.src = imageUrl;
      if (image.complete) start();
      // An unavailable background must never prevent scene interaction.
      if (!started) timers.push(setTimeout(start, 5000));
    });
    return () => {
      cancelled = true;
      timers.forEach(clearTimeout);
      image.onload = null;
      image.onerror = null;
      motion.removeEventListener("change", handleMotionChange);
    };
  }, [sceneKey, imageUrl, enabled]);

  const phase =
    enabled && entrance?.key === sceneKey ? entrance.phase : "black";
  return {
    showBackground: phase !== "black",
    showTitle: phase === "title" || phase === "content" || phase === "ready",
    showContent: phase === "content" || phase === "ready",
    isReady: phase === "ready",
  };
}
