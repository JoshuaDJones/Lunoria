import BorderBox from "../BorderBox";
import DataRow from "../DataRow";
import { DateTime } from "luxon";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faTrash } from "@fortawesome/free-solid-svg-icons";
import { Item } from "./item";

interface ItemItemProps {
  item: Item;
}

const ItemItem = ({ item }: ItemItemProps) => {
  return (
    <div>
      <BorderBox>
        <img src={item.photoUrl} className="w-[150px]" />
        <DataRow title={"Name:"} value={item.name} />
        <DataRow title={"Description:"} value={item.description} />
        <DataRow title={"Hp Effect:"} value={item.hpEffect.toString()} />
        <DataRow title={"Mp Effect:"} value={item.mpEffect.toString()} />
        <DataRow
          title={"Created:"}
          value={DateTime.fromISO(item.createDate).toFormat("MM-dd-yyyy")}
        />
        <DataRow
          title={"Updated:"}
          value={DateTime.fromISO(item.updateDate).toFormat("MM-dd-yyyy")}
        />

        <div className="flex-row flex gap-2 mt-5">
          <EasyButton
            className="gap-2"
            title={"Edit"}
            rightIcon={<FontAwesomeIcon icon={faEdit} color={"white"} />}
            variant={EasyButtonVariant.Primary}
          ></EasyButton>

          <EasyButton
            className="gap-2"
            title={"Delete"}
            rightIcon={<FontAwesomeIcon icon={faTrash} color={"white"} />}
            variant={EasyButtonVariant.Secondary}
          ></EasyButton>
        </div>
      </BorderBox>
    </div>
  );
};

export default ItemItem;
