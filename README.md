# Quiet Toilet

Mutes the toilet. Every other sound plays as normal.

Using the toilet in Survival Log plays a sound effect and the character's voice lines. Quiet Toilet mutes just those, so you can keep your volume up without hearing it every time someone goes.

## Features

- **Toilet sounds muted**: every toilet action plays without its sound effect.
- **Toilet voice lines muted**: every character's voice lines during a toilet action are muted too.
- **Nothing else goes quiet**: if the game also uses a toilet sound for another action, the mod leaves that sound alone.
- **Keyword backup**: a sound with a toilet word in its event path (like `toilet` or `poop`) is muted wherever it plays. This catches new toilet sounds a game update might add.
- **Add your own**: if you still hear a toilet sound, add its path to `ExtraEventPaths` in the config and it's muted.
- **Sound log**: turn on `LogSoundsHeard` and the mod writes each sound the first time it plays to `BepInEx\quiet-toilet.log`, so a missed sound is easy to find. It is off by default.

Quiet Toilet only changes what you hear. It adds nothing to your save and does not edit game files. You can remove it at any time.

The mod finds toilet actions by their English and Chinese names, so it works with the game in either language. Every config setting is explained in English and Simplified Chinese.

## How it works

When the game finishes loading its tables, the mod looks for every action whose name contains one of the action keywords (`toilet`, plus the Chinese words for toilet and using the toilet). The sounds and voice lines of those actions are muted. If another action also uses one of those sounds, the mod leaves that sound alone, and the log says which actions share it.

A shared sound you add to `ExtraEventPaths` yourself is muted anyway, and the log lists the other actions that go quiet with it. The main toilet sound is one of these. The game also plays it for two other toilet actions whose names don't have a toilet word in them, so it's in `ExtraEventPaths` by default.

If the game's tables still haven't loaded after about two minutes, the mod only mutes by path keywords and your extra paths, and says so in the log.

## Known issues and limits

- A toilet sound the game also uses for another action still plays, unless it's in `ExtraEventPaths`. The main toilet sound is there by default. If you still hear something, look in `BepInEx\quiet-toilet.log` for lines with "left alone because these actions use it too".
- The path keywords are matched as plain text. A keyword like `poop` mutes any sound with `poop` anywhere in its path, toilet or not.

## Requirements

- Survival Log 1.1.20719. A game update can break it until the mod is updated.
- The [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12), the BepInEx 6 (IL2CPP) build for the game.
- [Exiled's Toolkit](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit) 0.19.0 or newer, which applies settings changed on the Mod Manager's Mods screen without restarting the game. Quiet Toilet does not load without it.
- Optional: [Mod Manager](https://www.nexusmods.com/survivallog/mods/73). With it, you can change every setting in game on its Mods screen and see what each one does, without opening the config file.

## Install

1. Install the [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12) (if no other mods were installed before, start the game once so BepInEx finishes setup, then quit).
2. Install Exiled's Toolkit: download `ExiledToolkit-x.y.z.zip` from [its latest release](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/61), and extract it into the game folder (the folder with `SurvivalLog.exe`).
3. Download `QuietToilet-x.y.z.zip` from the [latest release](https://github.com/AnExiledDev/SurvivalLog-QuietToilet/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/31).
4. Extract the zip into the game folder (the folder with `SurvivalLog.exe`). It holds a `BepInEx` folder, so the DLL lands in the right place. When asked, merge the folders. The result should be:
   `C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\QuietToilet.dll`

## Uninstall

Delete `QuietToilet.dll` from the `BepInEx\plugins` folder. For a clean slate, also delete `BepInEx\config\com.dev.survivallog.quiettoilet.cfg`, `BepInEx\quiet-toilet.log` and `BepInEx\quiet-toilet.log.1` if it is there.

## Configuration

Settings live in `BepInEx\config\com.dev.survivallog.quiettoilet.cfg`, created the first time the game starts with the mod. Change them in game on the Mod Manager's Mods screen, where they take effect at once, or edit the file while the game is closed. See [CONFIG.md](CONFIG.md) for every setting.

## Troubleshooting

Built against BepInEx `6.0.0-be.788`.

Check `BepInEx\LogOutput.log` for `===== Quiet Toilet 0.3.3 started`. Once a game has loaded, a line starting with `read ... actions and ... voices` shows how many toilet actions were found and which sounds are muted. The same lines are in `BepInEx\quiet-toilet.log`, which survives restarts. Once it passes 1 MB, it's moved to `quiet-toilet.log.1` and a new one starts. With `LogSoundsHeard` on, it also lists every sound the first time it plays.

To mute a sound that still plays: set `LogSoundsHeard` to `true`, use the toilet in game, open `BepInEx\quiet-toilet.log`, find the event path logged at that moment (it looks like `event:/...`), and add it to `ExtraEventPaths`.

## Feedback and bug reports

Found a bug or have an idea? Come say so on the Core Directive Discord: https://discord.gg/trTsyxUFqg . You can also post in the Posts tab of the mod's Nexus Mods page. When reporting a bug, attach `BepInEx\LogOutput.log`, `BepInEx\quiet-toilet.log` and `BepInEx\quiet-toilet.log.1` if it is there.

To reach me directly, message `exiled.dev` on Discord. You can find me in the [official Survival Log Discord](https://discord.gg/MZGg4gQAUe). I'm also on X as [@AnExiledDev](https://x.com/AnExiledDev).

A Survival Log wiki with detailed documentation for all of my mods is coming soon.

## Build

This is a BepInEx 6 IL2CPP plugin. It compiles against BepInEx, the game's IL2CPP interop assemblies and Exiled's Toolkit, so a game install with BepInEx set up and started once is required. Those assemblies are not part of this repo. The .NET 8 SDK is required.

```
dotnet build src/QuietToilet -c Release -p:GameDir="C:\path\to\Survival Log" -p:ToolkitDll="C:\path\to\ExiledToolkit.dll"
```

`GameDir` is the game folder and `ToolkitDll` is a built `ExiledToolkit.dll` (0.19.0 or newer). The defaults in the project file are paths on my PC, so you'll need to set both. The output DLL is at `src\QuietToilet\bin\Release\QuietToilet.dll`.

The unit tests cover the muting rules and don't need the game:

```
dotnet test tests/QuietToilet.Tests
```

## License

MIT. Copyright (c) 2026 AnExiledDev. See [LICENSE](LICENSE).
