# 03 — The combat loop holds a monster rather than the dragon

**What to build:** The combat loop stops naming the dragon and holds the shared
monster type. The game still constructs a dragon and hands it over, so the dragon
fight is unchanged in every respect except one: the stats display now labels the
opponent by its own name instead of saying "Dragon". Second and final migrate
step, after which combat can host any monster.

**Blocked by:** 02 — The player attacks a monster rather than the dragon.

**Status:** ready-for-agent

- [ ] The combat loop accepts and holds the shared monster type rather than the dragon specifically.
- [ ] The game still constructs a dragon for the North path and the fight behaves as it always has.
- [ ] The combat stats display labels the opponent using its name rather than a hard-coded dragon heading, and the heading string is localized in all four languages.
- [ ] The defeat narrative moves onto the shared monster type so each creature supplies its own, and the dragon keeps the exact line it prints today.
- [ ] Attack rounds, defense rolls, retreat and the win and loss outcomes are all unchanged.
- [ ] Existing combat tests are updated only where they assert the old hard-coded heading, and nowhere else.
- [ ] `dotnet build` is clean and all tests pass.
