import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import { Button } from "@/components/ui";
import type { Scene } from "@/features/scenes/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faComments, faPen, faTrash } from "@fortawesome/free-solid-svg-icons";

interface SceneCardProps {
  scene: Scene;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
  onDelete: (scene: Scene) => void;
}

export function SceneCard({
  scene,
  onViewDialogs,
  onEdit,
  onDelete,
}: SceneCardProps) {
  const gridUrl = scene.gridUrl
    ? /^https?:\/\//i.test(scene.gridUrl)
      ? scene.gridUrl
      : `https://${scene.gridUrl}`
    : "";

  return (
    <MediaCard
      title={scene.name}
      description={scene.description}
      imageUrl={scene.photoUrl}
    >
      <StatGrid className="mt-4 px-4">
        <Stat
          label="Grid URL"
          value={
            gridUrl ? (
              <button
                type="button"
                onClick={() =>
                  window.open(
                    gridUrl,
                    "_blank",
                    "popup=yes,width=1200,height=800,noopener,noreferrer",
                  )
                }
                className="cursor-pointer text-brand-hover underline decoration-brand-subtle/60 underline-offset-2 hover:text-brand"
              >
                {scene.gridUrl}
              </button>
            ) : (
              "None"
            )
          }
        />
        <Stat
          label="Created"
          value={new Intl.DateTimeFormat().format(new Date(scene.createDate))}
        />
      </StatGrid>
      <div className="mt-4 flex items-center justify-end gap-2 border-t border-border px-4 py-3">
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onViewDialogs(scene);
          }}
          variant="magic"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faComments} />}
        >
          View dialogs
        </Button>
        <Button
          onClick={() => onDelete(scene)}
          variant="danger"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faTrash} />}
        >
          Delete
        </Button>
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onEdit(scene);
          }}
          variant="primary"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faPen} />}
        >
          Edit
        </Button>
      </div>
    </MediaCard>
  );
}
