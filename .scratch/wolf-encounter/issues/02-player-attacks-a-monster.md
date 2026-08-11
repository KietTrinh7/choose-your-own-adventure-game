# 02 — The player attacks a monster rather than the dragon

**What to build:** Still nothing the player can see. The player's attack stops
naming the dragon and takes the shared monster type instead. Every existing call
site keeps compiling untouched, because a dragon is a monster. First migrate step.

**Blocked by:** 01 — Introduce the shared monster type.

**Status:** ready-for-agent

- [ ] The player's attack accepts the shared monster type rather than the dragon specifically.
- [ ] Callers that pass a dragon are unchanged and still compile.
- [ ] The dragon fight plays out identically: same attack rolls, defense rolls, taunts, damage and defeat narrative.
- [ ] No new tests. Existing player and combat tests stay green untouched.
- [ ] `dotnet build` is clean and all existing tests pass.
