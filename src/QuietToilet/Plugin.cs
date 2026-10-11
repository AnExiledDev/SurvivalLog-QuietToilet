using System.IO;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using QuietToilet.Adapters;
using QuietToilet.Core;

namespace QuietToilet;

[BepInPlugin(Guid, "Quiet Toilet", Version)]
public sealed class Plugin : BasePlugin
{
    private const string Guid = "com.dev.survivallog.quiettoilet";
    private const string Version = "0.1.2";

    public override void Load()
    {
        var actionKeywords = Config.Bind("Mute", "ActionKeywords", "toilet,厕所,马桶,如厕,排便,大便,小便",
            "An action whose name contains any of these is a toilet action; its sound and its character voices are muted.").Value;
        var pathKeywords = Config.Bind("Mute", "PathKeywords", "toilet,poop,fart,matong,cesuo,lashi,dabian,xiaobian",
            "Any sound whose FMOD event path contains one of these is muted, wherever it plays.").Value;
        var extraPaths = Config.Bind("Mute", "ExtraEventPaths", "",
            "Exact FMOD event paths to mute as well, comma separated (copy them from BepInEx/quiet-toilet.log).").Value;
        var isLoggingHeard = Config.Bind("Diagnostics", "LogSoundsHeard", true,
            "Log each distinct sound the first time it plays, to BepInEx/quiet-toilet.log, so a missed toilet sound can be found.").Value;

        var log = new QuietLog(Log, Path.Combine(Paths.BepInExRootPath, "quiet-toilet.log"));
        var extras = MuteRules.ParseList(extraPaths);

        log.TrimIfLarge();
        log.Write($"===== Quiet Toilet {Version} started");
        QuietTicker.ActionKeywords = MuteRules.ParseList(actionKeywords);
        QuietTicker.ExtraPaths = extras;
        QuietTicker.Log = log.Write;
        SoundGate.Apply(new Harmony(Guid), MuteRules.ParseList(pathKeywords), isLoggingHeard, log.Write, log.WriteFileOnly);
        SoundGate.MuteKnownPaths(MuteRules.Plan(ToiletSounds.None, extras));
        AddComponent<QuietTicker>();
    }
}
