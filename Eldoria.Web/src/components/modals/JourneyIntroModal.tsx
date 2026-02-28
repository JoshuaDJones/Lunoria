import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faArrowRight,
  faSquareXmark,
} from "@fortawesome/free-solid-svg-icons";
import AppModal from "./AppModal";
import { useModalRouter } from "../../providers/ModalRouterProvider";

interface JourneyIntroModalProps {}

const JourneyIntroModal = () => {
  const modalRouter = useModalRouter();
  return (
    <AppModal centerContent backgroundOverride="bg-stone-500/50">
      <div className="h-screen w-screen items-center justify-center flex">
        <div className="flex flex-col items-center gap-4 relative w-[85%] h-[99%] bg-stone-800/95 border-8 border-stone-400 rounded-2xl p-5">
          {/* <div className="flex flex-1 overflow-hidden">
            <img
              src={currentPage?.photoUrl}
              className="self-center rounded-3xl h-full w-full object-contain"
            />
          </div> */}
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
          <button className="p-5 absolute bottom-0 left-0">
            <FontAwesomeIcon
              icon={faArrowLeft}
              size="3x"
              className="text-stone-400 hover:text-stone-200"
            />
          </button>
          <button className="p-5 absolute bottom-0 right-0">
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

export default JourneyIntroModal;
