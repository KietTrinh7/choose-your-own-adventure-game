# CYOA Merchant Encounter

A console choose-your-own-adventure game: create a character, explore mountain paths, trade with wandering merchants, and fight the dragon.

## Language

### Merchant encounter

**Wandering Merchant**:
A randomly-encountered trader on the South path (25% chance per visit). Each appearance is a fresh individual with no memory of past interactions.
_Avoid_: Shopkeeper, vendor, trader

**Gold**:
The player's currency. Every character starts with 50.
_Avoid_: Money, coins, currency

**Enchanted Sword**:
The one-of-a-kind magical weapon sold for 30 gold. Buying it replaces the player's current weapon with no refund.
_Avoid_: Magical weapon, magic sword

**Enchanted Armor**:
The one-of-a-kind magical armor sold for 30 gold. Fills the player's (initially empty) armor slot.
_Avoid_: Magical armor, shield

**Protection**:
The flat amount subtracted from incoming damage when the player wears armor; damage taken is never below zero.
_Avoid_: Defense, armor rating, damage reduction stat

**Haggle**:
An optional per-purchase gamble: one d20 roll against Agility. Success takes 10 gold off that item; failure ends the entire interaction with that merchant.
_Avoid_: Bargain, negotiate

**Sell**:
Trading the player's current weapon to the merchant for its MaxDamage in gold, leaving the player with Fists. Fists and the Enchanted Sword cannot be sold.
_Avoid_: Trade-in, pawn

**Fists**:
The unarmed fallback weapon (MaxDamage 4) a player holds after selling their weapon without buying another.
_Avoid_: Unarmed, bare hands

**Encounter Roll**:
The d20 roll made when the player goes South: 16–20 means a Wandering Merchant appears; otherwise the uneventful woods.
_Avoid_: Spawn chance, random event
