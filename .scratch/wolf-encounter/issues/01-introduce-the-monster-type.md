# 01 — Introduce the shared monster type

**What to build:** Nothing the player can see. A shared monster type is added
beside the existing code and the dragon is declared as one implementation of it.
Every existing caller keeps naming the dragon and keeps working, because the old
form still exists. This is the expand step: the new shape appears, nothing
migrates to it yet.

**Blocked by:** None — can start immediately.

**Status:** ready-for-agent

- [ ] A shared monster type exists carrying exactly what the combat loop needs: name, Strength, Agility, Health Points, a weapon, an attack against the player, and the taunt and damage-reply behavior the loop prints.
- [ ] The dragon declares itself as an implementation of that type and keeps every stat, taunt and reply it has today.
- [ ] Nothing outside the dragon changes. The combat loop and the player's attack still name the dragon.
- [ ] No new tests. The existing suite passing unchanged is the proof this altered no behavior.
- [ ] `dotnet build` is clean and all existing tests pass.
