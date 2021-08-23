/*

Shows countdown bars for each of your skeletal mages. 

Changelog
- May 3, 2021
	- changed the essence display from showing essence level when cast to essence spent casting each mage
	- added an option to show essence bars only when using Skeletal Mage - Singularity (on by default)
	- updated mage count calculation to take into consideration dual sims
	- the mage count calculation now takes into consideration Simulacrum again - Haunted Visions doesn't doesn't seem to prevent simulacrums from duplicating Skeletal Mage casts
	- added check for updated Razeth's Volition legendary power
	- fixed issue from the race condition of reading the mage count from the buff before it was fully incremented
- February 2, 2021
	- attempted to adjust the count for the new Haunted Visions power change
	- converted to use the Movable plugin system
- May 18, 2019
	- Initial version

*/

namespace Turbo.Plugins.Razor
{
	using System;
	using System.Collections.Generic;
	//using System.Drawing;
	using System.Linq;
	//using SharpDX;
	//using SharpDX.DirectWrite;

	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Movable;

    public class MageGauge : BasePlugin, IAfterCollectHandler, IInGameWorldPainter, IMovable, INewAreaHandler //, ICustomizer, IInGameTopPainter
    {
		public bool ShowResourceSpentForSingularityOnly { get; set; } = true;
		public bool ShowMageCircles { get; set; } = false;
		public bool ShowMageDamageMultiplierLabels { get; set; } = true; //only applies to singularity mages
		public WorldDecoratorCollection MageDecorator { get; set; }
		public WorldDecoratorCollection MageMultiplierDecorator { get; set; }

		//public float PositionX { get; set; }
		//public float PositionY { get; set; }
		public float BarWidth { get; set; } = 35;
		public float BarHeight { get; set; } = 4;
		
		public IBrush DividerBrush { get; set; }
		//public IBrush TimerBrush { get; set; }
		public IBrush BgBrush { get; set; }
		public IBrush ShadowBrush { get; set; }
		public IBrush EssenceBrush { get; set; }
		public IFont EssenceFont { get; set; }
		public IFont CountFont { get; set; }
		public IFont TimerFont { get; set; }
		public IBrush TimerHigh { get; set; }
		public IBrush TimerLow { get; set; }
		
		public class Mage
		{
			public uint AcdId { get; set; }
			public float Essence { get; set; }
			public float MaxEssence { get; set; }
			public double Duration { get; set; }
			public int DeathTick { get; set; } //gametick / 60 = seconds
			public Mage() {}
		}
		
		public List<Mage> Mages { get; set; } = new List<Mage>();		
		public int MageCountCap { get; set; } = 10;
		public ActorSnoEnum[] MageActorSnos { get; set; } = new ActorSnoEnum[]
		{
			ActorSnoEnum._p6_necro_skeletonmage_a,
			ActorSnoEnum._p6_necro_skeletonmage_b,
			ActorSnoEnum._p6_necro_skeletonmage_c,
			ActorSnoEnum._p6_necro_skeletonmage_d,
			ActorSnoEnum._p6_necro_skeletonmage_e,
			ActorSnoEnum._p6_necro_skeletonmage_f_archer
		};
		public ActorSnoEnum[] SimulacrumActorSnos = new ActorSnoEnum[]
		{
			ActorSnoEnum._p6_necro_simulacrum_a_set,
			ActorSnoEnum._p6_necro_simulacrum_a,
		};

		private float offsetX = 0;
		private float offsetY = 0;
		private bool UsingSingularityRune = false;
		public float LastEssenceSeen { get; set; }
		public float LastSpent { get; set; }
		
        public MageGauge()
        {
            Enabled = true;

			BarWidth = 35; //a ratio of the screen width
			BarHeight = 4; //a ratio of the screen height
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
			
			DividerBrush = Hud.Render.CreateBrush(650, 255, 255, 255, 1);
			//TimerBrush = Hud.Render.CreateBrush(120, 51, 255, 204, 0);
			BgBrush = Hud.Render.CreateBrush(150, 0, 0, 0, 0);
			ShadowBrush = Hud.Render.CreateBrush(75, 0, 0, 0, 0);
			EssenceBrush = Hud.Render.CreateBrush(120, 40, 255, 237, 0);
			EssenceFont = Hud.Render.CreateFont("tahoma", 7, 220, 40, 255, 237, true, false, 150, 0, 0, 0, true); //185, 185, 185
			CountFont = Hud.Render.CreateFont("tahoma", 9, 220, 255, 255, 255, true, false, 150, 0, 0, 0, true);
			TimerFont = Hud.Render.CreateFont("tahoma", 7, 200, 255, 255, 255, false, false, 150, 0, 0, 0, true);

			TimerHigh = Hud.Render.CreateBrush(255, 0, 255, 100, 0); //120 alpha
			TimerLow = Hud.Render.CreateBrush(255, 255, 0, 0, 0); //120 alpha
			
			MageDecorator = new WorldDecoratorCollection(
				new GroundCircleDecorator(Hud)
				{
					Brush = Hud.Render.CreateBrush(75, 40, 255, 237, 4),
					Radius = -1f,
				},			
				new GroundCircleDecorator(Hud)
				{
					Brush = Hud.Render.CreateBrush(150, 40, 255, 237, 0),
					Radius = 0.3f,
				}
			);
			
			MageMultiplierDecorator = new WorldDecoratorCollection(
                new GroundLabelDecorator(Hud)
                {
                    BackgroundBrush = Hud.Render.CreateBrush(125, 0, 0, 0, 0),
                    TextFont = Hud.Render.CreateFont("tahoma", 6.5f, 200, 40, 255, 237, false, false, false) //185, 185, 185 //EssenceFont, //Hud.Render.CreateFont("tahoma", 6.5f, 255, 255, 255, 255, false, false, false),
                }
			);
        }
		
		public void PaintWorld(WorldLayer layer)
		{
			if (ShowMageCircles)
			{
				foreach (IActor myMage in Hud.Game.Actors.Where(a => MageActorSnos.Contains(a.SnoActor.Sno) && (a.SummonerAcdDynamicId == Hud.Game.Me.SummonerId)))
					MageDecorator.Paint(layer, myMage, myMage.FloorCoordinate, null);
			}
			
			if (ShowMageDamageMultiplierLabels && UsingSingularityRune)
			{
				foreach (IActor myMage in Hud.Game.Actors.Where(a => MageActorSnos.Contains(a.SnoActor.Sno) && (a.SummonerAcdDynamicId == Hud.Game.Me.SummonerId)))
					MageMultiplierDecorator.Paint(layer, myMage, myMage.FloorCoordinate, "+" + (myMage.GetAttributeValue(Hud.Sno.Attributes.Multiplicative_Damage_Percent_Bonus, uint.MaxValue) - 1).ToString("F1") + "x"); //(myMage.GetAttributeValue(Hud.Sno.Attributes.Multiplicative_Damage_Percent_Bonus, uint.MaxValue) - 1) * 100d).ToString("F0") + "%"
			}
		}
		
		public void OnRegister(MovableController mover)
		{
			//initialize position and dimension elements
			if (offsetX == 0 && offsetY == 0)
			{
				var layout = CountFont.GetTextLayout("0000");
				//width = layout.Metrics.Width;
				offsetX = layout.Metrics.Width * 0.5f;
				offsetY = layout.Metrics.Height;
				
			}

			var PositionX = Hud.Window.Size.Width * 0.5f; //horizontally centered
			var PositionY = Hud.Window.Size.Height * 0.64f; //vertically below the center

			float width = Hud.Window.Size.Width * 0.00155f * BarWidth;
			mover.CreateArea(
				this,
				"Countdown", //area name
				new System.Drawing.RectangleF(PositionX - (offsetX + width), PositionY, (offsetX + width)*2, offsetY*10), //position + dimensions
				true, //enabled at start?
				true, //save to config file?
				ResizeMode.Off //resizable
			);
		}
		
		//public void PaintTopInGame(ClipState clipState)
		public void PaintArea(MovableController mover, MovableArea area, float deltaX = 0, float deltaY = 0)
		{
			var xPos = area.Rectangle.X + deltaX; //float x = PositionX - offsetX;
			var yPos = area.Rectangle.Y + deltaY; //float y = PositionY + offsetY*i;

			float h = Hud.Window.Size.Height * 0.001667f * BarHeight; //4
			float w = Hud.Window.Size.Width * 0.00155f * BarWidth; //35
			
			if (Mages.Count > 0)
			{
				float maxEssence = Mages.Select(m => m.MaxEssence).Max();
				//float maxDuration = Mages.Select(m => m.Duration).Max(); //should always be the same...
				int i = 1;

				ShadowBrush.DrawRectangle(xPos + w + 2, yPos + offsetY - 3, offsetX*2 - 4, offsetY*Mages.Count + 6);
				
				foreach (Mage m in Mages)
				{
					float x = xPos + offsetX + w;
					float y = yPos + offsetY*i;
					
					var layout = CountFont.GetTextLayout((Mages.Count - i + 1).ToString());
					CountFont.DrawText(layout, x - layout.Metrics.Width*0.5f, y);
					
					float width = w * (m.Essence / maxEssence);
					if (UsingSingularityRune || !ShowResourceSpentForSingularityOnly)
					{
						BgBrush.DrawRectangle(x - offsetX - w - 2, y + offsetY*0.5f - h*0.5f - 2, w + 4, h + 4);
						EssenceBrush.DrawRectangle(x - offsetX - width, y + offsetY*0.5f - h*0.5f, width, h);
						layout = TimerFont.GetTextLayout(m.Essence.ToString("F0")); // + "/" + m.MaxEssence.ToString("F0"));
						TimerFont.DrawText(layout, x - offsetX - w - layout.Metrics.Width - 5, y + offsetY*0.5f - layout.Metrics.Height*0.5f);
					}
					
					float timeLeft = (float)(m.DeathTick - Hud.Game.CurrentGameTick)/60f;
					float timeLeftPct = (float)(timeLeft / m.Duration);
					width = w * timeLeftPct;
					x = x + offsetX;
					BgBrush.DrawRectangle(x - 2, y + offsetY*0.5f - h*0.5f - 2, w + 4, h + 4);
					
					TimerLow.DrawRectangle(x, y + offsetY*0.5f - h*0.5f, width, h);
					TimerHigh.Opacity = timeLeftPct;
					TimerHigh.DrawRectangle(x, y + offsetY*0.5f - h*0.5f, width, h);
					
					layout = TimerFont.GetTextLayout(timeLeft.ToString(timeLeft < 1 ? "F1" : "F0") + "s");
					TimerFont.DrawText(layout, x + w + 5, y + offsetY*0.5f - layout.Metrics.Height*0.5f);
					
					i++;
				}
			}
        }
		
		public void OnNewArea(bool newGame, ISnoArea area)
		{
			if (newGame)
			{
				Mages.Clear();
				LastEssenceSeen = 0;
				LastSpent = 0;
				//LastSeenTick = 0;
			}
		}

		public void AfterCollect()
		{
			if (!Hud.Game.IsInGame) return;
			
			//sync mages recorded with buff readout
			IPlayerSkill skill = Hud.Game.Me.Powers.UsedNecromancerPowers.SkeletalMage; //Hud.Game.Me.Powers.UsedSkills.FirstOrDefault(x => x.SnoPower.Sno == Hud.Sno.SnoPowers.Necromancer_SkeletalMage.Sno);
			if (skill is object)
			{
				//Hud.Sound.Speak(skill.Rune.ToString());
				UsingSingularityRune = (skill.Rune == 1 || Hud.Game.Me.Powers.BuffIsActive(484311)); //+ Razeth's Volition
				
				//remove all expired mages
				Mages.RemoveAll(m => m.DeathTick < Hud.Game.CurrentGameTick || (Math.Abs(m.DeathTick - Hud.Game.CurrentGameTick) > (m.Duration+2)*60));
				
				//check resource levels
				var spent = LastEssenceSeen - Hud.Game.Me.Stats.ResourceCurEssence;
				if (spent > 0)
				{
					LastSpent = spent;
				}
				LastEssenceSeen = Hud.Game.Me.Stats.ResourceCurEssence;
				
				IBuff buff = skill.Buff; //player.Powers.GetBuff(Hud.Sno.SnoPowers.Necromancer_SkeletalMage.Sno);
				if (buff is object && buff.Active)
				{
					int count = buff.IconCounts[6];

					//there is a bug sometimes where the buff stays stuck at 1 after all mages have despawned, have to double check that there is time left on mages
					if (count > 0)
					{ 
						if (buff.TimeLeftSeconds[5] > 0)
						{
							int added = 0;
							int deathtick = Hud.Game.CurrentGameTick + (int)(buff.TimeLeftSeconds[5]*60d);
							
							int simCount = Hud.Game.Me.Powers.UsedNecromancerPowers.Simulacrum is object ? Hud.Game.Actors.Count(a => SimulacrumActorSnos.Contains(a.SnoActor.Sno) && (a.SummonerAcdDynamicId == Hud.Game.Me.SummonerId)) : 0;
							
							if (Mages.Count == 0)
							{ 
								//figure out how many mages were just summoned
								added = 1;
								added += simCount;
								if (Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.CircleOfNailujsEvol.Sno)) added *= 2;
							}
							else if (Mages.Count > 0)
							{
								//newer mages were summoned
								if ((deathtick - Mages[0].DeathTick) > 2)
								{ 
							
									//figure out how many mages were just summoned
									added = 1;
									added += simCount;
									if (Hud.Game.Me.Powers.BuffIsActive(Hud.Sno.SnoPowers.CircleOfNailujsEvol.Sno)) added *= 2;
								}
							}
							
							//add records for all the new mages
							for (; added > 0; --added)
							{
								//Mages.Add
								Mages.Insert(0, new Mage()
								{ 
									Essence = LastSpent, //LastEssenceSeen,
									MaxEssence = Hud.Game.Me.Stats.ResourceMaxEssence,
									Duration = buff.TimeLeftSeconds[5] + buff.TimeElapsedSeconds[5],
									DeathTick = deathtick
								});
							}
							
							//remove the overflow (oldest mages are despawned)
							if (Mages.Count > MageCountCap)
								Mages.RemoveRange(MageCountCap, Mages.Count - MageCountCap);
						}
					}
					else if (Mages.Count > 0)
					{
						Mages.Clear();
					}
				}
				else if (Mages.Count > 0)
				{
					Mages.Clear();
				}
			}
			else
				UsingSingularityRune = false;
		}
    }
}