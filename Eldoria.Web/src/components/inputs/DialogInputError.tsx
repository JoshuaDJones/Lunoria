import Text from "../typography/Text";

interface DialogInputErrorProps {
  message?: string;
}

const DialogInputError = ({ message }: DialogInputErrorProps) => {
  if (!message) return null;

  return <Text className="text-red-700 ml-2">{message}</Text>;
};

export default DialogInputError;
