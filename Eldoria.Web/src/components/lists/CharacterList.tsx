import Text, { TextColor, TextSize } from "../typography/Text";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { CharacterDto, CharacterSpellDto } from "../../types/character";
import ListItemRow, { RowTextSize } from "./ListItemRow";
import BoolListItemRow from "./BoolListItemRow";
import AddEditCharacterModal from "../modals/AddEditCharacterModal";
import ConfirmationModal from "../modals/ConfirmationModal";
import CharacterSpellsModal from "../modals/CharacterSpellsModal";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowCircleDown,
  faCaretDown,
  faCaretSquareDown,
  faCaretSquareUp,
} from "@fortawesome/free-solid-svg-icons";
import { useState } from "react";
import { SpellDto } from "../../types/spell";

interface CharacterListProps {
  characters: CharacterDto[] | undefined;
  onRefreshRequest: () => void;
}

const CharacterList = ({
  characters,
  onRefreshRequest,
}: CharacterListProps) => {
  return (
    <div className="flex flex-wrap gap-4 items-start">
      {characters?.map((c) => (
        <CharacterListItem
          key={c.id}
          character={c}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

interface CharacterListItemProps {
  character: CharacterDto;
  onRefreshRequest: () => void;
}

const CharacterListItem = ({
  character,
  onRefreshRequest,
}: CharacterListItemProps) => {
  const modalRouter = useModalRouter();
  const { del } = useApi();
  const { showToast } = useToast();

  const handleDeletion = async () => {
    try {
      await del(`${BASE_URL}/Character/${character.id}`);
      showToast("Success:", "Character deleted.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      console.error(err);
      showToast("Error:", "Character not deleted.", ToastType.success, 3000);
    }
  };

  const openDeleteModal = () => {
    modalRouter.push(
      <ConfirmationModal
        title="Confirmation"
        description="Are you sure you want to delete this character?"
        onConfirm={async () => await handleDeletion()}
      />,
    );
  };

  const openEditModal = () => {
    modalRouter.push(
      <AddEditCharacterModal character={character} onSave={onRefreshRequest} />,
    );
  };

  const openSpellsModal = () => {
    console.log(character.characterSpells);

    modalRouter.push(
      <CharacterSpellsModal
        characterId={character.id}
        characterSpells={character.characterSpells}
        onSave={onRefreshRequest}
      />,
    );
  };

  return (
    <div className="rounded-xl p-3 flex flex-col bg-stone-800/50 w-full sm:w-[calc(50%-1rem)] lg:w-[calc(33.333%-1rem)]">
      <div className="flex flex-1 gap-2 items-start">
        <div className="w-[60%] flex-col">
          <Title
            className="flex-1 break-words"
            size={TitleSize.medium}
            color={TitleColor.white}
          >
            {character.name}
          </Title>
          <Text
            size={TextSize.xl}
            textColor={TextColor.custom}
            className="text-gray-400 break-words line-clamp-3 mb-2"
          >
            {character.description}
          </Text>
          <ListItemRow
            title={"Max Hp"}
            value={character.maxHp.toString()}
            titleColor={TextColor.red}
            valueColor={TextColor.red}
          />
          <ListItemRow
            title={"Max Mp"}
            value={character.maxMp.toString()}
            className="mb-2"
            titleColor={TextColor.green}
            valueColor={TextColor.green}
          />
          <ListItemRow
            title={"Melee Attack Damage"}
            value={character.meleeAttackDamage?.toString() ?? "N/A"}
          />
          <ListItemRow
            title={"Bow Attack Damage"}
            value={character.bowAttackDamage?.toString() ?? "N/A"}
          />
          <ListItemRow
            title={"Movement"}
            value={character.movement?.toString()}
          />
          <ListItemRow
            title={"Max Inventory"}
            value={character.maxInventory?.toString()}
          />

          <BoolListItemRow
            title={"Is Player"}
            isSelected={character.isPlayer}
          />
          <BoolListItemRow title={"Is NPC"} isSelected={character.isNPC} />
          <BoolListItemRow title={"Is Enemy"} isSelected={character.isEnemy} />

          <CharacterSpellsSection spells={character.characterSpells} />
        </div>
        <img
          src={character.photoUrl}
          className="w-[40%] object-cover rounded-md"
        />
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
        <AppButton
          title={"Spells"}
          onClick={openSpellsModal}
          variant={AppButtonVariant.custom}
          size={AppButtonSize.sm}
          className="bg-purple-800 text-white"
        />
      </div>
    </div>
  );
};

interface CharacterSpellsSectionProps {
  spells: CharacterSpellDto[] | null;
}

export const CharacterSpellsSection = ({
  spells,
}: CharacterSpellsSectionProps) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div
      className="flex flex-col mt-2 gap-2 items-center active:opacity-60 cursor-pointer"
      onClick={() => setIsOpen(!isOpen)}
    >
      <div className="flex w-full gap-2">
        <FontAwesomeIcon
          icon={isOpen ? faCaretSquareUp : faCaretSquareDown}
          color="white"
          size="xl"
        />
        <Text textColor={TextColor.white} className="font-bold tracking-widest">
          Spells
        </Text>
      </div>

      {isOpen && (
        <div>
          {spells?.map((s) => (
            <div className="flex flex-1 gap-2 items-start rounded-lg mt-2 border p-2">
              <div className="flex-col flex-1">
                <Text
                  size={TextSize.lg}
                  textColor={TextColor.white}
                  className="break-words line-clamp-3 mb-2 font-bold"
                >
                  {s.spell.name}
                </Text>
                <Text
                  size={TextSize.sm}
                  textColor={TextColor.custom}
                  className="text-gray-400 break-words line-clamp-3 mb-2"
                >
                  {s.spell.description}
                </Text>
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Cost"}
                  value={s.spell.mpCost?.toString() ?? "N/A"}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Range"}
                  value={s.spell.range.toString()}
                />
                <BoolListItemRow
                  title={"Is Radius"}
                  isSelected={s.spell.isRadius}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Damage Effect"}
                  value={s.spell.damageEffect?.toString() ?? "N/A"}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Health Effect"}
                  value={s.spell.healthEffect?.toString() ?? "N/A"}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Magic Effect"}
                  value={s.spell.magicEffect?.toString() ?? "N/A"}
                />
              </div>
              <img
                src={s.spell.photoUrl}
                className="w-[80px] object-cover rounded-md"
              />
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default CharacterList;
