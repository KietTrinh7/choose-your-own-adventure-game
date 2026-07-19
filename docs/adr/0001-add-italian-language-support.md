# ADR 0001: Add Italian as a supported game language

Date: 2026-07-19

## Status

Accepted

## Context

The game supported three languages (English, Spanish, French). All player-facing
text lives in `language_data.json`, keyed by language name, and `Messages.cs`
loads the selected language's dictionary and translation maps at startup. The
course assignment asked for Italian as a fourth language, and more generally we
want to grow the audience for the game without touching game logic every time a
language is added.

Two things constrain the design. First, the existing pattern is data driven;
`Game.cs` only knows language names, and every string lookup goes through
`Messages`. Second, the JSON file deliberately uses unaccented ASCII text for
the existing languages, which keeps console rendering predictable across
platforms and code pages.

## Decision

Add Italian by extending the existing data-driven pattern rather than by any
new mechanism:

1. Add a complete `"Italian"` section to `language_data.json` with the same six
   parts the other languages have (dictionary, raceMap, occupationMap,
   displayRaceMap, displayOccupationMap, displayWeaponMap).
2. Add one menu line and one switch arm in `Game.cs` so choice `4` maps to
   `"Italian"`.
3. Follow the file's unaccented ASCII convention for the Italian text
   (e.g. `agilita` rather than `agilità`), matching how Spanish and French are
   already written.

## Alternatives considered

- **Hardcode Italian strings in C#.** Rejected. It would fork the translation
  approach, scatter text across classes, and make the next language harder.
- **Move to .NET resource files (.resx) and CultureInfo.** A reasonable
  long-term option with real tooling support, but it would rewrite the whole
  localization layer for a single added language, and the course codebase and
  its tests are built around the JSON pattern.
- **Accented Italian text.** More typographically correct, but inconsistent
  with the rest of the file and riskier on consoles with limited code pages.

## Consequences

- Adding a language remains a data change plus two lines in `Game.cs`. A future
  refactor could remove even that by listing languages from the JSON keys.
- All 94 existing tests pass unchanged, since no logic changed.
- The ASCII convention slightly compromises Italian orthography. If the team
  later adopts UTF-8 output explicitly, all four languages should be updated
  together.
- The language menu itself is still hardcoded English/Spanish/French/Italian
  text in `Game.cs`; that is acceptable because it must be readable before any
  language is chosen.
