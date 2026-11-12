# Changelog

## 0.3.3

- Now needs Exiled's Toolkit 0.19.0 or newer. Settings you change on the Mods screen always take effect right away.
- Once `BepInEx\quiet-toilet.log` passes 1 MB, it's now moved to `quiet-toilet.log.1` and a new one starts.
- Made for Survival Log 1.1.20719.

## 0.3.2

- After a game update, Mod Manager can tell you if this mod may need an update.
- Made for Survival Log 1.1.19131.

## 0.3.1

- Fixed: the main sound of using the toilet wasn't muted. The game also plays it for two other toilet actions, so the mod left it alone. It's now in ExtraEventPaths, so it's muted by default.
- If you already have a config file, the main toilet sound is added to its ExtraEventPaths once. Sounds you listed stay, and if you remove it later, it stays removed.

## 0.3.0

- Mod Manager's Mods screen shows the mod's name, description, category, links and this changelog.
- On the Mods screen, LogSoundsHeard is in the Advanced group, which starts collapsed.
- Exiled's Toolkit 0.4.0 or newer is optional. With it, settings you change on the Mods screen take effect right away. Without it, they take effect the next time you start the game.
- Built for game version 1.1.18902.

## 0.2.0

- The config file explains every setting in plain English and Simplified Chinese.
- LogSoundsHeard is off by default.
- The Nexus page has a Simplified Chinese description below the English one.

## 0.1.2

- First release.
- Mutes the toilet sound effects and the voice lines that go with them. Every other sound plays as normal.
