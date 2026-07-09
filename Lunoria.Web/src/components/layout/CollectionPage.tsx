import { useEffect, useState, type ReactNode } from "react";
import AppLayout from "@/app/layouts/AppLayout";
import { getApiError } from "@/lib/apiClient";
import { Button } from "@/components/ui";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";

interface CollectionPageProps<T> {
  title: string;
  itemName: string;
  loadItems: () => Promise<T[]>;
  renderItems: (items: T[]) => ReactNode;
  toolbar?: ReactNode;
  onAdd?: () => void;
  reloadKey?: unknown;
}

export function CollectionPage<T>({
  title,
  itemName,
  loadItems,
  renderItems,
  toolbar,
  onAdd,
  reloadKey,
}: CollectionPageProps<T>) {
  const [items, setItems] = useState<T[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let isCurrent = true;

    void loadItems()
      .then((loadedItems) => {
        if (isCurrent) {
          setItems(loadedItems);
          setError("");
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) {
          setError(getApiError(requestError).message);
        }
      })
      .finally(() => {
        if (isCurrent) {
          setIsLoading(false);
        }
      });

    return () => {
      isCurrent = false;
    };
  }, [loadItems, reloadKey]);

  const retryLoad = async () => {
    setIsLoading(true);
    setError("");

    try {
      setItems(await loadItems());
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout
      scrolling
      background={
        <div className="stone-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full p-6 sm:p-10">
        <header className="mb-6 flex items-center justify-between">
          <h1 className="text-6xl text-content">{title}</h1>
          <Button
            onClick={onAdd}
            disabled={!onAdd}
            title={
              onAdd ? `Add ${itemName}` : `${itemName} creation coming soon`
            }
            variant="add"
            size="lg"
            leftIcon={<FontAwesomeIcon icon={faPlus} />}
          >
            Add {itemName}
          </Button>
        </header>

        {toolbar && <div className="mb-6">{toolbar}</div>}

        {isLoading && (
          <p className="text-content-secondary" role="status">
            Loading {title.toLowerCase()}...
          </p>
        )}

        {!isLoading && error && (
          <div
            className="rounded-xl border border-danger/40 bg-surface/90 p-5"
            role="alert"
          >
            <p className="text-danger">{error}</p>
            <Button
              onClick={() => void retryLoad()}
              className="mt-4 text-content"
            >
              Try again
            </Button>
          </div>
        )}

        {!isLoading && !error && items.length === 0 && (
          <div className="rounded-xl border border-border bg-surface/80 p-8 text-center">
            <h2 className="text-2xl font-semibold text-content">
              No {title.toLowerCase()} yet
            </h2>
            <p className="mt-2 text-content-muted">
              Add your first {itemName} to get started.
            </p>
          </div>
        )}

        {!isLoading && !error && items.length > 0 && renderItems(items)}
      </main>
    </AppLayout>
  );
}
