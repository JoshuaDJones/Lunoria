import { JourneyDto } from "../../types/journey";
import Text, { TextColor, TextSize } from "../typography/Text";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import ConfirmationModal from "../Modal/ConfirmationModal";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { ItemDto } from "../../types/item";
import AddEditItemModal from "../Modal/AddEditItemModal";
import ListItemRow from "./ListItemRow";

interface ItemListProps {
  items: ItemDto[] | undefined;
  onRefreshRequest: () => void;
}

const ItemList = ({ items, onRefreshRequest }: ItemListProps) => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {items?.map((i) => (
        <ItemListItem key={i.id} item={i} onRefreshRequest={onRefreshRequest} />
      ))}
    </div>
  );
};

interface ItemListItemProps {
  item: ItemDto;
  onRefreshRequest: () => void;
}

const ItemListItem = ({ item, onRefreshRequest }: ItemListItemProps) => {
  const modalRouter = useModalRouter();
  const { del } = useApi();
  const { showToast } = useToast();

  const handleDeletion = async () => {
    try {
      await del(`${BASE_URL}/Item/${item.id}`);
      showToast("Success:", "Item deleted.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      console.error(err);
      showToast("Error:", "Item not deleted.", ToastType.success, 3000);
    }
  };

  const openEditModal = () => {
    modalRouter.push(
      <AddEditItemModal item={item} onSave={onRefreshRequest} />,
    );
  };

  const openDeleteModal = () => {
    modalRouter.push(
      <ConfirmationModal
        title="Confirmation"
        description="Are you sure you want to delete this item?"
        onConfirm={async () => await handleDeletion()}
      />,
    );
  };

  return (
    <div
      className="rounded-xl p-3 flex flex-col bg-stone-800/50"
      onClick={() => {
        //TODO: Navigate
      }}
    >
      <div className="flex flex-1 gap-2 items-start">
        <div className="w-[60%] flex-col">
          <Title
            className="flex-1 break-words"
            size={TitleSize.medium}
            color={TitleColor.white}
          >
            {item.name}
          </Title>
          <Text
            size={TextSize.xl}
            textColor={TextColor.custom}
            className="text-gray-400 break-words line-clamp-3 mb-2"
          >
            {item.description}
          </Text>
          <ListItemRow
            title={"Hp Effect"}
            value={item.hpEffect.toString()}
            titleColor={TextColor.red}
            valueColor={TextColor.red}
          />
          <ListItemRow
            title={"Mp Effect"}
            value={item.mpEffect.toString()}
            className="mb-2"
            titleColor={TextColor.green}
            valueColor={TextColor.green}
          />
        </div>
        <img src={item.photoUrl} className="w-[40%] object-cover rounded-md" />
      </div>
      <div className="justify-end flex gap-2 mt-2">
        <AppButton
          title={"Edit"}
          onClick={openEditModal}
          variant={AppButtonVariant.primary}
          size={AppButtonSize.sm}
        />
        <AppButton
          title={"Delete"}
          onClick={openDeleteModal}
          variant={AppButtonVariant.warning}
          size={AppButtonSize.sm}
        />
      </div>
    </div>
  );
};

export default ItemList;
