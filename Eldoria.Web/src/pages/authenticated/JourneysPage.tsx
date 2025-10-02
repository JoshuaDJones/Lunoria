import AppPage from "../../components/Layout/AppPage";
import PageContent from "../../components/Layout/PageContent";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import SectionTitleWithAdd from "../../components/Layout/SectionTitleWithAdd";
import AddEditJourneyModal from "../../components/Modal/AddEditJourneyModal";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { JourneyDto } from "../../types/journey";
import { useEffect, useState } from "react";
import JourneyList from "../../components/lists/JourneyList";

const JourneysPage = () => {
  const modalRouter = useModalRouter();
  const api = useApi();
  const [journeys, setJourneys] = useState<JourneyDto[]>([]);

  const getJourneys = async () => {
    try {
      const journeys: JourneyDto[] = await api.get(`${BASE_URL}/Journey`);
      setJourneys(journeys);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    getJourneys();
  }, []);

  const openAddModal = () => {
    modalRouter.push(
      <AddEditJourneyModal onSave={async () => await getJourneys()} />,
    );
  };

  return (
    <AppPage hasNav backgroundImage="/Stone_Background.png">
      <PageContent noCentering>
        <SectionTitleWithAdd
          title="Journeys"
          onAddClick={openAddModal}
          className="mt-20 mb-5"
        />
        <JourneyList
          journeys={journeys}
          onRefreshRequest={async () => await getJourneys()}
        />
      </PageContent>
    </AppPage>
  );
};

export default JourneysPage;
