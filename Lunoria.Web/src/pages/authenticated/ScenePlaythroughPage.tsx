import { useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";

export function ScenePlaythroughPage() {
  const { sceneId } = useParams<{ sceneId: string }>();

  return (
    <AppLayout
      sidebar={<></>}
      scrolling
      bottomPadding={false}
      background={
        <div className="valley-village-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full flex-1 p-10">
        <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
          Scene {sceneId}
        </h1>
      </main>
    </AppLayout>
  );
}
