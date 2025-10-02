import React, {
  createContext,
  PropsWithChildren,
  ReactElement,
  useCallback,
  useContext,
  useMemo,
  useState,
} from "react";

const SUB_STACK_KEY: keyof NavigableModal = "subContentStack";

export interface StaticModal {
  modal: ReactElement;
}

export interface NavigableModal {
  modal: ReactElement;
  subContentStack?: ReactElement[];
}

export type ModalItem = StaticModal | NavigableModal;

const isNavigableModal = (
  modalItem: ModalItem,
): modalItem is NavigableModal => {
  return SUB_STACK_KEY in modalItem;
};

const getLastModal = (modalItems: ModalItem[]): ModalItem | undefined => {
  if (modalItems.length === 0) return undefined;
  return modalItems[modalItems.length - 1];
};

const getNavigableLastModal = (
  modalItems: ModalItem[],
): NavigableModal | undefined => {
  const lastModal = getLastModal(modalItems);
  if (!lastModal) return undefined;

  return isNavigableModal(lastModal) ? lastModal : undefined;
};

interface IModalRouterContext {
  push: (modal: ReactElement, initialSubContentStack?: ReactElement[]) => void;
  pop: () => void;
  dismissAll: () => void;
  pushSubContent: (subContent: ReactElement) => void;
  pushSubContentRoot: (subContent: ReactElement) => void;
  popSubContent: () => void;
  popSubContentToRoot: () => void;
  canPopSubContent: boolean;
}

const ModalRouterContext = createContext<IModalRouterContext>({
  push: () => undefined,
  pop: () => undefined,
  dismissAll: () => undefined,
  pushSubContent: () => null,
  pushSubContentRoot: () => null,
  popSubContent: () => null,
  popSubContentToRoot: () => null,
  canPopSubContent: false,
});

export const ModalRouterProvider = ({ children }: PropsWithChildren) => {
  const [modals, setModals] = useState<ModalItem[]>([]);

  const push = useCallback(
    (modal: ReactElement, initialSubContentStack?: ReactElement[]) => {
      setModals((prev) => [
        ...prev,
        initialSubContentStack
          ? { modal: modal, subContentStack: initialSubContentStack }
          : { modal: modal },
      ]);
    },
    [],
  );

  const pop = useCallback(() => {
    setModals((prev) => prev.slice(0, -1));
  }, []);

  const dismissAll = useCallback(() => {
    setModals([]);
  }, []);

  const pushSubContent = useCallback((subContent: ReactElement) => {
    setModals((prev) => {
      const navigableModal = getNavigableLastModal(prev);
      if (!navigableModal) return prev;

      const updatedModal = {
        ...navigableModal,
        subContentStack: [
          ...(navigableModal.subContentStack ?? []),
          subContent,
        ],
      };

      return [...prev.slice(0, -1), updatedModal];
    });
  }, []);

  const pushSubContentRoot = useCallback((subContent: ReactElement) => {
    setModals((prev) => {
      const navigableModal = getNavigableLastModal(prev);
      if (!navigableModal) return prev;

      const updatedModal = { ...navigableModal, subContentStack: [subContent] };

      return [...prev.slice(0, -1), updatedModal];
    });
  }, []);

  const popSubContent = useCallback(() => {
    setModals((prev) => {
      const navigableModal = getNavigableLastModal(prev);
      if (!navigableModal) return prev;

      const subContentStack = navigableModal.subContentStack;
      if (!subContentStack || subContentStack.length === 0) return prev;

      const updatedModal = {
        ...navigableModal,
        subContentStack: subContentStack.slice(0, -1),
      };

      return [...prev.slice(0, -1), updatedModal];
    });
  }, []);

  const popSubContentToRoot = useCallback(() => {
    setModals((prev) => {
      const navigableModal = getNavigableLastModal(prev);
      if (!navigableModal) return prev;

      const subContentStack = navigableModal.subContentStack;
      if (!subContentStack || subContentStack.length <= 0) return prev;

      const updatedLastModal = {
        ...navigableModal,
        subContentStack: subContentStack.slice(0, 1),
      };

      return [...prev.slice(0, -1), updatedLastModal];
    });
  }, []);

  const canPopSubContent = useMemo(() => {
    const navigableModal = getNavigableLastModal(modals);
    if (!navigableModal) return false;

    const subContentStack = navigableModal.subContentStack ?? [];

    return subContentStack.length > 1;
  }, [modals]);

  const modalRouter = useMemo(
    () => ({
      push,
      pop,
      dismissAll,
      pushSubContent,
      pushSubContentRoot,
      popSubContent,
      popSubContentToRoot,
      canPopSubContent,
    }),
    [
      push,
      pop,
      dismissAll,
      pushSubContent,
      pushSubContentRoot,
      popSubContent,
      popSubContentToRoot,
      canPopSubContent,
    ],
  );

  return (
    <ModalRouterContext.Provider value={modalRouter}>
      {modals.map((modal, idx) => {
        const content = isNavigableModal(modal)
          ? modal.subContentStack?.[modal.subContentStack.length - 1]
          : undefined;

        return React.cloneElement(modal.modal, { key: idx }, content);
      })}

      {children}
    </ModalRouterContext.Provider>
  );
};

export const useModalRouter = () => {
  return useContext(ModalRouterContext);
};
