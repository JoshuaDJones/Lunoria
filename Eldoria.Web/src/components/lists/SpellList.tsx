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
import ListItemRow from "./ListItemRow";
import { SpellDto } from "../../types/spell";
import AddEditSpellModal from "../Modal/AddEditSpellModal";
import BoolListItemRow from "./BoolListItemRow";

interface SpellListProps {
  spells: SpellDto[] | undefined;
  onRefreshRequest: () => void;
}

const SpellList = ({ spells, onRefreshRequest }: SpellListProps) => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {spells?.map((s) => (
        <SpellListItem
          key={s.id}
          spell={s}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

interface SpellListItemProps {
  spell: SpellDto;
  onRefreshRequest: () => void;
}

const SpellListItem = ({ spell, onRefreshRequest }: SpellListItemProps) => {
  const modalRouter = useModalRouter();
  const { del } = useApi();
  const { showToast } = useToast();

  const handleDeletion = async () => {
    try {
      await del(`${BASE_URL}/Spell/${spell.id}`);
      showToast("Success:", "Spell deleted.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      console.error(err);
      showToast("Error:", "Spell not deleted.", ToastType.success, 3000);
    }
  };

  const openEditModal = () => {
    modalRouter.push(
      <AddEditSpellModal spell={spell} onSave={onRefreshRequest} />,
    );
  };

  const openDeleteModal = () => {
    modalRouter.push(
      <ConfirmationModal
        title="Confirmation"
        description="Are you sure you want to delete this spell?"
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
            {spell.name}
          </Title>
          <Text
            size={TextSize.xl}
            textColor={TextColor.custom}
            className="text-gray-400 break-words line-clamp-3 mb-2"
          >
            {spell.description}
          </Text>
          <ListItemRow
            title={"Cost"}
            value={spell.mpCost?.toString() ?? "N/A"}
          />
          <ListItemRow title={"Range"} value={spell.range.toString()} />
          <BoolListItemRow title={"Is Radius"} isSelected={spell.isRadius} />
          <ListItemRow
            title={"Damage Effect"}
            value={spell.damageEffect?.toString() ?? "N/A"}
          />
          <ListItemRow
            title={"Health Effect"}
            value={spell.healthEffect?.toString() ?? "N/A"}
          />
          <ListItemRow
            title={"Magic Effect"}
            value={spell.magicEffect?.toString() ?? "N/A"}
          />
        </div>
        <img src={spell.photoUrl} className="w-[40%] object-cover rounded-md" />
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

export default SpellList;
