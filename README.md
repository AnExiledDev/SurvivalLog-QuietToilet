# Quiet Toilet

Mutes the toilet. Every other sound plays as normal.

Using the toilet in Survival Log plays a sound effect and the character's voice lines. Quiet Toilet silences those, and only those, so you can keep your volume up without hearing it every time someone goes.

## Features

- **Toilet sounds muted**: the sound effect of every toilet action is silenced.
- **Toilet voice lines muted**: the character voices played during a toilet action are silenced too, for every character.
- **Nothing else goes quiet**: the mod reads the game's own action and voice tables when a game loads. If a toilet sound is also used by some other action, it is left alone, so muting the toilet never silences anything else.
- **Keyword backup**: any sound whose event path contains a toilet keyword (such as `toilet` or `poop`) is muted wherever it plays, in case the game adds a new toilet sound in an update.
- **Add your own**: if a toilet sound still gets through, you can name it in the config and it is muted from then on.
- **Sound log**: turn on `LogSoundsHeard` and the mod writes each sound the first time it plays to `BepInEx\quiet-toilet.log`, so a missed sound is easy to find. It is off by default.

Quiet Toilet only changes what you hear. It adds nothing to your save and does not edit game files. You can remove it at any time.

Toilet actions are found by name in English and Chinese, so it works in either language. Every config setting is explained in English and Simplified Chinese.

## How it works

When the game finishes loading its tables, the mod looks for every action whose name contains one of the action keywords (`toilet`, plus the Chinese words for toilet and using the toilet). The sounds and voice lines of those actions are muted. A sound that another, non-toilet action also uses is skipped, and the log says which actions share it.

If you add a shared sound to `ExtraEventPaths` yourself, it is muted anyway, because you asked for it. The log then lists the other actions that go quiet with it. The main toilet sound is one of these: the game also plays it for two other toilet actions whose names have no toilet word, so it is in `ExtraEventPaths` by default.

If the game's tables never finish loading (about two minutes of waiting), the mod falls back to muting by path keywords and your extra paths only, and says so in the log.

## Known issues and limits

- The main toilet sound is muted by default. Any other toilet sound that the game shares with another action stays audible by default. Check `BepInEx\quiet-toilet.log` for lines ending in "left alone because these actions use it too" if you still hear something.
- The path keywords are matched as plain text. A keyword like `poop` mutes any sound with `poop` anywhere in its path, toilet or not.

## Requirements

- Survival Log 1.1.19131. Built for this version; a game update can break it until the mod is updated.
- The [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12), the BepInEx 6 (IL2CPP) build for the game.
- Optional: [Exiled's Toolkit](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit) 0.4.0 or newer. With it, settings changed on the Mod Manager's Mods screen take effect without restarting the game. Without it, they take effect at the next game start.

## Install

1. Install the [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12) (if no other mods were installed before, start the game once so BepInEx finishes setup, then quit).
2. Optional, to change settings without a restart: install Exiled's Toolkit. Download `ExiledToolkit-x.y.z.zip` from [its latest release](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/61), and extract it into the game folder (the folder with `SurvivalLog.exe`).
3. Download `QuietToilet-x.y.z.zip` from the [latest release](https://github.com/AnExiledDev/SurvivalLog-QuietToilet/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/31).
4. Extract the zip into the game folder (the folder with `SurvivalLog.exe`). It holds a `BepInEx` folder, so the DLL lands in the right place. When asked, merge the folders. The result should be:
   `C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\QuietToilet.dll`

## Uninstall

Delete `QuietToilet.dll` from the `BepInEx\plugins` folder. For a clean slate, also delete `BepInEx\config\com.dev.survivallog.quiettoilet.cfg` and `BepInEx\quiet-toilet.log`.

## Configuration

Settings live in `BepInEx\config\com.dev.survivallog.quiettoilet.cfg`, created the first time the game starts with the mod. Change them in game on the Mod Manager's Mods screen, where they take effect at once, or edit the file while the game is closed. See [CONFIG.md](CONFIG.md) for every setting. Without Exiled's Toolkit installed, changes made on the Mods screen take effect at the next game start instead.

## Troubleshooting

Built against BepInEx `6.0.0-be.788`.

Check `BepInEx\LogOutput.log` for `===== Quiet Toilet 0.3.2 started`. Once a game has loaded, a line starting with `read ... actions and ... voices` shows how many toilet actions were found and which sounds are muted. The same lines are in `BepInEx\quiet-toilet.log`, which survives restarts and trims itself past 1 MB. With `LogSoundsHeard` on, it also lists every sound the first time it plays.

To mute a sound that still plays: set `LogSoundsHeard` to `true`, use the toilet in game, open `BepInEx\quiet-toilet.log`, find the event path logged at that moment (it looks like `event:/...`), and add it to `ExtraEventPaths`.

## Feedback and bug reports

Found a bug, have an idea, or something feels off? Come say so on the Core Directive Discord: https://discord.gg/trTsyxUFqg . You can also post in the Posts tab of the mod's Nexus Mods page. When reporting a bug, attach `BepInEx\LogOutput.log` and `BepInEx\quiet-toilet.log`.

To reach me directly, message `exiled.dev` on Discord; you can find me in the [official Survival Log Discord](https://discord.gg/MZGg4gQAUe). I'm also on X as [@AnExiledDev](https://x.com/AnExiledDev).

A Survival Log wiki with detailed documentation for all of my mods is coming soon.

## Build

This is a BepInEx 6 IL2CPP plugin. It compiles against BepInEx and the game's IL2CPP interop assemblies, so a game install with BepInEx set up and started once is required. Those assemblies come from the game and are not part of this repo. The .NET 8 SDK is required.

```
dotnet build src/QuietToilet -c Release -p:GameDir="C:\path\to\Survival Log"
```

`GameDir` is the game folder. The default in the project file is a local path you will almost certainly need to override. The output DLL is at `src\QuietToilet\bin\Release\QuietToilet.dll`.

The unit tests cover the pure muting rules and need nothing from the game:

```
dotnet test tests/QuietToilet.Tests
```

## License

MIT. Copyright (c) 2026 AnExiledDev. See [LICENSE](LICENSE).
