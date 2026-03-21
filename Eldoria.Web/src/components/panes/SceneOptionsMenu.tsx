import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBars, faClose } from "@fortawesome/free-solid-svg-icons";
import { useState } from "react";
import clsx from "clsx";
import Text, { TextColor } from "../typography/Text";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import IntroViewerModal from "../modals/IntroViewerModal";

interface SceneOptionsMenuProps {
}

const SceneOptionsMenu = ({
}: SceneOptionsMenuProps) => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const[isButtonHovered, setIsButtonHovered] = useState(false);
  const modalRouter = useModalRouter();

  const OpenIntro = () => {
    setIsMenuOpen(false);
    modalRouter.push(<IntroViewerModal />)
  }

  return !isMenuOpen ? <button 
    onClick={() => setIsMenuOpen(true)}
    onMouseOver={() => setIsButtonHovered(true)}
    onMouseOut={() => setIsButtonHovered(false)}
     className={clsx("absolute right-0 top-0 p-4 rounded-lg z-30",
      {
        "bg-slate-200/10": isButtonHovered,
        "bg-slate-200/5": !isButtonHovered
      }      
     )}>
      <FontAwesomeIcon
        icon={faBars}
        size={"2xl"}
        className={clsx({
            "text-stone-900/10": !isButtonHovered,
            "text-stone-900/50": isButtonHovered
        })}/>
    </button>
    : 
    <div className="absolute right-0 top-0 w-[200px] rounded-lg z-30 bg-slate-400/50 flex flex-col">
      <button className="self-end p-2" onClick={() => setIsMenuOpen(false)}>
        <FontAwesomeIcon
        icon={faClose}
        size={"xl"}
        className="text-white hover:text-slate-500"
        />
      </button>
      <div className="border-b-4 border-stone-600/20" />
      <button onClick={OpenIntro} className="text-white px-2 py-2 hover:bg-slate-500/20 rounded-b-lg">        
        <Text textColor={TextColor.white}>Begin Intro</Text>
      </button>
    </div>

}

export default SceneOptionsMenu