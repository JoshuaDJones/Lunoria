import Title from "../typography/Title";
import AppModal from "./AppModal";
import { RingLoader } from "react-spinners";

const LoadingModal = () => {
  return (
    <AppModal centerContent backgroundOverride="bg-stone-500/50">
      <div className="p-5 rounded-2xl bg-black">
        <RingLoader color="white" />
      </div>
    </AppModal>
  );
};

export default LoadingModal;
