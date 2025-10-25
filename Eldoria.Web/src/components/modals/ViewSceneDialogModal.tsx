import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import { DialogPageDto, SceneDialogDto } from "../../types/scene";
import AppModal from "./AppModal";
import { faArrowAltCircleRight, faArrowCircleLeft, faArrowCircleRight, faArrowLeft, faArrowRight, faClose } from "@fortawesome/free-solid-svg-icons";
import { useState } from "react";
import ViewDialogSection from "../dialogs/ViewDialogSection";

interface ViewableDialogPage{
    isViewing: boolean;
    page: DialogPageDto;
}

interface ViewSceneDialogModalProps{
    sceneDialog: SceneDialogDto;
}

const ViewSceneDialogModal = ({
    sceneDialog
}: ViewSceneDialogModalProps) => {
    const modalRouter = useModalRouter();

    const pages = sceneDialog.dialogPages.sort((a, b) => a.orderNum - b.orderNum);
    const [viewableDialogPages, setViewableDialogPages] = useState(pages.map((p, idx) => {
        return {
            isViewing: idx === 0,
            page: p
        }
    }));

    const currentPage = viewableDialogPages.find(p => p.isViewing === true)?.page;

// modalRouter.pop
  return (
        <AppModal centerContent backgroundOverride="bg-stone-500/50">
      <div className="h-screen w-screen items-center justify-center flex">
            <div className="flex flex-col items-center gap-4 relative w-[85%] h-[95%] bg-stone-800 border-8 border-stone-400 rounded-2xl p-10">                
                <div className="flex h-[33%] bg-red-200">
                    <img src={currentPage?.photoUrl} className="self-center rounded-3xl h-full object-contain"/>
                </div>
                {currentPage?.dialogPageSections.map(s => <ViewDialogSection pageSection={s} />)}
                
                    

                



                <button className="p-5 absolute top-0 right-0" onClick={modalRouter.pop}>
                    <FontAwesomeIcon icon={faClose} size="3x" className="text-red-400 hover:text-red-200" />
                </button>
                <button className="p-5 absolute bottom-0 left-0">
                    <FontAwesomeIcon icon={faArrowLeft} size="3x" className="text-blue-400 hover:text-blue-200"/>
                </button>
                <button className="p-5 absolute bottom-0 right-0">
                    <FontAwesomeIcon icon={faArrowRight} size="3x" className="text-blue-400 hover:text-blue-200" />
                </button>
            </div>
      </div>
    </AppModal>
  )
}

export default ViewSceneDialogModal