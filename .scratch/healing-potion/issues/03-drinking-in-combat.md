# 03 — Drinking a potion in combat

**What to build:** In a fight the player can drink instead of swinging. The
attack-or-retreat prompt becomes attack, drink or retreat, and the drink option
only appears when a potion is actually carried. Drinking restores a rolled amount
up to the character's maximum and costs the round: no attack, and the monster
still gets its turn.

**Blocked by:** 02 — The Wandering Merchant sells Healing Potions.

**Status:** ready-for-agent

- [ ] The combat prompt offers drink alongside attack and retreat, in both a short and a long form.
- [ ] The drink option is absent entirely when the player carries no potions.
- [ ] Drinking restores a d10 roll of Health Points.
- [ ] Healing never takes the player above the maximum health they rolled at creation.
- [ ] Drinking at full health is refused, the potion is kept, and the player is told why.
- [ ] A successful drink decrements the count by exactly one; a refused drink decrements nothing.
- [ ] Drinking costs the round: the player does not attack, and the monster's attack runs immediately afterwards.
- [ ] Drinking works identically against the wolf and the dragon, since both go through the same combat loop.
- [ ] Every new string is localized in English, Spanish, French and Italian.
- [ ] Tests cover: healing to a known result from a known roll, healing clamped at the maximum rather than exceeding it, refusal at full health with the potion kept, refusal with none carried, and exactly one potion consumed per successful drink.
- [ ] `dotnet build` is clean and all tests pass.
