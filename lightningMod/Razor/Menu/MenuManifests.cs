/*
	This file contains manifest data to describe plugins that don't implement IPluginManifest
*/

namespace Turbo.Plugins.Razor.Menu
{
	using System.Collections.Generic;
	
	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Plugin;

	public class MenuManifests : BasePlugin, ICustomizer, IPluginManifest
	{
		//IPluginManifest
		public string Name { get; set; } //optional display name
        public string Description { get; set; } = "Descriptions for plugins in the Menu Plugin System";
		public string Version { get; set; } = "24-Sep-2021";
		public List<string> Dependencies { get; set; } //= new List<string>() {"Turbo.Plugins.Razor.Menu.MenuPlugin"};
		
		public MenuManifests() { Enabled = true; }

		public override void Load(IController hud) { base.Load(hud); }

		public void Customize()
		{
			Hud.RunOnPlugin<MenuTogglePlugins>(plugin =>
			{
				plugin.Manifests.AddRange(new IPluginManifest[] {
					//General
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Click.ClickEventHandler",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Notifies click handler plugins of mouse clicks and figures out whether or not they should be propagated to the game client",
						Version = "24-Jul-2021",
						//Dependencies = new List<string>() {} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Hotkey.HotkeyEventHandler",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Notifies hotkey handler plugins of key events that occur only when the game client is in the foreground and the chat prompt is not being used",
						Version = "19-Apr-2021",
						//Dependencies = new List<string>() {} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Label.LabelController",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Handles all of the click events and hover tooltips for Labels",
						Version = "5-Sep-2021",
						//Dependencies = new List<string>() {} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Log.TextLogger",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Manages delayed file logging calls for text writing plugins",
						Version = "5-Sep-2021",
						//Dependencies = new List<string>() {} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Util.LocationInfo",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Computes additional details and applies corrections to your current game world location data that can be used by other plugins",
						Version = "26-Jun-2021",
						//Dependencies = new List<string>() {} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Util.UIOverlapHelper",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Keeps track of the visibility of user interface elements that should block custom plugin displays from rendering or being interacted with",
						Version = "15-Jul-2021",
						//Dependencies = new List<string>() {} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Util.UIOverlapHelper",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Keeps track of the visibility of user interface elements that should block custom plugin displays from rendering or being interacted with",
						Version = "15-Jul-2021",
						//Dependencies = new List<string>() {} //list the full namespace path of plugins that this plugin requires
					},
					
					//supplemental plugins
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.SpiritBarrageHelper",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Shows countdown bars and ground indicators for spirit barrage phantasms and keep tracks of snapshot/buff status for each phantasm",
						Version = "15-Jul-2021",
						Dependencies = new List<string>() {"Turbo.Plugins.Razor.Label.LabelController"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuSpiritBarrage",
						Name = "Phantasm Inspector", //leave blank if using class name as the display name
						Description = "Shows a table of recent phantasms cast and their buff statuses",
						Version = "11-Jul-2021",
						Dependencies = new List<string>() {
							"Turbo.Plugins.Razor.Menu.MenuPlugin", 
							"Turbo.Plugins.Razor.SpiritBarrageHelper", 
						} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Proc.PartyProcTracker",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Shows countdown bars for cheat death effects and announces them with audio or TTS cues",
						Version = "31-Jul-2021",
						Dependencies = new List<string>() {"Turbo.Plugins.Razor.Movable.MovableController"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.Proc.PartyProcScreenshots",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Takes a screenshot whenever you or a teammate triggers a cheat death effect",
						Version = "26-Jul-2020",
						Dependencies = new List<string>() {"Turbo.Plugins.Razor.Proc.PartyProcTracker"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.ItemMapMarkers",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Remembers (and marks on the minimap) items that match customizable rules.",
						Version = "23-Sep-2021",
						//Dependencies = new List<string>() {"Turbo.Plugins.Razor.Movable.MovableController"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.BountyDropTracker",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Shows countdown bars for cheat death effects and announces them with audio or TTS cues",
						Version = "25-Jun-2021",
						Dependencies = new List<string>() {
							"Turbo.Plugins.Razor.Movable.MovableController",
							"Turbo.Plugins.Razor.Util.LocationInfo",
						} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.ImmunityHelper",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Checks a variety of statuses to show you whether or not your character is currently immune to damage",
						Version = "5-Sep-2021",
						Dependencies = new List<string>() {
							"Turbo.Plugins.Razor.Label.LabelController",
							"Turbo.Plugins.Razor.Movable.MovableController",
						} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.GreaterRiftHints",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Shows gem upgrade status for the party under Urshi's and Orek's feet, and shows Greater Rift levels \nunlocked and shard caps for each party member in the obelisk menu, marks monsters that are still alive",
						Version = "1-Aug-2021",
						Dependencies = new List<string>() {"Turbo.Plugins.Razor.Label.LabelController"} //list the full namespace path of plugins that this plugin requires
					},
					
					//Config files
					new PluginManifest()
					{
						Path = "Turbo.Plugins.User.CustomPluginEnablerOrDisabler",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Plugin toggle states for custom plugins (auto-generated by Menu Plugin System)",
						//Version = "24-Sep-2021",
						//Dependencies = new List<string>() {"Turbo.Plugins.Razor.Label.LabelController"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.User.DefaultPluginEnablerOrDisabler",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Plugin toggle states for default plugins (auto-generated by Menu Plugin System)",
						//Version = "24-Sep-2021",
						//Dependencies = new List<string>() {"Turbo.Plugins.Razor.Label.LabelController"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.User.LightningModPluginEnablerOrDisabler",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Plugin toggle states for LightningMod plugins (auto-generated by Menu Plugin System)",
						//Version = "24-Sep-2021",
						//Dependencies = new List<string>() {"Turbo.Plugins.Razor.Label.LabelController"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.User.MenuPluginConfig",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Menu addon toggle states and configuration settings (auto-generated by Menu Plugin System)",
						//Version = "24-Sep-2021",
						//Dependencies = new List<string>() {"Turbo.Plugins.Razor.Label.LabelController"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.User.MovablePluginConfig",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Movable plugin positions and enabled/disabled state settings (auto-generated by Movable Plugin System)",
						//Version = "24-Sep-2021",
						//Dependencies = new List<string>() {"Turbo.Plugins.Razor.Label.LabelController"} //list the full namespace path of plugins that this plugin requires
					},
					
					//RunStats
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuAttackSpeed",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Shows your mainhand, offhand, pet and bonus attack speeds, as well as pain enhancer stack count",
						Version = "7-Jul-2021",
						Dependencies = new List<string>() {"Turbo.Plugins.Razor.Menu.MenuPlugin"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuBounties",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Tracks the progress of bounty quests and rewards in the current game",
						Version = "12-Sep-2021",
						Dependencies = new List<string>() {
							"Turbo.Plugins.Razor.Menu.MenuPlugin",
							"Turbo.Plugins.Razor.BountyDropTracker",
						} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuCrowdControl",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Shows icons and running duration of crowd control effects applied on you, elite monsters and bosses",
						Version = "7-Jul-2021",
						Dependencies = new List<string>() {"Turbo.Plugins.Razor.Menu.MenuPlugin"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuDamageReduction",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Graphs your current damage reduction value history",
						Version = "7-Jul-2021",
						Dependencies = new List<string>() {
							"Turbo.Plugins.Razor.ImmunityHelper",
							"Turbo.Plugins.Razor.Proc.PartyProcTracker",
							"Turbo.Plugins.Razor.Menu.MenuPlugin",
						} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuHealth",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Graphs your health and shield value history",
						Version = "1-Jul-2021",
						Dependencies = new List<string>() {
							"Turbo.Plugins.Razor.ImmunityHelper",
							"Turbo.Plugins.Razor.Proc.PartyProcTracker",
							"Turbo.Plugins.Razor.Menu.MenuPlugin",
						} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuDamageDone",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Records and graphs the values of various damage statistics",
						Version = "10-Jul-2021",
						Dependencies = new List<string>() {"Turbo.Plugins.Razor.Menu.MenuPlugin"} //list the full namespace path of plugins that this plugin requires
					},
					new PluginManifest()
					{
						Path = "Turbo.Plugins.Razor.RunStats.MenuPools",
						//Name = string.Empty, //leave blank if using class name as the display name
						Description = "Shows pool count of your characters, your party, and history of pools found in the current game",
						Version = "25-Sep-2021",
						Dependencies = new List<string>() {
							"Turbo.Plugins.Razor.Menu.MenuPlugin",
							"Turbo.Plugins.Razor.Util.LocationInfo",
						} //list the full namespace path of plugins that this plugin requires
					},
				});
			});
		}
	}
}