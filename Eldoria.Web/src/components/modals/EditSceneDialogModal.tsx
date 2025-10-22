import { useState } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";
import AppModal from "./AppModal";
import RightModalContent from "./RightModalContent";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import { SceneDialogDto } from "../../types/scene";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import AppInput from "../inputs/AppInput";

enum ModalMode {
  view,
  add,
}

interface EditSceneDialogModalProps {
  dialogs?: SceneDialogDto[];
}

const EditSceneDialogModal = ({ dialogs }: EditSceneDialogModalProps) => {
  const modalRouter = useModalRouter();
  const [mode, setMode] = useState<ModalMode>(ModalMode.view);

  let content;
  let title = "";

  if (mode === ModalMode.view) {
    content = (
      <ViewDialogsSection
        onCreateDialogClick={() => setMode(ModalMode.add)}
        dialogs={dialogs}
      />
    );
    title = "Dialogs";
  } else {
    content = (
      <CreateDialogSection
        onCreateDialogClick={() => setMode(ModalMode.view)}
      />
    );
    title = "Create Dialog";
  }

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <RightModalContent title={title}>{content}</RightModalContent>
    </AppModal>
  );
};

export default EditSceneDialogModal;

interface ViewDialogsSectionProps {
  dialogs?: SceneDialogDto[];
  onCreateDialogClick: () => void;
}

const ViewDialogsSection = ({
  dialogs = [],
  onCreateDialogClick,
}: ViewDialogsSectionProps) => {
  return (
    <div className="flex flex-1 flex-col">
      <AppButton
        title={"Create"}
        variant={AppButtonVariant.go}
        size={AppButtonSize.md}
        onClick={() => onCreateDialogClick()}
        rightIcon={<FontAwesomeIcon icon={faPlus} className="ml-1" />}
        className="self-end"
      />
      <div className="flex flex-1 flex-col mt-3 gap-4">
        {dialogs.map((d) => (
          <div className="flex justify-between p-4 border rounded-xl">
            <Text
              className="self-start"
              size={TextSize.xl}
              textColor={TextColor.white}
            >
              {d.title}
            </Text>
            <Text
              className="self-end"
              textColor={TextColor.white}
              size={TextSize.xl}
            >
              Pages: {d.dialogPages.length}
            </Text>
          </div>
        ))}
      </div>
    </div>
  );
};

interface CreateDialogSectionProps {
  onCreateDialogClick: () => void;
}

const CreateDialogSection = ({
  onCreateDialogClick,
}: CreateDialogSectionProps) => {
  return (
    <div className="flex flex-1 flex-col">
      <div className="flex-1">
        <AppInput />
      </div>
      <AppButton
        title={"Save"}
        onClick={onCreateDialogClick}
        variant={AppButtonVariant.primary}
        size={AppButtonSize.md}
        className="self-center"
      />
    </div>
  );
};
