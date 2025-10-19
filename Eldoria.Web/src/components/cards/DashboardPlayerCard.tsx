import { useState } from "react";
import { JourneyCharacterDto } from "../../types/journey";
import Text, { TextColor, TextSize } from "../typography/Text";
import DashboardSpellCard from "./DashboardSpellCard";
import CharacterMenu from "../CharacterMenu";
import DashboardItemCard from "./DashboardItemCard";
import NotificationsList, {
  CardNotification,
  NotificationType,
} from "../lists/NotificationsList";
import { v4 as uuidv4 } from "uuid";
import { BASE_URL, useApi } from "../../hooks/useApi";

interface DashboardPlayerCardProps {
  player: JourneyCharacterDto;
  onRefreshRequest: () => void;
}

const DashboardPlayerCard = ({
  player,
  onRefreshRequest,
}: DashboardPlayerCardProps) => {
  const [notifications, setNotifications] = useState<CardNotification[]>([]);
  const { patch } = useApi();
  const character = player.character;

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
        onDropAdded={async (text, type) => {
          if (type === NotificationType.hp) {
            await patch(`${BASE_URL}/JourneyCharacter/${player.id}`, {
              Hp: Math.min(player.character.maxHp, player.currentHp + 4),
              Mp: player.currentMp,
            });
          } else {
            await patch(`${BASE_URL}/JourneyCharacter/${player.id}`, {
              Hp: player.currentHp,
              Mp: Math.min(player.character.maxMp, player.currentMp + 4),
            });
          }

          onRefreshRequest();

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
        journeyCharacterId={player.id}
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
            Hp: {player.currentHp} / {character.maxHp}
          </Text>

          <Text
            size={TextSize.lg}
            className="font-bold text-green-400"
            textColor={TextColor.custom}
          >
            Mp: {player.currentMp} / {character.maxMp}
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
          {character.meleeAttackDamage && (
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

        {!!player.journeyCharacterItems?.filter((i) => !i.isUsed).length && (
          <div className="p-2 bg-gray-800/50 flex flex-col rounded-2xl">
            <Text
              className="self-center mb-1 text-gray-300"
              size={TextSize.xs}
              textColor={TextColor.custom}
            >
              Items
            </Text>

            <div className="flex flex-col gap-5">
              {player.journeyCharacterItems
                .filter((i) => !i.isUsed)
                .map((i) => (
                  <DashboardItemCard
                    journeyCharacterItem={i}
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

export default DashboardPlayerCard;
