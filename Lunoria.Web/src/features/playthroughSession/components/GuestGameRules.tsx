const rules = [
  {
    title: "Your turn",
    description:
      "Roll for movement, then tell the game master your result. Move your character on the board as directed. The game master handles enemy activation before you choose your action.",
  },
  {
    title: "Choose an action",
    description:
      "Use an available action such as melee, bow, casting a spell, using a consumable, trading, or transforming. The game master resolves actions in the app. You can also skip your action.",
  },
  {
    title: "Attacking and counterattacking",
    description:
      "Roll as requested and apply the character's attack modifiers. Negative modifiers can reduce damage, but damage cannot go below zero. Extra attacks from equipment stay on the original target; unused attacks are forfeited if that target falls. A surviving defender may counterattack once after the final attack. Counterattacks use normal spell costs and do not consume the defender's normal turn.",
  },
  {
    title: "Spells and MP",
    description:
      "You need enough current MP to pay a spell's cost. Dimmed spells cost more MP than you currently have. Spells can deal damage, restore HP or MP, or have other effects. Follow the spell's range, target rules, and the game master's instructions for rolls.",
  },
  {
    title: "HP and being down",
    description:
      "HP and MP cannot fall below zero. At zero HP, a character is down. Let the game master resolve recovery and any special scene rules before continuing.",
  },
  {
    title: "Equipment, consumables, and trading",
    description:
      "Equipment in your inventory can modify your stats or grant abilities. Consumables apply their listed effects when used. Respect inventory limits and confirm trades with the game master before moving items.",
  },
  {
    title: "Alternate forms",
    description:
      "A character with an alternate form can transform when that action is available. The alternate form provides melee, bow, movement, and spells; HP and MP remain shared with the base character. Characters return to their normal form when the scene ends.",
  },
  {
    title: "Campfires",
    description:
      "When your player character is within one tile of a campfire on the board, you may use your action to refill either HP or MP. Choose one; using the campfire ends your turn.",
  },
  {
    title: "Objectives and scene events",
    description:
      "Follow the current objectives and the game master's guidance. Scene events can change stats, grant rewards, change alternate forms, or enable and disable players. Check event logs for updates.",
  },
];

export function GuestGameRules() {
  return (
    <div className="space-y-6">
      <p className="text-sm text-content-secondary">
        A quick guide to playing Lunoria. The game master explains board
        positioning, dice, and any special rules for your journey.
      </p>
      {rules.map((rule) => (
        <section key={rule.title}>
          <h3 className="mb-2 text-lg font-semibold text-content">{rule.title}</h3>
          <p className="text-sm leading-relaxed text-content-secondary">
            {rule.description}
          </p>
        </section>
      ))}
    </div>
  );
}
