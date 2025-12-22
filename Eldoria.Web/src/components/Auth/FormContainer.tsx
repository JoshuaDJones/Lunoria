import { Form } from "formik";
import { InputHTMLAttributes, PropsWithChildren } from "react";

const FormContainer = ({
  children,
  ...props
}: PropsWithChildren<InputHTMLAttributes<HTMLFormElement>>) => {
  return (
    <div className="flex justify-center">
      <Form
        className="flex flex-col items-center justify-center pt-10 w-[90%]"
        {...props}
      >
        {children}
      </Form>
    </div>
  );
};

export default FormContainer;
