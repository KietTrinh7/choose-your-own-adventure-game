# 02 — Migrate Game's nine prompts

**What to build:** Still nothing the player can see. The nine hand-rolled loops in
`Game` are replaced by calls to `Prompt`: the main menu, the Profile launch menu,
the Profile list, the path prompt, the yes-or-no helper, the merchant shop menu,
and the attack-or-retreat prompts in both encounters. The largest file goes first.

**Blocked by:** 01 — Build the Prompt module.

**Status:** ready-for-agent

- [ ] Every `Console.ReadLine` in `Game` is gone, replaced by a `Prompt` call.
- [ ] The yes-or-no helper in `Game` is gone, replaced by an alias call.
- [ ] The merchant shop still builds its options from what is actually in stock and still maps the chosen number back to the right action.
- [ ] Path selection still accepts north, south, east and exit in the forms it accepts today, including East as the full word only.
- [ ] Both encounters still accept attack and retreat in both their short and long forms.
- [ ] The game plays identically. No prompt text, no accepted input and no outcome changes.
- [ ] No new tests. The existing suite staying green is the proof.
- [ ] `dotnet build` is clean and all tests pass.
