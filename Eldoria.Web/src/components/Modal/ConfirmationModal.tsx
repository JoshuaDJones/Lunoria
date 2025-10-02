import AppModal from "./AppModal";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import Text, { TextColor, TextSize } from "../typography/Text";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";

interface ConfirmationModalProps {
  title?: string;
  description: string;
  onConfirm: () => void;
}

const ConfirmationModal = ({
  title,
  description,
  onConfirm,
}: ConfirmationModalProps) => {
  const modalRouter = useModalRouter();

  return (
    <AppModal centerContent onBackgroundClose={modalRouter.pop}>
      <div className={`flex flex-col bg-stone-800 py-10 rounded-3xl w-[30%]`}>
        <Title
          className="text-center"
          color={TitleColor.white}
          size={TitleSize.medium}
        >
          {title}
        </Title>
        <Text
          textColor={TextColor.white}
          size={TextSize.xl}
          className="text-center mt-8"
        >
          {description}
        </Text>
        <div className="flex justify-center gap-6 mt-8">
          <AppButton
            title={"No"}
            variant={AppButtonVariant.warning}
            size={AppButtonSize.lg}
            onClick={modalRouter.pop}
          />
          <AppButton
            title={"Yes"}
            variant={AppButtonVariant.primary}
            size={AppButtonSize.lg}
            onClick={() => {
              modalRouter.pop();
              onConfirm();
            }}
          />
        </div>
      </div>
    </AppModal>
  );
};

export default ConfirmationModal;
