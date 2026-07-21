import type { Series } from "@/features/series/types";
import { SeriesCard } from "./SeriesCard";
import { CardGrid } from "@/components/ui";

interface SeriesGridProps {
    series: Series[];
    onSelect?: (series: Series) => void;
    onDelete?: (series: Series) => void;
    onViewJourneys?: (series: Series) => void;
}

export function SeriesGrid({
    series,
    onSelect,
    onDelete,
    onViewJourneys,
}: SeriesGridProps) {
    return (
        <CardGrid>
            {series.map((seriesItem) => (
                <SeriesCard
                    key={seriesItem.id}
                    series={seriesItem}
                    onSelect={onSelect!}
                    onDelete={onDelete!}
                    onViewJourneys={onViewJourneys!}
                />
            ))}
        </CardGrid>
    )
}