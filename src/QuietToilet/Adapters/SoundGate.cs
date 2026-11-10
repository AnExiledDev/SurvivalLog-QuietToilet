using System;
using System.Collections.Generic;
using System.Reflection;
using ExiledToolkit.Game;
using FMODUnity;
using GameCore.HotUpdate;
using QuietToilet.Core;
using UnityEngine;

namespace QuietToilet.Adapters;

/// <summary>
/// Harmony hooks on every way the game starts a sound by event path. A muted sound never starts; one started
/// through the instance cache gets its volume set to zero instead. Each hook swallows its own exceptions:
/// a sound mod must never be the crash.
/// </summary>
internal static class SoundGate
{
    // What each hook was doing when it failed; each is logged once, so a failure on every sound cannot flood the log.
    private const string SilencingCachedSound = "muting a sound from the instance cache";
    private const string CheckingSound = "checking a sound";

    // Main thread only: FMOD play calls and plan changes all arrive on Unity's main thread, so neither map is locked.
    private static readonly Dictionary<string, bool> Verdicts = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, bool> Reported = new(StringComparer.Ordinal);
    private static MutePlan _plan = MuteRules.Plan(ToiletSounds.None, Array.Empty<string>());
    private static IReadOnlyList<string> _pathKeywords = Array.Empty<string>();
    private static bool _isLoggingHeard;
    private static PatchKit? _hooks;
    private static Action<string> _log = _ => { };
    private static Action<string> _logFileOnly = _ => { };

    public static void Apply(PatchKit hooks, Action<string> log, Action<string> logFileOnly)
    {
        _hooks = hooks;
        _log = log;
        _logFileOnly = logFileOnly;

        foreach (var name in new[] { "Play2dAction", "PlayBattle3dAction", "PlayBattle3dActionFollow", "PlayEnv3dAction" })
        {
            Patch(hooks, typeof(FmodManager), name, null, prefix: nameof(SkipIfMuted));
        }

        Patch(hooks, typeof(FmodManager), "GetOrCreateInstance", null, postfix: nameof(SilenceIfMuted));
        Patch(hooks, typeof(RuntimeManager), "PlayOneShot", new[] { typeof(string), typeof(Vector3) }, prefix: nameof(SkipIfMuted));
        Patch(hooks, typeof(RuntimeManager), "PlayOneShotAttached", new[] { typeof(string), typeof(GameObject) }, prefix: nameof(SkipIfMuted));
    }

    /// <summary>The player's path keywords and log switch, at start and after every change; sounds are judged again.</summary>
    public static void Configure(IReadOnlyList<string> pathKeywords, bool isLoggingHeard)
    {
        _pathKeywords = pathKeywords;
        _isLoggingHeard = isLoggingHeard;
        Verdicts.Clear();
        Reported.Clear();
    }

    /// <summary>Cached verdicts are dropped, or a toilet sound judged audible before the tables loaded would stay audible.</summary>
    public static void MuteKnownPaths(MutePlan plan)
    {
        _plan = plan;
        Verdicts.Clear();
    }

    private static void Patch(PatchKit hooks, Type owner, string target, Type[]? parameters, string? prefix = null, string? postfix = null)
    {
        hooks.Patch(typeof(SoundGate), owner, target, parameters, prefix: prefix, postfix: postfix, done: $"hooked {owner.Name}.{target}");
    }

    /// <summary>Prefix for methods whose first argument is the event path. False skips the sound.</summary>
    private static bool SkipIfMuted(string __0, MethodBase __originalMethod) => !Judge(__0, __originalMethod.Name);

    private static void SilenceIfMuted(FmodCache __result, string __0)
    {
        try
        {
            if (__result == null || !Judge(__0, "GetOrCreateInstance")) return;

            __result.Instance.setVolume(0f);
        }
        catch (Exception ex)
        {
            _hooks?.Fail(SilencingCachedSound, ex);
        }
    }

    private static bool Judge(string? eventPath, string via)
    {
        try
        {
            if (eventPath is null) return false;
            if (Verdicts.TryGetValue(eventPath, out var isCachedMuted)) return isCachedMuted;

            var isMuted = MuteRules.ShouldMute(eventPath, _plan, _pathKeywords);

            Verdicts[eventPath] = isMuted;
            Report(eventPath, isMuted, via);

            return isMuted;
        }
        catch (Exception ex)
        {
            _hooks?.Fail(CheckingSound, ex);

            return false;
        }
    }

    /// <summary>
    /// Logs a path the first time it plays and again whenever its verdict flips, so a sound first heard before the
    /// tables loaded and muted after still gets its MUTED line. Heard lines go to the file only; there are dozens.
    /// </summary>
    private static void Report(string eventPath, bool isMuted, string via)
    {
        if (!_isLoggingHeard) return;
        if (Reported.TryGetValue(eventPath, out var wasMuted) && wasMuted == isMuted) return;

        Reported[eventPath] = isMuted;

        if (isMuted) _log($"MUTED {eventPath} (via {via})");
        else _logFileOnly($"heard {eventPath} (via {via})");
    }
}
