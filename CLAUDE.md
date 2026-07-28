# CLAUDE.md

Choose Your Own Adventure (CYOA) — a .NET console game for the CIDM 3320 AI
Agentic Coding homework series. Build with `dotnet build`, run with `dotnet run`,
and run tests with `dotnet test` (tests live in `CYOATests/`).

All player-facing text is data-driven: `language_data.json` holds a dictionary
and display maps per language (English, Spanish, French, Italian), loaded by
`Messages`. New user-facing strings must be added to every language section
(see ADR 0001).

## Agent skills

### Issue tracker

Issues and specs live as local markdown files under `.scratch/<feature-slug>/` in this repo. See `docs/agents/issue-tracker.md`.

### Triage labels

Default label vocabulary — the five canonical role names are used as-is. See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: one `CONTEXT.md` and `docs/adr/` at the repo root. See `docs/agents/domain.md`.
