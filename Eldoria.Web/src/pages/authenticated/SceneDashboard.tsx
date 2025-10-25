import { useEffect, useState } from "react";
import { useParams, useSearchParams } from "react-router-dom";
import PageContent from "../../components/layout/PageContent";
import AppPage from "../../components/layout/AppPage";
import { SceneDashboardDto } from "../../types/scene";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import {
  CharacterSelection,
  CharacterTabs,
} from "../../components/CharacterTabs";
import SceneEditorPane from "../../components/panes/SceneEditorPane";
import DashboardPlayerList from "../../components/lists/DashboardPlayerList";
import DashboardSceneCharacterList from "../../components/lists/DashboardSceneCharacterList";
import DashboardNpcList from "../../components/lists/DashboardNpcList";

const SceneDashboard = () => {
  const { id } = useParams();
  const [searchParams] = useSearchParams();
  const journeyId = searchParams.get("journeyId");
  const { get } = useApi();
  const { showToast } = useToast();

  const [characterTabSelection, setCharacterTabSelection] = useState(
    CharacterSelection.players,
  );
  const [sceneDashboard, setSceneDashboard] = useState<SceneDashboardDto>();
  const scene = sceneDashboard?.scene;
  const players = sceneDashboard?.players;

  const getSceneDashboard = async () => {
    try {
      const sceneDashboard: SceneDashboardDto = await get(
        `${BASE_URL}/scene/${id}/dashboard`,
        {
          journeyId: journeyId!,
        },
      );
      setSceneDashboard(sceneDashboard);
    } catch (err) {
      console.error(err);
      showToast("Error", "Error getting scene.", ToastType.error, 3000);
    }
  };

  useEffect(() => {
    getSceneDashboard();
  }, []);

  const renderList = () => {
    switch (characterTabSelection) {
      case CharacterSelection.players:
        return (
          <DashboardPlayerList
            players={players}
            onRefreshRequest={async () => await getSceneDashboard()}
          />
        );
      case CharacterSelection.enemies:
        return (
          <DashboardSceneCharacterList
            characters={scene?.sceneCharacters.filter(
              (c) => c.character.isEnemy,
            )}
            onRefreshRequest={async () => await getSceneDashboard()}
          />
        );
      case CharacterSelection.npcs:
        return (
          <DashboardNpcList
            characters={scene?.sceneCharacters.filter((c) => c.character.isNPC)}
            onRefreshRequest={async () => await getSceneDashboard()}
          />
        );
      default:
        return (
          <DashboardPlayerList
            onRefreshRequest={async () => await getSceneDashboard()}
          />
        );
    }
  };

  return (
    <AppPage
      backgroundImage={"/Landing_Page.png"}
      pane={
        <SceneEditorPane
          sceneId={Number(id)}
          sceneDialogs={scene?.sceneDialogs ?? []}
          players={players}
          enemies={scene?.sceneCharacters.filter((c) => c.character.isEnemy)}
          npcs={scene?.sceneCharacters.filter((c) => c.character.isNPC)}
          onRefreshRequest={async () => await getSceneDashboard()}
        />
      }
    >
      <PageContent noTopMargin noCentering className="mt-5">
        <CharacterTabs
          selection={characterTabSelection}
          onSelection={(selection) => setCharacterTabSelection(selection)}
        />
        {renderList()}
      </PageContent>
    </AppPage>
  );
};

export default SceneDashboard;
