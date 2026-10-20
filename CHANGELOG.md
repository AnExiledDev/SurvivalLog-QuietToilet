# Changelog

## 0.3.1

- Fixed: the sound of using the toilet (`event:/Model/Human/zuoxiajiukaishi`) was not muted. The game plays it for two more toilet actions, "sat down and it starts" and "can't hold it in", so the mod treated it as shared and left it alone. It is now in `ExtraEventPaths` by default.
- An existing config file gets the path added to its `ExtraEventPaths` once, at the first start with this version. Paths you already listed stay. If you remove it afterwards, it stays removed.

## 0.3.0

- Works with the Mod Manager's Mods screen: it shows the mod's name, description, category and links, and links this changelog.
- On the Mods screen, the diagnostics setting sits in the folded Advanced group.
- Exiled's Toolkit 0.4.0 or newer is optional. With it, settings changed on the Mods screen take effect without a restart; without it, at the next game start.
- Built for game version 1.1.18902.

## 0.2.0

- The config file explains every setting in plain English and Simplified Chinese.
- LogSoundsHeard is off by default.
- The Nexus page has a Simplified Chinese description below the English one.

## 0.1.2

First release. Mutes the toilet sound effects and character voice lines. Every other sound plays as normal.
