import { ReactNode, useState } from 'react';
import { Link } from 'react-router-dom';
import clsx from 'clsx';
import { type IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
  faAnglesLeft,
  faAnglesRight,
  faBookOpen,
  faUser,
  faWandMagicSparkles,
  faBottleDroplet,
  faShield,
} from '@fortawesome/free-solid-svg-icons';

export interface SidebarItem {
  label: string;
  to?: string;
  href?: string;
  icon?: IconDefinition;
  active?: boolean;
}

interface SidebarProps {
  title?: string;
  subtitle?: string;
  items?: SidebarItem[];
  className?: string;
  children?: ReactNode;
}

const defaultItems: SidebarItem[] = [
  { label: 'Journeys', to: '/', active: true, icon: faBookOpen },
  { label: 'Characters', to: '/characters', icon: faUser },
  { label: 'Spells', to: '/spells', icon: faWandMagicSparkles },
  { label: 'Consumables', to: '/consumables', icon: faBottleDroplet },
  { label: 'Equipment', to: '/equipment', icon: faShield },
];

const Sidebar = ({
  title = 'Lunoria',
  subtitle = 'Adventure Creator',
  items = defaultItems,
  className,
  children,
}: SidebarProps) => {
  const [collapsed, setCollapsed] = useState(false);

  return (
    <aside
      className={clsx(
        'absolute inset-y-0 left-0 z-20 flex flex-col border-r border-white/10 bg-slate-900/85 p-5 text-slate-100 shadow-xl backdrop-blur-sm transition-all duration-200',
        collapsed ? 'w-20' : 'w-72',
        className,
      )}
    >
      <div className="mb-8 flex items-center gap-3">        
        {!collapsed && (
          <div>
            <p className="text-md text-white font-semibold font-cinzel">{title}</p>
            <p className="text-xs text-slate-400 font-cinzel">{subtitle}</p>
          </div>
        )}
      </div>

      <nav className="flex flex-col gap-2"> 
        {items.map((item) => {
          const content = (
            <>  
              <span className="text-base">
                {item.icon ? <FontAwesomeIcon icon={item.icon} /> : null}
              </span>
              {!collapsed && <span>{item.label}</span>}
            </>
          );

          const className = clsx(
            'flex items-center gap-3 rounded-xl px-3 py-2 text-sm font-medium transition-colors',
            collapsed ? 'justify-center px-2' : '',
            item.active
              ? 'text-amber-400 bg-white/10'
              : 'text-slate-300 hover:bg-white/10 hover:text-white',
          );

          if (item.to) {
            return (
              <Link key={item.label} to={item.to} className={className}>
                {content}
              </Link>
            );
          }

          if (item.href) {
            return (
              <a key={item.label} href={item.href} className={className}>
                {content}
              </a>
            );
          }

          return (
            <button key={item.label} type="button" className={className}>
              {content}
            </button>
          );
        })}
      </nav>

      <div className="mt-auto pt-6">
        {children}
        <button
          type="button"
          onClick={() => setCollapsed((value) => !value)}
          className="mt-3 flex w-full items-center justify-center rounded-xl border border-white/10 bg-white/5 p-2 text-slate-300 transition-colors hover:bg-white/10 hover:text-white"
          aria-label={collapsed ? 'Expand sidebar' : 'Collapse sidebar'}
        >
          <FontAwesomeIcon icon={collapsed ? faAnglesRight : faAnglesLeft} />
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
