# Spec: Wolf Encounter

Status: ready-for-agent

## Problem Statement

The map has two directions and both are terminal in their own way. North is the
dragon, which ends the run in every outcome. South is the Wandering Merchant,
which is a shop rather than a test of the character the player just rolled. There
is nothing in the game that asks whether a character can actually fight until the
moment that fight is also the last thing they will ever do. A player who buys the
Enchanted Sword has no way to find out whether it was worth the Gold until the run
is already over, and a player who wants to know whether their Halfling archer can
survive anything has to bet the whole run to find out.

## Solution

East becomes a real direction, and a wolf lives there. An Encounter Roll on
arrival decides whether it appears, the same rule that governs the Wandering
Merchant on South, so the player learns one rule for both wilderness directions.

The wolf is a genuine fight through the same combat the dragon uses: attack
rounds, defense rolls, damage, and the option to retreat. It is markedly weaker
than the dragon. Its Strength, Agility and Health Points roll a d10 against the
dragon's d20, and its fangs do at most 6 against the dragon's 12.

The difference that matters is what winning means. Beating the wolf returns the
player to the adventure menu carrying whatever wounds they took, so the encounter
is an obstacle rather than an ending. Dying to it ends the run, exactly as zero
Health Points already does. Retreating returns them to the menu.

There is no reward. East costs Health Points and gives nothing back, which leaves
the merchant economy exactly as Part 3 tuned it and makes travelling East an
honest test rather than a way to farm.

Supporting this requires one preparatory change with no behavior of its own. The
combat loop and the player's attack are currently written against the dragon
specifically, so a shared monster type is extracted first, with the dragon as one
implementation and the wolf as another.

## User Stories

1. As a player, I want East to be a direction I can travel, so that the map is more than a shop and a boss.
2. As a player, I want the path prompt to tell me East exists, so that I can discover it without guessing.
3. As a player entering East, I want an Encounter Roll to decide whether anything is there, so that the direction behaves like South rather than like a scripted event.
4. As a player travelling East when nothing appears, I want a short uneventful narrative, so that the trip still reads as part of the world.
5. As a player meeting the wolf, I want to see its stats before I commit, so that I can judge the fight the way I judge the dragon.
6. As a player fighting the wolf, I want the same attack and defense exchanges the dragon fight uses, so that I am not learning a second combat system.
7. As a player fighting the wolf, I want my weapon's damage to matter, so that the Enchanted Sword is worth the Gold I spent.
8. As a player wearing Enchanted Armor, I want its Protection to reduce the wolf's bite, so that armor works the same everywhere.
9. As a player who beats the wolf, I want to return to the adventure menu, so that surviving means I get to keep playing.
10. As a player who beats the wolf, I want to keep the wounds I took, so that the fight has a lasting cost.
11. As a player who beats the wolf, I want my progress saved as usual when I land back on the menu, so that a hard-won survival is not lost.
12. As a player killed by the wolf, I want the run to end the way it does at zero Health Points anywhere else, so that death is consistent.
13. As a player who wants out, I want to retreat from the wolf, so that starting a fight is not the same as finishing it.
14. As a player who retreats, I want to return to the adventure menu, so that retreating is survival rather than surrender of the run.
15. As a player, I want the wolf to be clearly weaker than the dragon, so that East is a test and North is the ending.
16. As a player, I want the wolf's stats rolled rather than fixed, so that no two wolves are identical.
17. As a player, I want the wolf never to be tougher than the dragon, so that the difficulty order of the map is trustworthy.
18. As a player, I want no Gold from the wolf, so that the merchant's prices still mean what they meant before.
19. As a player who goes East repeatedly, I want each trip to be a fresh gamble, so that the direction is a risk rather than a guaranteed toll.
20. As a Spanish-, French- or Italian-speaking player, I want the whole encounter localized, so that East matches the rest of the game.
21. As a developer, I want the combat loop written against a monster rather than the dragon specifically, so that adding a third monster later costs almost nothing.
22. As a developer, I want the existing tests to stay green through the preparatory change, so that I can prove it altered no behavior.

## Implementation Decisions

- **A shared monster type is extracted first**, in its own preparatory step with no
  behavior change. The combat loop and the player's attack stop naming the dragon
  and take the shared type instead. The dragon becomes one implementation of it
  and keeps every stat, taunt and reply it has today. This is prefactoring: make
  the change easy, then make the easy change.
- **The shared type carries what combat actually needs**: a name, Strength,
  Agility, Health Points, a weapon, an attack against the player, and the taunt
  and damage-reply behavior the loop already prints. Anything only the dragon has
  stays on the dragon.
- **The wolf is a second implementation**, constructed with an injected `Die` the
  same way the dragon and the Wandering Merchant are. Its Strength, Agility and
  Health Points each roll a d10; its weapon is fangs with a damage ceiling of 6.
  A d10 guarantees it can never out-roll a d20 dragon on any stat.
- **The Encounter Roll governs East**, reusing the domain term rather than
  inventing a second concept. A roll decides whether the wolf appears; otherwise
  the player gets an uneventful narrative and returns to the adventure menu.
- **East is added to path validation and the path prompt** alongside North, South
  and exit, accepting both the full word and its first letter, matching how the
  existing directions are parsed.
- **Winning or retreating returns to the adventure menu.** Only death ends the
  run, and it ends it through the same zero-Health-Points path the game already
  has. This is the one place the wolf deliberately differs from the dragon, whose
  encounter ends the run in every outcome.
- **No Gold, no drops, no state.** Nothing is recorded about whether a wolf was
  met or beaten. The encounter leaves its mark only in the player's Health Points,
  which the Profile already persists.
- **Every new player-facing string** is added to the language data in English,
  Spanish, French and Italian per ADR-0001: the East path option, the uneventful
  narrative, the wolf's appearance and stats introduction, its taunts and replies,
  and the outcome lines.
- **`CONTEXT.md` gains Wolf and Fangs** in the existing glossary style, and a
  Monster entry naming the shared type.

## Testing Decisions

- No new seam types. The wolf is tested exactly where the dragon already is:
  constructed with a fixed `Die`, its rolled stats and its attack asserted
  directly. Its Encounter Roll is tested the way the Wandering Merchant's already
  is. Prior art for both is in the existing dragon and merchant tests.
- **The preparatory change gets no tests of its own.** If the extraction is
  correct the existing suite stays green untouched, and that is the proof it
  changed nothing. A test written against the new shared type at that stage would
  only assert the shape of a refactor.
- **Wolf stats**: with a fixed die, Strength, Agility and Health Points all come
  from a d10 and the weapon's damage ceiling is 6.
- **Relative difficulty**: a wolf rolled on the highest possible d10 is still no
  stronger than a dragon rolled on the lowest comparable d20 result would need to
  be, asserted as an explicit ceiling rather than left to chance.
- **Encounter Roll**: forced rolls produce a wolf on the appearing range and
  nothing on the rest, mirroring the merchant's encounter tests.
- **Armor still applies**: a player wearing Enchanted Armor takes the wolf's bite
  reduced by Protection, asserted through the existing damage-reduction method.
- **Localization**: every new key resolves in all four languages, caught by the
  bracket check the message lookup already provides for missing keys.

## Out of Scope

- Gold, items or any other reward from the wolf.
- Remembering that a wolf was met, beaten or fled from, in a Profile or anywhere else.
- A third monster, or West as a direction.
- Healing, potions or any way to recover Health Points.
- Changing the dragon's stats, behavior or run-ending outcome.
- Fleeing that costs something, or pursuit after a retreat.

## Further Notes

- The dragon is still constructed once at start-up rather than when the player
  travels North. This spec does not depend on changing that, but the shared
  monster type makes moving it later much cheaper, and it remains a candidate for
  the separate refactoring step.
- Extracting the shared type touches the combat loop, the player's attack, the
  dragon and the game loop at once. The blast radius is small enough to land in a
  single preparatory ticket rather than an expand-and-contract sequence, and the
  existing suite is the safety net.
