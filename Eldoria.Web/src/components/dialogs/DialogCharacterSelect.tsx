import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import PageSectionCharacterModal from "../modals/PageSectionCharacterModal";
import Text, { TextColor, TextSize } from "../typography/Text";
import { faClose } from "@fortawesome/free-solid-svg-icons";
import clsx from "clsx";

export interface DialogCharacter {
  characterId: number;
  characterPhoto: string;
}

interface DialogCharacterSelectProps {
  selectedCharacter?: DialogCharacter;
  onCharacterSelect: (character?: DialogCharacter) => void;
  className?: string;
}

const DialogCharacterSelect = ({
  selectedCharacter,
  onCharacterSelect,
  className,
}: DialogCharacterSelectProps) => {
  const modalRouter = useModalRouter();

  console.log(selectedCharacter);

  return (
    <div className={clsx("flex items-center gap-4", className)}>
      <div>
        <Text size={TextSize.xl} textColor={TextColor.white}>
          Character
        </Text>
        <AppButton
          title={
            !selectedCharacter?.characterId
              ? "Add Character"
              : "Change Character"
          }
          variant={AppButtonVariant.primary}
          size={AppButtonSize.sm}
          onClick={() =>
            modalRouter.push(
              <PageSectionCharacterModal
                onSelect={(characterId, photoUrl) => {
                  onCharacterSelect({
                    characterId: characterId,
                    characterPhoto: photoUrl,
                  });
                }}
              />,
            )
          }
        />
      </div>
      {selectedCharacter?.characterId && (
        <div className="relative">
          <img
            src={selectedCharacter.characterPhoto}
            className="h-[80px] rounded-lg border-2"
          />

          <button
            className="bg-black h-[30px] w-[30px] rounded-full hover:opacity-65 cursor-pointer flex justify-center items-center absolute -top-4 -right-3"
            onClick={() => onCharacterSelect(undefined)}
          >
            <FontAwesomeIcon
              icon={faClose}
              size="lg"
              className="text-red-400"
            />
          </button>
        </div>
      )}
    </div>
  );
};

export default DialogCharacterSelect;
