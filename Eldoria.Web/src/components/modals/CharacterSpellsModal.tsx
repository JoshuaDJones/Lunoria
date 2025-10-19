import React, { useEffect, useState } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";
import AppModal from "./AppModal";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import RightModalContent from "./RightModalContent";
import { CharacterSpellDto } from "../../types/character";
import { SpellDto } from "../../types/spell";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { useLoading } from "../../providers/LoadingProvider";
import { ToastType, useToast } from "../../providers/ToastProvider";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import ListItemRow from "../lists/ListItemRow";
import BoolListItemRow from "../lists/BoolListItemRow";

interface CharacterSpellsModal {
  characterId: number;
  characterSpells: CharacterSpellDto[] | null;
  onSave: () => void;
}

const CharacterSpellsModal = ({
  characterId,
  characterSpells,
  onSave,
}: CharacterSpellsModal) => {
  const modalRouter = useModalRouter();
  const { showLoading, closeLoading } = useLoading();
  const { showToast } = useToast();
  const { get, put } = useApi();

  const [spells, setSpells] = useState<SpellDto[]>([]);
  const [characterSpellIds, setCharacterSpellIds] = useState<number[]>(
    characterSpells?.map((s) => s.spell.id) ?? [],
  );

  const getSpells = async () => {
    try {
      showLoading();
      const spells: SpellDto[] = await get(`${BASE_URL}/Spell`);
      setSpells(spells);
    } catch (err) {
      showToast("Error:", "Unable to get spells.", ToastType.error, 3000);
      console.error(err);
    } finally {
      closeLoading();
    }
  };

  useEffect(() => {
    getSpells();
  }, []);

  const saveCharacterSpells = async () => {
    try {
      showLoading();
      console.log(characterSpellIds);

      await put(`${BASE_URL}/CharacterSpells/${characterId}`, {
        SpellIds: characterSpellIds,
      });

      onSave();
      showToast("Success", "Spells saved.", ToastType.success, 3000);
    } catch (err) {
      showToast("Error", "Spells not saved.", ToastType.error, 3000);
      console.error(err);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  const toggleAttachSpell = (spellId: number) => {
    setCharacterSpellIds(
      (prev) =>
        prev.includes(spellId)
          ? prev.filter((s) => s !== spellId) // remove
          : [...prev, spellId], // add
    );
  };

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <RightModalContent title={"Character Spells"}>
        {spells.map((s) => (
          <div
            key={s.id}
            className="rounded-xl p-3 flex mt-5 border border-gray-600 gap-3"
          >
            <img src={s.photoUrl} className="w-[100px] h-[100px] rounded-xl" />
            <div className="flex flex-col w-[70%]">
              <Text
                className="break-words"
                size={TextSize.xl}
                textColor={TextColor.white}
              >
                {s.name}
              </Text>
              <Text
                className="break-words text-gray-300"
                size={TextSize.lg}
                textColor={TextColor.custom}
              >
                {s.description}
              </Text>
              <ListItemRow
                className="mt-2"
                title={"Cost"}
                value={s.mpCost?.toString() ?? "N/A"}
              />
              <ListItemRow title={"Range"} value={s.range.toString()} />
              <BoolListItemRow title={"Is Radius"} isSelected={s.isRadius} />
              <ListItemRow
                title={"Damage Effect"}
                value={s.damageEffect?.toString() ?? "N/A"}
              />
              <ListItemRow
                title={"Health Effect"}
                value={s.healthEffect?.toString() ?? "N/A"}
              />
              <ListItemRow
                title={"Magic Effect"}
                value={s.magicEffect?.toString() ?? "N/A"}
              />
            </div>
            <div className="flex items-end">
              {characterSpellIds?.find((i) => i === s.id) ? (
                <AppButton
                  title={"Detach"}
                  variant={AppButtonVariant.warning}
                  size={AppButtonSize.sm}
                  onClick={() => toggleAttachSpell(s.id)}
                />
              ) : (
                <AppButton
                  title={"Attach"}
                  variant={AppButtonVariant.go}
                  size={AppButtonSize.sm}
                  onClick={() => toggleAttachSpell(s.id)}
                />
              )}
            </div>
          </div>
        ))}
        <AppButton
          title={"Save"}
          variant={AppButtonVariant.primary}
          size={AppButtonSize.lg}
          onClick={async () => await saveCharacterSpells()}
          className="self-center mt-5"
        />
      </RightModalContent>
    </AppModal>
  );
};

export default CharacterSpellsModal;
