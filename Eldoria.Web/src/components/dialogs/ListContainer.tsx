import { PropsWithChildren } from "react";
import DialogListTitle from "./DialogListTitle";

interface ListContainerProps {
  title: string;
}

const ListContainer = ({
  title,
  children,
}: PropsWithChildren<ListContainerProps>) => {
  return (
    <div className="flex flex-col flex-1">
      <DialogListTitle title={title} />
      <div className="flex-1 flex flex-col mt-5 p-5 overflow-y-auto scrollbar-hide">
        {children}
      </div>
    </div>
  );
};

export default ListContainer;
