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
    <label htmlFor={id} className="block">
      <span className="mb-2 block text-sm font-medium text-content-secondary">
        {label}
      </span>
      <input
        id={id}
        name={id}
        type={type}
        value={value}
        required
        autoComplete={autoComplete}
        onChange={(event) => onChange(event.target.value)}
        className="w-full rounded-lg border border-border bg-surface-raised px-4 py-3 text-content outline-none transition placeholder:text-content-placeholder focus:border-brand-hover focus:ring-2 focus:ring-brand-hover/20"
      />
    </label>
  );
}
