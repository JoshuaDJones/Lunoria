import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type PropsWithChildren,
} from "react";
import {
  faCircleCheck,
  faCircleExclamation,
  faCircleInfo,
  faXmark,
  type IconDefinition,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  ToastContext,
  type ToastOptions,
  type ToastVariant,
} from "./toastContext";

interface ToastEntry extends Required<Omit<ToastOptions, "title">> {
  id: number;
  title?: string;
}

const variantStyles: Record<
  ToastVariant,
  { icon: IconDefinition; accent: string; iconColor: string }
> = {
  success: {
    icon: faCircleCheck,
    accent: "border-l-add",
    iconColor: "text-add-hover",
  },
  info: {
    icon: faCircleInfo,
    accent: "border-l-brand",
    iconColor: "text-brand-hover",
  },
  error: {
    icon: faCircleExclamation,
    accent: "border-l-danger",
    iconColor: "text-danger",
  },
};

interface ToastItemProps {
  toast: ToastEntry;
  onDismiss: (id: number) => void;
}

function ToastItem({ toast, onDismiss }: ToastItemProps) {
  const timerRef = useRef<ReturnType<typeof setTimeout>>(undefined);
  const style = variantStyles[toast.variant];

  const startTimer = useCallback(() => {
    if (toast.duration <= 0) return;

    clearTimeout(timerRef.current);
    timerRef.current = setTimeout(() => onDismiss(toast.id), toast.duration);
  }, [onDismiss, toast.duration, toast.id]);

  const pauseTimer = () => clearTimeout(timerRef.current);

  useEffect(() => {
    startTimer();
    return () => clearTimeout(timerRef.current);
  }, [startTimer]);

  return (
    <section
      role={toast.variant === "error" ? "alert" : "status"}
      className={`pointer-events-auto flex w-full items-start gap-3 rounded-lg border-2 border-l-6 border-border bg-surface-raised p-4 text-content shadow-2xl ${style.accent}`}
      onMouseEnter={pauseTimer}
      onMouseLeave={startTimer}
      onFocus={pauseTimer}
      onBlur={startTimer}
    >
      <FontAwesomeIcon
        icon={style.icon}
        className={`mt-0.5 text-lg ${style.iconColor}`}
        aria-hidden="true"
      />
      <div className="min-w-0 flex-1">
        {toast.title && <h2 className="font-semibold">{toast.title}</h2>}
        <p className="text-sm text-content-secondary">{toast.message}</p>
      </div>
      <button
        type="button"
        className="-m-1 cursor-pointer rounded-md p-1 text-content-muted transition hover:text-content focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-hover/40"
        aria-label="Dismiss notification"
        onClick={() => onDismiss(toast.id)}
      >
        <FontAwesomeIcon icon={faXmark} />
      </button>
    </section>
  );
}

export function ToastProvider({ children }: PropsWithChildren) {
  const [toasts, setToasts] = useState<ToastEntry[]>([]);
  const nextIdRef = useRef(1);

  const dismiss = useCallback((id: number) => {
    setToasts((current) => current.filter((toast) => toast.id !== id));
  }, []);

  const dismissAll = useCallback(() => setToasts([]), []);

  const show = useCallback((options: ToastOptions) => {
    const id = nextIdRef.current++;
    const variant = options.variant ?? "info";

    setToasts((current) => [
      ...current.slice(-4),
      {
        id,
        title: options.title,
        message: options.message,
        variant,
        duration: options.duration ?? (variant === "error" ? 8000 : 5000),
      },
    ]);

    return id;
  }, []);

  const value = useMemo(
    () => ({
      show,
      success: (message: string, title?: string) =>
        show({ message, title, variant: "success" }),
      info: (message: string, title?: string) =>
        show({ message, title, variant: "info" }),
      error: (message: string, title?: string) =>
        show({ message, title, variant: "error" }),
      dismiss,
      dismissAll,
    }),
    [dismiss, dismissAll, show],
  );

  return (
    <ToastContext.Provider value={value}>
      {children}
      <div
        aria-label="Notifications"
        className="pointer-events-none fixed right-4 top-4 z-[10000] flex w-[calc(100%-2rem)] max-w-sm flex-col gap-3"
      >
        {toasts.map((toast) => (
          <ToastItem key={toast.id} toast={toast} onDismiss={dismiss} />
        ))}
      </div>
    </ToastContext.Provider>
  );
}
