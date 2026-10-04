import { PropsWithChildren, ReactNode } from "react";
import clsx from "clsx";
import { Sidebar } from ".";
import BreakpointIndicator from "@/components/ui";

const SHOW_BREAKPOINT_INDICATOR = false;

interface AppLayoutProps {
  sidebar?: ReactNode;
  background?: ReactNode;
  pane?: ReactNode;
  bottomPadding?: boolean;
  scrolling?: boolean;
  fixedViewport?: boolean;
}

const AppLayout = ({
  sidebar,
  background,
  pane,
  bottomPadding,
  scrolling,
  fixedViewport = false,
  children,
}: PropsWithChildren<AppLayoutProps>) => {
  return (
    <div
      className={clsx(
        "flex flex-col overflow-hidden bg-slate-800 lg:flex-row",
        fixedViewport
          ? "fixed inset-0 h-dvh w-full overscroll-none"
          : "relative h-screen w-screen",
      )}
    >
      {background}
      {pane}
      {sidebar ?? <Sidebar />}

      <div
        className={clsx("relative z-10 flex min-h-0 min-w-0 flex-1 flex-col", {
          "pb-20": !bottomPadding,
          "overflow-y-auto scrollbar-hide": scrolling,
          "min-h-0 overflow-hidden": fixedViewport && !scrolling,
        })}
      >
        {children}
      </div>

      {SHOW_BREAKPOINT_INDICATOR && <BreakpointIndicator />}
    </div>
  );
};

export default AppLayout;
