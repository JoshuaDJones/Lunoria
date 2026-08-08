import type { JSONContent } from "@tiptap/core";
import { IntroPageType } from "@/features/journeys/types";

export interface IntroPageConfig {
  version: 1;
  imageAlt: string;
  content: JSONContent;
}

export const emptyIntroPageConfig: IntroPageConfig = {
  version: 1,
  imageAlt: "",
  content: {
    type: "doc",
    content: [{ type: "paragraph" }],
  },
};

export function parseIntroPageConfig(config: string): IntroPageConfig {
  try {
    const parsed = JSON.parse(config) as Partial<IntroPageConfig>;
    if (parsed.version === 1 && parsed.content?.type === "doc") {
      return {
        version: 1,
        imageAlt: parsed.imageAlt ?? "",
        content: parsed.content,
      };
    }
  } catch {
    // Older or malformed content opens as an empty document instead of crashing.
  }

  return structuredClone(emptyIntroPageConfig);
}

export const introPageTypeLabels: Record<IntroPageType, string> = {
  [IntroPageType.ImageTopContentBottom]: "Image above content",
  [IntroPageType.ImageLeftContentRight]: "Image left, content right",
  [IntroPageType.ImageRightContentLeft]: "Content left, image right",
  [IntroPageType.ImageCenterOverlayCenterText]: "Centered image with overlay",
  [IntroPageType.CharacterShowcase]: "Character showcase",
};
