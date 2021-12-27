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

				plugin.ConfigureDock("BottomRight", "MenuLoot", "MenuLatency", "MenuMaterialKeystone", "MenuBounties", "MenuMaterialDeathsBreath", "MenuMaterialBloodShard", "MenuMaterialGold", "MenuNephalemRift", "MenuGreaterRift");
				plugin.ConfigureAddon("MenuLoot", false, "");
				plugin.ConfigureAddon("MenuLatency", true, "");
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
				plugin.ConfigureAddon("MenuPools", true, "1 148438166:LSP:0:1:0:0:24:1;148484836:LSP:6:0:0:0:24:1;147000012:LSP:4:1:1.42085907361964:0:24:1;151470409:LSP:4:1:0:0:25:1;151899786:LSP:0:1:4.38644106755271:0:25:1;139410641:LSP:3:1:0:0:22:0;139838869:LSP:5:1:1.29302402475592:0:22:0;139347189:LSP:2:1:7.03179390574781:0:22:0;139127679:LSP:4:1:6.42870042892715:0:22:0;138858330:LSP:0:1:4.64534299531056:0:22:0;149634858:mule:4:1:0:0:24:0;152047212:LSP:0:1:0:0:0:0;152047234:LSP:4:1:0:0:0:0;141155518:LSP:6:1:5.46266287245443:0:22:0");

				plugin.ConfigureDock("TopRight", "MenuToggleAddons", "MenuTogglePlugins");
				plugin.ConfigureAddon("MenuToggleAddons", true, "");
				plugin.ConfigureAddon("MenuTogglePlugins", true, "");
			});
		}
	}
}