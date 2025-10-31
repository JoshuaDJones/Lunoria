import Text, { TextColor } from "../typography/Text";
import { SceneDialogDto } from "../../types/scene";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import ViewSceneDialogModal from "../modals/ViewSceneDialogModal";

interface DashboardDialogListProps {
  sceneDialogs: SceneDialogDto[];
}

const DashboardDialogList = ({ sceneDialogs }: DashboardDialogListProps) => {
  const modalRouter = useModalRouter();

  return (
    <div className="flex flex-col flex-1 p-5 gap-2">
      {sceneDialogs.map((d) => (
        <button
          className="rounded bg-stone-600 flex p-3 hover:opacity-50"
          onClick={() =>
            modalRouter.push(<ViewSceneDialogModal sceneDialog={d} />)
          }
        >
          <Text className="flex-1 text-start" textColor={TextColor.white}>
            {d.title}
          </Text>
          <Text textColor={TextColor.white}>Pages: {d.dialogPages.length}</Text>
        </button>
      ))}
    </div>
  );
};

export default DashboardDialogList;
