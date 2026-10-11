import { useCallback, useEffect, useRef, useState } from "react";
import { getJourneyCharacterSyncSummary } from "@/features/characterSync/api";
import type { JourneyCharacterSyncSummary } from "@/features/characterSync/summaryTypes";
import { getApiError } from "@/lib/apiClient";

export function useJourneyCharacterSyncSummary(journeyId: number) {
  const request = useRef<AbortController | null>(null);

  const [state, setState] = useState<{
    journeyId: number;
    loading: boolean;
    error: string;
    summary?: JourneyCharacterSyncSummary;
  }>({ journeyId, loading: true, error: "" });

  const load = useCallback(() => {
    request.current?.abort();

    if (!Number.isInteger(journeyId) || journeyId <= 0) 
      return;

    const controller = new AbortController();
    request.current = controller;

    void getJourneyCharacterSyncSummary(journeyId, controller.signal)
      .then((summary) => {
        if (!controller.signal.aborted)
          setState({ journeyId, summary, loading: false, error: "" });
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted)
          setState({
            journeyId,
            loading: false,
            error: getApiError(error).message,
          });
      });
  }, [journeyId]);

  const refresh = useCallback(() => {
    // Never show a stale warning as a freshly checked result.
    setState({ journeyId, loading: true, error: "" });
    load();
  }, [journeyId, load]);

  useEffect(() => {
    load();

    const onFocus = () => {
      if (document.visibilityState === "visible") 
        refresh();
    };

    window.addEventListener("focus", onFocus);
    
    return () => {
      request.current?.abort();
      window.removeEventListener("focus", onFocus);
    };
  }, [load, refresh]);

  return {
    summary: state.journeyId === journeyId ? state.summary : undefined,
    loading: state.journeyId !== journeyId || state.loading,
    error: state.journeyId === journeyId ? state.error : "",
    refresh,
  };
}
