import * as DropdownMenu from "@radix-ui/react-dropdown-menu";
import { Button, Card } from "@/components/ui";
import type { Scene } from "@/features/scenes/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBolt,
  faBullseye,
  faBoxOpen,
  faComments,
  faPen,
  faEllipsis,
  faTableCells,
  faTrash,
  faUsers,
} from "@fortawesome/free-solid-svg-icons";

interface SceneCardProps {
  scene: Scene;
  onViewEvents: (scene: Scene) => void;
  onViewObjectives: (scene: Scene) => void;
  onViewChests: (scene: Scene) => void;
  onViewCharacters: (scene: Scene) => void;
  onViewDialogs: (scene: Scene) => void;
  onEdit: (scene: Scene) => void;
  onDelete: (scene: Scene) => void;
}

export function SceneCard({
  scene,
  onViewEvents,
  onViewObjectives,
  onViewChests,
  onViewCharacters,
  onViewDialogs,
  onEdit,
  onDelete,
}: SceneCardProps) {
  const gridUrl = scene.gridUrl
    ? /^https?:\/\//i.test(scene.gridUrl)
      ? scene.gridUrl
      : `https://${scene.gridUrl}`
    : "";
  const openGrid = () => {
    const url = scene.grid
      ? `${window.location.origin}/scene-grids/${scene.id}`
      : gridUrl;
    if (!url) return;

    window.open(
      url,
      "_blank",
      `popup=yes,width=${window.screen.availWidth},height=${window.screen.availHeight},left=0,top=0,noopener,noreferrer`,
    );
  };

  const contentActions = [
    { label: "Characters", icon: faUsers, onSelect: onViewCharacters },
    { label: "Chests", icon: faBoxOpen, onSelect: onViewChests },
    { label: "Events", icon: faBolt, onSelect: onViewEvents },
    { label: "Objectives", icon: faBullseye, onSelect: onViewObjectives },
    { label: "Dialogs", icon: faComments, onSelect: onViewDialogs },
  ];

  return (
    <Card className="shadow-sm">
      <div className="flex flex-col gap-5 p-5 sm:flex-row">
        {scene.photoUrl && (
          <img
            src={scene.photoUrl}
            alt=""
            className="aspect-[4/3] w-full shrink-0 self-start rounded-lg object-cover sm:w-44 lg:w-52"
          />
        )}
        <div className="min-w-0 flex-1">
          <div className="flex items-start justify-between gap-3">
            <div className="flex min-w-0 flex-wrap items-center gap-x-4 gap-y-2">
              <h2 className="wrap-break-word text-2xl font-semibold text-content">
                {scene.name}
              </h2>
              {(scene.grid || gridUrl) && (
                <Button
                  onClick={openGrid}
                  size="sm"
                  className="shrink-0 border-border/60 text-content-secondary"
                  leftIcon={<FontAwesomeIcon icon={faTableCells} />}
                >
                  Open Grid
                </Button>
              )}
            </div>
            <DropdownMenu.Root>
              <DropdownMenu.Trigger asChild>
                <Button
                  aria-label={`Options for ${scene.name}`}
                  size="sm"
                  className="size-[30px] shrink-0 p-0 border-content-muted/50 bg-content/10 text-content hover:border-content-secondary hover:bg-content/20 hover:text-content"
                >
                  <FontAwesomeIcon icon={faEllipsis} />
                </Button>
              </DropdownMenu.Trigger>
              <DropdownMenu.Portal>
                <DropdownMenu.Content
                  align="end"
                  sideOffset={6}
                  className="z-50 min-w-36 rounded-xl border border-border bg-surface p-1.5 shadow-xl"
                >
                  <DropdownMenu.Item
                    onSelect={() => onEdit(scene)}
                    className="flex cursor-pointer items-center gap-3 rounded-lg px-3 py-2 text-sm text-content outline-none data-highlighted:bg-content/10"
                  >
                    <FontAwesomeIcon icon={faPen} />
                    Edit
                  </DropdownMenu.Item>
                  <DropdownMenu.Separator className="my-1 h-px bg-border" />
                  <DropdownMenu.Item
                    onSelect={() => onDelete(scene)}
                    className="flex cursor-pointer items-center gap-3 rounded-lg px-3 py-2 text-sm text-red-300 outline-none data-highlighted:bg-red-400/10"
                  >
                    <FontAwesomeIcon icon={faTrash} />
                    Delete
                  </DropdownMenu.Item>
                </DropdownMenu.Content>
              </DropdownMenu.Portal>
            </DropdownMenu.Root>
          </div>
          <p className="mt-2 line-clamp-4 wrap-break-word text-sm leading-relaxed text-content-secondary">
            {scene.description}
          </p>
          <div className="mt-3 flex flex-wrap gap-x-4 gap-y-1 text-xs text-content-muted">
            <span className="font-semibold text-content-secondary">
              Scene {String(scene.sortOrder + 1).padStart(2, "0")}
            </span>
            {scene.grid && (
              <span>{scene.grid.rows} × {scene.grid.columns} grid</span>
            )}
          </div>
          <div className="mt-5 flex flex-wrap gap-2 border-t border-border/60 pt-4">
            {contentActions.map(({ label, icon, onSelect }) => (
              <Button
                key={label}
                onClick={() => onSelect(scene)}
                size="md"
                className="min-h-11 border-border/60 bg-content/5 text-content-secondary hover:border-content-muted hover:bg-content/10 hover:text-content"
                leftIcon={<FontAwesomeIcon icon={icon} />}
              >
                {label}
              </Button>
            ))}
          </div>
        </div>
      </div>
    </Card>
  );
}
