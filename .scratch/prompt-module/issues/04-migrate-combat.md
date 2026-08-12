# 04 — Migrate Combat's prompt

**What to build:** The last hand-rolled loop, the attack-or-retreat prompt inside
the fight, is replaced by a `Prompt` call. After this ticket no file outside
`Prompt` reads the console.

**Blocked by:** 03 — Migrate Player's six prompts.

**Status:** ready-for-agent

- [ ] The `Console.ReadLine` in `Combat` is gone, replaced by a `Prompt` call.
- [ ] Attack and retreat are still accepted in both their short and long forms, and retreat still sets the retreated flag.
- [ ] A search of the codebase finds `Console.ReadLine` in `Prompt` and nowhere else.
- [ ] No new tests. The existing suite staying green is the proof.
- [ ] `dotnet build` is clean and all tests pass.
