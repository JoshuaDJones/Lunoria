import React from "react";
import { SceneDto } from "../../types/scene";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import Text, { TextColor, TextSize } from "../typography/Text";
import EditIconButton from "../buttons/EditIconButton";
import DeleteIconButton from "../buttons/DeleteIconButton";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import ConfirmationModal from "../modals/ConfirmationModal";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { useLoading } from "../../providers/LoadingProvider";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { useNavigate } from "react-router";
import EditSceneDialogModal from "../modals/EditSceneDialogModal";

interface SceneListProps {
  journeyId: number;
  scenes?: SceneDto[];
  onRefreshRequest: () => void;
}

const SceneList = ({ journeyId, scenes, onRefreshRequest }: SceneListProps) => {
  if (!scenes) return null;

  console.log(scenes);

  return (
    <div>
      {scenes.map((s) => (
        <SceneListItem
          journeyId={journeyId}
          scene={s}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

interface SceneListItemProps {
  journeyId: number;
  scene: SceneDto;
  onRefreshRequest: () => void;
}

const SceneListItem = ({
  journeyId,
  scene,
  onRefreshRequest,
}: SceneListItemProps) => {
  const modalRouter = useModalRouter();
  const { del } = useApi();
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();
  const navigate = useNavigate();

  const handleDeletion = async () => {
    try {
      showLoading();
      await del(`${BASE_URL}/Scene/${scene.id}`, {
        JourneyId: journeyId,
      });
      onRefreshRequest();
      showToast("Success", "Scene deleted.", ToastType.success, 3000);
    } catch (err) {
      console.error(err);
      showToast("Error", "Scene not deleted.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  const openConfirmDeleteModal = () => {
    modalRouter.push(
      <ConfirmationModal
        title="Confirmation"
        onConfirm={async () => await handleDeletion()}
        description={"Are you sure you want to delete this scene?"}
      />,
    );
  };

  const openDialogModal = () => {
    modalRouter.push(<EditSceneDialogModal dialogs={scene.sceneDialogs} />);
  };

  const navigateScene = () => {
    navigate(`/SceneDashboard/${scene.id}?journeyId=${journeyId}`);
  };

  const navigateSceneDialogs = () => {
    navigate(`/SceneDialogsPage/${scene.id}`);
  };

  return (
    <div className="flex flex-col mt-2 bg-red-200 p-4 rounded bg-stone-700/70 rounded-xl">
      <div className="flex">
        <div className="w-[65%]">
          <Title size={TitleSize.medium} color={TitleColor.white}>
            {scene.name}
          </Title>
          <Text
            size={TextSize.xl}
            textColor={TextColor.custom}
            className="text-gray-400"
          >
            {scene.description}
          </Text>
        </div>
        <div className="w-[35%]">
          <img src={scene.photoUrl} className="rounded-xl" />
        </div>
      </div>
      <div className="flex justify-end gap-2 mt-2">
        <AppButton
          title={"Play Scene"}
          onClick={navigateScene}
          variant={AppButtonVariant.go}
          size={AppButtonSize.sm}
        />
        <AppButton
          title={"View Dialog"}
          onClick={navigateSceneDialogs}
          variant={AppButtonVariant.primary}
          size={AppButtonSize.sm}
        />
        <AppButton
          title={"Delete Scene"}
          onClick={openConfirmDeleteModal}
          variant={AppButtonVariant.warning}
          size={AppButtonSize.sm}
        />
        <AppButton
          title={"Edit Scene"}
          variant={AppButtonVariant.primary}
          size={AppButtonSize.sm}
        />
      </div>
    </div>
  );
};

export default SceneList;
