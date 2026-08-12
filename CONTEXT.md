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

### Healing

**Healing Potion**:
Ordinary merchant stock at 10 Gold, unlike the one-of-a-kind Enchanted Sword and Enchanted Armor. Drunk during combat at the cost of the round's attack, restoring a d10 roll of Health Points. Carried as a count, not an inventory.
_Avoid_: Elixir, remedy, health pack, consumable

**Maximum Health**:
The Health Points a character rolled at creation. Healing never passes it and nothing in the game raises it, so a character can be restored but never improved.
_Avoid_: Max HP, health cap, total health

### Prompting

**Prompt**:
Asking the player something and insisting on an acceptable answer. Owns printing the question, reading the line, trimming it, matching it, rejecting what does not fit and asking again. The only place in the game that reads the console.
_Avoid_: Input handler, reader, console helper

### Wolf encounter

**Monster**:
Anything the player can fight. Supplies a name, stats, a weapon, an attack, taunts and its own death line; the combat loop knows nothing else about it. The Dragon and the Wolf are the two that exist.
_Avoid_: Enemy, creature, mob, NPC

**Wolf**:
The lone animal on the East path, met on an Encounter Roll. Rolls every stat on a d10 against the Dragon's d20, so it is always the lesser fight. Beating it or fleeing it returns the player to the adventure menu; only dying to it ends the run.
_Avoid_: Beast, animal, wolves

**Fangs**:
The Wolf's weapon, MaxDamage 6 against the Dragon's claws at 12.
_Avoid_: Teeth, bite, jaws

### Save profiles

**Profile**:
One saved character, identified by the name given at character creation. Holds the whole character and nothing else: no dragon, no path history, no purchase log.
_Avoid_: Save, savegame, save file, slot, checkpoint

**Autosave**:
The automatic write of the current Profile that happens every time the player returns to the adventure menu. There is no save command.
_Avoid_: Quicksave, save point

**Profile store**:
The single JSON document under the player's local application data holding every Profile, keyed by character name.
_Avoid_: Database, save folder
