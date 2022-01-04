Skill Cooldowns Plugin by Razorfish
[Tested with TurboHUD verson 21.3.31.3, API version 9.2]

This plugin shows the potential cooldown duration for all equipped skills that have cooldowns, after all current cooldown effects are applied - buffs, cdr, passives, and legendary powers. Skills that have charges also have their regeneration rate affected and are displayed. This might be helpful to see at a glance whether or not you're hitting key cooldown reduction breakpoints and whether or not there are uptime gaps for your skills.

The plugin hides the default skill bar damage display by default. You can switch between the default skill bar damage display (if you have that enabled), and the skill cooldown display with a configurable hotkey (F10 by default). If you don't have the default skill bar damage display enabled, it just toggles the skill cooldown display off or on.

CUSTOMIZATION
It displays the cooldown values on your skill bar by default when you launch TurboHUD. You can set it initially to be off by changing the property ShowCooldownLabels to false.

To edit the toggle hotkey, change ToggleHotkey, which is set to its default value at line 42. If you don't set any value, toggle hotkey functionality will be disabled.

DOWNLOAD

https://turbohud.razorfish.dev/2021/05/skill-cooldowns.html

INSTALLATION
Extract the folders from the download archive into your TurboHUD folder. The files should go in the following places:

TurboHUD \ plugins \ Razor \ SkillCooldowns.cs
TurboHUD \ plugins \ Razor \ Hotkey \ HotkeyEventHandler.cs
TurboHUD \ plugins \ Razor \ Hotkey \ IHotkeyEventHandler.cs

Then (re)start TurboHUD.