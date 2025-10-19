import { faClose, faEllipsisVertical } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useState } from "react";
import Text, { TextColor, TextSize } from "./typography/Text";
import { ItemDto } from "../types/item";
import { BASE_URL, useApi } from "../hooks/useApi";
import { ToastType, useToast } from "../providers/ToastProvider";
import { NotificationType } from "./lists/NotificationsList";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "./buttons/AppButton";

interface CharacterMenuProps {
  sceneCharacterId?: number;
  journeyCharacterId?: number;
  onRefreshRequest: () => void;
  onDropAdded: (text: string, type: NotificationType) => void;
}

const CharacterMenu = ({
  sceneCharacterId,
  journeyCharacterId,
  onRefreshRequest,
  onDropAdded,
}: CharacterMenuProps) => {
  const { del } = useApi();
  const [menuOpen, setMenuOpen] = useState(false);
  const [itemMenuOpen, setItemMenuOpen] = useState(false);
  const [deleteConfirmationOpen, setDeleteConfirmationOpen] = useState(false);
  const { showToast } = useToast();

  if (!menuOpen && !itemMenuOpen) {
    return (
      <button
        className="absolute top-1 right-3 cursor-pointer hover:opacity-50 p-2"
        onClick={() => setMenuOpen(true)}
      >
        <FontAwesomeIcon icon={faEllipsisVertical} size="lg" color="white" />
      </button>
    );
  }

  if (itemMenuOpen) {
    return (
      <div className="absolute flex flex-col top-3 right-3 rounded-lg bg-gray-700 border-2 border-gray-500 w-[75%] font-cinzel text-white">
        <CharacterMenuItemsList
          onRefreshRequest={onRefreshRequest}
          sceneCharacterId={sceneCharacterId}
          journeyCharacterId={journeyCharacterId}
        />
        <button
          className="items-center justify-center flex gap-1 self-end py-2 pr-2 hover:opacity-50"
          onClick={() => setItemMenuOpen(false)}
        >
          <Text size={TextSize.xs} textColor={TextColor.red}>
            Close
          </Text>
          <FontAwesomeIcon icon={faClose} size="sm" className="text-red-400" />
        </button>
      </div>
    );
  }

  const handleDrop = () => {
    const randomFloat = Math.random() * 100;

    if (randomFloat > 50) {
      onDropAdded("+ 4 Hp", NotificationType.hp);
    } else {
      onDropAdded("+ 4 Mp", NotificationType.mp);
    }

    setMenuOpen(false);
    setItemMenuOpen(false);
  };

  const handleConfirmDelete = async () => {
    if (!journeyCharacterId && !sceneCharacterId) return;

    try {
      if (journeyCharacterId)
        await del(`${BASE_URL}/JourneyCharacter/${journeyCharacterId}`);
      else await del(`${BASE_URL}/SceneCharacter/${sceneCharacterId}`);

      setMenuOpen(false);
      showToast("Success", "Character deleted.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      showToast("Error", "Character not deleted.", ToastType.error, 3000);
      console.error(err);
    }
  };

  return (
    <div className="absolute flex flex-col top-3 right-3 rounded-lg bg-gray-700 border-2 border-gray-500 w-[55%] font-cinzel text-white">
      <button
        className="py-2 hover:opacity-50 text-sm"
        onClick={() => {
          setMenuOpen(false);
          setItemMenuOpen(true);
        }}
      >
        Add Item
      </button>
      <div className="h-[2px] bg-gray-500"></div>
      <button
        className="py-2 hover:opacity-50 text-sm"
        onClick={() => handleDrop()}
      >
        Add Drop
      </button>
      <div className="h-[2px] bg-gray-500"></div>
      <button
        className="py-2 hover:opacity-50 text-sm"
        onClick={() => setDeleteConfirmationOpen(!deleteConfirmationOpen)}
      >
        Delete Character
      </button>
      {deleteConfirmationOpen && (
        <div className="flex flex-col gap-1 py-2 border border-black mb-1 mx-1 rounded-xl">
          <Text className="self-center" textColor={TextColor.white}>
            Are you sure?
          </Text>
          <div className="flex gap-2 justify-center">
            <AppButton
              title={"Yes"}
              variant={AppButtonVariant.go}
              size={AppButtonSize.xs}
              onClick={async () => await handleConfirmDelete()}
            />
            <AppButton
              title={"No"}
              variant={AppButtonVariant.warning}
              size={AppButtonSize.xs}
              onClick={() => setDeleteConfirmationOpen(false)}
            />
          </div>
        </div>
      )}
      <div className="h-[2px] bg-gray-500"></div>
      <button
        className="items-center justify-center flex gap-1 self-end py-2 pr-2 hover:opacity-50"
        onClick={() => {
          setMenuOpen(false);
          setItemMenuOpen(false);
        }}
      >
        <Text size={TextSize.xs} textColor={TextColor.red}>
          Close
        </Text>{" "}
        <FontAwesomeIcon icon={faClose} size="sm" className="text-red-400" />
      </button>
    </div>
  );
};

interface CharacterMenuItemListProps {
  sceneCharacterId?: number;
  journeyCharacterId?: number;
  onRefreshRequest: () => void;
}

const CharacterMenuItemsList = ({
  journeyCharacterId,
  sceneCharacterId,
  onRefreshRequest,
}: CharacterMenuItemListProps) => {
  const { get, post } = useApi();
  const { showToast } = useToast();

  const [items, setItems] = useState<ItemDto[]>([]);

  const getItems = async () => {
    try {
      const items: ItemDto[] = await get(`${BASE_URL}/Item`);
      setItems(items);
    } catch (err) {
      console.error(err);
      showToast("Error", "Unable to get items.", ToastType.error, 3000);
    }
  };

  useEffect(() => {
    getItems();
  }, []);

  const addItem = async (itemId: number) => {
    try {
      if (!!journeyCharacterId) {
        await post(`${BASE_URL}/JourneyCharacterItem`, {
          JourneyCharacterId: journeyCharacterId,
          ItemId: itemId,
        });
      } else {
        await post(`${BASE_URL}/SceneCharacterItem`, {
          SceneCharacterId: sceneCharacterId,
          ItemId: itemId,
        });
      }

      showToast("Success", "Item Added.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      showToast("Error", "Item Not Added.", ToastType.error, 3000);
      console.error(err);
    }
  };

  return (
    <div className="flex flex-col">
      <div className="flex items-center justify-center pt-2">
        <Text
          className="self-center mb-3 text-gray-300"
          size={TextSize.xs}
          textColor={TextColor.custom}
        >
          Add Item
        </Text>
      </div>

      {items.map((i) => (
        <button
          onClick={async () => await addItem(i.id)}
          className="flex flex-1 p-2 items-center gap-2 hover:opacity-50 cursor-pointer"
        >
          <img src={i.photoUrl} className="w-[40px] rounded-xl" />
          <Text textColor={TextColor.white} size={TextSize.sm}>
            {i.name}
          </Text>
        </button>
      ))}
    </div>
  );
};

export default CharacterMenu;
