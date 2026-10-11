using QuietToilet.Core;
using Xunit;

namespace QuietToilet.Tests;

public class MuteRulesTests
{
    private static readonly string[] ActionKeywords = { "toilet", "厕所" };

    private static ActionSound Action(int id, string name, string? fmod) => new(id, new[] { name, null }, fmod);

    private static readonly ActionSound Toilet = Action(10, "E_Action_Furniture_Bar_Toilet", "event:/Model/Human/toilet_use");
    private static readonly ActionSound ToiletAuto = Action(11, "E_Action_Furniture_Bar_Toilet_Auto", "event:/Model/Human/toilet_use");
    private static readonly ActionSound Sleep = Action(20, "E_Action_Furniture_Bed_Sleep", "event:/Model/Human/snore");
    private static readonly ActionSound SitDown = Action(40, "E_Action_Sit", "event:/Model/Human/sit");
    private static readonly ActionSound ToiletSits = Action(12, "E_Action_Toilet_Sit", "event:/Model/Human/SIT");
    private static readonly string[] NoPathKeywords = Array.Empty<string>();

    [Fact]
    public void A_toilet_action_is_found_by_any_of_its_names_in_any_case()
    {
        var localizedOnly = new ActionSound(30, new string?[] { "E_Action_30", "上厕所" }, "event:/x");

        Assert.True(MuteRules.IsToiletAction(Toilet, ActionKeywords));
        Assert.True(MuteRules.IsToiletAction(localizedOnly, ActionKeywords));
        Assert.False(MuteRules.IsToiletAction(Sleep, ActionKeywords));
    }

    [Fact]
    public void The_toilet_actions_sound_and_their_character_voices_are_muted()
    {
        var voices = new[] { new VoiceSound(10, "event:/Voice/Anna/strain"), new VoiceSound(20, "event:/Voice/Anna/yawn") };

        var found = MuteRules.FindToiletSounds(new[] { Toilet, ToiletAuto, Sleep }, voices, ActionKeywords);

        Assert.Equal(new[] { 10, 11 }, found.ToiletActionIds);
        Assert.True(found.MutedPaths.SetEquals(new[] { "event:/Model/Human/toilet_use", "event:/Voice/Anna/strain" }));
    }

    [Fact]
    public void A_sound_another_action_also_plays_is_left_alone_so_nothing_else_goes_quiet()
    {
        var sitDown = Action(40, "E_Action_Sit", "event:/Model/Human/sit");
        var toiletSits = Action(12, "E_Action_Toilet_Sit", "event:/Model/Human/SIT");

        var found = MuteRules.FindToiletSounds(new[] { Toilet, toiletSits, sitDown }, Array.Empty<VoiceSound>(), ActionKeywords);

        Assert.DoesNotContain("event:/Model/Human/sit", found.MutedPaths);
        Assert.Equal(new[] { 40 }, found.SharedWith["event:/Model/Human/SIT"]);
        Assert.Contains("event:/Model/Human/toilet_use", found.MutedPaths);
    }

    [Fact]
    public void Actions_without_a_sound_add_nothing()
    {
        var silentToilet = Action(13, "Toilet_Flush", "  ");

        var found = MuteRules.FindToiletSounds(new[] { silentToilet }, new[] { new VoiceSound(13, null) }, ActionKeywords);

        Assert.Empty(found.MutedPaths);
    }

    [Fact]
    public void A_known_toilet_path_or_a_path_naming_a_keyword_is_muted_and_nothing_else()
    {
        var known = MuteRules.Plan(ToiletSounds.None, new[] { "event:/Model/Human/toilet_use" });
        var pathKeywords = new[] { "matong" };

        Assert.True(MuteRules.ShouldMute("EVENT:/Model/Human/Toilet_Use", known, pathKeywords));
        Assert.True(MuteRules.ShouldMute("event:/Model/Object/matong_chongshui", known, pathKeywords));
        Assert.False(MuteRules.ShouldMute("event:/Model/Human/snore", known, pathKeywords));
        Assert.False(MuteRules.ShouldMute(null, known, pathKeywords));
        Assert.False(MuteRules.ShouldMute("", known, new[] { "" }));
    }

    [Fact]
    public void A_shared_path_the_user_lists_is_muted_anyway_and_reported_as_overridden()
    {
        var found = MuteRules.FindToiletSounds(new[] { Toilet, ToiletSits, SitDown }, Array.Empty<VoiceSound>(), ActionKeywords);

        var plan = MuteRules.Plan(found, new[] { "event:/Model/Human/sit" });

        Assert.True(MuteRules.ShouldMute("event:/Model/Human/sit", plan, NoPathKeywords));
        Assert.Equal(new[] { 40 }, plan.Overridden["event:/Model/Human/sit"]);
        Assert.Empty(plan.LeftAlone);
    }

    [Fact]
    public void A_shared_path_the_user_did_not_list_keeps_playing_while_the_toilet_sound_goes_quiet()
    {
        var found = MuteRules.FindToiletSounds(new[] { Toilet, ToiletSits, SitDown }, Array.Empty<VoiceSound>(), ActionKeywords);

        var plan = MuteRules.Plan(found, Array.Empty<string>());

        Assert.False(MuteRules.ShouldMute("event:/Model/Human/sit", plan, NoPathKeywords));
        Assert.True(MuteRules.ShouldMute("event:/Model/Human/toilet_use", plan, NoPathKeywords));
        Assert.Equal(new[] { 40 }, plan.LeftAlone["event:/Model/Human/sit"]);
        Assert.Empty(plan.Overridden);
    }

    [Fact]
    public void Muted_paths_match_in_any_case_even_when_the_found_set_was_case_sensitive()
    {
        var caseSensitive = new HashSet<string>(StringComparer.Ordinal) { "event:/Model/Human/toilet_use" };
        var found = new ToiletSounds(new[] { 10 }, caseSensitive, new Dictionary<string, IReadOnlyList<int>> { ["event:/Model/Human/sit"] = new[] { 40 } });

        var plan = MuteRules.Plan(found, new[] { "EVENT:/MODEL/HUMAN/SIT" });

        Assert.True(MuteRules.ShouldMute("EVENT:/Model/Human/TOILET_USE", plan, NoPathKeywords));
        Assert.True(MuteRules.ShouldMute("event:/Model/Human/sit", plan, NoPathKeywords));
        Assert.Contains("event:/Model/Human/sit", plan.Overridden.Keys);
    }

    [Fact]
    public void Tables_count_as_loaded_only_once_both_have_rows_the_action_count_held_and_a_toilet_action_was_found()
    {
        Assert.True(MuteRules.AreTablesSettled(previousActionCount: 500, actionCount: 500, voiceCount: 80, toiletActionCount: 2));
        Assert.False(MuteRules.AreTablesSettled(previousActionCount: null, actionCount: 500, voiceCount: 80, toiletActionCount: 2));
        Assert.False(MuteRules.AreTablesSettled(previousActionCount: 480, actionCount: 500, voiceCount: 80, toiletActionCount: 2));
        Assert.False(MuteRules.AreTablesSettled(previousActionCount: 500, actionCount: 500, voiceCount: 0, toiletActionCount: 2));
        Assert.False(MuteRules.AreTablesSettled(previousActionCount: 500, actionCount: 500, voiceCount: 80, toiletActionCount: 0));
    }

    [Fact]
    public void A_config_list_ignores_blanks_and_spacing()
    {
        Assert.Equal(new[] { "toilet", "厕所" }, MuteRules.ParseList(" toilet , ,厕所,"));
        Assert.Empty(MuteRules.ParseList(null));
    }
}
