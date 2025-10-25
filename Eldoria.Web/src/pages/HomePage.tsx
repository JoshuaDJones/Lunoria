import AppPage from "../components/layout/AppPage";
import Title, { TitleColor, TitleSize } from "../components/typography/Title";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../components/buttons/AppButton";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";

const HomePage = () => {
  const navigate = useNavigate();

  useEffect(() => {}, []);

  return (
    <AppPage backgroundImage="/Landing_Page.png">
      <div className="mx-36 mt-20 flex flex-1 flex-col gap-2 items-center justify-center">
        <Title
          size={TitleSize.xlarge}
          color={TitleColor.stone800}
          className="self-center"
        >
          Lunoria
        </Title>
        <AppButton
          className="self-center mt-10"
          title={"Enter"}
          variant={AppButtonVariant.primary}
          size={AppButtonSize.xl}
          onClick={() => navigate("/Login")}
        />
      </div>
    </AppPage>
  );
};

export default HomePage;
