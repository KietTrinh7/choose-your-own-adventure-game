# 03 — Survive a missing or damaged store

**What to build:** A first-time player sees nothing unusual: no store file exists,
no Continue option appears, and no message mentions save data. A player whose store
file has been damaged is told plainly that it could not be read, keeps the damaged
file in case it can be salvaged, and can still play.

**Blocked by:** 01 — Save a character and continue it.

**Status:** ready-for-agent

- [ ] A missing store file means no Profiles, no message, and no Continue option on the launch menu.
- [ ] A missing store file causes no file to be created until the first save.
- [ ] A store file that cannot be parsed tells the player it could not be read.
- [ ] The unreadable file is renamed aside rather than overwritten or deleted, so its contents survive.
- [ ] After an unreadable file is set aside, the game starts normally with no Profiles, and the next save writes a fresh store.
- [ ] A damaged store never prevents the game from launching.
- [ ] The unreadable-store notice is added to the language data in English, Spanish, French and Italian.
- [ ] Tests cover: an empty directory yields no Profiles and no exception and no file written; a directory containing a malformed store yields no Profiles, reports the failure, and leaves the malformed content preserved under a different name.
- [ ] `dotnet build` is clean and `dotnet test` passes.
