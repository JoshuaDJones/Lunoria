import { PropsWithChildren } from "react";
import { Form } from "formik";

const FormContent = ({ children }: PropsWithChildren) => {
  return (
    <Form className="w-[40%] px-20 py-20 bg-white/80 rounded-2xl">
      {children}
    </Form>
  );
};

export default FormContent;
