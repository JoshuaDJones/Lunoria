import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import { Button } from "@/components/ui";
import type { Scene } from "@/features/scenes/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBolt,
  faBoxOpen,
  faComments,
  faPen,
  faTrash,
  faUsers,
} from "@fortawesome/free-solid-svg-icons";

interface SceneCardProps {
  scene: Scene;
  onViewEvents: (scene: Scene) => void;
  onViewChests: (scene: Scene) => void;
  onViewCharacters: (scene: Scene) => void;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
  onDelete: (scene: Scene) => void;
}

export function SceneCard({
  scene,
  onViewEvents,
  onViewChests,
  onViewCharacters,
  onViewDialogs,
  onEdit,
  onDelete,
}: SceneCardProps) {
  const createdAt = new Date(scene.createdAt);
  const formattedCreatedAt = Number.isNaN(createdAt.getTime())
    ? "Unknown"
    : new Intl.DateTimeFormat().format(createdAt);
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
                className="cursor-pointer break-all text-left text-brand-hover underline decoration-brand-subtle/60 underline-offset-2 hover:text-brand"
              >
                {scene.gridUrl}
              </button>
            ) : (
              "None"
            )
          }
        />
        <Stat label="Created" value={formattedCreatedAt} />
      </StatGrid>
      <div className="mt-4 flex flex-wrap items-center justify-end gap-2 border-t border-border px-4 py-3">
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onViewCharacters(scene);
          }}
          variant="secondary"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faUsers} />}
        >
          Characters
        </Button>
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onViewChests(scene);
          }}
          variant="add"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faBoxOpen} />}
        >
          Chests
        </Button>
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onViewEvents(scene);
          }}
          variant="utility"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faBolt} />}
        >
          Events
        </Button>
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
