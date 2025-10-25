import { DialogPageDto } from "../../types/scene";
import CreatePageSection from "../dialogs/CreatePageSection";
import DialogPageSectionCard from "../cards/DialogPageSectionCard";
import ListContainer from "../dialogs/ListContainer";

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
  const pageSections = selectedDialogPage?.dialogPageSections ?? [];

  return (
    <ListContainer title="Sections">
      {selectedDialogPage && (
        <CreatePageSection
          pageDialogId={selectedDialogPage.id}
          onRefreshRequest={onRefreshRequest}
        />
      )}
      {pageSections.map((s) => (
        <DialogPageSectionCard
          key={s.id}
          dialogPageSection={s}
          selectedPageSectionId={selectedDialogPageSectionId}
          onSelect={(pageSectionId) => onDialogPageSectionSelect(pageSectionId)}
          onRefreshRequest={onRefreshRequest}
        />
      ))}
    </ListContainer>
  );
};

export default DialogPageSectionsList;
