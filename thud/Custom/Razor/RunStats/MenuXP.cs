namespace Turbo.Plugins.Razor.RunStats
{
	using SharpDX.DirectWrite;
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Collections.Generic;

	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Log;
	using Turbo.Plugins.Razor.Label;
	using Turbo.Plugins.Razor.Menu;
	using Turbo.Plugins.Razor.Util;

	public class MenuXP : BasePlugin, IMenuAddon/*, IAfterCollectHandler, IBeforeRenderHandler , IInGameTopPainter, ILeftClickHandler, IRightClickHandler*/
	{
		public bool HideDefaultPlugin { get; set; } = true;
		
		//public new Dictionary<string, IStatTracker> Trackers { get; set; }
		public List<IStatTracker> Trackers { get; set; }
		public IStatTracker DefaultTracker { get; set; }
		
		public string Id { get; set; }
		public int Priority { get; set; } //the priority on the dock to show this addon (smaller to the left, higher to the right)
		public string DockId { get; set; }
		public string Config { get; set; }

		public ILabelDecorator Label { get; set; }
		public ILabelDecorator LabelHint { get; set; }
		public float LabelSize { get; set; }
		public ILabelDecorator Panel { get; set; }

		public IFont TextFont { get; set; }
		public IFont TimeFont { get; set; }
		public IFont LevelFont { get; set; }
		public IFont TrackerFont { get; set; }
		
		public IBrush PlayedBrush { get; set; }
		public IBrush FullBrush { get; set; }
		
		//private Dictionary<uint, string> ToLevel;
		private LabelTableDecorator TableUI;
		private LabelTableDecorator TableUI2;
		private LabelTableDecorator TableUI3;
		private object[,] SessionTrackers;
		private object[,] HeroTrackers;
		private object[,] AccountTrackers;
		private int LastUpdateTick;
		
        public MenuXP()
        {
            Enabled = true;
			Priority = 20;
			DockId = "TopCenter";
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
			
			/*Trackers = new Dictionary<string, IStatTracker>() {
				{"Session", Hud.Tracker.Session},
				{"Account Today", Hud.Tracker.CurrentAccountToday},
				{"Account Today (Current Difficulty)", Hud.Game.CurrentAccountTodayOnCurrentDifficulty},
				{"Account Yesterday", Hud.Tracker.CurrentAccountToday},
				{"Account Total", Hud.Tracker.CurrentAccountTotal},
				{"Hero Today", Hud.Game.CurrentHeroToday},
				{"Hero Today (Current Difficulty)", Hud.Game.CurrentHeroTodayOnCurrentDifficulty},
				{"Hero Yesterday", Hud.Game.CurrentHeroYesterday},
				{"Hero Total", Hud.Game.CurrentHeroTotal},
			};*/
			
			DefaultTracker = Hud.Tracker.Session;
        }
		
		/*public void Customize()
		{
			if (HideDefaultPlugin)
				Hud.TogglePlugin<TopExperienceStatistics>(false);
		}*/
		
		public void OnRegister(MenuPlugin plugin)
		{
			//LabelSize = Hud.Window.Size.Height * 0.11f;
			if (HideDefaultPlugin)
				Hud.TogglePlugin<TopExperienceStatistics>(false);
			
			TextFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 255, 255, 160, false, false, 135, 0, 0, 0, true);
			TimeFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 255, 255, 255, false, false, 135, 0, 0, 0, true);
			LevelFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 175, 175, 175, false, false, 135, 0, 0, 0, true); //57, 137, 205
			TrackerFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 87, 137, 255, false, false, 135, 0, 0, 0, true);
			
			PlayedBrush = Hud.Render.CreateBrush(35, 255, 245, 59, 0);
			FullBrush = Hud.Render.CreateBrush(35, 150, 150, 150, 0);
			
			//ToLevel = new Dictionary<uint, string>();
			SessionTrackers = new object[,] {{Hud.Tracker.Session, "Session", null, null, null, null, null, null}};
			AccountTrackers = new object[,] {
				{Hud.Tracker.CurrentAccountToday, "Account Today (Any Difficulty)", null, null, null, null, null, null},
				{Hud.Game.CurrentAccountTodayOnCurrentDifficulty, "Account Today (Current Difficulty)", null, null, null, null, null, null},
				{Hud.Tracker.CurrentAccountYesterday, "Account Yesterday", null, null, null, null, null, null},
				{Hud.Tracker.CurrentAccountTotal, "Account Total", null, null, null, null, null, null}
			};
			HeroTrackers = new object[,] {
				{Hud.Game.CurrentHeroToday, "Hero Today (Any Difficulty)", null, null, null, null, null, null},
				{Hud.Game.CurrentHeroTodayOnCurrentDifficulty, "Hero Today (Current Difficulty)", null, null, null, null, null, null},
				{Hud.Game.CurrentHeroYesterday, "Hero Yesterday", null, null, null, null, null, null},
				{Hud.Game.CurrentHeroTotal, "Hero Total", null, null, null, null, null, null}
			};
			//Cached = new string[Trackers.Length, 4] {{}};
			
			
			//Label
			Label = new LabelRowDecorator(Hud,
				//new LabelStringDecorator(Hud, "[P]") {Font = TrackerFont, SpacingRight = 2},
				new LabelStringDecorator(Hud, () => /*Hud.Game.CurrentHeroToday*/DefaultTracker.GainedExperiencePerHourPlay.ToHumanReadable(4) + " xp/h") {Font = TextFont, FuncCacheDuration = 2}
			); //{Hint = null}; //plugin.CreateHint("Session xp gain rate (Played)")};
			LabelHint = plugin.CreateHint("Experience Gain");
			
			//Row
			var rowUI = new LabelRowDecorator(Hud,
					//paragon indicator
					//new LabelStringDecorator(Hud) {Font = LevelFont, SpacingLeft = 5, SpacingRight = 5},
					//tracker name
					new LabelStringDecorator(Hud) {Font = TrackerFont, Alignment = HorizontalAlign.Right, SpacingRight = 5},
					//experience gained
					new LabelStringDecorator(Hud) {Hint = plugin.CreateHint("Experience Gained"), Font = LevelFont, Alignment = HorizontalAlign.Left, SpacingLeft = 5, SpacingRight = 10, SpacingTop = 2, SpacingBottom = 2},
					//play time
					new LabelStringDecorator(Hud) {Hint = plugin.CreateHint("Play Time"), Font = TimeFont, BackgroundBrush = PlayedBrush, Alignment = HorizontalAlign.Left, SpacingLeft = 10, SpacingRight = 10, SpacingTop = 2, SpacingBottom = 2},
					//xp rate play
					new LabelStringDecorator(Hud) {Hint = plugin.CreateHint("Gain Rate (Played - excludes town time)"), Font = TextFont, BackgroundBrush = PlayedBrush, Alignment = HorizontalAlign.Left, SpacingLeft = 10, SpacingRight = 10, SpacingTop = 2, SpacingBottom = 2},
					//full time
					new LabelStringDecorator(Hud) {Hint = plugin.CreateHint("Total Time"), Font = TimeFont, Alignment = HorizontalAlign.Left, BackgroundBrush = FullBrush, SpacingLeft = 10, SpacingRight = 10, SpacingTop = 2, SpacingBottom = 2},
					//xp rate full
					new LabelStringDecorator(Hud) {Hint = plugin.CreateHint("Gain Rate (Total - includes town time)"), Font = TextFont, BackgroundBrush = FullBrush, Alignment = HorizontalAlign.Left, SpacingLeft = 10, SpacingRight = 10, SpacingTop = 2, SpacingBottom = 2},
					//town time
 					new LabelStringDecorator(Hud) {Hint = plugin.CreateHint("Town Time"), Font = TimeFont, Alignment = HorizontalAlign.Left, SpacingLeft = 10, SpacingRight = 5, SpacingTop = 2, SpacingBottom = 2}
				) {Alignment = HorizontalAlign.Left};
			Func<object[,], LabelRowDecorator, int, bool> rowFunc = delegate(object[,] trackers, LabelRowDecorator row, int index) {
				if (index >= trackers.GetLength(0))
					return false;
				
				IStatTracker tracker = (IStatTracker)trackers[index, 0];
				
				//name
				LabelStringDecorator label = (LabelStringDecorator)row.Labels[0];
				label.StaticText = (string)trackers[index, 1];
				
				label = (LabelStringDecorator)row.Labels[1];
				label.StaticText = (string)trackers[index, 2]; //ValueToString(tracker.GainedExperience, ValueFormat.LongNumber) + "xp";
				
				label = (LabelStringDecorator)row.Labels[2];
				label.StaticText = (string)trackers[index, 3];
				
				label = (LabelStringDecorator)row.Labels[3];
				label.StaticText = (string)trackers[index, 4];

				label = (LabelStringDecorator)row.Labels[4];
				label.StaticText = (string)trackers[index, 5];

				label = (LabelStringDecorator)row.Labels[5];
				label.StaticText = (string)trackers[index, 6];
				
				label = (LabelStringDecorator)row.Labels[6];
				label.StaticText = (string)trackers[index, 7];
				
				return true;
			};
			
			TableUI = new LabelTableDecorator(Hud, rowUI) {
				Alignment = HorizontalAlign.Left,
				BackgroundBrush = plugin.BgBrush, 
				HoveredBrush = plugin.HighlightBrush,
				SpacingLeft = 10,
				SpacingRight = 10,
				FillWidth = false, //true,
				OnFillRow = (row, index) => rowFunc(SessionTrackers, row, index)
			};
			TableUI2 = new LabelTableDecorator(Hud, rowUI) {
				Alignment = HorizontalAlign.Left,
				BackgroundBrush = plugin.BgBrush, 
				HoveredBrush = plugin.HighlightBrush,
				SpacingLeft = 10,
				SpacingRight = 10,
				FillWidth = false, //true,
				OnFillRow = (row, index) => rowFunc(HeroTrackers, row, index)
			};
			TableUI3 = new LabelTableDecorator(Hud, rowUI) {
				Alignment = HorizontalAlign.Left,
				BackgroundBrush = plugin.BgBrush, 
				HoveredBrush = plugin.HighlightBrush,
				SpacingLeft = 10,
				SpacingRight = 10,
				SpacingBottom = 5,
				FillWidth = false, //true,
				OnFillRow = (row, index) => rowFunc(AccountTrackers, row, index)
			};
			
			Panel = new LabelColumnDecorator(Hud, 
				new LabelDelayedDecorator(Hud,
					new LabelAlignedDecorator(Hud, 
						new LabelStringDecorator(Hud, "EXPERIENCE GAIN") {Font = plugin.TitleFont, SpacingLeft = 10, SpacingRight = 10},
						//plugin.CreateReset(this.Reset),
						plugin.CreatePin(this)
					)
				) {BackgroundBrush = plugin.BgBrush},
				//new LabelStringDecorator(Hud, "Current Session") {Font = plugin.TitleFont},
				TableUI,
				new LabelStringDecorator(Hud, "CURRENT HERO") {Font = plugin.TitleFont, BackgroundBrush = plugin.BgBrushAlt, SpacingTop = 5, SpacingBottom = 3},
				TableUI2,
				new LabelStringDecorator(Hud, "CURRENT ACCOUNT") {Font = plugin.TitleFont, BackgroundBrush = plugin.BgBrushAlt, SpacingTop = 5, SpacingBottom = 3},
				TableUI3
			) {
				OnBeforeRender = (label) => {
					int diff = Hud.Game.CurrentGameTick - LastUpdateTick;
					if (diff < 0 || diff > 60) //1 second
					{
						for (int i = 0; i < SessionTrackers.GetLength(0); ++i) //object[] cached in SessionTrackers)
						{
							IStatTracker tracker = (IStatTracker)SessionTrackers[i, 0];
							SessionTrackers[i, 2] = ((double)tracker.GainedExperience).ToHumanReadable(5) + " xp"; //ValueToString(tracker.GainedExperience, ValueFormat.LongNumber) + " xp";
							SessionTrackers[i, 3] = plugin.MillisecondsToString(tracker.PlayElapsedMilliseconds);
							SessionTrackers[i, 4] = tracker.GainedExperiencePerHourPlay.ToHumanReadable(5) + " xp/h"; //ValueToString(tracker.GainedExperiencePerHourPlay, ValueFormat.ShortNumber) + " xp/h";
							SessionTrackers[i, 5] = plugin.MillisecondsToString(tracker.ElapsedMilliseconds);
							SessionTrackers[i, 6] = tracker.GainedExperiencePerHourFull.ToHumanReadable(5) + " xp/h"; //ValueToString(tracker.GainedExperiencePerHourFull, ValueFormat.ShortNumber) + " xp/h";
							SessionTrackers[i, 7] = plugin.MillisecondsToString(tracker.TownElapsedMilliseconds);
						}
						
						for (int i = 0; i < HeroTrackers.GetLength(0); ++i)
						{
							IStatTracker tracker = (IStatTracker)HeroTrackers[i, 0];
							HeroTrackers[i, 2] = ((double)tracker.GainedExperience).ToHumanReadable(5) + " xp"; //ValueToString(tracker.GainedExperience, ValueFormat.LongNumber) + " xp";
							HeroTrackers[i, 3] = plugin.MillisecondsToString(tracker.PlayElapsedMilliseconds);
							HeroTrackers[i, 4] = tracker.GainedExperiencePerHourPlay.ToHumanReadable(5) + " xp/h"; //ValueToString(tracker.GainedExperiencePerHourPlay, ValueFormat.ShortNumber) + " xp/h";
							HeroTrackers[i, 5] = plugin.MillisecondsToString(tracker.ElapsedMilliseconds);
							HeroTrackers[i, 6] = tracker.GainedExperiencePerHourFull.ToHumanReadable(5) + " xp/h"; //ValueToString(tracker.GainedExperiencePerHourFull, ValueFormat.ShortNumber) + " xp/h";
							HeroTrackers[i, 7] = plugin.MillisecondsToString(tracker.TownElapsedMilliseconds);
						}
						
						for (int i = 0; i < AccountTrackers.GetLength(0); ++i)
						{
							IStatTracker tracker = (IStatTracker)AccountTrackers[i, 0];
							AccountTrackers[i, 2] = ((double)tracker.GainedExperience).ToHumanReadable(5) + " xp"; //ValueToString(tracker.GainedExperience, ValueFormat.LongNumber) + " xp";
							AccountTrackers[i, 3] = plugin.MillisecondsToString(tracker.PlayElapsedMilliseconds);
							AccountTrackers[i, 4] = tracker.GainedExperiencePerHourPlay.ToHumanReadable(5) + " xp/h"; //ValueToString(tracker.GainedExperiencePerHourPlay, ValueFormat.ShortNumber) + " xp/h";
							AccountTrackers[i, 5] = plugin.MillisecondsToString(tracker.ElapsedMilliseconds);
							AccountTrackers[i, 6] = tracker.GainedExperiencePerHourFull.ToHumanReadable(5) + " xp/h"; //ValueToString(tracker.GainedExperiencePerHourFull, ValueFormat.ShortNumber) + " xp/h";
							AccountTrackers[i, 7] = plugin.MillisecondsToString(tracker.TownElapsedMilliseconds);
						}
						
						LastUpdateTick = Hud.Game.CurrentGameTick;
					}
					
					//sync table widths 
					if (TableUI is object && TableUI.RowWidths is object)
					{
						for (int i = 0; i < TableUI.RowWidths.Length; ++i)
						{
							float max = (float)Math.Max(Math.Max(TableUI.RowWidths[i], TableUI2.RowWidths[i]), TableUI3.RowWidths[i]);
							TableUI.RowWidths[i] = max;
							TableUI2.RowWidths[i] = max;
							TableUI3.RowWidths[i] = max;
						}
					}
					
					return true;
				}
			};
			
			//Plugin = plugin;
		}
		
		/*public void AfterCollect()
		{
			if (!Hud.Game.IsInGame || SessionTrackers == null)
				return;
			

		}
		
		public void BeforeRender()
		{
			//sync table widths 
			if (TableUI is object && TableUI.RowWidths is object)
			{
				for (int i = 0; i < TableUI.RowWidths.Length; ++i)
				{
					float max = (float)Math.Max(Math.Max(TableUI.RowWidths[i], TableUI2.RowWidths[i]), TableUI3.RowWidths[i]);
					TableUI.RowWidths[i] = max;
					TableUI2.RowWidths[i] = max;
					TableUI3.RowWidths[i] = max;
				}
			}
		}*/
		
		/*public string MillisecondsToString(long milliseconds)
		{
			TimeSpan timeSpent = TimeSpan.FromMilliseconds(milliseconds);
			if (timeSpent.Days > 0)
				//TimeToLevel[i] = ValueToString(timeRequired.Days, ValueFormat.NormalNumberNoDecimal) + timeRequired.ToString("'d 'hh'h 'mm'm 'ss's'"); //text +=
				return ValueToString(timeSpent.Days, ValueFormat.NormalNumberNoDecimal) + timeSpent.ToString("'d 'hh'h 'mm'm 'ss's'"); //text +=
			if (timeSpent.Hours > 0)
				//TimeToLevel[i] = timeRequired.ToString("h'h 'mm'm 'ss's'");
				return timeSpent.ToString("h'h 'mm'm 'ss's'");
			if (timeSpent.Minutes > 0)
				//TimeToLevel[i] = timeRequired.ToString("m'm 'ss's'");
				return timeSpent.ToString("m'm 'ss's'");
			//else 
				//TimeToLevel[i] = timeRequired.ToString("%s's'");
			return timeSpent.ToString("%s's'");
		}*/
		
		/*public void UpdateToLevel()
		{
			IPlayer me = Hud.Game.Me;
			uint current = me.CurrentLevelNormal;
			long xpTotal = 0;
			
			if (me.CurrentLevelNormal < me.CurrentLevelNormalCap)
				xpTotal = (long)(XpInfo.LevelXpTable[current+1] - Hud.Sno.GetExpToNextLevel(me));
			else
			{
				current += me.CurrentLevelParagon;
				xpTotal = me.ParagonTotalExp; //Hud.Sno.TotalParagonExperienceRequired(me.CurrentLevelParagon) - xpTotal;
			}
			
			if (LastLevelSeen == current)
			{
				int diff = Hud.Game.CurrentGameTick - LastUpdateTick;
				if (diff >= 0 || diff < 360) //5 seconds
					return;
			}
				
			for (int i = 0; i < LevelIncrements.Count; ++i)
			{
				long xpRemaining = 0;
				uint next = current + LevelIncrements[i];
				if (next <= me.CurrentLevelNormalCap)
				{
					xpRemaining = XpInfo.LevelXpTable[next] - xpTotal;
				}
				else
				{
					uint plvl = next - me.CurrentLevelNormalCap;
					xpRemaining = Hud.Sno.TotalParagonExperienceRequired(plvl) - xpTotal;
					
					//if (current < me.CurrentLevelNormalCap)
					//	xpRemaining += XpInfo.LevelXpTable[me.CurrentLevelNormalCap]; //- xpTotal;
				}
				
				try {
					TimeSpan timeRequired = TimeSpan.FromHours(xpRemaining / xph);
					//var ticks = Convert.ToInt64(Math.Ceiling(hours * 60.0d * 60.0d * 1000.0d * TimeSpan.TicksPerMillisecond));
					//text += ValueToString(ticks, ValueFormat.LongTimeNoSeconds);
					//text += hours + "h";
					if (timeRequired.Days > 0)
						ToLevel[i] = ValueToString(timeRequired.Days, ValueFormat.NormalNumberNoDecimal) + timeRequired.ToString("'d 'hh'h 'mm'm 'ss's'"); //text +=
					if (timeRequired.Hours > 0)
						ToLevel[i] = timeRequired.ToString("h'h 'mm'm 'ss's'");
					if (timeRequired.Minutes > 0)
						ToLevel[i] = timeRequired.ToString("m'm 'ss's'");
					else 
						ToLevel[i] = timeRequired.ToString("%s's'");
				} catch (Exception) {
					ToLevel[i] = "∞";
				}
			}
			
			LastLevelSeen = current;
			LastUpdateTick = Hud.Game.CurrentGameTick;
		}*/
	}
}