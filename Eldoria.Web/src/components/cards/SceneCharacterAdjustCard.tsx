import Text, { TextColor, TextSize } from "../typography/Text";
import { SceneCharacterDto } from "../../types/scene";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { useLoading } from "../../providers/LoadingProvider";
import { ToastType, useToast } from "../../providers/ToastProvider";
import CharacterHpMpAdjust from "../CharacterHpMpAdjust";

interface SceneCharacterAdjustCardProps {
  sceneCharacter: SceneCharacterDto;
  onRefreshRequest: () => void;
}

const SceneCharacterAdjustCard = ({
  sceneCharacter,
  onRefreshRequest,
}: SceneCharacterAdjustCardProps) => {
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();
  const { patch } = useApi();
  3;

  const character = sceneCharacter.character;

  const handleHpMpSave = async (hp: number, mp: number) => {
    try {
      showLoading();

      await patch(`${BASE_URL}/SceneCharacter/${sceneCharacter.id}`, {
        Hp: hp,
        Mp: mp,
      });

      onRefreshRequest();

      showToast(
        "Success",
        "Scene character changes saved.",
        ToastType.success,
        3000,
      );
    } catch (err) {
      showToast(
        "Error",
        "Unable to save scene character changes.",
        ToastType.error,
        3000,
      );
      console.error(err);
    } finally {
      closeLoading();
    }
  };

  return (
    <div className="flex flex-col p-3 rounded-2xl gap-2 border-white border-2 items-center justify-center">
      <div className="flex flex-1 gap-4 w-full items-center">
        <img src={character.photoUrl} className="h-[90px] rounded-xl" />
        <Text
          className="text-center flex text-wrap break-all"
          size={TextSize.xl}
          textColor={TextColor.white}
        >
          {character.name}
        </Text>
      </div>
      <CharacterHpMpAdjust
        currentHp={sceneCharacter.currentHp}
        maxHp={character.maxHp}
        currentMp={sceneCharacter.currentMp}
        maxMp={character.maxMp}
        onSaveClick={async (hp, mp) => await handleHpMpSave(hp, mp)}
      />
    </div>
  );
};

export default SceneCharacterAdjustCard;
