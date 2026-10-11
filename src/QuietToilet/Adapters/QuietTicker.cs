using System;
using System.Collections.Generic;
using System.Linq;
using QuietToilet.Core;
using UnityEngine;

namespace QuietToilet.Adapters;

/// <summary>Waits for the game's tables to load, then hands the toilet sounds found in them to the sound gate.</summary>
public sealed class QuietTicker : MonoBehaviour
{
    private const float PollSeconds = 2f;
    private const int MaxPolls = 60;
    private const int MaxLoggedFailures = 3;

    internal static IReadOnlyList<string> ActionKeywords = Array.Empty<string>();
    internal static IReadOnlyList<string> ExtraPaths = Array.Empty<string>();
    internal static Action<string> Log = _ => { };
    private float _nextPoll;
    private int _polls;
    private int _failures;
    private int? _lastActionCount;

    public QuietTicker(IntPtr pointer) : base(pointer)
    {
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextPoll) return;

        _nextPoll = Time.unscaledTime + PollSeconds;
        _polls++;

        if (TryAdopt())
        {
            Finish();
        }
        else if (_polls >= MaxPolls)
        {
            Log($"the game's action tables never settled after {MaxPolls} polls, muting by path keywords and ExtraEventPaths only");
            Finish();
        }
    }

    // Disabled so Unity stops calling Update, which crosses from native into managed code every frame.
    private void Finish() => enabled = false;

    private bool TryAdopt()
    {
        try
        {
            var previousActionCount = _lastActionCount;
            var tables = GameTables.TryRead();

            _lastActionCount = tables?.Actions.Count;

            if (tables is not { } read) return false;

            var found = MuteRules.FindToiletSounds(read.Actions, read.Voices, ActionKeywords);

            if (!MuteRules.AreTablesSettled(previousActionCount, read.Actions.Count, read.Voices.Count, found.ToiletActionIds.Count)) return false;

            Adopt(found, read.Actions, read.Voices.Count);

            return true;
        }
        catch (Exception ex)
        {
            _lastActionCount = null;
            _failures++;

            if (_failures <= MaxLoggedFailures) Log($"could not read the game's action tables yet, retrying (attempt {_failures}): {ex}");

            return false;
        }
    }

    private static void Adopt(ToiletSounds found, IReadOnlyList<ActionSound> actions, int voiceCount)
    {
        var plan = MuteRules.Plan(found, ExtraPaths);

        SoundGate.MuteKnownPaths(plan);

        foreach (var action in actions.Where(action => found.ToiletActionIds.Contains(action.ActionId)))
        {
            Log($"toilet action {action.ActionId}: {NamesOf(action)} sound={action.FmodPath ?? "none"}");
        }

        Log($"read {actions.Count} actions and {voiceCount} voices; {found.ToiletActionIds.Count} toilet actions; muting [{string.Join(", ", plan.MutedPaths)}]");

        foreach (var (path, otherIds) in plan.Overridden)
        {
            Log($"{path} muted anyway via ExtraEventPaths, so these go quiet too: {Describe(actions, otherIds)}");
        }

        foreach (var (path, otherIds) in plan.LeftAlone)
        {
            Log($"{path} left alone because these actions use it too: {Describe(actions, otherIds)}");
        }
    }

    private static string Describe(IReadOnlyList<ActionSound> actions, IReadOnlyList<int> ids) =>
        string.Join("; ", actions.Where(action => ids.Contains(action.ActionId)).Select(action => $"{action.ActionId} ({NamesOf(action)})"));

    private static string NamesOf(ActionSound action) => string.Join(" | ", action.Names.Where(name => !string.IsNullOrEmpty(name)));
}
