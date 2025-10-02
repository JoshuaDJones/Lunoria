import React, { useEffect, useState } from "react";
import { useAuth } from "../../../providers/AuthProvider";
import Modal from "../../Modal/Modal";
import ModalContent from "../../Modal/ModalContent";
import Title from "../../typography/Title";
import EasyButton, { EasyButtonVariant } from "../../EasyButton";
import { BASE_URL, get, post } from "../../../api/requests";

interface CharacterSpellsProps {
  characterId: number;
  visible: boolean;
  onClose: () => void;
  onSave: () => void;
}

export interface SpellAttachDto {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  cost: number;
  damage?: number | null;
  health?: number | null;
  isAttached: boolean;
}

const get_character_spells_url = `${BASE_URL}character-spells/`;
const attach_spell_url = `${BASE_URL}character-spells`;

const CharacterSpells = ({
  characterId,
  visible,
  onClose,
  onSave,
}: CharacterSpellsProps) => {
  const { token } = useAuth();
  const [spells, setSpells] = useState<SpellAttachDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const attachSpell = async (spellId: number) => {
    try {
      const url = `${attach_spell_url}/${characterId}/attach/${spellId}`;
      await post(url, {}, undefined, token);

      // after attaching, reload spells so UI is updated
      await getSpellsWithAttachment();
    } catch (e: any) {
      console.error("Failed to attach spell", e);
      setErr(e?.message ?? "Failed to attach spell.");
    }
  };

  const getSpellsWithAttachment = async () => {
    try {
      setLoading(true);
      setErr(null);

      console.log("before");

      // If your get() returns { data }, use res.data; otherwise use res directly.
      const res = await get(
        `${get_character_spells_url}${characterId}`,
        undefined,
        token,
      );
      const data = res?.data ?? res;

      console.log(res);

      // Validate/normalize just in case
      setSpells(Array.isArray(data) ? data : []);
    } catch (e: any) {
      console.error("Failed to load character spells", e);
      setErr(e?.message ?? "Failed to load spells.");
      setSpells([]);
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    // fire when the modal is open and characterId is a number (including 0)
    if (!visible) return;
    if (characterId === null || characterId === undefined) return;

    getSpellsWithAttachment();
    // include token if your get() requires it to set headers
  }, [visible, characterId, token]);
  const save = async () => {
    onSave();
    onClose();
  };

  if (!visible) return null;

  return (
    <Modal visible={visible} onBackgroundClose={onClose}>
      <ModalContent>
        <div className="flex flex-1 flex-col gap-12">
          <Title>Character Spells</Title>

          {loading && <div className="text-sm text-gray-500">Loading…</div>}
          {err && <div className="text-sm text-red-600">{err}</div>}

          <div className="flex-1 space-y-4 overflow-y-auto">
            {spells.map((spell) => (
              <div
                key={spell.id}
                className="rounded-lg border border-gray-300 p-4 shadow-sm flex flex-col gap-2"
              >
                <div className="flex items-center gap-4">
                  <img
                    src={spell.photoUrl}
                    alt={spell.name}
                    className="w-16 h-16 object-cover rounded"
                  />
                  <div>
                    <h3 className="text-lg font-semibold">{spell.name}</h3>
                    <p className="text-sm text-gray-600">{spell.description}</p>
                  </div>
                </div>

                <div className="flex justify-between text-sm text-gray-700 items-center">
                  <span>Cost: {spell.cost}</span>
                  {spell.damage != null && <span>Damage: {spell.damage}</span>}
                  {spell.health != null && <span>Health: {spell.health}</span>}
                  <span
                    className={`font-semibold ${
                      spell.isAttached ? "text-green-600" : "text-red-600"
                    }`}
                  >
                    {spell.isAttached ? "Attached" : "Not Attached"}
                  </span>
                  <EasyButton
                    title={spell.isAttached ? "Remove" : "Attach"}
                    variant={EasyButtonVariant.Secondary}
                    onClick={() => attachSpell(spell.id)}
                  />
                </div>
              </div>
            ))}
            {!loading && !err && spells.length === 0 && (
              <div className="text-sm text-gray-500">No spells found.</div>
            )}
          </div>

          <div className="flex justify-center">
            <EasyButton
              title="Save Character"
              variant={EasyButtonVariant.Primary}
              onClick={save}
            />
          </div>
        </div>
      </ModalContent>
    </Modal>
  );
};

export default CharacterSpells;
