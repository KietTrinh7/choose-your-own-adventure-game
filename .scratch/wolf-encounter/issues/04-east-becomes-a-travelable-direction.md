# 04 — East becomes a travelable direction

**What to build:** The player can travel East. The path prompt offers it alongside
North and South, the input is accepted as either the full word or its first
letter, and going there gives a short uneventful narrative before returning to the
adventure menu. Nothing lives there yet.

**Blocked by:** 03 — The combat loop holds a monster rather than the dragon.

**Status:** ready-for-agent

- [ ] The path prompt offers East alongside North, South and exit.
- [ ] Path validation accepts East as both the full word and its first letter, matching how the existing directions are parsed.
- [ ] Travelling East prints an uneventful narrative and returns the player to the adventure menu.
- [ ] Returning to the adventure menu autosaves as it already does, so a trip East is captured in the Profile.
- [ ] The East path option and its narrative are localized in English, Spanish, French and Italian.
- [ ] Existing path-validation tests are extended to cover East.
- [ ] `dotnet build` is clean and all tests pass.
