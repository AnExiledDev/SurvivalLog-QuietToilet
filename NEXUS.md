Name: Quiet Toilet
Summary: Mutes the toilet sound effects and voice lines. Every other sound plays as normal. 静音上厕所的音效和角色语音，其他声音照常播放。

Description (BBCode):
Tired of hearing it every time someone uses the toilet? Quiet Toilet mutes the toilet sounds and the voice lines that go with them. Nothing else gets quieter.

[size=5]Main features[/size]
[list]
[*][b]Toilet sounds muted[/b]: the sound effect of every toilet action is silenced.[/*]
[*][b]Voice lines muted[/b]: character voices during a toilet action are silenced too.[/*]
[*][b]Nothing else goes quiet[/b]: if the game reuses a toilet sound for something else, that sound is left alone.[/*]
[*][b]Add your own[/b]: name any sound that still gets through in the config and it is muted.[/*]
[/list]
Quiet Toilet only changes what you hear. It adds nothing to your save and does not edit game files. You can remove it at any time.

Works in English and Chinese.

[size=5]Known issues[/size]
A toilet sound that the game also uses for another action stays on by default. Turn on [font=Courier New]LogSoundsHeard[/font] in the config and the mod writes every sound it hears to [font=Courier New]BepInEx\quiet-toilet.log[/font], so you can find that sound and add it in the config. If you run into anything strange, please tell me (see Feedback below).

[size=5]Requirements[/size]
[list]
[*]Survival Log 1.1.18902. A game update can break it until the mod is updated.[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]. Install it first.[/*]
[/list]

[size=5]Installation[/size]
[list=1]
[*]Install [url=https://www.nexusmods.com/survivallog/mods/12]BepInEx[/url] [i](if no other mods were installed before, start the game once so BepInEx finishes setup, then quit)[/i].[/*]
[*]Put [font=Courier New]QuietToilet.dll[/font] in the BepInEx\plugins folder inside the game folder. Full path example:
[code]C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\QuietToilet.dll[/code][/*]
[/list]

[size=5]Uninstall[/size]
Delete [font=Courier New]QuietToilet.dll[/font] from the [font=Courier New]BepInEx\plugins[/font] folder.

[size=5]Configuration[/size]
To change settings, edit [font=Courier New]BepInEx\config\com.dev.survivallog.quiettoilet.cfg[/font] while the game is closed (see [url=https://github.com/AnExiledDev/SurvivalLog-QuietToilet/blob/main/CONFIG.md]CONFIG.md[/url] for every setting). Changes apply at the next game start.

[size=5]Feedback[/size]
Found a bug or have an idea? Come say hi on the [url=https://discord.gg/trTsyxUFqg]Core Directive Discord[/url], or leave a comment here. For bugs, please include [font=Courier New]BepInEx\LogOutput.log[/font] and [font=Courier New]BepInEx\quiet-toilet.log[/font].

[size=5]Source and license[/size]
Open source under MIT: [url=https://github.com/AnExiledDev/SurvivalLog-QuietToilet]github.com/AnExiledDev/SurvivalLog-QuietToilet[/url]

[line]
[size=5]简体中文[/size]

每次有人上厕所都要听一遍？Quiet Toilet 会把上厕所的音效和随之而来的角色语音静音，其他声音不受任何影响。

[size=5]主要功能[/size]
[list]
[*][b]厕所音效静音[/b]：所有上厕所动作的音效都会被静音。[/*]
[*][b]角色语音静音[/b]：上厕所时角色的语音也会被静音。[/*]
[*][b]其他声音照常[/b]：如果游戏把某个厕所音效也用在别的动作上，这个声音会保留。[/*]
[*][b]可自行添加[/b]：仍能听到的声音，只要在配置文件里写上它的名称就会被静音。[/*]
[/list]
Quiet Toilet 只改变你听到的声音，不会往存档里添加任何内容，也不会修改游戏文件，随时可以移除。

支持英文和中文。

[size=5]已知问题[/size]
游戏同时用于其他动作的厕所音效默认不会被静音。在配置中打开 [font=Courier New]LogSoundsHeard[/font] 后，模组会把听到的每个声音写入 [font=Courier New]BepInEx\quiet-toilet.log[/font]，你可以从中找到这个声音并把它加到配置里。遇到任何异常，欢迎告诉我（见下方「反馈」）。

[size=5]前置要求[/size]
[list]
[*]Survival Log 1.1.18902。游戏更新后模组可能暂时失效，直到模组更新为止。[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]，请先安装。[/*]
[/list]

[size=5]安装[/size]
[list=1]
[*]安装 [url=https://www.nexusmods.com/survivallog/mods/12]BepInEx[/url] [i]（如果之前没有装过其他模组，先启动一次游戏，让 BepInEx 完成初始化，然后退出）[/i]。[/*]
[*]把 [font=Courier New]QuietToilet.dll[/font] 放进游戏目录下的 BepInEx\plugins 文件夹。完整路径示例：
[code]C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\QuietToilet.dll[/code][/*]
[/list]

[size=5]卸载[/size]
从 [font=Courier New]BepInEx\plugins[/font] 文件夹中删除 [font=Courier New]QuietToilet.dll[/font]。

[size=5]配置[/size]
如需修改设置，请在游戏关闭时编辑 [font=Courier New]BepInEx\config\com.dev.survivallog.quiettoilet.cfg[/font]。配置文件中每项设置都有中文说明，完整说明见 [url=https://github.com/AnExiledDev/SurvivalLog-QuietToilet/blob/main/CONFIG.md]CONFIG.md[/url]（英文）。修改会在下次启动游戏时生效。

[size=5]反馈[/size]
发现 bug 或有好点子？欢迎来 [url=https://discord.gg/trTsyxUFqg]Core Directive Discord[/url] 打个招呼，或在这里留言。报告 bug 时，请附上 [font=Courier New]BepInEx\LogOutput.log[/font] 和 [font=Courier New]BepInEx\quiet-toilet.log[/font]。

[size=5]源代码与许可[/size]
基于 MIT 许可开源：[url=https://github.com/AnExiledDev/SurvivalLog-QuietToilet]github.com/AnExiledDev/SurvivalLog-QuietToilet[/url]
