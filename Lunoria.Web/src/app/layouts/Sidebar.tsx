import { ReactNode, useState } from "react";
import { Link } from "react-router-dom";
import clsx from "clsx";
import { type IconDefinition } from "@fortawesome/fontawesome-svg-core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faAnglesLeft,
  faAnglesRight,
  faBookOpen,
  faUser,
  faWandMagicSparkles,
  faBottleDroplet,
  faShield,
  faArrowRightFromBracket,
  faTableCellsLarge,
  faScroll,
} from "@fortawesome/free-solid-svg-icons";
import { useAuth } from "@/features/auth/hooks/useAuth";

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
  { label: "Series", to: "/series", icon: faScroll },
  { label: "Journeys", to: "/home", icon: faBookOpen },
  { label: "Characters", to: "/characters", icon: faUser },
  { label: "Spells", to: "/spells", icon: faWandMagicSparkles },
  { label: "Consumables", to: "/consumables", icon: faBottleDroplet },
  { label: "Equipment", to: "/equipment", icon: faShield },
  { label: "Components", to: "/components", icon: faTableCellsLarge },
];

const sidebarCollapsedKey = "lunoria.sidebar.collapsed";

function getInitialCollapsedState() {
  try {
    return window.localStorage.getItem(sidebarCollapsedKey) === "true";
  } catch {
    return false;
  }
}

const Sidebar = ({
  title = "Lunoria",
  subtitle = "Adventure Creator",
  items = defaultItems,
  className,
  children,
}: SidebarProps) => {
  const [collapsed, setCollapsed] = useState(getInitialCollapsedState);
  const { signOut } = useAuth();
  const activePath = window.location.pathname;

  const updatedItems = items.map((item) => ({
    ...item,
    active: item.to === activePath,
  }));

  const toggleCollapsed = () => {
    setCollapsed((current) => {
      const next = !current;

      try {
        window.localStorage.setItem(sidebarCollapsedKey, String(next));
      } catch {
        // The sidebar still works for this session when storage is unavailable.
      }

      return next;
    });
  };

  return (
    <aside
      className={clsx(
        "relative z-20 flex h-full shrink-0 flex-col border-r border-white/10 bg-stone-900/85 p-5 text-stone-100 shadow-xl backdrop-blur-sm transition-all duration-200",
        collapsed ? "w-20" : "w-72",
        className,
      )}
    >
      <div className="mb-8 flex items-center gap-3">
        {collapsed ? (
          <p className="w-full text-center text-xl font-semibold text-white">
            L
          </p>
        ) : (
          <div>
            <p className="text-md font-semibold text-white">{title}</p>
            <p className="text-xs text-stone-400">{subtitle}</p>
          </div>
        )}
      </div>

      <nav className="flex flex-col gap-2">
        {updatedItems.map((item) => {
          const content = (
            <>
              <span className="text-base">
                {item.icon ? <FontAwesomeIcon icon={item.icon} /> : null}
              </span>
              {!collapsed && <span>{item.label}</span>}
            </>
          );

          const className = clsx(
            "flex items-center gap-3 rounded-xl px-3 py-2 text-sm font-medium transition-colors",
            collapsed ? "justify-center px-2" : "",
            item.active
              ? "text-blue-500 bg-white/10"
              : "text-stone-300 hover:bg-white/10 hover:text-white",
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
          onClick={signOut}
          className="rounded-lg border border-stone-600 px-5 py-2 w-full text-sm text-stone-200 transition hover:border-danger hover:text-danger cursor-pointer flex items-center justify-center"
        >
          {collapsed ? (
            <FontAwesomeIcon icon={faArrowRightFromBracket} />
          ) : (
            "Sign out"
          )}
        </button>

        <button
          type="button"
          onClick={toggleCollapsed}
          className="mt-3 flex w-full items-center justify-center rounded-xl border border-white/10 bg-white/5 p-2 text-slate-300 transition-colors hover:bg-white/10 hover:text-white cursor-pointer"
          aria-label={collapsed ? "Expand sidebar" : "Collapse sidebar"}
        >
          <FontAwesomeIcon icon={collapsed ? faAnglesRight : faAnglesLeft} />
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
