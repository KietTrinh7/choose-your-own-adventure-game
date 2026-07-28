# 02 — Wandering Merchant encounter: appear, browse, buy, decline

**What to build:** Traveling South now makes an Encounter Roll: on a d20 result of 16–20 (25%) a Wandering Merchant appears; otherwise the existing uneventful-woods narrative plays as before. The merchant greets the player (approved English greeting: "Ah, a traveler! Fate smiles on us both. I carry rare goods — enchanted steel and armor touched by magic. Care to see my wares?") and shows a numbered shop menu with the player's current Gold. The player can buy the Enchanted Sword (30 Gold, MaxDamage 16, replaces current weapon after an explicit no-refund discard warning) or the Enchanted Armor (30 Gold, Protection 3, fills the Armor slot), decline and leave, or be politely refused when Gold is insufficient (no state change). Merchants only offer items the player doesn't already own; leaving by any route returns to the adventure menu. All new text in English, Spanish, French, and Italian.

Shop logic lives in a Merchant module with the Die injected, returning outcome values; Game's console loop only reads input and prints localized results (per the spec's seam decisions and ADR-0002 — merchants are stateless, ownership is tracked on the Player).

**Blocked by:** 01 — Gold and Armor primitives.

**Status:** done

- [x] Encounter Roll: d20 of 16–20 produces a merchant; 1–15 shows the existing south-path narrative (unit-tested with a deterministic Die).
- [x] Buying an in-stock item with ≥30 Gold deducts exactly 30 and auto-equips it.
- [x] Weapon purchase is preceded by a discard warning the player must confirm; declining cancels the purchase.
- [x] Buying with insufficient Gold is refused with a localized message and no state change.
- [x] A player who owns an item is never offered it again (unit-tested).
- [x] The shop menu shows current Gold and only valid options; leave returns to the adventure menu.
- [x] Localization test: every new dictionary key resolves in all four languages.
- [x] `dotnet build` and `dotnet test` pass; a full shopping trip is playable via `dotnet run`.

Use the vocabulary in `CONTEXT.md` (Wandering Merchant, Encounter Roll, Enchanted Sword, Enchanted Armor, Gold).
