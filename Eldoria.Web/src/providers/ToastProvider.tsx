import { createContext, PropsWithChildren, useContext, useState } from "react";
import Toast from "../components/Toast";

interface Toast {
  id: string;
  hidden: boolean;
  title: string;
  type: ToastType;
  message: string;
  duration: ToastDuration;
}

export enum ToastType {
  error,
  success,
  informational,
}

export enum ToastDuration {
  short = 3000,
  medium = 5000,
  long = 7000,
}

interface IToastContext {
  showToast: (
    title: string,
    message: string,
    type: ToastType,
    duration: ToastDuration,
  ) => void;
}

const ToastContext = createContext<IToastContext | undefined>(undefined);

const ToastProvider = ({ children }: PropsWithChildren) => {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const showToast = (
    title: string,
    message: string,
    type: ToastType,
    duration: ToastDuration,
  ) => {
    setToasts((prev) => [
      ...prev,
      {
        id: crypto.randomUUID(),
        title,
        message,
        type,
        duration,
        hidden: false,
      },
    ]);
  };

  const dismissToast = (id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  };

  return (
    <ToastContext.Provider value={{ showToast }}>
      {children}
      <div className="fixed bottom-20 left-20 flex flex-col gap-4 z-50">
        {toasts
          .filter((x) => !x.hidden)
          .map((toast) => (
            <Toast
              key={toast.id}
              id={toast.id}
              title={toast.title}
              message={toast.message}
              type={toast.type}
              duration={toast.duration}
              onDismiss={dismissToast}
            />
          ))}
      </div>
    </ToastContext.Provider>
  );
};

const useToast = () => {
  const context = useContext(ToastContext);
  if (context === undefined) {
    throw new Error("useToast must be used within a ToastProvider");
  }
  return context;
};

export { ToastProvider, useToast };
