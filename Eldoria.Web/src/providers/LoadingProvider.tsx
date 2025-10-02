import { createContext, PropsWithChildren, useContext, useState } from "react";
import LoadingModal from "../components/Modal/LoadingModal";

interface ILoadingContext {
  showLoading: () => void;
  closeLoading: () => void;
}

const LoadingContext = createContext<ILoadingContext | undefined>(undefined);

const LoadingProvider = ({ children }: PropsWithChildren) => {
  const [isShowLoading, setShowLoading] = useState(false);

  const showLoading = () => {
    setShowLoading(true);
  };

  const closeLoading = () => {
    setShowLoading(false);
  };

  return (
    <LoadingContext.Provider value={{ showLoading, closeLoading }}>
      {children}
      {isShowLoading && <LoadingModal />}
    </LoadingContext.Provider>
  );
};

const useLoading = () => {
  const context = useContext(LoadingContext);

  if (context === undefined)
    throw new Error("useLoading must be used within LoadingProvider");

  return context;
};

export { LoadingProvider, useLoading };
