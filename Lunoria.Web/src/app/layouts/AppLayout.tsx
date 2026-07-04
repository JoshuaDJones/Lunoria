import { PropsWithChildren, ReactNode } from 'react';
import clsx from 'clsx';
import { Sidebar } from '.';
import BreakpointIndicator from '@/components/ui';

interface AppLayoutProps {
  sidebar?: ReactNode;
  background?: ReactNode;
  pane?: ReactNode;
  bottomPadding?: boolean;
  scrolling?: boolean;
}

const AppLayout = ({
  sidebar,
  background,
  pane,
  bottomPadding,
  scrolling,
  children,
}: PropsWithChildren<AppLayoutProps>) => {
  return (
    <div className="relative h-screen w-screen overflow-hidden bg-slate-800">
      {background}
      {pane}
      {sidebar ?? <Sidebar />}

      <div
        className={clsx('absolute inset-0 z-10 flex flex-col', {
          'pb-20': !bottomPadding,
          'overflow-y-auto scrollbar-hide': scrolling,
        })}
      >
        {children}
      </div>

      <BreakpointIndicator />
    </div>
  );
};

export default AppLayout;