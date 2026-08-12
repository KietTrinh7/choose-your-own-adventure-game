# Spec: Prompt Module

Status: done — all four tickets implemented

## Problem Statement

The same seven lines are written sixteen times across three files: print a prompt,
read a line, trim it, compare it against the accepted answers, print the invalid
message, loop. Nobody chose the differences between them, but they are there. The
path prompt lowercases its input; the Profile menu does not. Combat accepts `a`
and `attack`; the wolf encounter independently reimplements the same pair. Two
separate yes-or-no helpers exist, one in `Game` and one in `Player`, written weeks
apart.

None of it is tested. There is no interface to test it through, because the logic
lives inside `while(true)` bodies that read the console directly. So the most
common thing a player does, typing something the game did not expect, is the one
behaviour with no coverage at all.

It is also the reason `Game` keeps growing. Nine of its sixteen prompt loops live
there, and the file now runs to 364 lines and appears in five of the last eight
commits.

## Solution

One module owns asking a question and getting an acceptable answer back.

Callers say what they are asking and what they will accept, and get the answer.
`Prompt` handles printing, reading, trimming, matching, rejecting and re-asking.
Console access happens in exactly one place.

Three methods cover every existing site, because the sixteen reduce to three
shapes. Choosing from a set of aliases where several inputs mean the same thing.
Choosing a number from a menu whose length varies. Entering free text that a
validator accepts or rejects.

The line reader and line writer are constructor parameters defaulting to the
console, so production code is unchanged in spirit and tests drive the module with
a scripted sequence of answers. Nothing about the game's behaviour changes: this is
a deepening, and the existing suite passing untouched is what proves it.

## User Stories

1. As a player, I want every prompt in the game to accept input the same way, so that what works at one prompt works at the next.
2. As a player, I want a rejected answer to re-ask rather than crash or skip ahead, at every prompt without exception.
3. As a player, I want uppercase and padded input accepted wherever lowercase and trimmed input is, so that typing habits do not matter.
4. As a player creating a character, I want my name stored exactly as I typed it, so that capitalisation survives.
5. As a developer, I want one place that decides what an acceptable answer is, so that the rule cannot drift between prompts again.
6. As a developer, I want input handling covered by tests, so that the most common player action is no longer the least tested.
7. As a developer, I want to write a new prompt without writing another loop, so that adding a menu is not an invitation to copy and paste.
8. As a developer, I want the module usable without configuration in production, so that adopting it is not a burden at every call site.
9. As a developer, I want to script a sequence of answers in a test, so that I can assert what happens when a player types nonsense three times and then something valid.
10. As a developer, I want the migration to happen a file at a time, so that a mistake is localised and the suite stays green between steps.
11. As a developer, I want no behaviour to change, so that the existing tests are the proof rather than a new set of assertions written to match whatever the code now does.
12. As a developer, I want `Game` to shrink, so that the file every feature touches stops being the file every feature touches.

## Implementation Decisions

- **A new `Prompt` module** owns printing a prompt, reading a line, normalising it,
  matching it against what the caller accepts, printing the invalid message and
  re-asking. No caller reads the console once migration is complete.
- **Three methods, one per shape observed in the existing code.** One takes a set
  of aliases mapped to canonical answers and returns the canonical form, covering
  path selection, attack or retreat, yes or no, and the roll confirmation. One
  takes a count and returns the chosen index, covering the main menu, the Profile
  list and the merchant shop. One takes a validator and returns the accepted text,
  covering name, race and occupation.
- **The line reader and line writer are constructor parameters** with console
  defaults, alongside `Messages`. Production constructs it with the messages alone.
  Tests pass a queue of scripted answers and collect output into a list. This
  follows the existing lightweight injection idiom rather than introducing a
  console interface with two implementations.
- **Normalisation is fixed and uniform.** Alias matching trims and lowercases,
  which is what every alias site already does. Number selection trims, and case is
  meaningless for digits. Free text trims but never lowercases, so a character name
  keeps the capitalisation the player typed.
- **The invalid message stays the one already in the language data**, so no new
  strings are added in any language and the localisation surface does not grow.
- **Migration is expand then migrate.** The module is built and tested while every
  caller keeps its own loop. Then callers convert one file at a time, largest
  first.
- **`CONTEXT.md` gains a Prompt entry**, since asking the player something and
  insisting on an acceptable answer is now a named concept in the domain.

## Testing Decisions

- One seam: the three `Prompt` methods, driven with a scripted reader. Never the
  real console. Prior art is the injected `Die` used by `Dragon`, `Wolf` and
  `Merchant`, and the injected directory used by `ProfileStore`.
- **Alias matching**: a full word and its short form both return the same canonical
  answer; uppercase and padded input are accepted; an unrecognised answer re-asks
  and the next valid answer is returned.
- **Rejection is not silent**: after a bad answer, the invalid message is written
  before the prompt is asked again.
- **Persistence of asking**: several bad answers in a row keep re-asking, and the
  eventual valid one is returned rather than the game giving up or advancing.
- **Number selection**: valid indices are returned; zero, negatives, values above
  the count, and non-numeric input are all rejected and re-asked.
- **Free text**: the validator decides; a rejected value re-asks; the returned
  value preserves the player's capitalisation.
- **The migration tickets add no tests.** If a site is converted correctly the
  existing suite stays green, and that is the proof it changed nothing.

## Out of Scope

- Any change to what the game says or how it behaves.
- New strings in any language.
- Extracting the encounters or the merchant shop interface out of `Game`, which
  was a separate candidate in the architecture review.
- Injecting a `Die` into `Player`, likewise separate.
- Renaming the dragon-named message keys, likewise separate.
- Replacing the console with any other kind of interface.

## Further Notes

- Shrinking the prompt loops removes roughly a third of `Game`, which makes the
  encounter extraction from the architecture review a materially smaller job
  afterwards. That is the main reason this candidate was taken first.
- The merchant shop builds its menu options dynamically and then maps the chosen
  number back to an action. That is the most involved of the sixteen sites and the
  one most likely to need care during migration.
