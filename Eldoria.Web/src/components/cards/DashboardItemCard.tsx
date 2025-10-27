import Text, { TextColor, TextSize } from "../typography/Text";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleCheck } from "@fortawesome/free-solid-svg-icons";
import { JourneyCharacterItemDto } from "../../types/journey";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { SceneCharacterItemDto } from "../../types/scene";

interface DashboardItemCardProps {
  journeyCharacterItem?: JourneyCharacterItemDto;
  sceneCharacterItem?: SceneCharacterItemDto;
  onRefreshRequest: () => void;
}

const DashboardItemCard = ({
  journeyCharacterItem,
  sceneCharacterItem,
  onRefreshRequest,
}: DashboardItemCardProps) => {
  const { patch } = useApi();
  const { showToast } = useToast();

  const item = journeyCharacterItem?.item ?? sceneCharacterItem?.item;

  if (!item) return null;

  const handleUseItem = async () => {
    if (!journeyCharacterItem && !sceneCharacterItem) return;

    try {
      if (journeyCharacterItem) {
        await patch(`${BASE_URL}/JourneyCharacterItem`, {
          JourneyCharacterItemId: journeyCharacterItem.id,
        });
      } else {
        if (!sceneCharacterItem) return;

        await patch(`${BASE_URL}/SceneCharacterItem`, {
          SceneCharacterItemId: sceneCharacterItem.id,
        });
      }

      onRefreshRequest();
      showToast("Success", "Item used.", ToastType.success, 3000);
    } catch (err) {
      showToast("Error", "Item was not used.", ToastType.error, 3000);
      console.error(err);
    }
  };

  return (
    <div key={item.id} className="flex items-start gap-2">
      <img className="w-[15%] rounded-xl" src={item.photoUrl} alt="" />
      <div className="flex flex-col w-full">
        <div className="flex items-center">
          <Text className="font-bold flex-1" textColor={TextColor.white}>
            {item.name}
          </Text>
          <button
            className="cursor-pointer hover:opacity-50"
            onClick={() => handleUseItem()}
          >
            <FontAwesomeIcon
              icon={faCircleCheck}
              className="text-green-400"
              size="lg"
            />
          </button>
        </div>
        <Text size={TextSize.xs} textColor={TextColor.white}>
          {item.description}
        </Text>
      </div>
    </div>
  );
};

export default DashboardItemCard;
