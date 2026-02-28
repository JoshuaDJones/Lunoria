import { useEffect, useState } from "react";
import AppPage from "../../components/layout/AppPage";
import PageContent from "../../components/layout/PageContent";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../../components/buttons/AppButton";
import { useParams } from "react-router";
import { JourneyDto } from "../../types/journey";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import Title, {
  TitleColor,
  TitleSize,
} from "../../components/typography/Title";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import JourneyPlayersModal from "../../components/modals/JourneyPlayersModal";
import AddEditSceneModal from "../../components/modals/AddEditSceneModal";
import SceneList from "../../components/lists/SceneList";
import IntroCreationModal from "../../components/modals/IntroCreationModal";

const JourneyDetailsPage = () => {
  const { get } = useApi();
  const modalRouter = useModalRouter();
  const { showToast } = useToast();
  const { id } = useParams();
  const journeyId = Number(id);

  const [journey, setJourney] = useState<JourneyDto>();

  const getJourney = async () => {
    try {
      const journey = await get(`${BASE_URL}/Journey/${journeyId}`);
      setJourney(journey);
    } catch (err) {
      showToast(
        "Error:",
        "Unable to get journey details.",
        ToastType.error,
        3000,
      );
      console.error(err);
    }
  };

  useEffect(() => {
    getJourney();
  }, []);

  const openPlayersModal = () => {
    modalRouter.push(
      <JourneyPlayersModal
        journeyId={journeyId}
        selectedCharacterIds={
          journey?.journeyCharacters.map((jc) => jc.characterId) ?? []
        }
        onRefreshRequest={async () => await getJourney()}
      />,
    );
  };

  const openAddSceneModal = () => {
    modalRouter.push(
      <AddEditSceneModal
        journeyId={journeyId}
        onSave={async () => await getJourney()}
      />,
    );
  };

  const openIntroCreateModal = () => {
    modalRouter.push(<IntroCreationModal 
      introPages={journey?.introPages ?? []}
      onRefreshRequest={async () => await getJourney()} />);
  };

  return (
    <AppPage
      backgroundImage="/Stone_Background.png"
      pane={
        <div className="flex absolute bottom-[160px] -left-[135px] gap-2 rotate-90 z-20">
          <AppButton
            onClick={openIntroCreateModal}
            title={"Intro"}
            variant={AppButtonVariant.primary}
            size={AppButtonSize.lg}
            noRounded
            className="rounded-t-2xl"
          />
          <AppButton
            onClick={openPlayersModal}
            title={"Players"}
            variant={AppButtonVariant.go}
            size={AppButtonSize.lg}
            noRounded
            className="rounded-t-2xl"
          />
        </div>
      }
    >
      <PageContent useBackButton noTopMargin noCentering className="mt-10">
        <div className="flex">
          <div className="flex w-[40%] justify-center items-start">
            <img className="w-[80%] rounded-3xl" src={journey?.photoUrl} />
          </div>
          <div className="flex flex-col w-[60%]">
            <div className="flex w-full border-b-4 border-black items-center">
              <Title
                className="flex-1"
                color={TitleColor.default}
                size={TitleSize.medium}
              >
                Scenes
              </Title>
              <AppButton
                title={"Add Scene"}
                variant={AppButtonVariant.go}
                size={AppButtonSize.md}
                onClick={openAddSceneModal}
                rightIcon={
                  <FontAwesomeIcon icon={faPlus} size="lg" className="ml-2" />
                }
              />
            </div>
            <SceneList
              journeyId={journeyId}
              scenes={journey?.scenes}
              onRefreshRequest={async () => getJourney()}
            />
          </div>
        </div>
      </PageContent>
    </AppPage>
  );
};

export default JourneyDetailsPage;
