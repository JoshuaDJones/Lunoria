export function playAttackSlashSounds() {
  const timers: Array<ReturnType<typeof setTimeout>> = [];
  const sounds = new Set<HTMLAudioElement>();

  const playSlash = () => {
    const sound = new Audio("/sounds/sword_slash.wav");
    sound.volume = 0.75;
    sounds.add(sound);

    const releaseSound = () => sounds.delete(sound);
    sound.addEventListener("ended", releaseSound, { once: true });
    void sound.play().catch(releaseSound);
  };

  timers.push(setTimeout(playSlash, 160));
  timers.push(setTimeout(playSlash, 720));

  return () => {
    timers.forEach(clearTimeout);
    sounds.forEach((sound) => {
      sound.pause();
      sound.currentTime = 0;
    });
    sounds.clear();
  };
}

export function delay(milliseconds: number) {
  return new Promise<void>((resolve) => {
    window.setTimeout(resolve, milliseconds);
  });
}
