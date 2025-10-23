import Title, { TitleColor, TitleSize } from "../typography/Title";
import { DialogPageDto } from "../../types/scene";
import CreatePageSection from "../dialogs/CreatePageSection";
import DialogPageSectionCard from "../cards/DialogPageSectionCard";
import DialogListTitle from "../dialogs/DialogListTitle";

interface DialogPageSectionListProps {
  selectedDialogPageSectionId?: number;
  selectedDialogPage?: DialogPageDto;
  onDialogPageSectionSelect: (dialogPageSectionId: number) => void;
  onRefreshRequest: () => void;
}

const DialogPageSectionsList = ({
  selectedDialogPageSectionId,
  selectedDialogPage,
  onDialogPageSectionSelect,
  onRefreshRequest,
}: DialogPageSectionListProps) => {
  const pageSections = selectedDialogPage?.dialogPageSections;

  return (
    <div className="flex flex-col flex-1">
      <DialogListTitle title="Sections" />
      <div className="flex-1 flex-col mt-5 p-5">
        {selectedDialogPage && (
          <CreatePageSection
            pageDialogId={selectedDialogPage.id}
            onRefreshRequest={onRefreshRequest}
          />
        )}
        {pageSections &&
          pageSections.map((s) => (
            <DialogPageSectionCard
              dialogPageSection={s}
              selectedPageSectionId={selectedDialogPageSectionId}
              onSelect={(pageSectionId) =>
                onDialogPageSectionSelect(pageSectionId)
              }
            />
          ))}
      </div>
    </div>
  );
};

export default DialogPageSectionsList;
