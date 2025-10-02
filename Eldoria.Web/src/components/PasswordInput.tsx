import React from "react";
import EasyInput, { EasyInputProps } from "./EasyInput";

interface PasswordInputProps extends EasyInputProps {}

const PasswordInput = ({ type = "password", ...props }: PasswordInputProps) => {
  return <EasyInput {...props} type={type} />;
};

export default PasswordInput;
