using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuietToilet.Core;

/// <summary>
/// Brings a config file written by an older version up to date without touching anything the player set. BepInEx
/// keeps a saved value over a new default, so a sound added to the default list would never reach an existing file.
/// </summary>
public static class ConfigUpgrade
{
    /// <summary>
    /// The sound of using the toilet. Two other toilet actions ("sat down and it starts", "can't hold it in") play it
    /// too, and their names carry no toilet word, so the shared-sound rule would leave it audible without this.
    /// </summary>
    public const string ToiletUseSound = "event:/Model/Human/zuoxiajiukaishi";

    /// <summary>The first version that ships <see cref="ToiletUseSound"/> in ExtraEventPaths.</summary>
    public static readonly Version ToiletUseSoundAdded = new(0, 3, 1);

    private static readonly Regex HeaderVersion = new(@"\bv(\d+\.\d+(?:\.\d+)?)\s*$");

    /// <summary>
    /// The version named in the file's first line ("## Settings file was created by plugin Quiet Toilet v0.3.0"),
    /// which BepInEx rewrites on every save. Null when there is no file or the line names no version.
    /// </summary>
    public static Version? WrittenBy(string? firstLine)
    {
        var match = HeaderVersion.Match(firstLine ?? "");

        return match.Success && Version.TryParse(match.Groups[1].Value, out var version) ? version : null;
    }

    /// <summary>
    /// A file from before <see cref="ToiletUseSoundAdded"/> gets the sound once. A new file already has it as the
    /// default, and a file saved by this version or later is left alone, so a player who removed it keeps it removed.
    /// A file whose version cannot be read is treated as old: adding a path it already holds changes nothing.
    /// </summary>
    public static bool NeedsToiletUseSound(bool doesFileExist, Version? writtenBy) =>
        doesFileExist && (writtenBy is null || writtenBy < ToiletUseSoundAdded);

    /// <summary>The list with <paramref name="path"/> added at the end, unless it is already there in any case.</summary>
    public static string AppendPath(string? csv, string path)
    {
        var paths = MuteRules.ParseList(csv);

        if (paths.Contains(path, StringComparer.OrdinalIgnoreCase)) return csv ?? "";

        return string.Join(",", paths.Append(path));
    }
}
