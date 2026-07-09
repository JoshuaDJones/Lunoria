import AppLayout from "@/app/layouts/AppLayout";
import { Button } from "@/components/ui";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowRight,
  faEye,
  faHatWizard,
  faPen,
  faPlus,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";

const buttonExamples = [
  { variant: "primary", label: "Primary", icon: faArrowRight },
  { variant: "secondary", label: "Secondary", icon: faEye },
  { variant: "accent", label: "Accent", icon: faPen },
  { variant: "add", label: "Add", icon: faPlus },
  { variant: "magic", label: "Magic", icon: faHatWizard },
  { variant: "danger", label: "Danger", icon: faTrash },
] as const;

export function ComponentDisplayPage() {
  return (
    <AppLayout
      scrolling
      background={
        <div className="stone-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full p-6 sm:p-10">
        <header className="mb-8">
          <h1 className="text-6xl text-content">Components</h1>
          <p className="mt-3 max-w-2xl text-content-secondary">
            Button variants and inverted states.
          </p>
        </header>

        <section className="rounded-xl border border-border bg-surface/85 p-6">
          <h2 className="text-2xl font-semibold text-content">Buttons</h2>

          <div className="mt-6 overflow-x-auto">
            <table className="w-full min-w-[45rem] border-separate border-spacing-y-3 text-left">
              <thead>
                <tr className="text-sm text-content-muted">
                  <th className="px-3 font-medium">Variant</th>
                  <th className="px-3 font-medium">Default</th>
                  <th className="px-3 font-medium">Inverted</th>
                  <th className="px-3 font-medium">With right icon</th>
                </tr>
              </thead>
              <tbody>
                {buttonExamples.map(({ variant, label, icon }) => (
                  <tr key={variant} className="bg-surface-raised/60">
                    <th className="rounded-l-lg px-3 py-4 text-sm font-semibold text-content-secondary">
                      {label}
                    </th>
                    <td className="px-3 py-4">
                      <Button
                        variant={variant}
                        leftIcon={<FontAwesomeIcon icon={icon} />}
                      >
                        {label}
                      </Button>
                    </td>
                    <td className="px-3 py-4">
                      <Button
                        variant={variant}
                        inverted
                        leftIcon={<FontAwesomeIcon icon={icon} />}
                      >
                        {label}
                      </Button>
                    </td>
                    <td className="rounded-r-lg px-3 py-4">
                      <Button
                        variant={variant}
                        rightIcon={<FontAwesomeIcon icon={faArrowRight} />}
                      >
                        {label}
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      </main>
    </AppLayout>
  );
}
