interface AuthFieldProps {
  id: string;
  label: string;
  type: "email" | "password";
  value: string;
  autoComplete: string;
  onChange: (value: string) => void;
}

export function AuthField({
  id,
  label,
  type,
  value,
  autoComplete,
  onChange,
}: AuthFieldProps) {
  return (
    <FormField htmlFor={id} label={label}>
      <Input
        id={id}
        name={id}
        type={type}
        value={value}
        required
        autoComplete={autoComplete}
        onChange={(event) => onChange(event.target.value)}
      />
    </FormField>
  );
}
import { FormField, Input } from "@/components/ui";
