import AppLayout from "@/app/layouts/AppLayout";

export function ConsumablesPage() {
  return (
    <AppLayout
      background={
        <div className="absolute right-0 left-0 w-full h-full top-0 bottom-0 z-0 stone-image" />
      }
    >
      <main className="p-10">
        <h1 className="text-5xl font-semibold text-content">Consumables</h1>
      </main>
    </AppLayout>
  );
}