# 02 — Warn before overwriting an existing Profile

**What to build:** A player who creates a new character using a name that already
has a Profile is told the existing character will be lost and has to confirm before
anything is replaced. Backing out costs them nothing. This matches how buying a
weapon already warns that the current weapon will be discarded, so replacing a
character is a decision rather than a gotcha.

**Blocked by:** 01 — Save a character and continue it.

**Status:** ready-for-agent

- [ ] Entering a character name that already has a Profile produces a warning naming that character and stating it will be lost.
- [ ] The player must explicitly confirm before the existing Profile is replaced.
- [ ] Confirming replaces that Profile and leaves every other Profile untouched.
- [ ] Declining writes nothing and returns the player to the name prompt.
- [ ] Declining and then choosing a different name creates a new Profile with the original one intact.
- [ ] The warning and its confirmation prompt are added to the language data in English, Spanish, French and Italian.
- [ ] Tests cover: saving under an existing name replaces only that entry, and other entries survive.
- [ ] `dotnet build` is clean and `dotnet test` passes.
