using System;
using System.Linq;
using System.Collections.Generic;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.DAV
{
	public static class DAV_ExtendOther {
		public static IController Hud { get; private set; }

	// ISnoArea variable
		public static string riftMapName { get; set; }
		public static Dictionary<uint, int> ActFix { get; private set; } = new Dictionary<uint, int> { // from RNN
			{ 288482, 0 }, { 288684, 0 }, { 288686, 0 }, { 288797, 0 }, { 288799, 0 }, // GR
			{ 288801, 0 }, { 288803, 0 }, { 288809, 0 }, { 288812, 0 }, { 288813, 0 }, // GR
			{  63666, 1 }, { 445426, 1 },
			{ 456638, 2 }, { 460671, 2 }, { 464092, 2 }, { 464830, 2 }, { 465885, 2 }, { 467383, 2 },
			{ 444307, 3 }, { 445762, 3 },
			{ 444396, 4 }, { 445792, 4 }, { 446367, 4 }, { 446550, 4 }, { 448011, 4 }, { 448039, 4 },
			{ 464063, 4 }, { 464065, 4 }, { 464066, 4 }, { 464810, 4 }, { 464820, 4 }, { 464821, 4 },
			{ 464822, 4 }, { 464857, 4 }, { 464858, 4 }, { 464865, 4 }, { 464867, 4 }, { 464868, 4 },
			{ 464870, 4 }, { 464871, 4 }, { 464873, 4 }, { 464874, 4 }, { 464875, 4 }, { 464882, 4 },
			{ 464886, 4 }, { 464889, 4 }, { 464890, 4 }, { 464940, 4 }, { 464941, 4 }, { 464942, 4 },
			{ 464943, 4 }, { 464944, 4 }, { 475854, 4 }, { 475856, 4 },
			{ 448391, 5 }, { 448368, 5 }, { 448375, 5 }, { 448398, 5 }, { 448404, 5 }, { 448411, 5 },
		};

	// IMonster variable
		public static Dictionary<MonsterAffix, int> AffixPriority { get; set; } = new Dictionary<MonsterAffix, int> {
			{ MonsterAffix.Wormhole, 1 },		{ MonsterAffix.Juggernaut, 2 },		{ MonsterAffix.Waller, 3 },
			{ MonsterAffix.Teleporter, 4 },		{ MonsterAffix.Shielding, 5 },		{ MonsterAffix.Illusionist, 6 },
			{ MonsterAffix.Electrified, 7 },	{ MonsterAffix.Orbiter, 8 },		{ MonsterAffix.Thunderstorm, 9 },
			{ MonsterAffix.FireChains, 10 },	{ MonsterAffix.Desecrator, 11 },	{ MonsterAffix.Molten, 12 },
			{ MonsterAffix.Mortar, 13 },		{ MonsterAffix.Frozen, 14 },		{ MonsterAffix.FrozenPulse, 15 },
			{ MonsterAffix.Arcane, 16 },		{ MonsterAffix.Jailer, 17 },		{ MonsterAffix.Plagued, 18 },
			{ MonsterAffix.Poison, 19 },		{ MonsterAffix.Horde, 20 },			{ MonsterAffix.MissileDampening, 21 },
			{ MonsterAffix.Fast, 23 },
			{ MonsterAffix.Knockback, 25 },		{ MonsterAffix.Nightmarish, 26 },	{ MonsterAffix.Vortex, 27 },
			{ MonsterAffix.Avenger, 28 },		{ MonsterAffix.Reflect, 29 },		{ MonsterAffix.HealthLink, 30 },
		};

	// Initial Setting
		public static void Init(IController hud) {
			Hud = hud;
		}

	// IActor Extends
		public static IScreenCoordinate ScreenFloorCoordinate(this IActor actor, float offsetX = 0f, float offsetY = 0f) {
            var screenCoord = actor.FloorCoordinate.ToScreenCoordinate(true);

            return Hud.Window.CreateScreenCoordinate(screenCoord.X + offsetX, screenCoord.Y + offsetY);
        }

		public static bool isOculus(this IActor actor) {
			return (actor.SnoActor.Sno == ActorSnoEnum._generic_proxy && actor.GetAttributeValueAsInt(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_None, Hud.Sno.SnoPowers.OculusRing.Sno) == 1);
		}

	// IMonster Extends
		public static int Priority(this ISnoMonsterAffix mAffix) { return AffixPriority[mAffix.Affix]; }
		public static void SetPriority(MonsterAffix affic, int priority) { AffixPriority[affic] = priority; }

		public static bool IceBlinked(this IMonster m) { return m.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_1_Visual_Effect_None, 428354) == 1; }

		// Necromancer
		public static bool Frailtied(this IMonster m) { return m.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_2_Visual_Effect_None, 471845) == 1; }
		public static bool Leeched(this IMonster m) { return m.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_2_Visual_Effect_None, 471869) == 1; }
		public static bool Decrepified(this IMonster m) { return m.GetAttributeValue(Hud.Sno.Attributes.Power_Buff_2_Visual_Effect_None, 471738) == 1; }

		public static float PctHealth(this IMonster m, bool base100 = true) {
			var healthRatio = Math.Min(m.CurHealth / m.MaxHealth, 1);
			return (float) (healthRatio * (base100 ? 100 : 1));
		}

	// ISnoArea Extends
		public static void SetRiftMapName(string map_to) { riftMapName = map_to; }
		public static int ActFixed(this ISnoArea snoArea) { return ActFix.ContainsKey(snoArea.Sno) ? ActFix[snoArea.Sno] : snoArea.Act; }

		public static string CustAreaName(this ISnoArea snoArea, bool showAct = false) {
			if (snoArea.Code.StartsWith("x1_lr_l"))
				return snoArea.Code.Replace("x1_lr_level_", riftMapName);
			if (showAct) {
				var actrev = snoArea.ActFixed();
				if (actrev > 0)
					return ("A" + actrev + " - " + snoArea.NameLocalized);
			}
			return snoArea.NameLocalized;
		}
	}

	public class DAV_Extend_OtherPlugin : BasePlugin {

		public DAV_Extend_OtherPlugin() {
			Enabled = true;
		}

		public override void Load(IController hud) {
			base.Load(hud);

			DAV_ExtendOther.Init(hud);
			DAV_ExtendOther.SetRiftMapName("Rift ");
			Enabled = false;
		}
	}
}