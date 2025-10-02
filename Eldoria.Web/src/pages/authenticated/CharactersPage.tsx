import { useEffect, useState } from "react";
import AppPage from "../../components/Layout/AppPage";
import PageContent from "../../components/Layout/PageContent";
import SectionTitleWithAdd from "../../components/Layout/SectionTitleWithAdd";
import { CharacterDto } from "../../types/character";
import { BASE_URL, useApi } from "../../hooks/useApi";
import CharacterList from "../../components/lists/CharacterList";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import AddEditCharacterModal from "../../components/Modal/AddEditCharacterModal";
import { useLoading } from "../../providers/LoadingProvider";

const CharactersPage = () => {
  const { get } = useApi();
  const modalRouter = useModalRouter();

  const [characters, setCharacters] = useState<CharacterDto[]>([]);

  const getCharacters = async () => {
    try {
      const characters: CharacterDto[] = await get(`${BASE_URL}/Character`);
      setCharacters(characters);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    getCharacters();
  }, []);

  const openAddModal = () => {
    modalRouter.push(
      <AddEditCharacterModal onSave={async () => await getCharacters()} />,
    );
  };

  return (
    <AppPage hasNav backgroundImage="/Stone_Background.png">
      <PageContent noCentering>
        <SectionTitleWithAdd
          title="Characters"
          onAddClick={openAddModal}
          className="mt-20 mb-5"
        />
        <CharacterList
          characters={characters}
          onRefreshRequest={async () => await getCharacters()}
        />
      </PageContent>
    </AppPage>
  );
};

export default CharactersPage;
