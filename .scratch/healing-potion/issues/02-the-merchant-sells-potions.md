# 02 — The Wandering Merchant sells Healing Potions

**What to build:** The shop gains a third item. A Healing Potion costs 10 Gold and
the merchant sells as many as the player can afford, so unlike the Enchanted Sword
and the Enchanted Armor it is always in stock. The player can see how many they are
carrying. Nothing can be drunk yet.

**Blocked by:** 01 — A character remembers its maximum health.

**Status:** ready-for-agent

- [ ] A character carries a count of Healing Potions, starting at zero.
- [ ] The count persists in the Profile and survives a save and reload.
- [ ] The shop lists a Healing Potion at 10 Gold, numbered dynamically alongside the existing options.
- [ ] The merchant offers it regardless of how many the player already owns, unlike the one-of-a-kind goods.
- [ ] Buying deducts exactly the price and increases the count by one.
- [ ] Buying without enough Gold is refused politely and changes nothing.
- [ ] Haggling applies to a potion exactly as it applies to the other goods.
- [ ] The character stats display shows how many potions are carried, alongside Gold and Armor.
- [ ] Every new string is localized in English, Spanish, French and Italian.
- [ ] Tests cover: a purchase deducting the price and incrementing the count, a refused purchase changing nothing, repeated purchases continuing to work, the potion being offered whether the player owns none or many, and the count surviving a Profile round trip.
- [ ] `dotnet build` is clean and all tests pass.
