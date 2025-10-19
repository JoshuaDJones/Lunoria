import React, { useEffect, useState } from "react";
import LeftModalContent from "./LeftModalContent";
import Modal from "./Modal";
import AppModal from "./AppModal";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { CharacterDto } from "../../types/character";
import JourneyCharacterList from "../lists/JourneyCharacterList";
import { setIn } from "formik";
import { ToastType, useToast } from "../../providers/ToastProvider";

interface JourneyPlayersModalProps {
  journeyId: number;
  selectedCharacterIds: number[];
  onRefreshRequest: () => void;
}

const JourneyPlayersModal = ({
  journeyId,
  selectedCharacterIds,
  onRefreshRequest,
}: JourneyPlayersModalProps) => {
  const modalRouter = useModalRouter();
  const { get, put } = useApi();
  const { showToast } = useToast();

  const [characters, setCharacters] = useState<CharacterDto[]>([]);
  const [internalSelectedCharacterIds, setInternalSelectedCharacterIds] =
    useState<number[]>(selectedCharacterIds);

  const getCharacters = async () => {
    try {
      const characters: CharacterDto[] = await get(`${BASE_URL}/Character`, {
        typeFilter: "Player",
      });
      setCharacters(characters);
    } catch (err) {
      console.error(err);
      showToast("Error", "Unable to get characters.", ToastType.error, 3000);
    }
  };

  useEffect(() => {
    getCharacters();
  }, []);

  const savePlayers = async () => {
    console.log(internalSelectedCharacterIds);

    try {
      await put(`${BASE_URL}/JourneyCharacter/${journeyId}`, {
        CharacterIds: internalSelectedCharacterIds,
      });

      showToast("Success", "Characters Added.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      console.error(err);
      showToast("Error", "Unable to add characters.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
    }
  };

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <LeftModalContent className="w-[20%] scrollbar-hide" title={"Players"}>
        <JourneyCharacterList
          selectedCharacterIds={internalSelectedCharacterIds}
          characters={characters}
          onSelectChange={(selectedCharacterIds) =>
            setInternalSelectedCharacterIds(selectedCharacterIds)
          }
        />
        <AppButton
          title={"Save"}
          variant={AppButtonVariant.ghost}
          size={AppButtonSize.md}
          onClick={async () => await savePlayers()}
        />
      </LeftModalContent>
    </AppModal>
  );
};

export default JourneyPlayersModal;
