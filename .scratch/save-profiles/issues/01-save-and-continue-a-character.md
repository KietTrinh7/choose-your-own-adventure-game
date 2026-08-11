# 01 — Save a character and continue it

**What to build:** A player can create a character, play, close the game, relaunch,
choose Continue, and land back on the adventure menu with that character exactly as
it was. Saving happens on its own every time the player returns to the adventure
menu, with no save command and no prompt. A player who has never saved anything
sees the game exactly as it behaves today.

**Blocked by:** None — can start immediately.

**Status:** ready-for-agent

- [ ] A Profile is one saved character, identified by the name given at character creation.
- [ ] Returning to the adventure menu writes the current character to the Profile store, every time, with no player action.
- [ ] Profiles persist in a CYOA folder under the operating system's local application data directory, created on first write. Not the build output directory.
- [ ] The store is a single JSON document holding a dictionary keyed by character name, one serialized player each, using the JSON support already in the base framework. No new package reference.
- [ ] The store module takes its directory as a constructor parameter, matching how the die is injected elsewhere.
- [ ] Launch shows a New Game / Continue menu before character creation, and shows it only when at least one Profile exists.
- [ ] Continue lists Profiles by character name and resumes the chosen one on the adventure menu.
- [ ] New Game goes straight to character creation, unchanged from today.
- [ ] A resumed character has its Race, Occupation, Strength, Agility, Health Points and Gold exactly as saved.
- [ ] A resumed character has its weapon restored, including Fists, and its armor restored with its Protection, or no armor if it had none.
- [ ] A resumed character who owns the Enchanted Sword or the Enchanted Armor is not offered it again by a Wandering Merchant.
- [ ] A resumed character holding Fists has no sell option at a Wandering Merchant.
- [ ] The dragon is rolled fresh on every run and nothing about it is stored.
- [ ] The chosen language is not stored, and the language prompt keeps its current position.
- [ ] Every new player-facing string is added to the language data in English, Spanish, French and Italian.
- [ ] `CONTEXT.md` gains a Profile entry in the existing glossary style.
- [ ] Tests drive the store through its public methods with the directory pointed at a temporary path, never console input and never the real user data directory.
- [ ] Tests cover: save and reload equal field for field, a null weapon and null armor round-tripping, equipment round-tripping so combat behaves identically after a reload, two named Profiles both persisting and both listed.
- [ ] `dotnet build` is clean and `dotnet test` passes.
