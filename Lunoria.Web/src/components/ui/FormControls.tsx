import {
  type InputHTMLAttributes,
  type ReactNode,
  type SelectHTMLAttributes,
  type TextareaHTMLAttributes,
} from "react";
import { twMerge } from "tailwind-merge";

const controlClasses =
  "w-full rounded-lg border border-border bg-surface-raised px-4 py-3 text-content outline-none transition placeholder:text-content-placeholder focus:border-brand-hover focus:ring-2 focus:ring-brand-hover/20 disabled:cursor-not-allowed disabled:opacity-60";

export function Input({
  className,
  ...props
}: InputHTMLAttributes<HTMLInputElement>) {
  return <input className={twMerge(controlClasses, className)} {...props} />;
}

export function Textarea({
  className,
  ...props
}: TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea className={twMerge(controlClasses, className)} {...props} />;
}

export function Select({
  className,
  ...props
}: SelectHTMLAttributes<HTMLSelectElement>) {
  return <select className={twMerge(controlClasses, className)} {...props} />;
}

interface FormFieldProps {
  htmlFor: string;
  label: ReactNode;
  children: ReactNode;
  className?: string;
}

export function FormField({
  htmlFor,
  label,
  children,
  className,
}: FormFieldProps) {
  return (
    <label htmlFor={htmlFor} className={twMerge("block", className)}>
      <span className="mb-2 block text-sm font-medium text-content-secondary">
        {label}
      </span>
      {children}
    </label>
  );
}
