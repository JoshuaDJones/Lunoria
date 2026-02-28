import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface IntroCreationTilesProps {}

const IntroCreationTiles = ({}: IntroCreationTilesProps) => {
  return (
    <div className="my-2 flex gap-3">
      <div className="rounded-lg border-2 border-white h-[120px] w-[100px]"></div>
      <div className="rounded-lg border-2 border-white h-[120px] w-[100px]"></div>
      <div className="rounded-lg border-2 border-white h-[120px] w-[100px]"></div>
      <div className="rounded-lg border-2 border-white h-[120px] w-[100px]"></div>
      <div className="rounded-lg border-2 border-white h-[120px] w-[100px] items-center justify-center flex bg-stone-600">
        <FontAwesomeIcon icon={faPlus} color="white" size="3x" />
      </div>
    </div>
  );
};

export default IntroCreationTiles;
