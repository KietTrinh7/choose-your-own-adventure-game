# Spec: Merchant Encounter

Status: done — all three tickets implemented

## Problem Statement

The South path is a dead end: the player walks into the woods, is told nothing adventurous happens, and returns bored. Half the game's map does nothing, and the player has no way to prepare for the dragon — no money to spend, nothing to buy, and no decisions to make outside of combat. The player wants the South path to matter and wants meaningful ways to strengthen (or gamble away) their character before facing the dragon.

## Solution

Traveling South now triggers an Encounter Roll: 25% of the time a Wandering Merchant appears with two one-of-a-kind magical goods — an Enchanted Sword (better damage) and Enchanted Armor (flat damage Protection). The player starts every game with 50 Gold, enough to buy exactly one item at the asking price of 30 Gold each. To afford more, the player can Sell their starting weapon (for its MaxDamage in Gold, leaving them fighting with Fists until they re-arm) or Haggle (a d20 roll against Agility for 10 Gold off — but failure insults the merchant and ends the whole interaction). Merchants are stateless: each encounter is a fresh individual, and a failed Haggle never carries over (see ADR-0002). Purchases auto-equip; the game warns before a weapon purchase discards the current weapon. Enchanted Armor reduces every dragon hit by its Protection value in combat. All new text is localized in English, Spanish, French, and Italian.

## User Stories

1. As a player, I want to start the game with 50 Gold, so that I can trade with merchants I meet.
2. As a player, I want to see my Gold in the character stats display, so that I always know what I can afford.
3. As a player traveling South, I want a 25% chance to meet a Wandering Merchant, so that finding one feels like an event rather than a guarantee.
4. As a player traveling South when no merchant appears, I want the existing uneventful-woods narrative, so that the world still behaves as before.
5. As a player meeting a merchant, I want a short greeting and a numbered shop menu, so that I understand my options at a glance.
6. As a player in the shop, I want to buy the Enchanted Sword for 30 Gold, so that I deal more damage to the dragon.
7. As a player in the shop, I want to buy the Enchanted Armor for 30 Gold, so that I take less damage from the dragon.
8. As a player buying a weapon, I want a warning that my current weapon will be discarded with no refund, so that replacing it is a decision rather than a gotcha.
9. As a player who cannot afford an item, I want the merchant to politely refuse and my Gold to stay untouched, so that I can't be tricked into a bad state.
10. As a player, I want to Sell my current weapon for its MaxDamage in Gold, so that I can raise money toward the magical items.
11. As a player who sold their weapon, I want to fight with Fists until I buy a new weapon, so that selling has a real cost.
12. As a player, I want to be unable to sell my Fists or the Enchanted Sword, so that the economy can't be exploited in a loop.
13. As a player buying an item, I want the option to Haggle before paying, so that I can gamble for a 10 Gold discount.
14. As a player who wins a Haggle (d20 roll at or under my Agility), I want the item for 20 Gold, so that a nimble character is rewarded.
15. As a player who loses a Haggle, I want the merchant to cancel everything and end the encounter, so that haggling carries real risk.
16. As a player who lost a Haggle, I want future merchants to be strangers with no memory of it, so that one bad roll doesn't poison the rest of my run.
17. As a player, I want each merchant to offer only the items I don't already own, so that I'm never sold a duplicate.
18. As a player, I want to decline to buy anything and simply leave the shop, so that visiting a merchant never forces a purchase.
19. As a player leaving a merchant (by any route), I want to return to the adventure menu, so that I can still travel North to fight the dragon.
20. As a player wearing Enchanted Armor, I want every dragon hit reduced by my Protection (never below zero damage), so that the armor visibly protects me in combat.
21. As a player wielding the Enchanted Sword, I want my damage rolls to use its higher MaxDamage, so that the upgrade matters in combat.
22. As a player, I want my Armor and its Protection shown in the stats display, so that I can see what I'm wearing.
23. As a Spanish-, French-, or Italian-speaking player, I want every merchant message in my selected language, so that the feature matches the rest of the game.

## Implementation Decisions

- **Economy invariants**: starting Gold is a fixed 50 (not rolled); both items cost 30; the Haggle discount is 10; sale price equals the sold weapon's MaxDamage (long sword 12, lightning bolt spell 12, dagger 6, long bow 8). By design, 50 buys one item; owning both requires selling and/or haggling. This interplay is deliberate — do not retune one number in isolation.
- **New `Merchant` module** owns all shop logic behind testable methods with the `Die` injected (constructor injection, matching `Dragon`): the Encounter Roll (merchant appears on a d20 result of 16–20), purchase (stock + affordability checks, Gold deduction, auto-equip), sell (Gold credit, swap to Fists), and Haggle (d20 vs. Agility; success discounts, failure ends the interaction). Methods take the `Player` and return outcome values; `Game`'s console loop only reads input, calls `Merchant`, and prints localized text.
- **`Player` gains** a `Gold` property, an `Armor` slot (empty by default), and a pure `ReduceDamage(raw)` method returning `max(0, raw − Protection)` (Protection is 0 without armor). `Dragon.Attack` applies damage to the player through `ReduceDamage`.
- **Items**: Enchanted Sword is a `Weapon` (MaxDamage 16) usable by every occupation; Enchanted Armor is a new `Armor` type with Protection 3. One of each exists from the player's perspective; ownership is tracked on the `Player`, and merchants offer only what the player lacks (per ADR-0002, merchants are stateless).
- **No inventory system**: two equipment slots, auto-equip on purchase, weapon replacement discards the old weapon after an explicit warning.
- **Shop menu is numbered** (matching the main-menu idiom), rebuilt from current state each visit: available items, the sell option (only when holding a sellable weapon), and leave.
- **Localization**: every new player-facing string is added to `language_data.json` in English, Spanish, French, and Italian (per ADR 0001's data-driven pattern). Canonical item names ("enchanted sword", "enchanted armor", "fists") get `displayWeaponMap`/`displayArmorMap` entries per language; `Messages` gains `TranslateArmorForDisplay`. The approved English greeting is: "Ah, a traveler! Fate smiles on us both. I carry rare goods — enchanted steel and armor touched by magic. Care to see my wares?"
- **Stats display** gains lines for Gold and Armor (name + Protection, or "none").

## Testing Decisions

- Tests target external behavior through the two seams (`Merchant` methods and `Player.ReduceDamage`) with a deterministic or forced `Die` — never console I/O, never private internals. `Die.Roll` becomes virtual so tests can substitute a fixed die (prior art: `Dragon` takes the `Die` via constructor).
- **Armor math**: raw 8 with Protection 3 → 5; raw 2 with Protection 3 → 0 (never negative); no armor → unchanged.
- **Buying**: deducts exactly 30 Gold and equips the item; buying with insufficient Gold is refused with no state change.
- **Selling**: credits the weapon's MaxDamage in Gold and leaves Fists (MaxDamage 4); no sell option when holding Fists or the Enchanted Sword.
- **Haggling**: forced success → price paid is 20; forced failure → no purchase, no Gold change, interaction reported as ended.
- **Encounter Roll**: d20 results 16–20 produce a merchant; 1–15 do not.
- **Localization**: every new dictionary key resolves in all four languages (`Messages.GetMessage` returns `[key]` for missing keys, so a bracket check catches gaps).

## Out of Scope

- Inventory system beyond the two equipment slots
- Merchant memory, reputation, restocking, or buying back sold goods (see ADR-0002)
- Additional merchant stock, potions, or consumables
- Gold rewards from combat or exploration
