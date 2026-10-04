import { ReactNode, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { MobileNavigation } from "@/app/layouts/MobileNavigation";
import clsx from "clsx";
import { type IconDefinition } from "@fortawesome/fontawesome-svg-core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faAnglesLeft,
  faAnglesRight,
  faUser,
  faWandMagicSparkles,
  faBottleDroplet,
  faShield,
  faArrowRightFromBracket,
  faScroll,
} from "@fortawesome/free-solid-svg-icons";
import { useAuth } from "@/features/auth/hooks/useAuth";

export interface SidebarItem {
  label: string;
  to?: string;
  href?: string;
  icon?: IconDefinition;
  active?: boolean;
  group?: string;
}

interface SidebarProps {
  title?: string;
  subtitle?: string;
  items?: SidebarItem[];
  className?: string;
  children?: ReactNode;
}

const defaultItems: SidebarItem[] = [
  { label: "Series", to: "/home", icon: faScroll },
  { label: "Characters", to: "/characters", icon: faUser, group: "Library" },
  {
    label: "Spells",
    to: "/spells",
    icon: faWandMagicSparkles,
    group: "Library",
  },
  {
    label: "Consumables",
    to: "/consumables",
    icon: faBottleDroplet,
    group: "Library",
  },
  { label: "Equipment", to: "/equipment", icon: faShield, group: "Library" },
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
  const { pathname: activePath } = useLocation();
  const matchesPath = (path: string) =>
    activePath === path || activePath.startsWith(`${path}/`);

  const updatedItems = items.map((item) => ({
    ...item,
    active:
      item.active ??
      (item.to === "/home"
        ? [
            "/home",
            "/series",
            "/journeys",
            "/playthroughs",
            "/scene-grids",
          ].some(matchesPath)
        : Boolean(item.to && matchesPath(item.to))),
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

  const navigation = (compact: boolean) => (
    <>
      <nav
        aria-label="Main navigation"
        className="flex min-h-0 flex-1 flex-col gap-2 overflow-y-auto"
      >
        {updatedItems.map((item, index) => {
          const content = (
            <>
              <span className="text-base">
                {item.icon ? <FontAwesomeIcon icon={item.icon} /> : null}
              </span>
              {!compact && <span>{item.label}</span>}
            </>
          );

          const className = clsx(
            "flex min-h-11 items-center gap-3 rounded-xl px-3 py-2 text-sm font-medium transition-colors focus-visible:outline-2 focus-visible:outline-blue-400",
            compact ? "justify-center px-2" : "",
            item.active
              ? "text-blue-300 bg-white/10"
              : "text-stone-300 hover:bg-white/10 hover:text-white",
          );

          const accessibility = {
            "aria-label": item.label,
            "aria-current": item.active ? ("page" as const) : undefined,
            title: compact ? item.label : undefined,
          };
          return (
            <div key={item.label}>
              {item.group &&
                item.group !== updatedItems[index - 1]?.group &&
                (compact ? (
                  <div className="my-3 border-t border-white/10" />
                ) : (
                  <p className="mb-2 mt-5 px-3 text-xs font-semibold uppercase tracking-wider text-stone-400">
                    {item.group}
                  </p>
                ))}
              {item.to ? (
                <Link to={item.to} className={className} {...accessibility}>
                  {content}
                </Link>
              ) : item.href ? (
                <a href={item.href} className={className} {...accessibility}>
                  {content}
                </a>
              ) : (
                <button type="button" className={className} {...accessibility}>
                  {content}
                </button>
              )}
            </div>
          );
        })}
      </nav>

      <div className="mt-auto shrink-0 pt-6">
        {children}
        <button
          type="button"
          onClick={signOut}
          aria-label="Sign out"
          title={compact ? "Sign out" : undefined}
          className="rounded-lg border border-stone-600 px-5 py-2 w-full text-sm text-stone-200 transition hover:border-danger hover:text-danger cursor-pointer flex items-center justify-center"
        >
          {compact ? (
            <FontAwesomeIcon icon={faArrowRightFromBracket} />
          ) : (
            "Sign out"
          )}
        </button>
      </div>
    </>
  );

  return (
    <>
      <MobileNavigation title={title}>{navigation(false)}</MobileNavigation>
      <aside
        className={clsx(
          "relative z-20 hidden h-full shrink-0 flex-col border-r border-white/10 bg-stone-900/85 p-5 text-stone-100 shadow-xl backdrop-blur-sm transition-[width] duration-200 motion-reduce:transition-none lg:flex",
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
        {navigation(collapsed)}
        <button
          type="button"
          onClick={toggleCollapsed}
          className="mt-3 flex w-full items-center justify-center rounded-xl border border-white/10 bg-white/5 p-2 text-slate-300 transition-colors hover:bg-white/10 hover:text-white cursor-pointer"
          aria-label={collapsed ? "Expand sidebar" : "Collapse sidebar"}
          title={collapsed ? "Expand sidebar" : "Collapse sidebar"}
        >
          <FontAwesomeIcon icon={collapsed ? faAnglesRight : faAnglesLeft} />
        </button>
      </aside>
    </>
  );
};

export default Sidebar;
