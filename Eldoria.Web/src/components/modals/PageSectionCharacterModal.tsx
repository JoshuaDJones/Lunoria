import React, { useEffect, useState } from "react";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import AppModal from "./AppModal";
import BottomModalContent from "./BottomModalContent";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { CharacterDto } from "../../types/character";
import Text, { TextColor, TextSize } from "../typography/Text";
import RightModalContent from "./RightModalContent";
import LeftModalContent from "./LeftModalContent";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";

interface PageSectionCharacterModalProps {
  onSelect: (characterId: number, characterPhoto: string) => void;
}

const PageSectionCharacterModal = ({
  onSelect,
}: PageSectionCharacterModalProps) => {
  const { showToast } = useToast();
  const { get } = useApi();
  const modalRouter = useModalRouter();

  const [characters, setCharacters] = useState<CharacterDto[]>([]);

  useEffect(() => {
    getCharacters();
  }, []);

  const getCharacters = async () => {
    try {
      const characters = await get(`${BASE_URL}/Character`);
      setCharacters(characters);
    } catch (err) {
      console.log(err);
      showToast("Error", "Unable to get characters.", ToastType.error, 3000);
    }
  };

  return (
    <AppModal onBackgroundClose={() => modalRouter.pop()}>
      <LeftModalContent
        title={"Select Character"}
        className="scrollbar-hide w-[20%]"
        useCustomWidth
      >
        <div className="flex-1 flex flex-col gap-4 items-center mt-5">
          {characters.map((c) => (
            <div className="flex flex-col items-center border rounded-xl overflow-hidden p-5 bg-stone-800/80 w-[200px]">
              <img src={c.photoUrl} className="w-[120px] rounded-lg" />
              <Text
                className="break-all"
                textColor={TextColor.white}
                size={TextSize.xl}
              >
                {c.name}
              </Text>
              <AppButton
                className="mt-2"
                title={"Select"}
                variant={AppButtonVariant.primary}
                size={AppButtonSize.sm}
                onClick={() => {
                  onSelect(c.id, c.photoUrl);
                  modalRouter.pop();
                }}
              />
            </div>
          ))}
        </div>
      </LeftModalContent>
    </AppModal>
  );
};

export default PageSectionCharacterModal;
