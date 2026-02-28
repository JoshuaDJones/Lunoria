import { JourneyDto } from "../../types/journey";
import Text, { TextColor } from "../typography/Text";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import AddEditJourneyModal from "../modals/AddEditJourneyModal";
import ConfirmationModal from "../modals/ConfirmationModal";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { useNavigate } from "react-router";

interface JourneyListProps {
  journeys: JourneyDto[] | undefined;
  onRefreshRequest: () => void;
}

const JourneyList = ({ journeys, onRefreshRequest }: JourneyListProps) => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {journeys?.map((j) => (
        <JourneyListItem
          key={j.id}
          journey={j}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </div>
  );
};

interface JourneyListItemProps {
  journey: JourneyDto;
  onRefreshRequest: () => void;
}

const JourneyListItem = ({
  journey,
  onRefreshRequest,
}: JourneyListItemProps) => {
  const modalRouter = useModalRouter();
  const { del } = useApi();
  const { showToast } = useToast();
  const navigate = useNavigate();

  const handleDeletion = async () => {
    try {
      await del(`${BASE_URL}/Journey/${journey.id}`);
      showToast("Success:", "Journey deleted.", ToastType.success, 3000);
      onRefreshRequest();
    } catch (err) {
      console.error(err);
      showToast("Error:", "Journey not deleted.", ToastType.success, 3000);
    }
  };

  const navigateJourney = () => {
    navigate(`/JourneyDetails/${journey.id}`);
  };

  const openEditModal = () => {
    modalRouter.push(
      <AddEditJourneyModal journey={journey} onSave={onRefreshRequest} />,
    );
  };

  const openDeleteModal = () => {
    modalRouter.push(
      <ConfirmationModal
        title="Confirmation"
        description="Are you sure you want to delete this journey?"
        onConfirm={async () => await handleDeletion()}
      />,
    );
  };

  return (
    <div
      className="rounded-xl p-3 flex flex-col bg-stone-800/50 cursor-pointer hover:bg-stone-700/80"
      onClick={() => console.log("card")}
    >
      <div className="flex flex-1 gap-2 items-start">
        <div className="w-[60%] flex-col">
          <Title
            className="flex-1 break-words"
            size={TitleSize.medium}
            color={TitleColor.white}
          >
            {journey.name}
          </Title>
          <Text
            textColor={TextColor.custom}
            className="text-gray-300 break-words line-clamp-3"
          >
            {journey.description}
          </Text>
        </div>
        <img
          src={journey.photoUrl}
          className="w-[39%] object-cover rounded-md"
        />
      </div>
      <div className="justify-end flex gap-2 mt-2">
        <AppButton
          title={"View"}
          onClick={navigateJourney}
          variant={AppButtonVariant.go}
          size={AppButtonSize.sm}
        />
        <AppButton
          title={"Edit"}
          onClick={openEditModal}
          variant={AppButtonVariant.primary}
          size={AppButtonSize.sm}
        />
        <AppButton
          title={"Delete"}
          onClick={openDeleteModal}
          variant={AppButtonVariant.warning}
          size={AppButtonSize.sm}
        />
      </div>
    </div>
  );
};

export default JourneyList;
