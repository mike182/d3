/*
	This file contains "menu" plugin priorities and addon settings.
	Change the file extension from .txt to .cs and move this file into the TurboHUD / plugins / User folder
*/

namespace Turbo.Plugins.User
{
	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Menu;

	public class MenuPluginConfig : BasePlugin, ICustomizer
	{
		public MenuPluginConfig() { Enabled = true; }

		public override void Load(IController hud) { base.Load(hud); }

		public void Customize()
		{
			Hud.RunOnPlugin<MenuPlugin>(plugin =>
			{
				//ConfigureDock(string dockId, params string[])
				//ConfigureAddon(string addonId, bool enabled, string config)

				plugin.ConfigureDock("BottomRight", "MenuLatency", "MenuLoot", "MenuMaterialKeystone", "MenuBounties", "MenuMaterialDeathsBreath", "MenuMaterialBloodShard", "MenuMaterialGold", "MenuNephalemRift", "MenuGreaterRift");
				plugin.ConfigureAddon("MenuLatency", true, "");
				plugin.ConfigureAddon("MenuLoot", false, "");
				plugin.ConfigureAddon("MenuMaterialKeystone", true, "");
				plugin.ConfigureAddon("MenuBounties", false, "");
				plugin.ConfigureAddon("MenuMaterialDeathsBreath", true, "");
				plugin.ConfigureAddon("MenuMaterialBloodShard", true, "");
				plugin.ConfigureAddon("MenuMaterialGold", false, "");
				plugin.ConfigureAddon("MenuNephalemRift", true, "");
				plugin.ConfigureAddon("MenuGreaterRift", true, "");

				plugin.ConfigureDock("BottomLeft", "MenuVolume", "MenuCrowdControl", "MenuSpiritBarrage", "MenuUptime", "MenuDamageTypes");
				plugin.ConfigureAddon("MenuVolume", false, "");
				plugin.ConfigureAddon("MenuCrowdControl", false, "");
				plugin.ConfigureAddon("MenuSpiritBarrage", false, "");
				plugin.ConfigureAddon("MenuUptime", true, "");
				plugin.ConfigureAddon("MenuDamageTypes", true, "");

				plugin.ConfigureDock("BottomCenter", "MenuHealth", "MenuDamageReduction", "MenuDamageDone", "MenuAttackSpeed", "MenuMoveSpeed");
				plugin.ConfigureAddon("MenuHealth", false, "");
				plugin.ConfigureAddon("MenuDamageReduction", true, "");
				plugin.ConfigureAddon("MenuDamageDone", true, "");
				plugin.ConfigureAddon("MenuAttackSpeed", true, "");
				plugin.ConfigureAddon("MenuMoveSpeed", true, "");

				plugin.ConfigureDock("MinimapBottom", "MenuMapShrines");
				plugin.ConfigureAddon("MenuMapShrines", true, "");

				plugin.ConfigureDock("TopCenter", "MenuParagon", "MenuXP", "MenuPools");
				plugin.ConfigureAddon("MenuParagon", true, "0");
				plugin.ConfigureAddon("MenuXP", true, "");
				plugin.ConfigureAddon("MenuPools", true, "1 148438166:LSP:0:1:0:0:24:1");

				plugin.ConfigureDock("TopRight", "MenuToggleAddons", "MenuTogglePlugins");
				plugin.ConfigureAddon("MenuToggleAddons", true, "");
				plugin.ConfigureAddon("MenuTogglePlugins", true, "");
			});
		}
	}
}