using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using QuietToilet.Adapters;
using QuietToilet.Core;

namespace QuietToilet;

[BepInPlugin(Guid, "Quiet Toilet", Version)]
[BepInDependency(OptionalLive.ToolkitGuid, BepInDependency.DependencyFlags.SoftDependency)]
public sealed class Plugin : BasePlugin
{
    private const string Guid = "com.dev.survivallog.quiettoilet";
    private const string Version = "0.3.1";

    public override void Load()
    {
        if (!OptionalLive.IsToolkitInstalled) Log.LogInfo(OptionalLive.MissingNote);

        // Read before the first Bind: every Bind saves the file and rewrites this line with the current version.
        var configPath = Path.Combine(Paths.ConfigPath, Guid + ".cfg");
        var doesConfigExist = File.Exists(configPath);
        var configWrittenBy = doesConfigExist ? ConfigUpgrade.WrittenBy(ReadFirstLine(configPath)) : null;

        var actionKeywords = Config.Bind("Mute", "ActionKeywords", "toilet,厕所,马桶,如厕,排便,大便,小便",
            "Any action whose name contains one of these words counts as using the toilet. Its sound effects and character voice lines are muted, unless another action uses the same sound. Separate words with commas.\n" +
            "名称中含有以下任一词语的动作都算作上厕所，这些动作的音效和角色语音都会被静音，除非其他动作也使用同一个声音。多个词语用英文逗号分隔。");
        var pathKeywords = Config.Bind("Mute", "PathKeywords", "toilet,poop,fart,matong,cesuo,lashi,dabian,xiaobian",
            "Any sound whose event path contains one of these words is muted, wherever it plays. Separate words with commas.\n" +
            "事件路径中含有以下任一词语的声音，无论在哪里播放都会被静音。多个词语用英文逗号分隔。");
        var extraPaths = Config.Bind("Mute", "ExtraEventPaths", ConfigUpgrade.ToiletUseSound,
            "Exact sound event paths to mute as well, separated by commas. To find a path, set LogSoundsHeard to true, play the sound in game, then copy its path (it starts with event:/) from BepInEx/quiet-toilet.log.\n" +
            "额外要静音的完整声音事件路径，用英文逗号分隔。查找方法：把 LogSoundsHeard 设为 true，在游戏中让这个声音播放一次，然后从 BepInEx/quiet-toilet.log 中复制它的路径（以 event:/ 开头）。");
        var isLoggingHeard = Config.Bind("Diagnostics", "LogSoundsHeard", false,
            new ConfigDescription("When true, each sound is written to BepInEx/quiet-toilet.log the first time it plays, marked as muted or heard. Turn it on to find a toilet sound that still plays.\n" +
            "设为 true 时，每个声音第一次播放时都会写入 BepInEx/quiet-toilet.log，并注明已静音或未静音。想找出仍在播放的厕所声音时再打开。", null, "ModManager.Advanced"));

        var log = new QuietLog(Log, Path.Combine(Paths.BepInExRootPath, "quiet-toilet.log"));

        log.TrimIfLarge();
        log.Write($"===== Quiet Toilet {Version} started");

        if (ConfigUpgrade.NeedsToiletUseSound(doesConfigExist, configWrittenBy)) AddToiletUseSound(extraPaths, log);

        QuietTicker.Log = log.Write;
        SoundGate.Apply(new Harmony(Guid), log.Write, log.WriteFileOnly);
        OptionalLive.Watch(() => SoundGate.Configure(MuteRules.ParseList(pathKeywords.Value), isLoggingHeard.Value), pathKeywords, isLoggingHeard);
        OptionalLive.Watch(() => UseToiletWords(MuteRules.ParseList(actionKeywords.Value), MuteRules.ParseList(extraPaths.Value)), actionKeywords, extraPaths);
        AddComponent<QuietTicker>();
    }

    /// <summary>Null when the file cannot be read, which counts as an old file; the mod must still start.</summary>
    private static string? ReadFirstLine(string path)
    {
        try
        {
            return File.ReadLines(path).FirstOrDefault();
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <summary>Appended rather than reset, so paths the player listed in an older version stay.</summary>
    private static void AddToiletUseSound(ConfigEntry<string> extraPaths, QuietLog log)
    {
        var upgraded = ConfigUpgrade.AppendPath(extraPaths.Value, ConfigUpgrade.ToiletUseSound);

        if (upgraded == extraPaths.Value) return;

        extraPaths.Value = upgraded;
        log.Write($"added {ConfigUpgrade.ToiletUseSound} to ExtraEventPaths in the existing config");
    }

    /// <summary>At start this only sets the words; after a change the tables are read again with them.</summary>
    private static void UseToiletWords(IReadOnlyList<string> actionKeywords, IReadOnlyList<string> extraPaths)
    {
        QuietTicker.ActionKeywords = actionKeywords;
        QuietTicker.ExtraPaths = extraPaths;
        SoundGate.MuteKnownPaths(MuteRules.Plan(ToiletSounds.None, extraPaths));
        QuietTicker.Rescan();
    }
}
