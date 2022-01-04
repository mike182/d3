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

				plugin.ConfigureDock("BottomRight", "MenuLatency", "MenuLoot", "MenuBounties", "MenuMaterialKeystone", "MenuMaterialDeathsBreath", "MenuMaterialBloodShard", "MenuMaterialGold", "MenuNephalemRift", "MenuGreaterRift");
				plugin.ConfigureAddon("MenuLatency", true, "");
				plugin.ConfigureAddon("MenuLoot", false, "");
				plugin.ConfigureAddon("MenuBounties", false, "");
				plugin.ConfigureAddon("MenuMaterialKeystone", true, "");
				plugin.ConfigureAddon("MenuMaterialDeathsBreath", false, "");
				plugin.ConfigureAddon("MenuMaterialBloodShard", false, "");
				plugin.ConfigureAddon("MenuMaterialGold", false, "");
				plugin.ConfigureAddon("MenuNephalemRift", true, "");
				plugin.ConfigureAddon("MenuGreaterRift", true, "");

				plugin.ConfigureDock("BottomLeft", "MenuVolume", "MenuCrowdControl", "MenuSpiritBarrage", "MenuUptime", "MenuDamageTypes");
				plugin.ConfigureAddon("MenuVolume", true, "");
				plugin.ConfigureAddon("MenuCrowdControl", false, "");
				plugin.ConfigureAddon("MenuSpiritBarrage", false, "");
				plugin.ConfigureAddon("MenuUptime", true, "");
				plugin.ConfigureAddon("MenuDamageTypes", true, "");

				plugin.ConfigureDock("BottomCenter", "MenuHealth", "MenuDamageReduction", "MenuDamageDone", "MenuAttackSpeed", "MenuMoveSpeed");
				plugin.ConfigureAddon("MenuHealth", false, "");
				plugin.ConfigureAddon("MenuDamageReduction", false, "");
				plugin.ConfigureAddon("MenuDamageDone", false, "");
				plugin.ConfigureAddon("MenuAttackSpeed", false, "");
				plugin.ConfigureAddon("MenuMoveSpeed", false, "");

				plugin.ConfigureDock("MinimapBottom", "MenuMapShrines");
				plugin.ConfigureAddon("MenuMapShrines", true, "");

				plugin.ConfigureDock("TopCenter", "MenuParagon", "MenuXP", "MenuPools");
				plugin.ConfigureAddon("MenuParagon", true, "0");
				plugin.ConfigureAddon("MenuXP", true, "");
				plugin.ConfigureAddon("MenuPools", true, "1 152118208:LSP:6:0:0:0:25:1;151470409:LSP:4:1:0:0:25:1");

				plugin.ConfigureDock("TopRight", "MenuToggleAddons", "MenuTogglePlugins");
				plugin.ConfigureAddon("MenuToggleAddons", true, "");
				plugin.ConfigureAddon("MenuTogglePlugins", true, "");
			});
		}
	}
}