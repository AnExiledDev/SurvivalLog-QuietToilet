using QuietToilet.Core;
using Xunit;

namespace QuietToilet.Tests;

public class ConfigUpgradeTests
{
    private const string Sound = ConfigUpgrade.ToiletUseSound;

    [Fact]
    public void The_version_comes_from_the_header_BepInEx_writes()
    {
        Assert.Equal(new Version(0, 3, 0), ConfigUpgrade.WrittenBy("## Settings file was created by plugin Quiet Toilet v0.3.0"));
        Assert.Null(ConfigUpgrade.WrittenBy("[Mute]"));
        Assert.Null(ConfigUpgrade.WrittenBy(null));
    }

    [Fact]
    public void A_config_from_an_older_version_gets_the_toilet_sound_but_a_new_or_current_one_does_not()
    {
        Assert.True(ConfigUpgrade.NeedsToiletUseSound(doesFileExist: true, new Version(0, 3, 0)));
        Assert.True(ConfigUpgrade.NeedsToiletUseSound(doesFileExist: true, new Version(0, 1, 2)));
        Assert.True(ConfigUpgrade.NeedsToiletUseSound(doesFileExist: true, writtenBy: null));
        Assert.False(ConfigUpgrade.NeedsToiletUseSound(doesFileExist: false, writtenBy: null));
        Assert.False(ConfigUpgrade.NeedsToiletUseSound(doesFileExist: true, new Version(0, 3, 1)));
        Assert.False(ConfigUpgrade.NeedsToiletUseSound(doesFileExist: true, new Version(0, 4, 0)));
    }

    [Fact]
    public void The_toilet_sound_is_appended_and_the_players_own_paths_stay()
    {
        Assert.Equal(Sound, ConfigUpgrade.AppendPath("", Sound));
        Assert.Equal(Sound, ConfigUpgrade.AppendPath(null, Sound));
        Assert.Equal("event:/a,event:/b," + Sound, ConfigUpgrade.AppendPath("event:/a, event:/b", Sound));
    }

    [Fact]
    public void A_config_that_already_lists_the_toilet_sound_is_not_changed()
    {
        var listed = "event:/a," + Sound.ToUpperInvariant();

        Assert.Equal(listed, ConfigUpgrade.AppendPath(listed, Sound));
    }
}
