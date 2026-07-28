# Merchants are stateless random encounters

The merchant on the South path is a Wandering Merchant: a 25% random encounter, and every appearance is a fresh individual with no memory. A failed Haggle ends the entire interaction with that merchant (nothing bought or sold), but does not lock the player out of future merchants.

## Considered Options

We rejected a persistent single merchant with a permanent haggle-failure lockout. Stateless merchants delete an entire class of saved state (no lockout flag to carry, reset on play-again, or test), while keeping haggling genuinely risky — failure still costs you the current shopping trip and forces you to gamble on the 25% Encounter Roll to try again.

## Consequences

Merchant stock cannot live on the merchant; ownership is tracked on the Player (a merchant only offers what the player doesn't already own). Any future feature that wants merchant memory (grudges, reputation, restocking) reverses this decision and should supersede this ADR.
