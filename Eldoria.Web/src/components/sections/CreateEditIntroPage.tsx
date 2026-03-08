import { useEffect, useState } from "react";
import { IntroPage } from "../../types/journey";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import UpdateIntroPage from "./UpdateIntroPage";

export enum CreateEditIntroPageState {
  CreateStyleSelect,
  CreateContentSubmission,
  EditContent,
}

interface CreateEditIntroPageProps {
  introPage?: IntroPage;
  onSave: (introPage: IntroPage) => void;
  onCancel: () => void;
}

const CreateEditIntroPage = ({
  introPage,
  onSave,
  onCancel,
}: CreateEditIntroPageProps) => {
  const [state, setState] = useState<CreateEditIntroPageState>(
    introPage?.id === undefined
      ? CreateEditIntroPageState.CreateStyleSelect
      : CreateEditIntroPageState.EditContent,
  );
  const [workingIntroPage, setWorkingIntroPage] = useState<
    IntroPage | undefined
  >(introPage ? structuredClone(introPage) : undefined);

  useEffect(() => {
    setState(
      introPage?.id === undefined
        ? CreateEditIntroPageState.CreateStyleSelect
        : CreateEditIntroPageState.EditContent,
    );
    setWorkingIntroPage(introPage ? structuredClone(introPage) : undefined);
  }, [introPage]);

  const renderTitle = () => {
    if (state === CreateEditIntroPageState.CreateStyleSelect) {
      return "Please Select A Style";
    } else if (state === CreateEditIntroPageState.CreateContentSubmission) {
      return "Create Intro Page";
    } else if (state === CreateEditIntroPageState.EditContent) {
      return "Edit Intro Page";
    }
  };

  return (
    <div className="flex flex-col h-full min-h-0 mx-[150px]">
      <Title
        size={TitleSize.small}
        color={TitleColor.white}
        className="self-center mt-3 mb-1"
      >
        {renderTitle()}
      </Title>

      <div className="flex-1 flex flex-col justify-start gap-2 px-[300px] overflow-y-auto scrollbar-hide pt-5">
        <UpdateIntroPage
          state={state}
          workingIntroPage={workingIntroPage}
          onStateChange={(newState) => setState(newState)}
          onSave={onSave}
          onCancel={onCancel}
        />
      </div>
    </div>
  );
};

export default CreateEditIntroPage;

// When Im creating a page these are the states
// 1. Select style (which will be the template for the page)
// 2. Edit content (which will be the form with the inputs for the content and image url if applicable)

// When Im editing a page I want to skip the first step and go directly to the
// form with the inputs for the content and image url if applicable, pre-populated with the existing content
