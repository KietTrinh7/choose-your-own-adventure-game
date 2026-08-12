# 01 — Build the Prompt module

**What to build:** Nothing the player can see. The module that will own every
prompt in the game is built and tested while every existing caller keeps its own
loop. This is the expand step.

**Blocked by:** None — can start immediately.

**Status:** ready-for-agent

- [ ] A `Prompt` module exists, constructed with `Messages` plus an optional line reader and line writer that default to the console.
- [ ] It offers three methods: choosing from aliases and returning the canonical answer, choosing a number from a menu and returning the index, and entering free text that a validator accepts.
- [ ] Alias matching trims and lowercases. Number selection trims. Free text trims but never lowercases.
- [ ] A rejected answer writes the existing invalid message and asks again, indefinitely, until an acceptable answer arrives.
- [ ] No new strings are added to the language data in any language.
- [ ] No existing caller changes. All sixteen loops still exist.
- [ ] `CONTEXT.md` gains a Prompt entry.
- [ ] Tests drive all three methods with a scripted reader and collect output into a list, never touching the real console.
- [ ] Tests cover: a word and its short form returning the same answer, uppercase and padded input accepted, several bad answers in a row followed by a good one, the invalid message written before each re-ask, out-of-range and non-numeric menu input rejected, and free text preserving the player's capitalisation.
- [ ] `dotnet build` is clean and all existing tests pass unchanged.
