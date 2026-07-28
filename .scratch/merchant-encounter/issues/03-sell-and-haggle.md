# 03 — Sell and Haggle

**What to build:** The shop gains two mechanics. **Sell:** the player can sell their current weapon to the merchant for its MaxDamage in Gold (long sword 12, lightning bolt spell 12, dagger 6, long bow 8), leaving them fighting with Fists (MaxDamage 4) until they buy the Enchanted Sword; Fists and the Enchanted Sword cannot be sold, and the sell option only appears when the player holds a sellable weapon. **Haggle:** when buying an item, the player may haggle before paying — one d20 roll, success at or under their Agility takes 10 Gold off that item's price; failure insults the merchant, cancels the transaction (no Gold moves, nothing equips), and ends the entire interaction, returning the player to the adventure menu. Per ADR-0002 there is no lockout: the next randomly-encountered merchant has a clean slate. All new text in English, Spanish, French, and Italian.

**Blocked by:** 02 — Wandering Merchant encounter.

**Status:** done

- [x] Selling credits the weapon's MaxDamage in Gold and leaves the player holding Fists (MaxDamage 4).
- [x] No sell option is offered when the player holds Fists or the Enchanted Sword (unit-tested).
- [x] Haggle success (forced Die): item is bought for 20 Gold instead of 30.
- [x] Haggle failure (forced Die): no purchase, no Gold change, the encounter ends, and the player is returned to the adventure menu.
- [x] After a failed Haggle, a subsequent merchant encounter behaves normally (no persistent state).
- [x] A player who sells a 12-MaxDamage weapon can afford both items (62 Gold ≥ 60) — the earned exception from the spec's economy invariants.
- [x] Localization test: every new dictionary key resolves in all four languages.
- [x] `dotnet build` and `dotnet test` pass; the sell-then-buy-both strategy and the haggle gamble are playable via `dotnet run`.

Use the vocabulary in `CONTEXT.md` (Sell, Haggle, Fists). Respect ADR-0002 — merchants are stateless.
