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
      <span className="mb-2 block text-sm font-medium text-slate-200">
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
        className="w-full rounded-lg border border-slate-700 bg-slate-900 px-4 py-3 text-slate-100 outline-none transition placeholder:text-slate-600 focus:border-amber-400 focus:ring-2 focus:ring-amber-400/20"
      />
    </label>
  );
}
