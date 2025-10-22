import { useParams } from "react-router-dom";
import AppPage from "../../components/layout/AppPage";
import PageContent from "../../components/layout/PageContent";
import Title, {
  TitleColor,
  TitleSize,
} from "../../components/typography/Title";
import { useEffect, useState } from "react";
import { SceneDialogDto } from "../../types/scene";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../../components/buttons/AppButton";
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
  const [selectedDialogPageSectionId, setSelectedDialogPageSectionId] = useState<number>();

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
    <AppPage backgroundImage="/Stone_Background.png" noBottomPadding>
      <PageContent noHorizontalSpacing noTopMargin noCentering>
        <div className="py-6 pl-20 pr-5 bg-stone-800/90 flex justify-between items-center">
          <Title color={TitleColor.white} size={TitleSize.medium}>
            Scene Dialogs
          </Title>
          <AppButton
            title={"Save Dialogs"}
            variant={AppButtonVariant.go}
            size={AppButtonSize.lg}
          />
        </div>
        <div className="flex flex-1 bg-stone-700/50 p-5">
          <HorizontalDialogDivider />
          <DialogsList
            sceneId={sceneId}
            sceneDialogs={sceneDialogs}
            selectedDialogId={selectedDialogId}
            onSceneDialogSelect={(sceneDialogId) => {
              setSelectedDialogId(sceneDialogId);
              setSelectedDialogPageId(undefined);
              setSelectedDialogPageSectionId(undefined);
            }}
            onDialogCreated={async () => await getSceneDialogs()}
          />
          <HorizontalDialogDivider />
          <DialogPagesList
            selectedDialogPageId={selectedDialogPageId}
            onRefreshRequest={async () => await getSceneDialogs()}
            selectedDialog={sceneDialogs.find((d) => d.id === selectedDialogId)} 
            onDialogPageSelect={(dialogPageId) => {
              setSelectedDialogPageId(dialogPageId)
              setSelectedDialogPageSectionId(undefined);
            }}/>
          <HorizontalDialogDivider />
          <DialogPageSectionsList />
        </div>
      </PageContent>
    </AppPage>
  );
};

const HorizontalDialogDivider = () => {
  return <div className="bg-white/50 w-2 my-20" />;
};

export default SceneDialogsPage;
