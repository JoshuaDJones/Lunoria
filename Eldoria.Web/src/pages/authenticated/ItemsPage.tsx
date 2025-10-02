import { useEffect, useState } from "react";
import AppPage from "../../components/Layout/AppPage";
import PageContent from "../../components/Layout/PageContent";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { useLoading } from "../../providers/LoadingProvider";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import { ItemDto } from "../../types/item";
import AddEditItemModal from "../../components/Modal/AddEditItemModal";
import SectionTitleWithAdd from "../../components/Layout/SectionTitleWithAdd";
import ItemList from "../../components/lists/ItemList";

const ItemsPage = () => {
  const { get } = useApi();
  const modalRouter = useModalRouter();

  const [items, setItems] = useState<ItemDto[]>([]);

  const getItems = async () => {
    try {
      const items: ItemDto[] = await get(`${BASE_URL}/Item`);
      setItems(items);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    getItems();
  }, []);

  const openAddModal = () => {
    modalRouter.push(
      <AddEditItemModal onSave={async () => await getItems()} />,
    );
  };

  return (
    <AppPage hasNav backgroundImage="/Stone_Background.png">
      <PageContent noCentering>
        <SectionTitleWithAdd
          title="Items"
          onAddClick={openAddModal}
          className="mt-20 mb-5"
        />
        <ItemList
          items={items}
          onRefreshRequest={async () => await getItems()}
        />
      </PageContent>
    </AppPage>
  );
};

export default ItemsPage;
