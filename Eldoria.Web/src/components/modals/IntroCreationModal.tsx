import { useModalRouter } from "../../providers/ModalRouterProvider";
import AppModal from "./AppModal";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { faDoorOpen } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import Text, { TextColor, TextSize } from "../typography/Text";
import HorizontalDivider from "../layout/HorizontalDivider";
import IntroCreationTiles from "../lists/IntroCreationTiles";
import { useState } from "react";

enum IntroCreationState {
  Create,
  Edit,
}

interface IntroCreationModalProps {}

const IntroCreationModal = (props: IntroCreationModalProps) => {
  const modalRouter = useModalRouter();

  const [state, setState] = useState(IntroCreationState.Create);

  const renderStatusIndicator = () => {
    return state === IntroCreationState.Create ? (
      <Text
        textColor={TextColor.white}
        size={TextSize.xs}
        className="self-center"
      >
        Create Mode
      </Text>
    ) : (
      <Text textColor={TextColor.white} className="self-center">
        Edit Mode
      </Text>
    );
  };

















  return (
    <AppModal centerContent>
      <div className="flex flex-col h-full w-full bg-stone-900/80 backdrop-blur-md px-5 py-2 relative">
        <div className="flex justify-end absolute top-5 right-5">
          <AppButton
            onClick={modalRouter.pop}
            title={"Close"}
            variant={AppButtonVariant.warning}
            size={AppButtonSize.sm}
            rightIcon={<FontAwesomeIcon icon={faDoorOpen} />}
          />
        </div>
        <Title
          size={TitleSize.large}
          color={TitleColor.stone300}
          className="self-center my-2"
        >
          Intro Creation System
        </Title>
        <HorizontalDivider />

        <IntroCreationTiles />

        <HorizontalDivider />

        <div className="absolute flex items-center justify-center bottom-5 right-5 border border-white px-3 py-2 rounded-xl opacity-50">
          {renderStatusIndicator()}
        </div>

        <Title
          size={TitleSize.small}
          color={TitleColor.white}
          className="self-center mt-3 mb-1"
        >
          Please Select A Style
        </Title>
        <div className="flex-1 flex flex-col justify-start gap-2 px-[300px] overflow-y-auto scrollbar-hide pt-5">
          <div></div>
        </div>
      </div>
    </AppModal>
  );
};

export default IntroCreationModal;
