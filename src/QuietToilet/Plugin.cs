using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using ExiledToolkit;
using HarmonyLib;
using QuietToilet.Adapters;
using QuietToilet.Core;

namespace QuietToilet;

[BepInPlugin(Guid, "Quiet Toilet", Version)]
[BepInDependency(ExiledToolkit.Plugin.Guid, ">=0.4.0")]
public sealed class Plugin : BasePlugin
{
    private const string Guid = "com.dev.survivallog.quiettoilet";
    private const string Version = "0.3.0";

    public override void Load()
    {
        var actionKeywords = Config.Bind("Mute", "ActionKeywords", "toilet,厕所,马桶,如厕,排便,大便,小便",
            "Any action whose name contains one of these words counts as using the toilet. Its sound effects and character voice lines are muted, unless another action uses the same sound. Separate words with commas.\n" +
            "名称中含有以下任一词语的动作都算作上厕所，这些动作的音效和角色语音都会被静音，除非其他动作也使用同一个声音。多个词语用英文逗号分隔。");
        var pathKeywords = Config.Bind("Mute", "PathKeywords", "toilet,poop,fart,matong,cesuo,lashi,dabian,xiaobian",
            "Any sound whose event path contains one of these words is muted, wherever it plays. Separate words with commas.\n" +
            "事件路径中含有以下任一词语的声音，无论在哪里播放都会被静音。多个词语用英文逗号分隔。");
        var extraPaths = Config.Bind("Mute", "ExtraEventPaths", "",
            "Exact sound event paths to mute as well, separated by commas. To find a path, set LogSoundsHeard to true, play the sound in game, then copy its path (it starts with event:/) from BepInEx/quiet-toilet.log.\n" +
            "额外要静音的完整声音事件路径，用英文逗号分隔。查找方法：把 LogSoundsHeard 设为 true，在游戏中让这个声音播放一次，然后从 BepInEx/quiet-toilet.log 中复制它的路径（以 event:/ 开头）。");
        var isLoggingHeard = Config.Bind("Diagnostics", "LogSoundsHeard", false,
            "When true, each sound is written to BepInEx/quiet-toilet.log the first time it plays, marked as muted or heard. Turn it on to find a toilet sound that still plays.\n" +
            "设为 true 时，每个声音第一次播放时都会写入 BepInEx/quiet-toilet.log，并注明已静音或未静音。想找出仍在播放的厕所声音时再打开。");

        var log = new QuietLog(Log, Path.Combine(Paths.BepInExRootPath, "quiet-toilet.log"));

        log.TrimIfLarge();
        log.Write($"===== Quiet Toilet {Version} started");
        QuietTicker.Log = log.Write;
        SoundGate.Apply(new Harmony(Guid), log.Write, log.WriteFileOnly);
        LiveSettings.Watch(() => SoundGate.Configure(MuteRules.ParseList(pathKeywords.Value), isLoggingHeard.Value), pathKeywords, isLoggingHeard);
        LiveSettings.Watch(() => UseToiletWords(MuteRules.ParseList(actionKeywords.Value), MuteRules.ParseList(extraPaths.Value)), actionKeywords, extraPaths);
        AddComponent<QuietTicker>();
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
