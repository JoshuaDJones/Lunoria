import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { CharacterDto } from "../../types/character";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import Text, { TextColor, TextSize } from "../typography/Text";

interface EnemyNPCActivationCardProps {
  sceneId: number;
  character: CharacterDto;
  onRefreshRequest: () => void;
}

const EnemyNPCActivationCard = ({
  sceneId,
  character,
  onRefreshRequest,
}: EnemyNPCActivationCardProps) => {
  const { post } = useApi();
  const { showToast } = useToast();

  const addSceneCharacter = async () => {
    console.log(`${character.id} ${sceneId}`);

    try {
      await post(`${BASE_URL}/SceneCharacter`, {
        SceneId: sceneId,
        CharacterId: character.id,
      });

      onRefreshRequest();
      showToast("Success", "Character Attached.", ToastType.success, 3000);
    } catch (err) {
      showToast("Error", "Character not attached.", ToastType.error, 3000);
      console.error(err);
    }
  };

  return (
    <div className="flex flex-col p-3 rounded-2xl gap-2 border-white border-2 items-center justify-center">
      <div className="flex flex-1 gap-4 w-full items-center">
        <img src={character.photoUrl} className="h-[90px] rounded-xl" />
        <div>
          <Text
            className="text-center flex text-wrap break-all"
            size={TextSize.xl}
            textColor={TextColor.white}
          >
            {character.name}
          </Text>
          <AppButton
            title={"Activate"}
            variant={AppButtonVariant.go}
            size={AppButtonSize.sm}
            onClick={async () => await addSceneCharacter()}
            className="mt-2"
          />
        </div>
      </div>
    </div>
  );
};

export default EnemyNPCActivationCard;
