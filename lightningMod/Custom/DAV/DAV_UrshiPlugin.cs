using System;
using System.Linq;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.DAV
{
	public class DAV_UrshiPlugin : BasePlugin, IInGameTopPainter {
		public float xPos { get; set; }
		public float yPos { get; set; }
		public float iconSize { get; set; }
		public bool showCount { get; set; }
		public IFont textFont { get; set; }
		public ITexture gemIcon { get; set; }

		public DAV_UrshiPlugin() {
			Enabled = true;
		}

		public override void Load(IController hud) {
			base.Load(hud);

			xPos = Hud.Window.Size.Width * 0.5f;
			yPos = Hud.Window.Size.Height * 160 / 1080;
			iconSize = 60;
			showCount = true;
			textFont = Hud.Render.CreateFont("arial", 9, 255, 51, 255, 51, true, true, true);
			//gemIcon = Hud.Texture.GetTexture(3435159457); // -- Original Icon
			gemIcon = Hud.Texture.GetTexture(525359980); // -- Purple Gem Minimap Icon

		}

		public void PaintTopInGame(ClipState clipState) {
			if (clipState != ClipState.BeforeClip) return;
			if (!Hud.Game.Quests.Any(q => q.SnoQuest.Sno == 337492 && q.QuestStepId == 34)) return;

			var outMSG = "";
			foreach (var player in Hud.Game.Players) {
				var reminder = player.GetAttributeValueAsInt(Hud.Sno.Attributes.Jewel_Upgrades_Bonus, 2147483647, 0) + player.GetAttributeValueAsInt(Hud.Sno.Attributes.Jewel_Upgrades_Max, 2147483647, 0) - player.GetAttributeValueAsInt(Hud.Sno.Attributes.Jewel_Upgrades_Used, 2147483647, 0);
				if (reminder > 0)
					outMSG += (showCount ? (reminder.ToString() + " x ") : "") + player.BattleTagAbovePortrait + "\n";
			}

			gemIcon?.Draw(xPos, yPos, iconSize, iconSize);
			textFont.DrawText(outMSG, xPos + iconSize, yPos);
		}
	}
}