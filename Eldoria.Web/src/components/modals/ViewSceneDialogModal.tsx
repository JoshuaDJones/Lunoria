import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import { DialogPageDto, SceneDialogDto } from "../../types/scene";
import AppModal from "./AppModal";
import {
  faArrowAltCircleRight,
  faArrowCircleLeft,
  faArrowCircleRight,
  faArrowLeft,
  faArrowRight,
  faClose,
  faSquareXmark,
  faWindowClose,
} from "@fortawesome/free-solid-svg-icons";
import { useState } from "react";
import ViewDialogSection from "../dialogs/ViewDialogSection";

interface ViewableDialogPage {
  isViewing: boolean;
  page: DialogPageDto;
}

interface ViewSceneDialogModalProps {
  sceneDialog: SceneDialogDto;
}

const ViewSceneDialogModal = ({ sceneDialog }: ViewSceneDialogModalProps) => {
  const modalRouter = useModalRouter();

  const pages = sceneDialog.dialogPages.sort((a, b) => a.orderNum - b.orderNum);
  const [viewableDialogPages, setViewableDialogPages] = useState(
    pages.map((p, idx) => {
      return {
        isViewing: idx === 0,
        page: p,
      };
    }),
  );

  const currentPage = viewableDialogPages.find(
    (p) => p.isViewing === true,
  )?.page;

  const nextPage = () => {
    const currentIdx = viewableDialogPages.findIndex(
      (p) => p.page.id === currentPage?.id,
    );
    const nextIdx = currentIdx + 1;

    const next = viewableDialogPages[nextIdx];

    if (!next) return;

    const nextId = next.page.id;

    setViewableDialogPages((prev) =>
      prev.map((p) => ({
        ...p,
        isViewing: p.page.id === nextId,
      })),
    );
  };

  const prevPage = () => {
    const currentIdx = viewableDialogPages.findIndex(
      (p) => p.page.id === currentPage?.id,
    );
    const prevIdx = currentIdx - 1;

    const prev = viewableDialogPages[prevIdx];

    if (!prev) return;

    const prevId = prev.page.id;

    setViewableDialogPages((prev) =>
      prev.map((p) => ({
        ...p,
        isViewing: p.page.id === prevId,
      })),
    );
  };

  return (
    <AppModal centerContent backgroundOverride="bg-stone-500/50">
      <div className="h-screen w-screen items-center justify-center flex">
        <div className="flex flex-col items-center gap-4 relative w-[85%] h-[99%] bg-stone-800/95 border-8 border-stone-400 rounded-2xl p-5">
          <div className="flex flex-1 overflow-hidden">
            <img
              src={currentPage?.photoUrl}
              className="self-center rounded-3xl h-full w-full object-contain"
            />
          </div>

          <div className="absolute top-0 right-0 left-0 bottom-0 flex flex-col items-center justify-center gap-10">
            {currentPage?.dialogPageSections.map((s) => (
              <ViewDialogSection pageSection={s} />
            ))}
          </div>

          <button
            className="p-5 absolute top-0 right-0"
            onClick={modalRouter.pop}
          >
            <FontAwesomeIcon
              icon={faSquareXmark}
              size="3x"
              className="text-red-400 hover:text-red-200"
            />
          </button>
          <button className="p-5 absolute bottom-0 left-0" onClick={prevPage}>
            <FontAwesomeIcon
              icon={faArrowLeft}
              size="3x"
              className="text-stone-400 hover:text-stone-200"
            />
          </button>
          <button className="p-5 absolute bottom-0 right-0" onClick={nextPage}>
            <FontAwesomeIcon
              icon={faArrowRight}
              size="3x"
              className="text-stone-400 hover:text-stone-200"
            />
          </button>
        </div>
      </div>
    </AppModal>
  );
};

export default ViewSceneDialogModal;
