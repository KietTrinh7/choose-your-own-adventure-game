# 01 — Gold and Armor primitives, wired into combat and stats

**What to build:** Every new character starts with 50 Gold, visible in the character stats display. The Player gains an Armor slot (empty by default, "none" in stats) and Protection-based damage reduction: when the dragon lands a hit, the damage the player takes is reduced by their armor's Protection, never below zero, with a localized "armor absorbs the blow" style message when a wearing player is hit. Armor itself is not yet obtainable — this slice proves the plumbing end-to-end (tracer bullet) so the Merchant tickets can sell into it.

**Blocked by:** None — can start immediately.

**Status:** done

- [x] A newly created character has exactly 50 Gold, not rolled.
- [x] The stats display shows Gold and Armor (name + Protection, or "none") in English, Spanish, French, and Italian.
- [x] `Player.ReduceDamage(raw)` returns `max(0, raw − Protection)`; with no armor, damage is unchanged.
- [x] Dragon combat applies damage to the player through `ReduceDamage`.
- [x] Unit tests (following the existing logic-class test style): raw 8 with Protection 3 → 5; raw 2 with Protection 3 → 0; no armor → unchanged.
- [x] Localization test: every new dictionary key resolves in all four languages.
- [x] `dotnet build` and `dotnet test` pass; the game runs and shows Gold in stats.

Use the vocabulary in `CONTEXT.md` (Gold, Protection, Fists). Respect ADR-0002.
