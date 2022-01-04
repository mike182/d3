/*
	This file contains "movable" plugin positions and enabled/disabled state settings.
	Change the file extension from .txt to .cs and move this file into the TurboHUD / plugins / User folder
*/

namespace Turbo.Plugins.User
{
	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Movable;

	public class MovablePluginConfig : BasePlugin, ICustomizer
	{
		public MovablePluginConfig() { Enabled = true; }

		public override void Load(IController hud) { base.Load(hud); }

		public void Customize()
		{
			Hud.RunOnPlugin<MovableController>(plugin =>
			{
				//Configure(string pluginName, string areaName, float x, float y, float width, float height, bool enabled = true, string configFileName = null, string areaSettings = null)
				plugin.Configure("VolumeControls", "Panel", 369f, 1039f, 96f, 19f, true, "MovablePluginConfig");
				plugin.Configure("TempestTracker", "Self", 1447f, 132f, 117f, 83f, true, "MovablePluginConfig");
				plugin.Configure("TempestTracker", "Party", 1447f, 224f, 117f, 104f, true, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "PlayerBottom", 941f, 583f, 37f, 37f, false, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "PlayerTop", 941f, 324f, 37f, 37f, true, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "PlayerLeft", 771f, 448f, 37f, 37f, true, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "PlayerRight", 1111f, 448f, 37f, 37f, true, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "MiniMapLeft", 1562f, 188f, 37f, 37f, true, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "MiniMapRight", 1877f, 188f, 37f, 37f, true, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "TopLeft", 0f, 1f, 37f, 37f, true, "MovablePluginConfig");
				plugin.Configure("MovableBuffList", "TopRight", 960f, 1f, 37f, 37f, true, "MovablePluginConfig");
				plugin.Configure("MageGauge", "Countdown", 835f, 691f, 249f, 195f, true, "MovablePluginConfig");
				plugin.Configure("BountyDropTracker", "Alert", 860f, 216f, 200f, 100f, true, "MovablePluginConfig");
				plugin.Configure("SpiritBarrageHelper", "Bars", 960f, 702f, 141f, 64f, true, "MovablePluginConfig");
				plugin.Configure("PartyProcTracker", "Bars", 1584f, 381f, 187f, 142f, true, "MovablePluginConfig");
				plugin.Configure("MenuPools", "PortraitAnchor", 5f, 41f, 42f, 18f, true, "MovablePluginConfig");
				plugin.Configure("ImmunityHelper", "Countdown", 920f, 334f, 89f, 9f, true, "MovablePluginConfig");
			});
		}
	}
}