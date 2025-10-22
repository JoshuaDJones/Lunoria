import React from "react";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import { DialogPageDto } from "../../types/scene";

interface DialogPageSectionListProps{
  selectedDialogPageSectionId?: number;
  selectedDialogPage?: DialogPageDto;
  onDialogPageSectionSelect: (dialogPageSectionId: number) => void;
  onRefreshRequest: () => void;
}

const DialogPageSectionsList = ({
  selectedDialogPageSectionId,
  selectedDialogPage,
  onDialogPageSectionSelect,
  onRefreshRequest
}: DialogPageSectionListProps) => {
  const pageSections = selectedDialogPage?.dialogPageSections;
  
  return (
    <div className="flex flex-col flex-1">
      <Title
        className="self-center border-b-2 text-3xl"
        color={TitleColor.white}
        size={TitleSize.custom}
      >
        Sections
      </Title>
      <div className="flex-1 flex-col mt-5 p-5">
        {selectedDialogPage && (
          <></>
        )}
        {pageSections && pageSections.map((s) => (
          <></>
        ))}
      </div>
    </div>
  );
};

export default DialogPageSectionsList;
