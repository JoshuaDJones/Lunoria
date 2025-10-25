import { useParams } from "react-router-dom";
import AppPage from "../../components/layout/AppPage";
import PageContent from "../../components/layout/PageContent";
import Title, {
  TitleColor,
  TitleSize,
} from "../../components/typography/Title";
import { useEffect, useState } from "react";
import { SceneDialogDto } from "../../types/scene";
import DialogPageSectionsList from "../../components/lists/DialogPageSectionsList";
import DialogPagesList from "../../components/lists/DialogPagesList";
import DialogsList from "../../components/lists/DialogsList";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";

const SceneDialogsPage = () => {
  const { showToast } = useToast();
  const { get } = useApi();

  const { id } = useParams();
  const sceneId = Number(id);

  const [sceneDialogs, setSceneDialogs] = useState<SceneDialogDto[]>([]);
  const [selectedDialogId, setSelectedDialogId] = useState<number>();
  const [selectedDialogPageId, setSelectedDialogPageId] = useState<number>();
  const [selectedDialogPageSectionId, setSelectedDialogPageSectionId] =
    useState<number>();

  const getSceneDialogs = async () => {
    try {
      const response = await get(`${BASE_URL}/SceneDialog/${sceneId}`);
      setSceneDialogs(response.value);
    } catch (err) {
      console.error(err);
      showToast("Error", "Could not get scene dialogs.", ToastType.error, 3000);
    }
  };

  useEffect(() => {
    getSceneDialogs();
  }, []);

  return (
    <AppPage
      backgroundImage="/Stone_Background.png"
      noBottomPadding
      useScrolling={false}
    >
      <PageContent noHorizontalSpacing noTopMargin noCentering>
        <div className="flex flex-col h-screen">
          <div className="py-6 pl-20 pr-5 bg-stone-800/90 flex justify-between items-center">
            <Title
              className="text-3xl flex-1"
              color={TitleColor.white}
              size={TitleSize.custom}
            >
              Scene Dialogs
            </Title>
          </div>
          <div className="flex flex-1 bg-stone-700/50 p-5 overflow-hidden">
            <DialogsList
              sceneId={sceneId}
              sceneDialogs={sceneDialogs}
              selectedDialogId={selectedDialogId}
              onSceneDialogSelect={(sceneDialogId) => {
                setSelectedDialogId(sceneDialogId);
                setSelectedDialogPageId(undefined);
                setSelectedDialogPageSectionId(undefined);
              }}
              onRefreshRequest={async () => await getSceneDialogs()}
            />
            <DialogPagesList
              selectedDialogPageId={selectedDialogPageId}
              onRefreshRequest={async () => await getSceneDialogs()}
              selectedDialog={sceneDialogs.find(
                (d) => d.id === selectedDialogId,
              )}
              onDialogPageSelect={(dialogPageId) => {
                setSelectedDialogPageId(dialogPageId);
                setSelectedDialogPageSectionId(undefined);
              }}
            />
            <DialogPageSectionsList
              selectedDialogPageSectionId={selectedDialogPageSectionId}
              selectedDialogPage={sceneDialogs
                .find((d) => d.id === selectedDialogId)
                ?.dialogPages.find((p) => p.id === selectedDialogPageId)}
              onDialogPageSectionSelect={(dialogPageSectionId) =>
                setSelectedDialogPageSectionId(dialogPageSectionId)
              }
              onRefreshRequest={async () => await getSceneDialogs()}
            />
          </div>
        </div>
      </PageContent>
    </AppPage>
  );
};

export default SceneDialogsPage;
