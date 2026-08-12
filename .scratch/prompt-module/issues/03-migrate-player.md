# 03 — Migrate Player's six prompts

**What to build:** The six loops in `Player` are replaced by calls to `Prompt`:
race, name, occupation, the three roll confirmations, the next-action menu, and the
overwrite confirmation. Character creation behaves exactly as it does today.

**Blocked by:** 02 — Migrate Game's nine prompts.

**Status:** ready-for-agent

- [ ] Every `Console.ReadLine` in `Player` is gone, replaced by a `Prompt` call.
- [ ] The yes-or-no helper in `Player` is gone, replaced by an alias call, leaving one such helper in the codebase rather than two.
- [ ] A character name is still stored with the capitalisation the player typed.
- [ ] Race and occupation still validate and normalise through `Messages` exactly as they do now.
- [ ] The Profile overwrite warning still requires confirmation and still returns to the name prompt when declined.
- [ ] No new tests. The existing suite staying green is the proof.
- [ ] `dotnet build` is clean and all tests pass.
