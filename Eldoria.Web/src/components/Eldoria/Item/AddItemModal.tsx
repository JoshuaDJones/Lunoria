import React, { useEffect, useState } from "react";
import { useAuth } from "../../../providers/AuthProvider";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import Title from "../Title";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import EasyText from "../../EasyText";
import { BASE_URL, get, post, put } from "../../../api/requests";

interface AddItemModalProps {
  visible: boolean;
  onClose: () => void;
  onSave: () => void;
  characterId: number;
}

interface ItemDto {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  hpEffect?: number | null;
  mpEffect?: number | null;
}

const list_items_url = `${BASE_URL}items`;
const upsert_item_url = `${BASE_URL}scene-character-items/upsert`;

const AddItemModal = ({
  visible,
  onClose,
  onSave,
  characterId,
}: AddItemModalProps) => {
  const { token } = useAuth();
  const [items, setItems] = useState<ItemDto[]>([]);
  const [loading, setLoading] = useState(false);

  const loadItems = async () => {
    setLoading(true);
    try {
      const res = await get(list_items_url, undefined, token);
      console.log("Items response:", res);
      setItems(Array.isArray(res) ? res : (res?.items ?? []));
    } catch (err) {
      console.error("Failed to load items", err);
      setItems([]);
    } finally {
      setLoading(false);
    }
  };

  const attachItem = async (itemId: number) => {
    try {
      const body = {
        sceneCharacterId: characterId,
        itemId: itemId,
        isUsed: false,
      };

      await put(upsert_item_url, body, undefined, token); // 👈 use PUT not POST
      onSave();
      onClose();
    } catch (err) {
      console.error("Failed to attach item", err);
    }
  };

  useEffect(() => {
    if (visible) loadItems();
  }, [visible]);

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-6">
          <Title>Add Item</Title>

          {loading && <EasyText>Loading…</EasyText>}

          <div className="flex flex-col gap-4 max-h-[500px] overflow-y-auto">
            {items.map((item) => (
              <div
                key={item.id}
                className="flex flex-row gap-4 p-3 border rounded-lg items-center"
              >
                <img
                  src={item.photoUrl}
                  alt={item.name}
                  className="w-16 h-16 object-cover rounded"
                />
                <div className="flex-1 flex-col flex">
                  <EasyText className="text-lg font-bold">{item.name}</EasyText>
                  <EasyText className="text-sm">{item.description}</EasyText>
                </div>
                <EasyButton
                  title="Add"
                  variant={EasyButtonVariant.Primary}
                  onClick={() => attachItem(item.id)}
                />
              </div>
            ))}
            {items.length === 0 && !loading && (
              <EasyText className="italic text-gray-500">
                No items available
              </EasyText>
            )}
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default AddItemModal;
