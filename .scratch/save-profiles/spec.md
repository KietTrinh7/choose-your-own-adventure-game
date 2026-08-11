# Spec: Save Profiles

Status: done — all three tickets implemented

## Problem Statement

Every run of the game starts at character creation. A player who rolls a Halfling
archer, travels South a dozen times waiting on the Encounter Roll, finally meets a
Wandering Merchant, sells their long bow and haggles their way into the Enchanted
Armor, then closes the window has lost all of it. There is no way to stop playing
and come back later, and no way for two people sharing a machine to each keep a
character. The only thing a player can do with a character they like is finish the
dragon fight in one sitting or lose it.

## Solution

The game gains Profiles. A Profile is one saved character, identified by the name
the player typed at character creation. Whenever the player returns to the
adventure menu, their Profile is written to disk automatically, so progress
survives closing the game by any means including killing the window. There is no
save command to remember and no save prompt to dismiss.

At launch, before character creation, the player is offered New Game or Continue.
Continue lists their Profiles by name and drops them back into the adventure menu
with the character exactly as it was: same Race, Occupation, Strength, Agility,
Health Points, Gold, weapon and armor. Continue is shown only when at least one
Profile exists, so a first-time player sees exactly the flow the game has today.

Creating a character with a name that already has a Profile warns the player that
the existing character will be lost and requires confirmation before replacing it,
matching how a weapon purchase warns before discarding the current weapon.

A Profile captures the player character and nothing else. The dragon is rolled
fresh every run, consistent with ADR-0002's treatment of encounters as stateless.
Language stays a per-run choice, because every stored value is canonical English
and is translated only at display time, so a character saved in Italian resumes
correctly in French.

## User Stories

1. As a player, I want my character saved automatically whenever I return to the adventure menu, so that I never lose progress by forgetting to save.
2. As a player, I want my progress to survive closing the game window, so that stopping play is not a punishment.
3. As a player, I want no save command or save prompt, so that saving never interrupts the adventure.
4. As a returning player, I want a Continue option at launch, so that I can pick up a character I played earlier.
5. As a returning player, I want my Profiles listed by character name, so that I can tell them apart at a glance.
6. As a first-time player, I want the launch menu skipped entirely when I have no Profiles, so that the game behaves exactly as it did before.
7. As a player choosing New Game, I want to go straight into character creation, so that starting fresh is unchanged.
8. As a player resuming a Profile, I want to land on the adventure menu rather than mid-encounter, so that I always resume from a clean point.
9. As a player resuming a Profile, I want my Race and Occupation restored, so that my character is the one I created.
10. As a player resuming a Profile, I want my Strength, Agility and Health Points restored exactly, so that a wounded character stays wounded.
11. As a player resuming a Profile, I want my Gold restored exactly, so that money I earned by selling is still mine.
12. As a player resuming a Profile, I want the weapon I was holding restored, including Fists if I sold my weapon and never re-armed.
13. As a player resuming a Profile, I want my Enchanted Armor and its Protection restored, so that armor I bought still protects me.
14. As a player who bought the Enchanted Sword and resumed, I want merchants to stop offering it to me, so that the one-of-a-kind rule survives a reload.
15. As a player who bought the Enchanted Armor and resumed, I want merchants to stop offering it to me, for the same reason.
16. As a player who sold my weapon and resumed, I want the sell option correctly unavailable while I hold Fists, so that the economy rules survive a reload.
17. As a player creating a character with a name I have already used, I want a clear warning that the existing character will be lost, so that overwriting is a decision rather than a gotcha.
18. As a player who sees that warning, I want to confirm before anything is replaced, so that a typo cannot destroy a character.
19. As a player who declines the warning, I want to choose a different name with nothing destroyed, so that backing out is safe.
20. As a player launching for the first time, I want no message about missing save data, so that a normal first run is not alarming.
21. As a player whose save file has been damaged, I want to be told it could not be read, so that I understand why my characters are gone.
22. As a player whose save file has been damaged, I want the damaged file kept rather than overwritten, so that it could be recovered by hand.
23. As a player whose save file has been damaged, I want the game to still start, so that one bad file does not lock me out.
24. As a player resuming a Profile, I want to face a freshly rolled dragon, so that each run's fight is its own gamble.
25. As a Spanish-, French- or Italian-speaking player, I want every new message localized, so that the feature matches the rest of the game.
26. As a player who plays one character in Italian and resumes it in French, I want the character intact, so that language choice never damages a save.
27. As a player, I want my saved characters kept outside the project's build output, so that rebuilding the game does not delete them.

## Implementation Decisions

- **New `ProfileStore` module** owns everything about Profiles: reading the store, writing a Player into it, listing Profile names, testing whether a name is taken, and distinguishing a missing store from an unreadable one. Its methods take and return `Player` objects and plain values. `Game`'s console loop only reads input, calls `ProfileStore`, and prints localized text, matching how `Merchant` is used.
- **The save directory is a constructor parameter** on `ProfileStore`, the same injection idiom `Dragon` and `Merchant` already use for `Die`. Production passes a CYOA folder under the user's local application data; tests pass a temporary directory. No filesystem interface, no mocking library.
- **Storage location** is a subfolder of the operating system's local application data directory, created on first write if absent. Deliberately not the application base directory used for the language data, because that resolves to the build output, which is wiped by a clean or rebuild and would destroy every Profile.
- **One store file, not one file per Profile.** A single JSON document holding a dictionary keyed by character name, each entry one serialized `Player`. Listing Profiles is a single read, the whole file writes at once, and character names never have to be made legal as filenames. The accepted tradeoff is that damage to the file affects every Profile.
- **A Profile is exactly one `Player`.** Nine properties, two of which are nullable object references for the weapon and armor. No progress, path history, turn count or purchase log is stored, because none exists: the adventure loop holds no state between menu visits, and every merchant stock and eligibility rule is derived from the player's current weapon and armor (ADR-0002).
- **The dragon is not persisted.** Its stats are rolled per run. The player can never have met it before saving, since the dragon encounter ends the run in every outcome.
- **Serialization uses the JSON support already in the base framework**, the same library the language data loader uses, so no package reference is added. The weapon and armor types serialize and deserialize without attributes or new constructors, because their public properties and single constructors already line up by name.
- **Language is not part of a Profile.** The language prompt keeps its current position ahead of everything else. Stored values are canonical English and are translated at display time, so Profiles are language-agnostic. Persisting language would also require rendering the Continue list before the language is known.
- **Autosave points are returns to the adventure menu**, which is a quiescent state. No encounter is ever mid-flight at that moment, so no combat or shop state is ever serialized.
- **Name collision** is a dictionary key check. On a match the player is warned that the existing character will be lost and must confirm; declining returns them to the name prompt with nothing written.
- **Missing store versus unreadable store** are different outcomes. Missing means no Profiles and no message, which is the ordinary first run. Unreadable means the player is told, the damaged file is renamed aside rather than overwritten, and the game continues with no Profiles.
- **New player-facing strings** are added to the language data in English, Spanish, French and Italian per ADR-0001: the launch menu and its options, the Profile list prompt, the overwrite warning and its confirmation, and the unreadable-store notice.
- **`CONTEXT.md` gains a Profile entry** in the existing glossary style, since no save or profile concept exists in the domain language today.

## Testing Decisions

- Tests target external behavior through one seam, the `ProfileStore` methods, with the store pointed at a temporary directory. Never console I/O, never private internals, never the real user data directory. Prior art: `Merchant` tests drive shop logic through public methods with an injected `Die`, and `Player.ReduceDamage` is tested as a pure function.
- **Round trip**: a Player saved and reloaded is equal field for field, including Race and Occupation, and including a null weapon and a null armor.
- **Equipment round trip**: a Player holding the Enchanted Sword and wearing the Enchanted Armor reloads with the weapon's damage ceiling and the armor's Protection intact, so combat behaves identically after a reload.
- **Merchant rules after reload**: a reloaded Player who owns the Enchanted Sword is not offered it again; a reloaded Player holding Fists has no sell option. These assert through the existing `Merchant` methods, proving the derived rules survive persistence without `ProfileStore` knowing anything about merchants.
- **Multiple Profiles**: two characters with different names both persist, and listing returns both names.
- **Overwrite**: saving a second Player under an existing name replaces that entry and leaves other entries untouched.
- **Missing store**: pointing the store at an empty directory yields no Profiles, no exception and no file created until the first write.
- **Unreadable store**: pointing the store at a directory containing a malformed file yields no Profiles, reports the failure, and leaves the malformed content preserved under a different name rather than destroyed.
- **Localization**: every new key resolves in all four languages. The message lookup returns the key wrapped in brackets when absent, so a bracket check catches gaps.

## Out of Scope

- Deleting a Profile from inside the game. The overwrite path already lets a player reclaim a name, and the store file can be removed by hand.
- Saving mid-encounter, mid-combat or mid-shop.
- Persisting the dragon, path history, turn counts or any purchase log.
- Storing the chosen language per Profile.
- Save file versioning or migration between schema changes.
- Cloud, shared or networked saves.
- Renaming, copying or exporting a Profile.

## Further Notes

- The dragon is currently constructed once at start-up rather than at the moment the player travels North, and that object outlives returns to the adventure menu. It is harmless today because the dragon encounter ends the run in every outcome, and this spec does not depend on changing it. It is a reasonable candidate for the separate refactoring step.
- `Player` builds its own die internally rather than receiving one, unlike `Dragon` and `Merchant`. That is untouched here, but it means a reloaded Player is not die-injectable, which would matter if a future feature needed deterministic player rolls.
- The default JSON text encoder escapes angle brackets, so weapon art stored in the file is escaped. It round-trips exactly and only affects how the file reads to a human opening it directly.
