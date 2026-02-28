import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { IntroPage } from "../../types/journey";

interface IntroCreationTilesProps {
  workingIntroPages: IntroPage[];
  onAddNew: () => void;
  onEdit: (introPage: IntroPage) => void;
}

const IntroCreationTiles = ({
  workingIntroPages = [],
  onAddNew,
  onEdit,
}: IntroCreationTilesProps) => {
  return (
    <div className="my-2 flex gap-3">
      {workingIntroPages.map((ip) => (
        <div
          key={ip.id}
          className="rounded-lg border-2 border-white h-[120px] w-[100px]"
          onClick={() => onEdit(ip)}
        >
          <img
            src={ip.previewPhotoUrl}
            alt="Intro Page Preview"
            className="h-full w-full object-cover rounded-lg"
          />
        </div>
      ))}
      <div
        onClick={onAddNew}
        className="rounded-lg border-2 border-white h-[120px] w-[100px] items-center justify-center flex bg-stone-600"
      >
        <FontAwesomeIcon icon={faPlus} color="white" size="3x" />
      </div>
    </div>
  );
};

export default IntroCreationTiles;
