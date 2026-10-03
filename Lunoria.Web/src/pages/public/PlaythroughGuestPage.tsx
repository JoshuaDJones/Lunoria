import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from "react";
import { GuestEventToast } from "@/features/playthroughSession/components/GuestEventToast";
import type { PublicPlaythroughEventLog } from "@/features/playthroughSession/types";
import { useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { Drawer } from "@/components/ui";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
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
import { SceneObjectivesPanel } from "@/features/sceneplaythrough/components/SceneObjectivesPanel";
import { GuestCardDeck } from "@/features/playthroughSession/components/GuestCardDeck";

type DeckEntry = {
  key: string;
  character: PublicPlaythroughCharacter;
};

export function PlaythroughGuestPage() {
  const { token = "" } = useParams<{ token: string }>();
  const [snapshot, setSnapshot] = useState<PublicPlaythroughSnapshot>();
  const [selectedKey, setSelectedKey] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [isRealtimeConnected, setIsRealtimeConnected] = useState(false);
  const [isClosed, setIsClosed] = useState(false);
  const [showEventLogs, setShowEventLogs] = useState(false);
  const logsButtonRef = useRef<HTMLButtonElement>(null);
  const closeEventLogs = useCallback(() => {
    setShowEventLogs(false);
    logsButtonRef.current?.focus();
  }, []);
  const [eventQueue, setEventQueue] = useState<{
    token: string;
    events: PublicPlaythroughEventLog[];
  }>({ token, events: [] });
  const activeEvent =
    eventQueue.token === token ? eventQueue.events[0] : undefined;
  const dismissEvent = useCallback(
    (id: number) => {
      setEventQueue((current) =>
        current.token === token
          ? {
              ...current,
              events: current.events.filter((event) => event.id !== id),
            }
          : current,
      );
    },
    [token],
  );

  const entries = useMemo<DeckEntry[]>(() => {
    if (!snapshot) return [];
    return [
      ...snapshot.journeyCharacters.map((character) => ({
        key: `journey:${character.id}`,
        character,
      })),
      ...snapshot.sceneCharacters.map((character) => ({
        key: `scene:${character.id}`,
        character,
      })),
    ];
  }, [snapshot]);

  const selectedIndex = Math.max(
    0,
    entries.findIndex((entry) => entry.key === selectedKey),
  );

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
    let latestRefresh = 0;
    let seenEventIds: Set<number> | undefined;

    const refresh = async () => {
      const refreshId = ++latestRefresh;
      try {
        const nextSnapshot = await getPublicPlaythroughSnapshot(token);
        if (isCurrent && refreshId === latestRefresh) {
          if (seenEventIds) {
            const newEvents = nextSnapshot.eventLogs
              .filter((event) => !seenEventIds!.has(event.id))
              .sort(
                (a, b) =>
                  Date.parse(a.eventTime) - Date.parse(b.eventTime) ||
                  a.id - b.id,
              );
            nextSnapshot.eventLogs.forEach((event) =>
              seenEventIds!.add(event.id),
            );
            if (newEvents.length > 0) {
              setEventQueue((current) => ({
                token,
                events: [
                  ...(current.token === token ? current.events : []),
                  ...newEvents,
                ],
              }));
            }
          } else {
            // Joining establishes a baseline; never replay the existing event history.
            seenEventIds = new Set(
              nextSnapshot.eventLogs.map((event) => event.id),
            );
          }
          setSnapshot(nextSnapshot);
          setError("");
        }
      } catch (requestError: unknown) {
        if (isCurrent && refreshId === latestRefresh)
          setError(getApiError(requestError).message);
      } finally {
        if (isCurrent && refreshId === latestRefresh) setIsLoading(false);
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
        if (isCurrent) {
          setIsRealtimeConnected(true);
          // Catch objective changes between the initial fetch and joining the group.
          void refresh();
        }
      })
      .catch(() => {
        if (isCurrent) setIsRealtimeConnected(false);
      });

    return () => {
      isCurrent = false;
      void connection.stop();
    };
  }, [token]);

  return (
    <AppLayout
      sidebar={<></>}
      bottomPadding
      fixedViewport
      background={
        <GuestSceneBackground
          key={snapshot?.activeScenePhotoUrl?.trim() ?? ""}
          photoUrl={snapshot?.activeScenePhotoUrl?.trim()}
        />
      }
    >
      <main className="flex min-h-0 w-full flex-1 flex-col overflow-hidden">
        {!isClosed && activeEvent && (
          <GuestEventToast
            key={`${token}:${activeEvent.id}`}
            event={activeEvent}
            onDismiss={dismissEvent}
          />
        )}
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

        {!isLoading && !error && !isClosed && snapshot && (
          <>
            <h1 className="sr-only">{snapshot.name}</h1>
            <span className="sr-only" role="status">
              {isRealtimeConnected ? "Live" : "Reconnecting"}
            </span>
            <button
              ref={logsButtonRef}
              type="button"
              aria-label="Show event logs"
              aria-haspopup="dialog"
              aria-expanded={showEventLogs}
              title="Event logs"
              onClick={() => setShowEventLogs(true)}
              className="fixed right-0 top-[env(safe-area-inset-top)] z-40 flex h-11 w-11 items-center justify-center rounded-full bg-transparent text-content drop-shadow-md hover:text-utility-hover focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility"
            >
              <FontAwesomeIcon icon={faCircleInfo} className="h-5 w-5" />
            </button>

            {entries.length === 0 ? (
              <div className="p-6">
                <EmptyCollection label="No characters available." />
              </div>
            ) : (
              <GuestCardDeck
                key={token}
                selectedIndex={selectedIndex}
                onSelect={setSelectedKey}
                cards={entries.map((entry) => ({
                  key: entry.key,
                  label: entry.character.name,
                  imageUrl:
                    entry.character.portraitUrl?.trim() ||
                    entry.character.photoUrl?.trim(),
                  content: <PublicCharacterView character={entry.character} />,
                }))}
              />
            )}
            {showEventLogs && (
              <Drawer title="Event Logs" onClose={closeEventLogs}>
                <PublicEventLogView snapshot={snapshot} />
              </Drawer>
            )}
            <SceneObjectivesPanel
              aboveNavigation={entries.length > 0}
              key={snapshot.activeSceneId}
              objective={snapshot.currentObjective}
            />
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
              className="w-2/5 object-contain rounded-xl"
            />
          ) : (
            <span className="text-content-muted">No image</span>
          )}
        </div>

        <div className="p-6 sm:p-8">
          <h2 className="text-2xl font-semibold text-content sm:text-3xl">
            {character.name}
          </h2>
          <p className="break-words text-content-secondary">
            <span className="font-semibold text-utility-hover">
              {character.isSceneCharacter
                ? getCharacterTypeLabel(character.characterType)
                : "Player"}
            </span>
            {character.description && ` - ${character.description}`}
          </p>

          <div className="flex flex-wrap gap-2">
            {!character.isActive && <StatusBadge label="Inactive" />}
            {character.isDown && <StatusBadge label="Down" danger />}
            {character.isDead && <StatusBadge label="Dead" danger />}
          </div>

          <dl className="mt-2 grid grid-cols-2 gap-3 sm:grid-cols-3">
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
          </dl>
        </div>
      </div>

      <div className="space-y-8 border-t border-border p-6 sm:p-8">
        <PublicSection title="Spells">
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

        <PublicSection title="Equipment">
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

        <PublicSection title="Consumables">
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
    <section>
      {snapshot.eventLogs.length === 0 ? (
        <p className="text-content-muted">No events have been recorded.</p>
      ) : (
        <ol className="space-y-4">
          {[...snapshot.eventLogs]
            .sort(
              (a, b) =>
                Date.parse(b.eventTime) - Date.parse(a.eventTime) ||
                b.id - a.id,
            )
            .map((eventLog) => (
              <li
                key={eventLog.id}
                className="rounded-2xl border border-border bg-surface/80 p-4"
              >
                <p className="break-words font-semibold text-content">
                  {eventLog.message}
                </p>
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
      imageAbove
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
      imageAbove
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
      imageAbove
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
  imageAbove = false,
  name,
  description,
  children,
}: {
  imageUrl: string | null;
  imageAbove?: boolean;
  name: string;
  description: string;
  children: ReactNode;
}) {
  return (
    <article className="overflow-hidden rounded-2xl border border-border bg-surface/80">
      <div className={`flex min-h-36 ${imageAbove ? "flex-col" : ""}`}>
        <div
          className={`flex shrink-0 items-center justify-center bg-canvas/70 p-2 ${imageAbove ? "w-full" : "w-1/3"}`}
        >
          {imageUrl ? (
            <img
              src={imageUrl}
              alt=""
              className={`${imageAbove ? "max-h-20 w-1/2" : "max-h-40 w-full"} object-contain`}
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
      return "Enemy";
    default:
      return "Player";
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
