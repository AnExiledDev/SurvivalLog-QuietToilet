using System.Collections.Generic;
using GameCore.HotUpdate;
using QuietToilet.Core;

namespace QuietToilet.Adapters;

/// <summary>Reads the game's action and action-voice tables once its config manager has loaded them.</summary>
internal static class GameTables
{
    /// <summary>Null until the game has loaded rows into both tables.</summary>
    public static (List<ActionSound> Actions, List<VoiceSound> Voices)? TryRead()
    {
        if (!BaseSingleton<ConfigManager>.IsInstanceCreated) return null;

        var config = BaseSingleton<ConfigManager>.Instance;
        var actionTable = config?._Config_Action_Dict;
        var voiceTable = config?._Config_ActionVoice_Dict;

        if (actionTable == null || actionTable.Count == 0 || voiceTable == null || voiceTable.Count == 0) return null;

        var actions = new List<ActionSound>();
        var voices = new List<VoiceSound>();

        foreach (var entry in actionTable)
        {
            var action = entry.Value;

            if (action != null) actions.Add(new ActionSound(action.ID, new[] { action.Name, action.Name_Local, action.NameDoing, action.NameDoing_Local }, action.FmodPath));
        }

        foreach (var entry in voiceTable)
        {
            if (entry.Value != null) voices.Add(new VoiceSound(entry.Value.ActionID, entry.Value.FmodPath));
        }

        return (actions, voices);
    }
}
