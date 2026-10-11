using System;
using System.Collections.Generic;
using System.Linq;

namespace QuietToilet.Core;

/// <summary>One row of the game's action table: its id, every name it carries, and the sound it plays.</summary>
public sealed record ActionSound(int ActionId, IReadOnlyList<string?> Names, string? FmodPath);

/// <summary>One row of the game's per-character action voice table.</summary>
public sealed record VoiceSound(int ActionId, string? FmodPath);

/// <summary>
/// The toilet sounds found in the game's tables, and the ones left alone because other actions share them, each with
/// the ids of those other actions so the log can say what muting it by hand would also silence.
/// </summary>
public sealed record ToiletSounds(IReadOnlyList<int> ToiletActionIds, IReadOnlySet<string> MutedPaths, IReadOnlyDictionary<string, IReadOnlyList<int>> SharedWith)
{
    /// <summary>Before the game's tables are read, or when they never could be.</summary>
    public static ToiletSounds None { get; } = new(Array.Empty<int>(), new HashSet<string>(StringComparer.OrdinalIgnoreCase), new Dictionary<string, IReadOnlyList<int>>(StringComparer.OrdinalIgnoreCase));
}

/// <summary>
/// The exact paths the gate mutes, and the shared sounds the user's extra paths ruled on: muted anyway, or left alone
/// so the other actions that play them keep their sound. Each carries the ids of those other actions.
/// </summary>
public sealed class MutePlan
{
    internal MutePlan(IEnumerable<string> mutedPaths, IReadOnlyDictionary<string, IReadOnlyList<int>> overridden, IReadOnlyDictionary<string, IReadOnlyList<int>> leftAlone)
    {
        // Built here rather than trusted from the caller: the game writes the same path in more than one case.
        MutedPaths = new HashSet<string>(mutedPaths, StringComparer.OrdinalIgnoreCase);
        Overridden = overridden;
        LeftAlone = leftAlone;
    }

    public IReadOnlySet<string> MutedPaths { get; }

    public IReadOnlyDictionary<string, IReadOnlyList<int>> Overridden { get; }

    public IReadOnlyDictionary<string, IReadOnlyList<int>> LeftAlone { get; }
}

public static class MuteRules
{
    public static IReadOnlyList<string> ParseList(string? csv) =>
        (csv ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public static bool IsToiletAction(ActionSound action, IReadOnlyList<string> actionKeywords) =>
        action.Names.Any(name => ContainsAny(name, actionKeywords));

    /// <summary>
    /// The sounds of every toilet action and their character voices. A sound some other action also uses is
    /// left alone, so muting the toilet never silences anything else.
    /// </summary>
    public static ToiletSounds FindToiletSounds(IReadOnlyList<ActionSound> actions, IReadOnlyList<VoiceSound> voices, IReadOnlyList<string> actionKeywords)
    {
        var toiletIds = actions.Where(action => IsToiletAction(action, actionKeywords)).Select(action => action.ActionId).ToHashSet();
        var sounds = actions.Select(action => (action.ActionId, action.FmodPath)).Concat(voices.Select(voice => (voice.ActionId, voice.FmodPath)))
            .Where(sound => !string.IsNullOrWhiteSpace(sound.FmodPath))
            .ToList();
        var toiletPaths = PathsWhere(sounds, sound => toiletIds.Contains(sound.ActionId));
        var sharedWith = sounds
            .Where(sound => !toiletIds.Contains(sound.ActionId) && toiletPaths.Contains(sound.FmodPath!))
            .GroupBy(sound => sound.FmodPath!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<int>)group.Select(sound => sound.ActionId).Distinct().OrderBy(id => id).ToList(), StringComparer.OrdinalIgnoreCase);

        toiletPaths.ExceptWith(sharedWith.Keys);

        return new ToiletSounds(toiletIds.OrderBy(id => id).ToList(), toiletPaths, sharedWith);
    }

    /// <summary>
    /// Mutes the toilet sounds found plus every extra path the user listed. A listed path other actions share is
    /// muted anyway, because the user named it; a shared path they did not list stays audible.
    /// </summary>
    public static MutePlan Plan(ToiletSounds found, IReadOnlyList<string> extraPaths)
    {
        var extras = new HashSet<string>(extraPaths, StringComparer.OrdinalIgnoreCase);
        var overridden = SharedWhere(found, path => extras.Contains(path));
        var leftAlone = SharedWhere(found, path => !extras.Contains(path));

        return new MutePlan(found.MutedPaths.Concat(extras), overridden, leftAlone);
    }

    /// <summary>
    /// The game fills its tables over several frames and localizes action names after that, so a read only counts
    /// once both tables have rows, the action count held across two polls, and the keywords found a toilet action.
    /// </summary>
    public static bool AreTablesSettled(int? previousActionCount, int actionCount, int voiceCount, int toiletActionCount) =>
        actionCount > 0 && voiceCount > 0 && previousActionCount == actionCount && toiletActionCount > 0;

    /// <summary>A sound is muted when it is a known toilet sound or its event path names one of the keywords.</summary>
    public static bool ShouldMute(string? eventPath, MutePlan plan, IReadOnlyList<string> pathKeywords)
    {
        if (string.IsNullOrWhiteSpace(eventPath)) return false;

        return plan.MutedPaths.Contains(eventPath) || ContainsAny(eventPath, pathKeywords);
    }

    private static HashSet<string> PathsWhere(IEnumerable<(int ActionId, string? FmodPath)> sounds, Func<(int ActionId, string? FmodPath), bool> predicate) =>
        sounds.Where(predicate).Select(sound => sound.FmodPath!).ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyDictionary<string, IReadOnlyList<int>> SharedWhere(ToiletSounds found, Func<string, bool> isPicked) =>
        found.SharedWith.Where(shared => isPicked(shared.Key)).ToDictionary(shared => shared.Key, shared => shared.Value, StringComparer.OrdinalIgnoreCase);

    // A plain loop because this runs on every sound the game plays; a LINQ lambda capturing text would allocate each time.
    private static bool ContainsAny(string? text, IReadOnlyList<string> keywords)
    {
        if (text is null) return false;

        for (var i = 0; i < keywords.Count; i++)
        {
            if (text.Contains(keywords[i], StringComparison.OrdinalIgnoreCase)) return true;
        }

        return false;
    }
}
