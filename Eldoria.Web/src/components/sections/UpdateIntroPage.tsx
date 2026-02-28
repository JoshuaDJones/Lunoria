import { useEffect, useState } from "react";
import { IntroPage, IntroPageType } from "../../types/journey";
import { CreateEditIntroPageState } from "./CreateEditIntroPage";
import IntroPageStyleOptions from "./IntroPageStyleOptions";

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
          onStateChange && onStateChange(CreateEditIntroPageState.Create);
        }}
      />
    );
  };

  const renderEditor = () => {
    return <></>;
  };

  return (
    <div>
      {state === CreateEditIntroPageState.SelectStyle && renderStyleOptions()}
      {(state === CreateEditIntroPageState.Create ||
        state === CreateEditIntroPageState.Edit) &&
        renderEditor()}
    </div>
  );
};

export default UpdateIntroPage;
