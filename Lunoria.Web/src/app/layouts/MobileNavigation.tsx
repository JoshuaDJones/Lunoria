import { useEffect, useRef, type ReactNode } from "react";
import { createPortal } from "react-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBars, faXmark } from "@fortawesome/free-solid-svg-icons";
import { useLocation } from "react-router-dom";

export function MobileNavigation({
  title,
  children,
}: {
  title: string;
  children: ReactNode;
}) {
  const dialog = useRef<HTMLDialogElement>(null);
  const location = useLocation();

  useEffect(() => {
    dialog.current?.close();
  }, [location]);

  useEffect(() => {
    const desktop = window.matchMedia("(min-width: 1024px)");
    const closeOnDesktop = () => {
      if (desktop.matches) dialog.current?.close();
    };
    desktop.addEventListener("change", closeOnDesktop);
    return () => desktop.removeEventListener("change", closeOnDesktop);
  }, []);

  return (
    <>
      <header className="relative z-20 flex h-14 shrink-0 items-center gap-3 border-b border-white/10 bg-stone-900/95 px-3 text-stone-100 lg:hidden">
        <button
          type="button"
          aria-label="Open navigation"
          aria-haspopup="dialog"
          onClick={() => dialog.current?.showModal()}
          className="flex h-11 w-11 items-center justify-center rounded-lg hover:bg-white/10 focus-visible:outline-2 focus-visible:outline-blue-400"
        >
          <FontAwesomeIcon icon={faBars} />
        </button>
        <span className="font-semibold">{title}</span>
      </header>
      {createPortal(
        <dialog
          ref={dialog}
          aria-label="Main navigation"
          onClick={(event) => {
            if (event.target === event.currentTarget) dialog.current?.close();
          }}
          className="fixed inset-0 m-0 h-dvh max-h-none w-full max-w-none border-0 bg-transparent p-0 text-stone-100 backdrop:bg-black/60"
        >
          <div className="flex h-full w-72 max-w-[85vw] flex-col border-r border-white/10 bg-stone-900 p-5 shadow-xl">
            <div className="mb-6 flex items-center justify-between gap-3">
              <span className="font-semibold">{title}</span>
              <button
                type="button"
                aria-label="Close navigation"
                onClick={() => dialog.current?.close()}
                className="flex h-11 w-11 items-center justify-center rounded-lg hover:bg-white/10 focus-visible:outline-2 focus-visible:outline-blue-400"
              >
                <FontAwesomeIcon icon={faXmark} />
              </button>
            </div>
            {children}
          </div>
        </dialog>,
        document.body,
      )}
    </>
  );
}
