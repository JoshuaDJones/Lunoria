import { IntroPageType } from "../../types/journey";
import Text, { TextColor } from "../typography/Text";

interface IntroPageStyleOptionsProps {
  introPageType?: IntroPageType | undefined;
  onSelect: (type: IntroPageType) => void;
}

const IntroPageStyleOptions = ({
  introPageType,
  onSelect,
}: IntroPageStyleOptionsProps) => {
  const renderOption = (type: IntroPageType, className?: string) => {
    const renderTitle = () => {
      switch (type) {
        case IntroPageType.ImageCenter_OverlayCenterText:
          return "Image Center, Overlay Center Text";
        case IntroPageType.ImageLeft_ContentRight:
          return "Image Left, Content Right";
        case IntroPageType.ImageRight_ContentLeft:
          return "Image Right, Content Left";
        case IntroPageType.ImageTop_ContentBottom:
          return "Image Top, Content Bottom";
        case IntroPageType.CharacterShowcase:
          return "Character Showcase";
      }
    };

    const renderImagePlaceholder = () => {
      return (
        <div className="flex flex-1 bg-gradient-to-br from-gray-700 to-gray-900" />
      );
    };

    const renderPreview = () => {
      switch (type) {
        case IntroPageType.ImageCenter_OverlayCenterText:
          return (
            <div className="flex flex-1 relative">
              {renderImagePlaceholder()}
              <Text
                textColor={TextColor.white}
                className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"
              >
                Overlay Text
              </Text>
            </div>
          );
        case IntroPageType.ImageLeft_ContentRight:
          return (
            <div className="flex flex-1">
              {renderImagePlaceholder()}
              <div className="flex flex-1 items-center justify-center">
                <Text textColor={TextColor.white}>Content Right</Text>
              </div>
            </div>
          );
        case IntroPageType.ImageRight_ContentLeft:
          return (
            <div className="flex flex-1">
              <div className="flex flex-1 items-center justify-center">
                <Text textColor={TextColor.white}>Content Left</Text>
              </div>
              {renderImagePlaceholder()}
            </div>
          );
        case IntroPageType.ImageTop_ContentBottom:
          return (
            <div className="flex flex-1 flex-col">
              {renderImagePlaceholder()}
              <div className="flex flex-1 items-center justify-center">
                <Text textColor={TextColor.white}>Content Bottom</Text>
              </div>
            </div>
          );
        case IntroPageType.CharacterShowcase:
          return (
            <div className="flex flex-1 gap-2">
              <div className="flex flex-1 flex-col">
                {renderImagePlaceholder()}
                <div className="flex flex-1 items-center justify-center">
                  <Text textColor={TextColor.white}>Player Attributes</Text>
                </div>
              </div>

              <div className="flex flex-1 items-center justify-center">
                <Text textColor={TextColor.white}>Content Right</Text>
              </div>
            </div>
          );
      }
    };

    return (
      <div
        onClick={() => onSelect(type)}
        className={`w-full h-[250px] flex flex-col border-2 ${introPageType === type ? "border-white" : "border-gray-300"} p-1 rounded-lg cursor-pointer hover:border-blue-400 ${className}`}
      >
        <Text className="self-center" textColor={TextColor.white}>
          {renderTitle()}
        </Text>
        <div className="flex flex-1">{renderPreview()}</div>
      </div>
    );
  };

  return (
    <div className="flex flex-col gap-4">
      <div className="flex gap-4">
        {renderOption(IntroPageType.ImageCenter_OverlayCenterText)}
        {renderOption(IntroPageType.ImageTop_ContentBottom)}
      </div>
      <div className="flex gap-4">
        {renderOption(IntroPageType.ImageLeft_ContentRight)}
        {renderOption(IntroPageType.ImageRight_ContentLeft)}
      </div>
      <div className="flex gap-4 justify-center items-center">
        {renderOption(IntroPageType.CharacterShowcase, "w-[50%]")}
      </div>
    </div>
  );
};

export default IntroPageStyleOptions;
