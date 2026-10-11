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

Copy `QuietToilet.dll` into `BepInEx/plugins`. Needs BepInEx 6 (IL2CPP build).

## Build and test

```sh
dotnet build src/QuietToilet -c Release -p:GameDir="D:\path\to\Survival Log"
dotnet test tests/QuietToilet.Tests
```

The build references BepInEx and the game's interop assemblies under `GameDir`, so BepInEx must be installed and the game launched once. The tests compile only the pure `Core` code and need nothing from the game.
