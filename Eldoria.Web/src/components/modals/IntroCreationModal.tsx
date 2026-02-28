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
import { useEffect, useMemo, useState } from "react";
import { fromDto, IntroPage, IntroPageDto, toDto } from "../../types/journey";
import CreateEditIntroPage from "../sections/CreateEditIntroPage";

enum IntroCreationState {
  None,
  Create,
  Edit,
}

interface IntroCreationModalProps {
  introPages: IntroPageDto[];
  onRefreshRequest: () => void;
}

const IntroCreationModal = ({
  introPages = [],
  onRefreshRequest,
}: IntroCreationModalProps) => {
  const modalRouter = useModalRouter();
  const originalIntroPages = useMemo(() => {
    return [...introPages]
      .sort((a, b) => a.order - b.order)
      .map((ip) => fromDto(ip));
  }, [introPages]);

  const [workingIntroPages, setWorkingIntroPages] = useState<IntroPage[]>(() =>
    structuredClone(originalIntroPages),
  );
  const [state, setState] = useState(IntroCreationState.None);
  const [selectedIntroPage, setSelectedIntroPage] = useState<IntroPage | null>(
    null,
  );

  useEffect(() => {
    setWorkingIntroPages(structuredClone(originalIntroPages));
  }, [originalIntroPages]);

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

        <IntroCreationTiles
          workingIntroPages={workingIntroPages}
          onAddNew={() => setState(IntroCreationState.Create)}
          onEdit={(introPage) => {
            setSelectedIntroPage(introPage);
            setState(IntroCreationState.Edit);
          }}
        />

        <HorizontalDivider />

        <div className="absolute flex items-center justify-center bottom-5 right-5 border border-white px-3 py-2 rounded-xl opacity-50">
          {renderStatusIndicator()}
        </div>

        <CreateEditIntroPage
          introPage={selectedIntroPage ?? undefined}
          onSave={(introPage) => {
            // if id is undefined, it's a new intro page, otherwise it's an edit
            // put it into the working intro pages state, replacing the old one if it's an edit
            setWorkingIntroPages((prev) => {
              if (introPage.id === undefined) {
                // new intro page, add it to the end of the list with a temporary id
                const newIntroPage = {
                  ...introPage,
                  id: Math.floor(Math.random() * 1000000) * -1, // temporary negative id to avoid conflicts with existing pages
                  order:
                    prev.length > 0
                      ? Math.max(...prev.map((ip) => ip.order)) + 1
                      : 0, // set order to be last
                };
                return [...prev, newIntroPage];
              } else {
                // existing intro page, replace it in the list
                return prev.map((ip) =>
                  ip.id === introPage.id ? introPage : ip,
                );
              }
            });

            setState(IntroCreationState.None);
            setSelectedIntroPage(null);
          }}
          onCancel={() => {
            setState(IntroCreationState.None);
            setSelectedIntroPage(null);
          }}
        />
      </div>
    </AppModal>
  );
};

export default IntroCreationModal;
