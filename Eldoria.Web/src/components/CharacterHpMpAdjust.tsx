import { useEffect, useState } from "react";
import AdjustHpMpButton from "./AdjustHpMpButton";
import Text, { TextColor } from "../components/typography/Text";

interface CharacterHpMpAdjustProps {
  currentHp: number;
  maxHp: number;
  currentMp: number;
  maxMp: number;
  onSaveClick: (hp: number, mp: number) => void;
}

const CharacterHpMpAdjust = ({
  currentHp,
  maxHp,
  currentMp,
  maxMp,
  onSaveClick,
}: CharacterHpMpAdjustProps) => {
  const [internalHp, setInternalHp] = useState(currentHp);
  const [internalMp, setInternalMp] = useState(currentMp);

  useEffect(() => {
    setInternalHp(currentHp);
    setInternalMp(currentMp);
  }, [currentHp, currentMp]);

  const isHpModified = currentHp !== internalHp;
  const isMpModified = currentMp !== internalMp;

  const hpModifiedAmount = internalHp - currentHp;
  const mpModifiedAmount = internalMp - currentMp;

  const addHp = (amount: number) => {
    const val = Math.min(internalHp + amount, maxHp);
    setInternalHp(val);
  };

  const subtractHp = (amount: number) => {
    const val = Math.max(internalHp - amount, 0);
    setInternalHp(val);
  };

  const addMp = (amount: number) => {
    const val = Math.min(internalMp + amount, maxMp);
    setInternalMp(val);
  };

  const subtractMp = (amount: number) => {
    const val = Math.max(internalMp - amount, 0);
    setInternalMp(val);
  };

  return (
    <div className="flex-1 flex flex-col w-full">
      <div className="flex flex-1 w-full gap-1">
        <div className="p-2 rounded-xl bg-red-400/40 flex flex-col flex-1 tracking-wider">
          <Text
            className="self-center mb-2 font-bold"
            textColor={TextColor.white}
          >
            Hp: {internalHp}/{maxHp}
          </Text>
          <div className="flex">
            <div className="flex flex-col flex-1">
              <AdjustHpMpButton
                title={"+1"}
                onClick={() => addHp(1)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"+3"}
                onClick={() => addHp(3)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"+5"}
                onClick={() => addHp(5)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"+7"}
                onClick={() => addHp(7)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"Fill"}
                onClick={() => setInternalHp(maxHp)}
                type={"hp"}
              />
            </div>
            <div className="flex flex-col flex-1">
              <AdjustHpMpButton
                title={"-1"}
                onClick={() => subtractHp(1)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"-3"}
                onClick={() => subtractHp(3)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"-5"}
                onClick={() => subtractHp(5)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"-7"}
                onClick={() => subtractHp(7)}
                type={"hp"}
              />
              <AdjustHpMpButton
                title={"Zero"}
                onClick={() => setInternalHp(0)}
                type={"hp"}
              />
            </div>
          </div>
        </div>
        <div className="p-2 rounded-xl bg-green-400/40 flex flex-col flex-1">
          <Text
            className="self-center mb-2 font-bold tracking-wider"
            textColor={TextColor.white}
          >
            Mp: {internalMp}/{maxMp}
          </Text>
          <div className="flex">
            <div className="flex flex-col flex-1">
              <AdjustHpMpButton
                title={"+1"}
                onClick={() => addMp(1)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"+3"}
                onClick={() => addMp(3)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"+5"}
                onClick={() => addMp(5)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"+7"}
                onClick={() => addMp(7)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"Fill"}
                onClick={() => setInternalMp(maxMp)}
                type={"mp"}
              />
            </div>
            <div className="flex flex-col flex-1">
              <AdjustHpMpButton
                title={"-1"}
                onClick={() => subtractMp(1)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"-3"}
                onClick={() => subtractMp(3)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"-5"}
                onClick={() => subtractMp(5)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"-7"}
                onClick={() => subtractMp(7)}
                type={"mp"}
              />
              <AdjustHpMpButton
                title={"Zero"}
                onClick={() => setInternalMp(0)}
                type={"mp"}
              />
            </div>
          </div>
        </div>
      </div>
      {(isHpModified || isMpModified) && (
        <div className="flex w-full mt-2">
          <div className="flex flex-col flex-1 bg-white/20 rounded-lg p-2">
            {isHpModified && (
              <Text textColor={TextColor.white}>
                Hp:{" "}
                {hpModifiedAmount > 0
                  ? `+${hpModifiedAmount}`
                  : `${hpModifiedAmount}`}
              </Text>
            )}
            {isMpModified && (
              <Text textColor={TextColor.white}>
                Mp:{" "}
                {mpModifiedAmount > 0
                  ? `+${mpModifiedAmount}`
                  : `${mpModifiedAmount}`}
              </Text>
            )}
          </div>
          <div className="flex flex-1 justify-center items-center">
            <button
              className="rounded-lg bg-blue-800 text-white py-1 mt-2 px-4 hover:opacity-70 cursor-pointer"
              onClick={() => onSaveClick(internalHp, internalMp)}
            >
              Save
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default CharacterHpMpAdjust;
