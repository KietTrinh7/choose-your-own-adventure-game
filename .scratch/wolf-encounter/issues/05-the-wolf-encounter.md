# 05 — The wolf encounter on East

**What to build:** A wolf sometimes waits on the East path. An Encounter Roll on
arrival decides whether it appears, the same rule the Wandering Merchant follows
on South. It fights through the same combat the dragon uses, and it is markedly
weaker. Beating it or retreating from it returns the player to the adventure menu
carrying whatever wounds they took. Dying to it ends the run.

**Blocked by:** 04 — East becomes a travelable direction.

**Status:** ready-for-agent

- [ ] A wolf exists as an implementation of the shared monster type, constructed with an injected die.
- [ ] Its Strength, Agility and Health Points each roll a d10, so it can never out-roll a d20 dragon on any stat.
- [ ] Its weapon is fangs with a damage ceiling of 6, against the dragon's 12.
- [ ] An Encounter Roll on arriving East decides whether the wolf appears; otherwise the player gets the uneventful narrative from ticket 04.
- [ ] The player sees the wolf's stats before committing, the way they see the dragon's.
- [ ] The fight uses the existing combat loop: attack rounds, defense rolls, damage, and retreat.
- [ ] The player's weapon damage and their armor's Protection both apply exactly as they do against the dragon.
- [ ] Winning returns the player to the adventure menu with their wounds intact.
- [ ] Retreating returns the player to the adventure menu.
- [ ] Dying ends the run through the same zero-Health-Points path the game already has.
- [ ] No Gold, no drops, and nothing recorded about whether a wolf was met or beaten.
- [ ] The whole encounter is localized in English, Spanish, French and Italian.
- [ ] `CONTEXT.md` gains Wolf, Fangs and a Monster entry naming the shared type.
- [ ] Tests cover the wolf's rolled stats and fangs ceiling with a fixed die, an explicit assertion that a maximum-rolled wolf cannot exceed the dragon's ceiling, the Encounter Roll on both sides of its threshold, and armor Protection applying to the wolf's bite.
- [ ] `dotnet build` is clean and all tests pass.
