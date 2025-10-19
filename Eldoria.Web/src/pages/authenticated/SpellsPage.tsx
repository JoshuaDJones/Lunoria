import { useEffect, useState } from "react";
import AppPage from "../../components/layout/AppPage";
import PageContent from "../../components/layout/PageContent";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import { SpellDto } from "../../types/spell";
import AddEditSpellModal from "../../components/modals/AddEditSpellModal";
import SectionTitleWithAdd from "../../components/layout/SectionTitleWithAdd";
import SpellList from "../../components/lists/SpellList";

const SpellsPage = () => {
  const { get } = useApi();
  const modalRouter = useModalRouter();

  const [spells, setSpells] = useState<SpellDto[]>([]);

  const getSpells = async () => {
    try {
      const spells: SpellDto[] = await get(`${BASE_URL}/Spell`);
      setSpells(spells);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    getSpells();
  }, []);

  const openAddModal = () => {
    modalRouter.push(
      <AddEditSpellModal onSave={async () => await getSpells()} />,
    );
  };

  return (
    <AppPage hasNav backgroundImage="/Stone_Background.png">
      <PageContent noCentering>
        <SectionTitleWithAdd
          title="Spells"
          onAddClick={openAddModal}
          className="mt-20 mb-5"
        />
        <SpellList
          spells={spells}
          onRefreshRequest={async () => await getSpells()}
        />
      </PageContent>
    </AppPage>
  );
};

export default SpellsPage;
