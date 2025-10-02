import clsx from "clsx";
import { ErrorMessage } from "formik";

interface InputErrorProps {
  name: string;
  className?: string;
}

const InputError = ({ name, className }: InputErrorProps) => {
  return (
    <ErrorMessage
      name={name}
      component="span"
      className={clsx(
        "text-red-500 dark:text-red-300 self-start ml-3 font-cinzel",
        className,
      )}
    />
  );
};

export default InputError;
