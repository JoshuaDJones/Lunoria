import { useEffect, useState, type ReactNode } from "react";
import clsx from "clsx";
import AppLayout from "@/app/layouts/AppLayout";
import { getApiError } from "@/lib/apiClient";
import { ApiLoadError, Button, Input } from "@/components/ui";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMagnifyingGlass, faPlus } from "@fortawesome/free-solid-svg-icons";
import { Link } from "react-router-dom";

interface BackNavigationProps {
  to: string;
  label: string;
}

interface CollectionPageProps<T> {
  title: string;
  backNavigation?: BackNavigationProps;
  itemName: string;
  loadItems: () => Promise<T[]>;
  renderItems: (items: T[]) => ReactNode;
  toolbar?: ReactNode;
  onAdd?: () => void;
  reloadKey?: unknown;
  search?: {
    value: string;
    onChange: (value: string) => void;
    getText: (item: T) => string;
  };
}

export function CollectionPage<T>({
  title,
  backNavigation,
  itemName,
  loadItems,
  renderItems,
  toolbar,
  onAdd,
  reloadKey,
  search,
}: CollectionPageProps<T>) {
  const [items, setItems] = useState<T[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const query = search?.value.trim().toLowerCase() ?? "";
  const visibleItems =
    search && query
      ? items.filter((item) =>
          search.getText(item).toLowerCase().includes(query),
        )
      : items;

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
        <header className={clsx("mb-6 flex items-center justify-between")}>
          <div>
            <h1 className="text-6xl text-content">{title}</h1>
            {backNavigation && (
              <Link
                to={backNavigation.to}
                className="text-sm text-content-secondary hover:text-brand-hover"
              >
                ← {backNavigation.label}
              </Link>
            )}
          </div>

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

        {(toolbar || search) && (
          <div className="mb-6 flex flex-wrap items-center gap-4">
            {toolbar}
            {search && (
              <div className="flex min-w-0 max-w-lg flex-[1_1_18rem] items-center gap-3">
                <div className="relative min-w-0 flex-1">
                  <FontAwesomeIcon
                    icon={faMagnifyingGlass}
                    aria-hidden="true"
                    className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-content-muted"
                  />
                  <Input
                    type="search"
                    aria-label={`Search ${title.toLowerCase()} by name`}
                    placeholder={`Search ${title.toLowerCase()} by name...`}
                    value={search.value}
                    onChange={(event) => search.onChange(event.target.value)}
                    className="py-2 pl-10"
                  />
                </div>
              </div>
            )}
          </div>
        )}

        {isLoading && (
          <p className="text-content-secondary" role="status">
            Loading {title.toLowerCase()}...
          </p>
        )}

        {!isLoading && error && (
          <ApiLoadError error={error} onRetry={retryLoad} />
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

        {!isLoading &&
          !error &&
          query &&
          visibleItems.length === 0 &&
          items.length > 0 && (
            <p
              className="rounded-xl border border-border bg-surface/80 p-8 text-center text-content-secondary"
              role="status"
            >
              No matching {title.toLowerCase()}. Try another name or adjust your
              filters.
            </p>
          )}
        {!isLoading &&
          !error &&
          visibleItems.length > 0 &&
          renderItems(visibleItems)}
      </main>
    </AppLayout>
  );
}
