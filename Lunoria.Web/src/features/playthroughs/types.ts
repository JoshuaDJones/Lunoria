export interface JourneyPlaythrough {
  id: number;
  journeyId: number;
  startedAt: string;
  completedAt: string | null;
  isActive: boolean;
}
