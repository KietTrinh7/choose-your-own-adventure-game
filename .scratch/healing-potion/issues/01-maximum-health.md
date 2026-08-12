# 01 — A character remembers its maximum health

**What to build:** Almost nothing the player can see. A character now remembers the
Health Points it rolled at creation as its Maximum Health, and that value survives
saving and reloading. Nothing heals yet, so the value is only recorded, never used.

**Blocked by:** None — can start immediately.

**Status:** ready-for-agent

- [ ] A character has a Maximum Health, set to the Health Points rolled during character creation.
- [ ] It is never changed afterwards by anything in the game.
- [ ] It persists in the Profile and survives a save and reload.
- [ ] A character created before this change and loaded from an older Profile does not end up with a maximum of zero.
- [ ] Combat, the merchant and the encounters all behave exactly as they do today.
- [ ] Tests cover: the maximum matching the rolled health after creation, and surviving a Profile round trip.
- [ ] `dotnet build` is clean and all existing tests pass.
