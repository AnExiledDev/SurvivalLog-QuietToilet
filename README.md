> **AI-written from operator direction.** The intent is the operator's; the wording and scope are an agent's.
> `session: 127889d4-f8d8-4d17-8ded-f6f46da456fc | 2026-10-11 | source: op:2026-10-11-0745-b8ce`

# Quiet Toilet

A BepInEx 6 (IL2CPP) plugin for Survival Log that mutes the sounds and character voices played while using a toilet. Every other sound plays as normal.

## Config

`BepInEx/config/com.dev.survivallog.quiettoilet.cfg`:

- `Mute/ActionKeywords`: an action whose name contains any of these (English or Chinese) counts as a toilet action; its sound and voices are muted.
- `Mute/PathKeywords`: any FMOD event whose path contains one of these is muted wherever it plays.
- `Mute/ExtraEventPaths`: exact FMOD event paths to mute as well, comma separated.
- `Diagnostics/LogSoundsHeard` (default `true`): log each distinct sound the first time it plays to `BepInEx/quiet-toilet.log`.

If a toilet sound still gets through, find its path in `BepInEx/quiet-toilet.log` and add it to `ExtraEventPaths`.

## Install

Download `QuietToilet.dll` from the [latest release](https://github.com/AnExiledDev/SurvivalLog-QuietToilet/releases/latest) and copy it into `BepInEx/plugins`. Needs BepInEx 6 (IL2CPP build).

## Build and test

```sh
dotnet build src/QuietToilet -c Release -p:GameDir="D:\path\to\Survival Log"
dotnet test tests/QuietToilet.Tests
```

The build references BepInEx and the game's interop assemblies under `GameDir`, so BepInEx must be installed and the game launched once. The tests compile only the pure `Core` code and need nothing from the game.

## License

MIT, see [LICENSE](LICENSE).

## Provenance

> Alright, put all of these mods in separate GitHub repos, public is fine.
