import { useEffect, useState } from "react";
import { IntroPage, IntroPageType } from "../../types/journey";
import { CreateEditIntroPageState } from "./CreateEditIntroPage";
import IntroPageStyleOptions from "./IntroPageStyleOptions";
import Editor_ImageCenter_OverlayCenterText from "./Editor_ImageCenter_OverlayCenterText";

interface UpdateIntroPageProps {
  workingIntroPage?: IntroPage;
  state: CreateEditIntroPageState;
  onStateChange?: (state: CreateEditIntroPageState) => void;
  onSave: (introPage: IntroPage) => void;
  onCancel: () => void;
}

const UpdateIntroPage = ({
  workingIntroPage,
  state,
  onStateChange,
  onSave,
  onCancel,
}: UpdateIntroPageProps) => {
  const [type, setType] = useState<IntroPageType | undefined>(
    workingIntroPage ? workingIntroPage.type : undefined,
  );

  useEffect(() => {
    setType(workingIntroPage ? workingIntroPage.type : undefined);
  }, [workingIntroPage]);

  const renderStyleOptions = () => {
    return (
      <IntroPageStyleOptions
        introPageType={type}
        onSelect={(selectedType) => {
          setType(selectedType);
          onStateChange && onStateChange(CreateEditIntroPageState.CreateContentSubmission);
        }}
      />
    );
  };

  const renderEditor = () => {
    switch (type) {
        case IntroPageType.ImageCenter_OverlayCenterText:
          return <Editor_ImageCenter_OverlayCenterText workingIntroPage={workingIntroPage} onSave={function (introPage: IntroPage): void {
            throw new Error("Function not implemented.");
          } } />
        case IntroPageType.ImageLeft_ContentRight:
          return <Editor_ImageCenter_OverlayCenterText workingIntroPage={workingIntroPage} onSave={function (introPage: IntroPage): void {
            throw new Error("Function not implemented.");
          } } />
        case IntroPageType.ImageRight_ContentLeft:
          return <Editor_ImageCenter_OverlayCenterText workingIntroPage={workingIntroPage} onSave={function (introPage: IntroPage): void {
            throw new Error("Function not implemented.");
          } } />
        case IntroPageType.ImageTop_ContentBottom:
          return <Editor_ImageCenter_OverlayCenterText workingIntroPage={workingIntroPage} onSave={function (introPage: IntroPage): void {
            throw new Error("Function not implemented.");
          } } />
        case IntroPageType.CharacterShowcase:
          return <Editor_ImageCenter_OverlayCenterText workingIntroPage={workingIntroPage} onSave={function (introPage: IntroPage): void {
            throw new Error("Function not implemented.");
          } } />
      }
  };

  return (
    <div>
      {state === CreateEditIntroPageState.CreateStyleSelect && renderStyleOptions()}
      {(state === CreateEditIntroPageState.CreateContentSubmission ||
        state === CreateEditIntroPageState.EditContent) &&
        renderEditor()}
    </div>
  );
};

export default UpdateIntroPage;
