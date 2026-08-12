# Spec: Healing Potion

Status: done — all three tickets implemented

## Problem Statement

Nothing in this game heals. Health Points are rolled once at character creation
and from that moment only ever go down. A player who meets the wolf on the East
path and wins pays for it permanently, which means the correct way to play is to
avoid the only optional fight in the game. Once both magical items are bought the
Wandering Merchant has nothing left to sell, so the South path stops mattering
too. And the dragon at the end is fought with whatever health happens to be left
over, with no way to prepare.

## Solution

The Wandering Merchant sells a Healing Potion for 10 Gold. Unlike the Enchanted
Sword and the Enchanted Armor it is ordinary stock: the merchant will sell as many
as the player can afford, so the shop stays useful for the whole run.

A character now remembers the Health Points they rolled at creation as their
Maximum Health. A potion restores a d10 roll of Health Points and never takes
them above that maximum, so it is restoration rather than improvement. Nothing in
the game can make a character stronger than the one that was rolled.

Potions are drunk in combat, and drinking costs the turn. The attack-or-retreat
prompt becomes attack, drink or retreat; choosing to drink means no swing that
round, and the monster still attacks. That makes each potion a decision rather
than a button: heal now and take a hit, or gamble on ending the fight first.

The starting 50 Gold now buys one magical item and two potions, or five potions
and no item. The 30 Gold items stay the serious purchase.

## User Stories

1. As a player, I want a way to recover Health Points, so that surviving a fight is not permanently punishing.
2. As a player, I want the wolf on the East path to be worth fighting, so that the optional encounter is a choice rather than a trap.
3. As a player, I want the Wandering Merchant to still have something to sell after I own both magical items, so that the South path keeps mattering.
4. As a player, I want to buy a Healing Potion for 10 Gold, so that a potion is affordable without abandoning the magical items.
5. As a player, I want to buy more than one potion, so that stocking up before the dragon is possible.
6. As a player, I want the merchant to keep offering potions however many I own, so that they are ordinary stock rather than one-of-a-kind.
7. As a player who cannot afford a potion, I want the merchant to refuse politely and my Gold left untouched, so that I cannot be tricked into a bad state.
8. As a player, I want to Haggle over a potion the way I can over the other goods, so that the shop behaves consistently.
9. As a player, I want to see how many potions I am carrying in my stats, so that I can plan.
10. As a player in combat, I want to drink a potion as an alternative to attacking, so that a fight going badly is recoverable.
11. As a player who drinks, I want to lose my attack that round and still be attacked, so that healing is a real trade rather than a free action.
12. As a player, I want each potion to restore a rolled amount, so that drinking carries the same gamble as everything else in this game.
13. As a player, I want healing to stop at the Health Points I rolled at creation, so that I understand what full health means.
14. As a player at full health, I want drinking to be refused rather than wasting a potion, so that a misclick does not cost me 10 Gold.
15. As a player with no potions, I want no drink option offered at all, so that the prompt only shows what I can actually do.
16. As a player, I want my potions and my maximum health saved with my Profile, so that closing the game does not cost me what I bought.
17. As a player resuming a Profile, I want my maximum health to be the one my character rolled, so that a reloaded character heals to the right ceiling.
18. As a Spanish-, French- or Italian-speaking player, I want the whole feature localized, so that it matches the rest of the game.

## Implementation Decisions

- **`Player` gains a Maximum Health**, set to the Health Points rolled during
  character creation and never changed afterwards. Healing is clamped to it.
  Nothing else in the game reads or modifies it.
- **`Player` gains a potion count**, a plain integer with no explicit limit. Gold
  is the only constraint. This is deliberately not an inventory: Part 3 chose two
  equipment slots and no inventory system, and a consumable count does not
  reopen that.
- **Both new values persist in the Profile**, taking a saved character from nine
  fields to eleven. They serialise as plain integers with no special handling.
- **Drinking lives on `Player`** as a method taking the die roll and returning
  what happened: healed by some amount, refused because already at maximum, or
  refused because none are carried. It decrements the count only when it heals.
  Same shape as the existing damage reduction method, which is why it is testable
  the same way.
- **`Merchant` gains a potion purchase** alongside the existing two, following the
  same outcome vocabulary: purchased, or insufficient Gold. It differs from the
  other two in that the merchant always offers it regardless of what the player
  already owns, because it is ordinary stock rather than one-of-a-kind.
- **The potion price is 10 Gold**, a third of a magical item. The Haggle discount
  applies to it exactly as it applies to the other goods, so a successful haggle
  buys one for nothing less than the existing discount allows.
- **The shop menu gains a potion line**, built from current state the way the
  existing options already are, so it slots into the dynamic numbering with no new
  mechanism.
- **The combat prompt becomes three-way**, offering drink only when the player is
  carrying at least one potion. The prompt module added in the refactor makes this
  a matter of choosing which alias set to pass.
- **Drinking consumes the round.** The player does not attack, and the monster's
  attack sequence runs as normal immediately afterwards.
- **The stats display gains a potion count line**, alongside Gold and Armor.
- **New player-facing strings** go into the language data in English, Spanish,
  French and Italian per ADR-0001: the shop option, the purchase confirmation, the
  three-way combat prompt, the healing result, the refusals at full health and
  with none carried, and the stats line.
- **`CONTEXT.md` gains Healing Potion and Maximum Health.**

## Testing Decisions

- No new seam types. Drinking is tested on `Player` with a supplied roll, the way
  the existing damage reduction method is tested as a pure function. Buying is
  tested on `Merchant` alongside the existing purchase methods with an injected
  die. Prior art for both is already in the suite.
- **Healing maths**: a character at 5 of a maximum 20 healed by 7 reaches 12; the
  same character healed by 30 reaches 20 and not 35; a character already at their
  maximum is refused and keeps the potion.
- **Consumption**: a successful drink decrements the count by exactly one; a
  refused drink decrements nothing.
- **No potions**: drinking with none carried is refused and changes nothing.
- **Buying**: purchasing deducts exactly the price and increments the count;
  buying without enough Gold is refused with no state change; buying repeatedly
  keeps working, which is what distinguishes a potion from the one-of-a-kind
  goods.
- **Merchant stock**: the potion is offered whether the player owns none or many,
  unlike the sword and armor.
- **Profile round trip**: a saved and reloaded character keeps its potion count and
  its maximum health, and heals to the right ceiling afterwards.
- **Localization**: every new key resolves in all four languages, caught by the
  bracket check the message lookup already provides.

## Out of Scope

- Any potion that does anything other than restore Health Points.
- Finding potions while travelling, or receiving them from anything but the
  merchant.
- A carry limit, an inventory system, or equipment slots of any kind.
- Drinking outside combat.
- Healing the dragon or the wolf.
- Raising Maximum Health by any means.

## Further Notes

- Part 3's spec listed "additional merchant stock, potions, or consumables" as out
  of scope. This spec deliberately reopens that, and the reason is the gap the wolf
  exposed: an optional fight in a game with no healing is a fight the player is
  better off declining.
- Maximum Health is the first value on the character that is set once and then
  never changes. If a future feature ever needs to raise it, this is the field it
  would touch.
