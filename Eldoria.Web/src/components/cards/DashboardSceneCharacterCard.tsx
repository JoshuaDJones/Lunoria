import { useEffect, useState } from "react";
import { SceneCharacterDto } from "../../types/scene";
import NotificationsList, {
  CardNotification,
} from "../lists/NotificationsList";
import CharacterMenu from "../CharacterMenu";
import { v4 as uuidv4 } from "uuid";
import Text, { TextColor, TextSize } from "../typography/Text";
import DashboardSpellCard from "./DashboardSpellCard";
import DashboardItemCard from "./DashboardItemCard";

interface DashboardSceneCharacterCardProps {
  sceneCharacter: SceneCharacterDto;
  onRefreshRequest: () => void;
}

const DashboardSceneCharacterCard = ({
  sceneCharacter,
  onRefreshRequest,
}: DashboardSceneCharacterCardProps) => {
  const [notifications, setNotifications] = useState<CardNotification[]>([]);

  const character = sceneCharacter.character;

  useEffect(() => {
    return () => {
      setNotifications([]);
    };
  }, []);

  return (
    <div className="rounded-3xl p-3 flex flex-col bg-stone-800/50 w-[calc(20%-1rem)] border-white border-2 relative">
      <NotificationsList
        notifications={notifications}
        onHideNotification={(id) =>
          setNotifications((prev) =>
            prev.map((notification) =>
              notification.id === id
                ? { ...notification, isVisible: false }
                : notification,
            ),
          )
        }
      />
      <CharacterMenu
        hasAlternate={false}
        onDropAdded={(text, type) => {
          setNotifications((prev) => [
            ...prev,
            {
              id: uuidv4(),
              text: text,
              type: type,
              isVisible: true,
            },
          ]);
        }}
        sceneCharacterId={sceneCharacter.id}
        onRefreshRequest={onRefreshRequest}
      />

      <div className="flex flex-1 items-center gap-3">
        <img src={character.photoUrl} className="w-[75px] rounded-xl" />
        <div className="flex flex-col">
          <Text
            size={TextSize.xl}
            textColor={TextColor.white}
            className="text-wrap break-all font-bold"
          >
            {character.name}
          </Text>
          <Text
            size={TextSize.base}
            textColor={TextColor.custom}
            className="text-gray-300 text-wrap break-all"
          >
            {character.description}
          </Text>
          <Text
            size={TextSize.base}
            className="font-bold text-red-400"
            textColor={TextColor.custom}
          >
            Hp: {sceneCharacter.currentHp} / {character.maxHp}
          </Text>

          <Text
            size={TextSize.lg}
            className="font-bold text-green-400"
            textColor={TextColor.custom}
          >
            Mp: {sceneCharacter.currentMp} / {character.maxMp}
          </Text>
        </div>
      </div>
      <div className="flex flex-col gap-1">
        <div className="p-2 bg-gray-800/50 flex flex-col rounded-2xl mt-2">
          <Text
            className="self-center mb-1 text-gray-300"
            size={TextSize.xs}
            textColor={TextColor.custom}
          >
            Attributes
          </Text>
          <Text
            className="font-bold tracking-wider"
            textColor={TextColor.white}
          >
            Movement: {character.movement}
          </Text>
          {!!character.meleeAttackDamage && (
            <Text
              className="font-bold tracking-wider"
              textColor={TextColor.white}
            >
              Melee: {character.meleeAttackDamage ?? "N/A"}
            </Text>
          )}
          {character.bowAttackDamage && (
            <Text
              className="font-bold tracking-wider"
              textColor={TextColor.white}
            >
              Bow: {character.bowAttackDamage}
            </Text>
          )}
        </div>

        {!!character.characterSpells?.length && (
          <div className="p-2 bg-gray-800/50 flex flex-col rounded-2xl">
            <Text
              className="self-center mb-1 text-gray-300"
              size={TextSize.xs}
              textColor={TextColor.custom}
            >
              Spells
            </Text>

            <div className="flex flex-col gap-2">
              {character.characterSpells
                ?.sort((a, b) => a.spell.range - b.spell.range)
                .map((s) => <DashboardSpellCard spell={s.spell} />)}
            </div>
          </div>
        )}

        {!!sceneCharacter.sceneCharacterItems?.filter((i) => !i.isUsed)
          .length && (
          <div className="p-2 bg-gray-800/50 flex flex-col rounded-2xl">
            <Text
              className="self-center mb-1 text-gray-300"
              size={TextSize.xs}
              textColor={TextColor.custom}
            >
              Items
            </Text>

            <div className="flex flex-col gap-5">
              {sceneCharacter.sceneCharacterItems
                .filter((i) => !i.isUsed)
                .map((i) => (
                  <DashboardItemCard
                    sceneCharacterItem={i}
                    onRefreshRequest={onRefreshRequest}
                  />
                ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default DashboardSceneCharacterCard;
