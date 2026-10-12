# Quiet Toilet settings

Settings live in `BepInEx\config\com.dev.survivallog.quiettoilet.cfg`, created the first time the game starts with the mod. Edit the file while the game is closed; changes apply at the next game start. Delete the file to get the defaults back.

## Mute

| Setting | Default | What it does |
| --- | --- | --- |
| `ActionKeywords` | `toilet,厕所,马桶,如厕,排便,大便,小便` | An action whose name contains any of these counts as a toilet action. Its sound and its character voice lines are muted. |
| `PathKeywords` | `toilet,poop,fart,matong,cesuo,lashi,dabian,xiaobian` | Any sound whose event path contains one of these is muted, wherever it plays. |
| `ExtraEventPaths` | (empty) | Exact sound event paths to mute as well. |

All three are comma-separated lists. Matching ignores upper and lower case. Spaces around commas are ignored.

`ActionKeywords` is matched against every name the game gives an action, in English and Chinese. Remove a word to stop treating those actions as toilet actions.

`PathKeywords` is plain text matching, so a short keyword can catch more than you meant. If something other than the toilet goes quiet, remove the keyword that caught it (the sound log shows the path).

`ExtraEventPaths` takes full paths such as `event:/SFX/Something/Toilet_Flush`, copied from `BepInEx\quiet-toilet.log`. A path listed here is muted even if other actions share it.

## Diagnostics

| Setting | Default | What it does |
| --- | --- | --- |
| `LogSoundsHeard` | `true` | Writes each distinct sound the first time it plays to `BepInEx\quiet-toilet.log`, so a missed toilet sound can be found. |

Turn it off once everything you want is muted. The log trims itself to its newest part once it passes 1 MB.
