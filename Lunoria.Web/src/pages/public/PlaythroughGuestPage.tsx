import { useEffect, useMemo, useRef, useState, type ReactNode } from "react";
import { useParams } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faChevronLeft,
  faChevronRight,
  faCircleInfo,
} from "@fortawesome/free-solid-svg-icons";
import AppLayout from "@/app/layouts";
import { Button, Drawer } from "@/components/ui";
import { CharacterType } from "@/features/characters";
import {
  createPlaythroughSessionConnection,
  getPublicPlaythroughSnapshot,
  type PublicPlaythroughCharacter,
  type PublicPlaythroughConsumableItem,
  type PublicPlaythroughEquippableItem,
  type PublicPlaythroughSnapshot,
  type PublicPlaythroughSpell,
} from "@/features/playthroughSession";
import { getApiError } from "@/lib/apiClient";

type DeckEntry =
  | {
      key: string;
      type: "character";
      character: PublicPlaythroughCharacter;
    }
  | { key: "events"; type: "events" };

export function PlaythroughGuestPage() {
  const { token = "" } = useParams<{ token: string }>();
  const [snapshot, setSnapshot] = useState<PublicPlaythroughSnapshot>();
  const [selectedKey, setSelectedKey] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [isRealtimeConnected, setIsRealtimeConnected] = useState(false);
  const [isClosed, setIsClosed] = useState(false);
  const [showEventLogs, setShowEventLogs] = useState(false);
  const scrollContainerRef = useRef<HTMLDivElement>(null);

  const entries = useMemo<DeckEntry[]>(() => {
    if (!snapshot) return [];
    return [
      ...snapshot.journeyCharacters.map((character) => ({
        key: `journey:${character.id}`,
        type: "character" as const,
        character,
      })),
      ...snapshot.sceneCharacters.map((character) => ({
        key: `scene:${character.id}`,
        type: "character" as const,
        character,
      })),
      { key: "events" as const, type: "events" as const },
    ];
  }, [snapshot]);

  const selectedIndex = Math.max(
    0,
    entries.findIndex((entry) => entry.key === selectedKey),
  );
  const selectedEntry = entries[selectedIndex];

  useEffect(() => {
    if (entries.length === 0) return;
    if (!entries.some((entry) => entry.key === selectedKey)) {
      setSelectedKey(entries[0].key);
    }
  }, [entries, selectedKey]);

  useEffect(() => {
    if (!token) {
      setError("This playthrough invitation is invalid.");
      setIsLoading(false);
      return;
    }

    let isCurrent = true;
    const connection = createPlaythroughSessionConnection(token);

    const refresh = async () => {
      try {
        const nextSnapshot = await getPublicPlaythroughSnapshot(token);
        if (isCurrent) {
          setSnapshot(nextSnapshot);
          setError("");
        }
      } catch (requestError: unknown) {
        if (isCurrent) setError(getApiError(requestError).message);
      } finally {
        if (isCurrent) setIsLoading(false);
      }
    };

    connection.on("PlaythroughUpdated", () => void refresh());
    connection.on("SessionClosed", () => {
      if (isCurrent) {
        setIsClosed(true);
        setIsRealtimeConnected(false);
      }
    });
    connection.onreconnecting(() => {
      if (isCurrent) setIsRealtimeConnected(false);
    });
    connection.onreconnected(() => {
      if (isCurrent) {
        setIsRealtimeConnected(true);
        void refresh();
      }
    });
    connection.onclose(() => {
      if (isCurrent) setIsRealtimeConnected(false);
    });

    void refresh();
    void connection
      .start()
      .then(() => connection.invoke("JoinPlaythrough", token))
      .then(() => {
        if (isCurrent) setIsRealtimeConnected(true);
      })
      .catch(() => {
        if (isCurrent) setIsRealtimeConnected(false);
      });

    return () => {
      isCurrent = false;
      void connection.stop();
    };
  }, [token]);

  const navigateDeck = (index: number) => {
    const nextEntry = entries[index];
    if (!nextEntry) return;
    setSelectedKey(nextEntry.key);
    scrollContainerRef.current?.scrollTo({ top: 0, behavior: "smooth" });
  };

  return (
    <AppLayout
      sidebar={<></>}
      bottomPadding
      background={
        <GuestSceneBackground
          key={snapshot?.activeScenePhotoUrl?.trim() ?? ""}
          photoUrl={snapshot?.activeScenePhotoUrl?.trim()}
        />
      }
    >
      <main className="flex h-full w-full flex-col overflow-hidden pb-14 md:pb-0">
        {isLoading && (
          <div className="flex min-h-full items-center justify-center p-8">
            <p className="rounded-2xl bg-surface/75 p-6 text-xl text-content backdrop-blur-sm">
              Joining playthrough...
            </p>
          </div>
        )}

        {!isLoading && (error || isClosed) && (
          <div className="flex min-h-full items-center justify-center p-8">
            <div className="max-w-lg rounded-3xl bg-surface/80 p-8 text-center backdrop-blur-sm">
              <h1 className="text-3xl font-semibold text-content">
                Session unavailable
              </h1>
              <p className="mt-4 text-content-secondary">
                {isClosed
                  ? "The game master closed or replaced this guest session."
                  : error}
              </p>
            </div>
          </div>
        )}

        {!isLoading && !error && !isClosed && snapshot && selectedEntry && (
          <>
            <header className="z-20 flex flex-none items-center justify-between gap-2 border-b border-border bg-surface/85 px-3 py-1.5 backdrop-blur-md sm:px-4">
              <div className="min-w-0">
                <h1 className="truncate text-sm font-semibold leading-tight text-content sm:text-base">
                  {snapshot.name}
                </h1>
                {snapshot.activeSceneName && (
                  <p className="truncate text-[10px] leading-tight text-content-muted sm:text-xs">
                    Active scene: {snapshot.activeSceneName}
                  </p>
                )}
              </div>
              <div className="flex shrink-0 items-center gap-1.5">
                <span
                  className={`h-1.5 w-1.5 rounded-full ${
                    isRealtimeConnected ? "bg-add" : "bg-danger"
                  }`}
                />
                <span className="text-[10px] text-content-secondary sm:text-xs">
                  {isRealtimeConnected ? "Live" : "Reconnecting"}
                </span>
                <button
                  type="button"
                  aria-label="Show event logs"
                  title="Event logs"
                  onClick={() => setShowEventLogs(true)}
                  className="ml-2 hidden h-11 w-11 items-center justify-center rounded-full bg-transparent text-content-muted hover:text-content focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility md:flex"
                >
                  <FontAwesomeIcon icon={faCircleInfo} className="h-5 w-5" />
                </button>
              </div>
            </header>

            <div
              ref={scrollContainerRef}
              className="min-h-0 w-full flex-1 overflow-y-auto px-2 py-2 scrollbar-hide sm:px-4 sm:py-3"
            >
              <div className="md:hidden">
                {selectedEntry.type === "character" ? (
                  <PublicCharacterView character={selectedEntry.character} />
                ) : (
                  <PublicEventLogView snapshot={snapshot} />
                )}
              </div>
              <div className="mx-auto hidden w-full max-w-6xl space-y-3 md:block">
                <h2 className="px-2 py-2 text-xl font-semibold text-content">
                  Characters
                </h2>
                {entries
                  .filter((entry) => entry.type === "character")
                  .map((entry) => (
                    <TabletCharacter
                      key={entry.key}
                      character={entry.character}
                    />
                  ))}
                {!snapshot.journeyCharacters.length &&
                  !snapshot.sceneCharacters.length && (
                    <EmptyCollection label="No characters available." />
                  )}
              </div>
            </div>

            <nav
              aria-label="Playthrough cards"
              className="fixed inset-x-0 bottom-0 z-30 grid h-14 grid-cols-[1fr_auto_1fr] items-center gap-2 border-t border-border bg-surface/90 px-3 py-2 backdrop-blur-md sm:px-4 md:hidden"
            >
              <Button
                aria-label="Previous card"
                size="sm"
                className="h-10 w-12 justify-self-start gap-2 sm:w-32"
                disabled={selectedIndex === 0}
                onClick={() => navigateDeck(selectedIndex - 1)}
              >
                <FontAwesomeIcon icon={faChevronLeft} />
                <span className="hidden sm:inline">Previous</span>
              </Button>
              <span className="text-xs font-semibold text-content">
                {selectedIndex + 1} / {entries.length}
              </span>
              <Button
                aria-label="Next card"
                size="sm"
                className="h-10 w-12 justify-self-end gap-2 sm:w-32"
                disabled={selectedIndex === entries.length - 1}
                onClick={() => navigateDeck(selectedIndex + 1)}
              >
                <span className="hidden sm:inline">Next</span>
                <FontAwesomeIcon icon={faChevronRight} />
              </Button>
            </nav>
            {showEventLogs && (
              <Drawer
                title="Event logs"
                onClose={() => setShowEventLogs(false)}
              >
                <PublicEventLogView snapshot={snapshot} />
              </Drawer>
            )}
          </>
        )}
      </main>
    </AppLayout>
  );
}

function GuestSceneBackground({ photoUrl }: { photoUrl?: string }) {
  const [failed, setFailed] = useState(false);
  return (
    <div
      aria-hidden="true"
      className="pointer-events-none absolute inset-0 z-0"
    >
      <div className="valley-village-image absolute inset-0 h-full w-full" />
      {photoUrl && !failed && (
        <>
          <img
            src={photoUrl}
            alt=""
            className="absolute inset-0 h-full w-full object-cover object-center"
            onError={() => setFailed(true)}
          />
          <div className="absolute inset-0 bg-black/40" />
        </>
      )}
    </div>
  );
}

function Accordion({
  title,
  children,
}: {
  title: ReactNode;
  children: ReactNode;
}) {
  return (
    <details className="rounded-xl border border-border bg-surface/85 text-content [&[open]>summary>svg]:rotate-90">
      <summary className="flex min-h-12 cursor-pointer select-none list-none items-center justify-between gap-3 p-4 font-semibold focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility [&::-webkit-details-marker]:hidden">
        <span className="min-w-0 flex-1">{title}</span>
        <svg
          aria-hidden="true"
          viewBox="0 0 24 24"
          className="h-5 w-5 shrink-0 transition-transform motion-reduce:transition-none"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
        >
          <path d="m9 5 7 7-7 7" />
        </svg>
      </summary>
      <div className="space-y-3 border-t border-border p-4">{children}</div>
    </details>
  );
}

function TabletCharacter({
  character: c,
}: {
  character: PublicPlaythroughCharacter;
}) {
  const consumables = groupPublicConsumables(c.consumableItems);
  const alternate = c.alternateForm;
  return (
    <Accordion
      title={
        <span className="flex flex-wrap items-center gap-x-5 gap-y-1">
          <span className="text-xl">{c.name}</span>
          <span className="text-sm text-content-secondary">
            {getCharacterTypeLabel(c.characterType)} · HP {c.currentHp}/
            {c.maxHp} · MP {c.currentMp}/{c.maxMp}
          </span>
          {!c.isActive && <StatusBadge label="Inactive" />}
          {c.isDown && <StatusBadge label="Down" danger />}
          {c.isDead && <StatusBadge label="Dead" danger />}
          {c.isInAlternateForm && <StatusBadge label="Alternate form" />}
        </span>
      }
    >
      {c.description && (
        <p className="whitespace-pre-wrap text-content-secondary">
          {c.description}
        </p>
      )}
      <dl className="grid grid-cols-2 gap-3 lg:grid-cols-4">
        <PublicStat label="Movement" value={c.movement} />
        <PublicStat
          label="Melee"
          value={c.meleeAttackDamage ?? "Unavailable"}
        />
        <PublicStat label="Bow" value={c.bowAttackDamage ?? "Unavailable"} />
        <PublicStat
          label="Capacity: consumables / equipment"
          value={`${c.maxConsumableInventory} / ${c.maxEquippableInventory}`}
        />
      </dl>
      <Accordion title={`Spells (${c.spells.length})`}>
        {c.spells.length ? (
          c.spells.map((spell) => <SpellCard key={spell.id} spell={spell} />)
        ) : (
          <EmptyCollection label="No spells available." />
        )}
      </Accordion>
      <Accordion title={`Equipment (${c.equippableItems.length})`}>
        {c.equippableItems.length ? (
          c.equippableItems.map((item, index) => (
            <EquipmentCard key={`${item.id}-${index}`} item={item} />
          ))
        ) : (
          <EmptyCollection label="No equipment carried." />
        )}
      </Accordion>
      <Accordion title={`Consumables (${c.consumableItems.length})`}>
        {consumables.length ? (
          consumables.map(({ item, quantity }) => (
            <ConsumableCard
              key={`${item.id}-${item.isUsed}`}
              item={item}
              quantity={quantity}
            />
          ))
        ) : (
          <EmptyCollection label="No consumables carried." />
        )}
      </Accordion>
      <Accordion
        title={
          alternate
            ? `Alternate character: ${alternate.name}`
            : "Alternate character"
        }
      >
        {alternate ? (
          <>
            <p className="whitespace-pre-wrap text-content-secondary">
              {alternate.description}
            </p>
            <p className="text-sm text-content-muted">
              Saved base stats. Transformation uses this form’s movement, melee,
              bow, and spells. HP, MP, and inventory are preserved; equipment
              effects still apply.
            </p>
            <dl className="grid grid-cols-2 gap-3 lg:grid-cols-4">
              <PublicStat label="Base max HP" value={alternate.maxHp} />
              <PublicStat label="Base max MP" value={alternate.maxMp} />
              <PublicStat label="Base movement" value={alternate.movement} />
              <PublicStat
                label="Base melee"
                value={alternate.meleeAttackDamage ?? "Unavailable"}
              />
              <PublicStat
                label="Base bow"
                value={alternate.bowAttackDamage ?? "Unavailable"}
              />
              <PublicStat
                label="Base consumable capacity"
                value={alternate.maxConsumableInventory}
              />
              <PublicStat
                label="Base equipment capacity"
                value={alternate.maxEquippableInventory}
              />
            </dl>
            <Accordion title={`Base spells (${alternate.spells.length})`}>
              {alternate.spells.length ? (
                alternate.spells.map((spell) => (
                  <SpellCard key={spell.id} spell={spell} />
                ))
              ) : (
                <EmptyCollection label="No base spells." />
              )}
            </Accordion>
          </>
        ) : (
          <EmptyCollection label="No alternate character assigned." />
        )}
      </Accordion>
    </Accordion>
  );
}

function PublicCharacterView({
  character,
}: {
  character: PublicPlaythroughCharacter;
}) {
  const imageUrl = character.portraitUrl?.trim() || character.photoUrl?.trim();
  const consumableGroups = groupPublicConsumables(character.consumableItems);

  return (
    <article className="w-full overflow-hidden rounded-3xl bg-surface/75 backdrop-blur-[2px]">
      <div className="grid lg:grid-cols-[minmax(17rem,2fr)_minmax(0,3fr)]">
        <div className="flex items-center justify-center bg-canvas/80 p-4">
          {imageUrl ? (
            <img
              src={imageUrl}
              alt=""
              className="w-1/2 object-contain rounded-xl"
            />
          ) : (
            <span className="text-content-muted">No image</span>
          )}
        </div>

        <div className="p-6 sm:p-8">
          <p className="text-sm font-semibold uppercase tracking-wider text-utility-hover">
            {character.isSceneCharacter
              ? `Active scene ${getCharacterTypeLabel(character.characterType)}`
              : "Journey character"}
          </p>
          <h2 className="mt-1 text-4xl font-semibold text-content sm:text-5xl">
            {character.name}
          </h2>
          {character.description && (
            <p className="mt-4 text-content-secondary">
              {character.description}
            </p>
          )}

          <div className="mt-6 flex flex-wrap gap-2">
            {!character.isActive && <StatusBadge label="Inactive" />}
            {character.isDown && <StatusBadge label="Down" danger />}
            {character.isDead && <StatusBadge label="Dead" danger />}
            {character.isInAlternateForm && (
              <StatusBadge label="Alternate form" />
            )}
          </div>

          <dl className="mt-7 grid grid-cols-2 gap-3 sm:grid-cols-3">
            <PublicStat
              label="HP"
              value={`${character.currentHp} / ${character.maxHp}`}
            />
            <PublicStat
              label="MP"
              value={`${character.currentMp} / ${character.maxMp}`}
            />
            <PublicStat label="Movement" value={character.movement} />
            <PublicStat
              label="Melee"
              value={character.meleeAttackDamage ?? "—"}
            />
            <PublicStat label="Bow" value={character.bowAttackDamage ?? "—"} />
            <PublicStat
              label="Inventory"
              value={`${character.maxConsumableInventory} / ${character.maxEquippableInventory}`}
            />
          </dl>
        </div>
      </div>

      <div className="space-y-8 border-t border-border p-6 sm:p-8">
        <PublicSection title={`Spells (${character.spells.length})`}>
          {character.spells.length > 0 ? (
            <div className="grid gap-4 md:grid-cols-2">
              {character.spells.map((spell) => (
                <SpellCard key={spell.id} spell={spell} />
              ))}
            </div>
          ) : (
            <EmptyCollection label="No spells available." />
          )}
        </PublicSection>

        <PublicSection
          title={`Equipment (${character.equippableItems.length})`}
        >
          {character.equippableItems.length > 0 ? (
            <div className="grid gap-4 md:grid-cols-2">
              {character.equippableItems.map((item) => (
                <EquipmentCard key={item.id} item={item} />
              ))}
            </div>
          ) : (
            <EmptyCollection label="No equipment carried." />
          )}
        </PublicSection>

        <PublicSection
          title={`Consumables (${character.consumableItems.length})`}
        >
          {consumableGroups.length > 0 ? (
            <div className="grid gap-4 md:grid-cols-2">
              {consumableGroups.map(({ item, quantity }) => (
                <ConsumableCard
                  key={`${item.id}-${item.isUsed ? "used" : "available"}`}
                  item={item}
                  quantity={quantity}
                />
              ))}
            </div>
          ) : (
            <EmptyCollection label="No consumables carried." />
          )}
        </PublicSection>
      </div>
    </article>
  );
}

function PublicEventLogView({
  snapshot,
}: {
  snapshot: PublicPlaythroughSnapshot;
}) {
  return (
    <section className="min-h-full w-full rounded-3xl bg-surface/75 p-6 backdrop-blur-[2px] sm:p-8">
      <p className="text-sm font-semibold uppercase tracking-wider text-utility-hover">
        Live playthrough
      </p>
      <h2 className="mt-1 text-4xl font-semibold text-content sm:text-5xl">
        Event Log
      </h2>
      {snapshot.eventLogs.length === 0 ? (
        <p className="mt-6 text-content-muted">No events have been recorded.</p>
      ) : (
        <ol className="mt-7 space-y-4">
          {snapshot.eventLogs.map((eventLog) => (
            <li
              key={eventLog.id}
              className="rounded-2xl border border-border bg-surface/80 p-4"
            >
              <p className="font-semibold text-content">{eventLog.message}</p>
              <time className="mt-1 block text-sm text-content-muted">
                {formatDate(eventLog.eventTime)}
              </time>
            </li>
          ))}
        </ol>
      )}
    </section>
  );
}

function SpellCard({ spell }: { spell: PublicPlaythroughSpell }) {
  return (
    <ItemCard
      imageUrl={spell.photoUrl}
      name={spell.name}
      description={spell.description}
    >
      <div className="flex flex-wrap gap-2 text-xs text-content-secondary">
        <ItemValue label="Type" value={spell.spellType} />
        <ItemValue label="MP" value={spell.mpCost} />
        <ItemValue label="Range" value={spell.range} />
        {spell.damageEffect !== null && (
          <ItemValue label="Damage" value={spell.damageEffect} />
        )}
        {spell.healthEffect !== null && (
          <ItemValue label="Health" value={spell.healthEffect} />
        )}
        {spell.magicEffect !== null && (
          <ItemValue label="Magic" value={spell.magicEffect} />
        )}
        {spell.isRadius && <ItemValue label="Area" value="Radius" />}
      </div>
    </ItemCard>
  );
}

function EquipmentCard({ item }: { item: PublicPlaythroughEquippableItem }) {
  const modifiers = [
    ["Melee", item.meleeAttackDamageModifier],
    ["Bow", item.bowAttackDamageModifier],
    ["Movement", item.movementModifier],
    ["Max HP", item.maxHpModifier],
    ["Max MP", item.maxMpModifier],
    ["Consumable slots", item.maxConsumableInventoryModifier],
    ["Equipment slots", item.maxEquippableInventoryModifier],
    ["Additional attacks", item.additionalAttacksPerTurn],
    ["Melee reduction", item.meleeDamageReduction],
    ["Bow reduction", item.bowDamageReduction],
    ["Spell reduction", item.spellDamageReduction],
  ] as const;

  return (
    <ItemCard
      imageUrl={item.photoUrl}
      name={item.name}
      description={item.description}
    >
      <div className="flex flex-wrap gap-2 text-xs text-content-secondary">
        <ItemValue label="Status" value="Active" />
        {modifiers
          .filter(([, value]) => value !== 0)
          .map(([label, value]) => (
            <ItemValue
              key={label}
              label={label}
              value={formatModifier(value)}
            />
          ))}
        {item.affectedSpellType && (
          <ItemValue label="Spell type" value={item.affectedSpellType} />
        )}
        {item.spellDamageModifier !== null &&
          item.spellDamageModifier !== 0 && (
            <ItemValue
              label="Spell damage"
              value={formatModifier(item.spellDamageModifier)}
            />
          )}
      </div>
      {item.addedSpells.length > 0 && (
        <p className="mt-3 text-sm text-content-secondary">
          Adds {item.addedSpells.map((spell) => spell.name).join(", ")}
        </p>
      )}
    </ItemCard>
  );
}

function ConsumableCard({
  item,
  quantity,
}: {
  item: PublicPlaythroughConsumableItem;
  quantity: number;
}) {
  return (
    <ItemCard
      imageUrl={item.photoUrl}
      name={item.name}
      description={item.description}
    >
      <div className="flex flex-wrap gap-2 text-xs text-content-secondary">
        {quantity > 1 && <ItemValue label="Quantity" value={`×${quantity}`} />}
        <ItemValue label="Status" value={item.isUsed ? "Used" : "Available"} />
        {item.hpEffect !== 0 && (
          <ItemValue label="HP" value={formatModifier(item.hpEffect)} />
        )}
        {item.mpEffect !== 0 && (
          <ItemValue label="MP" value={formatModifier(item.mpEffect)} />
        )}
      </div>
    </ItemCard>
  );
}

function groupPublicConsumables(items: PublicPlaythroughConsumableItem[]) {
  const groups = new Map<
    string,
    { item: PublicPlaythroughConsumableItem; quantity: number }
  >();

  for (const item of items) {
    const key = `${item.id}:${item.isUsed}`;
    const existing = groups.get(key);
    if (existing) {
      existing.quantity += 1;
    } else {
      groups.set(key, { item, quantity: 1 });
    }
  }

  return Array.from(groups.values());
}

function ItemCard({
  imageUrl,
  name,
  description,
  children,
}: {
  imageUrl: string | null;
  name: string;
  description: string;
  children: ReactNode;
}) {
  return (
    <article className="overflow-hidden rounded-2xl border border-border bg-surface/80">
      <div className="flex min-h-36">
        <div className="flex w-1/3 shrink-0 items-center justify-center bg-canvas/70 p-2">
          {imageUrl ? (
            <img
              src={imageUrl}
              alt=""
              className="max-h-40 w-full object-contain"
            />
          ) : (
            <span className="text-xs text-content-muted">No image</span>
          )}
        </div>
        <div className="min-w-0 flex-1 p-4">
          <h4 className="text-xl font-semibold text-content">{name}</h4>
          {description && (
            <p className="mt-1 text-sm text-content-secondary">{description}</p>
          )}
          <div className="mt-4">{children}</div>
        </div>
      </div>
    </article>
  );
}

function PublicSection({
  title,
  children,
}: {
  title: string;
  children: ReactNode;
}) {
  return (
    <section>
      <h3 className="mb-4 text-2xl font-semibold text-content">{title}</h3>
      {children}
    </section>
  );
}

function PublicStat({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) {
  return (
    <div className="rounded-xl bg-surface/80 p-3">
      <dt className="text-xs uppercase tracking-wide text-content-muted">
        {label}
      </dt>
      <dd className="mt-1 text-lg font-semibold text-content">{value}</dd>
    </div>
  );
}

function ItemValue({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) {
  return (
    <span className="rounded-full border border-border px-2.5 py-1">
      {label}: {value}
    </span>
  );
}

function StatusBadge({
  label,
  danger = false,
}: {
  label: string;
  danger?: boolean;
}) {
  return (
    <span
      className={`rounded-full border px-3 py-1 text-xs font-semibold ${
        danger
          ? "border-danger text-danger"
          : "border-border text-content-secondary"
      }`}
    >
      {label}
    </span>
  );
}

function EmptyCollection({ label }: { label: string }) {
  return <p className="text-content-muted">{label}</p>;
}

function getCharacterTypeLabel(type: CharacterType) {
  switch (type) {
    case CharacterType.NPC:
      return "NPC";
    case CharacterType.Enemy:
      return "enemy";
    default:
      return "character";
  }
}

function formatModifier(value: number) {
  return value > 0 ? `+${value}` : value;
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}
