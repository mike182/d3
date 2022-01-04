namespace Turbo.Plugins.Razor.Menu
{
	using SharpDX.DirectWrite;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Text; //StringBuilder
	using System.IO; //File.WriteAllText, tier 3 only

	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Log;
	using Turbo.Plugins.Razor.Label;

	public class MenuTogglePlugins : BasePlugin, IMenuAddon, ITextLogger //, IMenuSaveHandler , IAfterCollectHandler , IInGameTopPainter, ILeftClickHandler, IRightClickHandler
	{
		//public string Namespace { get; set; } = "Turbo.Plugins.Default";
		//public string Namespace2 { get; set; } = "Turbo.Plugins.Enhanced";
		public string ConfigFileName { get; set; } = "CustomPluginEnablerOrDisabler";
		public string ConfigFilePath { get; set; } = @"plugins\User\"; //tier 3 only
		//public int MaxPluginsPerColumn { get; set; } = 40;
		
		public List<PluginFilter> Filters { get; set; }
		public class PluginFilter {
			public string Name { get; set; }
			public Func<IPlugin, bool> Check { get; set; }
			public string ConfigFile { get; set; }
			public string ConfigFilePath { get; set; }
			
			public IPlugin[] Plugins { get; set; }
			public LabelRowDecorator Decorator { get; set; }
		}
		
		public ILabelDecorator Label { get; set; }
		public ILabelDecorator LabelHint { get; set; }
		public float LabelSize { get; set; }
		public ILabelDecorator Panel { get; set; }

		public string Id { get; set; }
		public int Priority { get; set; } //the priority on the dock to show this addon (smaller to the left, higher to the right)
		public string DockId { get; set; }
		public string Config { get; set; }

		public IFont TextFont { get; set; }
		public IFont FadedFont { get; set; }
		public IFont EnabledFont { get; set; }
		public IFont DisabledFont { get; set; }
		
		public IBrush HoveredBrush { get; set; }
		public IBrush DrawBrush { get; set; }
		public IBrush SelectedBrush { get; set; }
		public ILabelDecorator HintActivate { get; set; }
		public ILabelDecorator HintDeactivate { get; set; }
		public LabelTableDecorator HintPerformance { get; set; }
		
		public string TextEnabled { get; set; } = "✔️";
		public string TextDisabled { get; set; } = "❌";
		
		//private List<IPlugin> Addons;
		private LabelTableDecorator Table;
		private LabelRowDecorator SelectUI;
		private PluginFilter SelectedFilter;
		private PluginFilter SaveFilter;
		//private LabelStringDecorator SaveUI;
		private LabelStringDecorator DeleteUI;
		private bool DeleteUIVisible;
		private int FileCheckUpdateTick;
		private MenuPlugin Plugin;
		private int ColumnCount;
		private int PluginsPerColumn;
		private string ConfigFile;
		private StringBuilder TextBuilder;
		
        public MenuTogglePlugins()
        {
            Enabled = true;
			Priority = 30;
			DockId = "TopRight";
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
			
			TextBuilder = new StringBuilder();
			Filters = new List<PluginFilter>() {
				//new PluginFilter() {Name = "All", ConfigFile = null, Check = (plugin) => true},
				//new PluginFilter() {Name = "Movable", ConfigFile = "MovablePluginEnablerOrDisabler", Check = (plugin) => plugin is Razor.Movable.IMovable},
				new PluginFilter() {Name = "Custom Plugins", ConfigFile = "CustomPluginEnablerOrDisabler", Check = (plugin) => !(plugin is IMenuAddon) && !Filters[1].Check(plugin)},
				new PluginFilter() {Name = "Default Plugins", ConfigFile = "DefaultPluginEnablerOrDisabler", Check = (plugin) => plugin.GetType().Namespace.StartsWith("Turbo.Plugins.Default") || plugin.GetType().Namespace.StartsWith("Turbo.Plugins.Enhanced")},
			};
        }
		
		public void OnRegister(MenuPlugin plugin)
		{
			//LabelSize = Hud.Window.Size.Height * 0.05f;
			//Pin = plugin.CreatePin(); //enable pin handling
			//Pin.Alignment = HorizontalAlign.Right;
			
			TextFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 0, 150, 225, false, false, true);
			FadedFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 200, 200, 200, false, false, true);
			EnabledFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 0, 255, 0, false, false, true);
			DisabledFont = Hud.Render.CreateFont("tahoma", plugin.FontSize, 255, 255, 0, 0, false, false, true); //170, 150, 120
			
			HoveredBrush = plugin.HighlightBrush;
			DrawBrush = Hud.Render.CreateBrush(255, 255, 255, 255, 1f);
			SelectedBrush = Hud.Render.CreateBrush(255, 0, 150, 225, 1f);
			
			//Addons = Hud.AllPlugins.Where(p => !(p is IMenuAddon) && !(p is Razor.Movable.IMovable) && p.GetType().Namespace != Namespace).OrderBy(p => p.GetType().Namespace + "." + p.GetType().Name).ToList(); //plugin.Addons.Values.Where(a => a != this).OrderBy(a => a.Id).ToList();

			
			//Label
			Label = new LabelCanvasDecorator(Hud, (label, x, y) => {
				DrawBrush.DrawRectangle(x, y, plugin.MenuHeight*0.5f - 2, plugin.MenuHeight*0.5f - 2);
				DrawBrush.DrawRectangle(x+plugin.MenuHeight*0.25f, y+plugin.MenuHeight*0.25f, plugin.MenuHeight*0.5f - 2, plugin.MenuHeight*0.5f - 2);
			}) {ContentHeight = plugin.MenuHeight - 7, ContentWidth = plugin.MenuHeight - 7, SpacingLeft = 4, SpacingRight = 3, SpacingTop = 4, SpacingBottom = 3}; //new LabelStringDecorator(Hud, "🔌") {Font = TextFont}; //LabelTextureDecorator(Hud, Hud.Texture.GetTexture(363251, 0)) {TextureHeight = 38, ContentHeight = plugin.MenuHeight}; //2710494966 //new LabelStringDecorator(Hud, "⚙") {Font = TextFont};
			
			HintActivate = plugin.CreateHint("Click to Activate");
			HintDeactivate = plugin.CreateHint("Click to Deactivate");
			HintPerformance = new LabelTableDecorator(Hud,
				new LabelRowDecorator(Hud,
					new LabelStringDecorator(Hud) {Font = plugin.TooltipFont, Alignment = HorizontalAlign.Right, SpacingLeft = 10, SpacingRight = 10},
					new LabelStringDecorator(Hud) {Font = plugin.TooltipFont, Alignment = HorizontalAlign.Left, SpacingLeft = 10, SpacingRight = 10}
				)
			) {
				SpacingTop = 5,
				SpacingBottom = 5,
				OnFillRow = (row, index) => {
					if (SelectedFilter == null || Table.HoveredRow <= 0 || Table.HoveredRow >= SelectedFilter.Plugins.Length || index >= SelectedFilter.Plugins[Table.HoveredRow].PerformanceCounters.Count)
						return false;
					
					KeyValuePair<string, IPerfCounter> pair = SelectedFilter.Plugins[Table.HoveredRow].PerformanceCounters.ElementAt(index);
					((LabelStringDecorator)row.Labels[0]).StaticText = pair.Key;
					((LabelStringDecorator)row.Labels[1]).StaticText = pair.Value.LastValue.ToString("0.##");
					
					return true;
				}
			};
			
			Table = new LabelTableDecorator(Hud, 
				new LabelRowDecorator(Hud, 
					new LabelStringDecorator(Hud, TextEnabled) {Font = EnabledFont, SpacingTop = 3, SpacingBottom = 3},
					new LabelRowDecorator(Hud,
						new LabelStringDecorator(Hud) {Font = FadedFont}, //namespace
						new LabelStringDecorator(Hud) {Font = TextFont}, //plugin name
						new LabelStringDecorator(Hud, "📊") {Hint = HintPerformance, Font = TextFont, SpacingLeft = 3} //plugin name //📊📈
					) {Alignment = HorizontalAlign.Left, SpacingTop = 3, SpacingBottom = 3, SpacingLeft = 3, SpacingRight = 3}
				) {OnClick = OnClick}
			) {
				BackgroundBrush = plugin.BgBrush,
				HoveredBrush = plugin.HighlightBrush,
				SpacingLeft = 10, 
				SpacingRight = 10,
				SpacingBottom = 5,
				FillWidth = false,
				Count = () => Filters.Count > 0 ? Filters.Max(f => f.Plugins.Length) : 0, //SelectedFilter is object ? SelectedFilter.Plugins.Length : 0, //optional, let the table know in advance how many rows of data it needs to render, it will try to partition the table to fit it all on screen
				OnFillRow = (row, index) => {
					if (SelectedFilter == null || index >= SelectedFilter.Plugins.Length)
						return false;
					
					var addon = SelectedFilter.Plugins[index];
					LabelStringDecorator statusUI = (LabelStringDecorator)row.Labels[0];
					LabelStringDecorator namespaceUI = (LabelStringDecorator)((LabelRowDecorator)row.Labels[1]).Labels[0];
					LabelStringDecorator nameUI = (LabelStringDecorator)((LabelRowDecorator)row.Labels[1]).Labels[1]; //row.Labels[1];
					
					//string nameSpace = addon.GetType().Namespace;
					//nameSpace = nameSpace.Remove(0, 14); //removeString.Length //nameSpace.IndexOf("Turbo.Plugins.");
					namespaceUI.StaticText = addon.GetType().Namespace.Remove(0, 14) + "."; //nameSpace + ".";
					nameUI.StaticText = addon.GetType().Name; //+ " (" + nameSpace + ")";
					
					bool enabled = addon.Enabled;
					if (enabled)
					{
						statusUI.StaticText = TextEnabled;
						statusUI.Font = EnabledFont;
						statusUI.Hint = HintDeactivate;
						//nameUI.StaticText = addon.GetType().Name;
						nameUI.Font = EnabledFont;
						//nameUI.Hint = HintDeactivate;
						row.Hint = HintDeactivate;
						
						//((LabelStringDecorator)((LabelRowDecorator)row.Labels[1]).Labels[2].Hint).StaticText = string.Join(System.Environment.NewLine, addon.PerformanceCounters.Keys); //row.Labels[1];
					}
					else
					{
						statusUI.StaticText = TextDisabled;
						statusUI.Font = DisabledFont;
						statusUI.Hint = HintActivate;
						//nameUI.StaticText = addon.GetType().Name;
						nameUI.Font = DisabledFont;
						//nameUI.Hint = HintActivate;
						row.Hint = HintActivate;
					}
					
					
					
					return true;
				}
			};
			
			//SaveUI = plugin.CreateSave();
			//SaveUI.Enabled = false;
			//SaveUI.Hint = plugin.CreateHint("Save to File");
			//SaveUI.OnClick = Save;
			
			//ConfigFile = string.Format("{0}\\{1}.cs", ConfigFilePath, ConfigFileName);
			foreach (var filter in Filters)
				filter.Plugins = Hud.AllPlugins.Where(p => filter.Check(p)).OrderBy(p => p.GetType().Namespace.Remove(0, 14) + "." + p.GetType().Name).ToArray();
			
			SelectedFilter = Filters[0];
			
			//string filepath = string.Format("{0}\\{1}.cs", !string.IsNullOrEmpty(SelectedFilter.ConfigFilePath) ? SelectedFilter.ConfigFilePath : "plugins\\User", SelectedFilter.ConfigFile);
			DeleteUIVisible = true;
			DeleteUI = new LabelStringDecorator(Hud, "❌") { //"ⓧ"
				Hint = plugin.DeleteConfigHint, /*new LabelStringDecorator(Hud, () => {
					var filepath = string.Format("{0}\\{1}.cs", !string.IsNullOrEmpty(SelectedFilter.ConfigFilePath) ? SelectedFilter.ConfigFilePath : "plugins\\User", SelectedFilter.ConfigFile);
					return filepath + " (" + System.IO.File.Exists(filepath) + ")";
				}) {Font = TextFont},*/ 
				Font = plugin.ResumeFont, //PinnedFont, //Hud.Render.CreateFont("tahoma", 13, 255, 255, 0, 0, false, false, true),
				Alignment = HorizontalAlign.Right, 
				SpacingLeft = 5, 
				SpacingRight = 5,
				SpacingTop = 2, //plugin.CreateDelete(filepath); //plugin.CreateDelete(ConfigFile); //CreateSave();
				OnBeforeRender = (label) => {
					int diff = Hud.Game.CurrentGameTick - FileCheckUpdateTick;
					if (diff < 0 || diff > 120)
					{
						FileCheckUpdateTick = Hud.Game.CurrentGameTick;
						DeleteUIVisible = System.IO.File.Exists(string.Format("{0}\\{1}.cs", !string.IsNullOrEmpty(SelectedFilter.ConfigFilePath) ? SelectedFilter.ConfigFilePath : "plugins\\User", SelectedFilter.ConfigFile));
						//label.Enabled = System.IO.File.Exists(string.Format("{0}\\{1}.cs", !string.IsNullOrEmpty(SelectedFilter.ConfigFilePath) ? SelectedFilter.ConfigFilePath : "plugins\\User", SelectedFilter.ConfigFile));
					}
					return DeleteUIVisible; //visible; //label.Enabled;
				},
				OnClick = (label) => {
					var file = string.Format("{0}\\{1}.cs", !string.IsNullOrEmpty(SelectedFilter.ConfigFilePath) ? SelectedFilter.ConfigFilePath : "plugins\\User", SelectedFilter.ConfigFile);
					System.IO.File.Delete(file);
					//label.Enabled = System.IO.File.Exists(file);
				}
			};

			/*DeleteUI.StaticText = "ⓧ"; //x encircled
			DeleteUI.Font = Hud.Render.CreateFont("tahoma", 12, 255, 255, 0, 0, false, false, true);
			DeleteUI.Enabled = File.Exists(ConfigFile);
			DeleteUI.Hint = plugin.CreateHint("Delete Saved Config File");
			DeleteUI.OnClick = (label) => {
				File.Delete(ConfigFile);
				DeleteUI.Enabled = File.Exists(ConfigFile); //false;
			};*/
			
			/*new LabelExpandDecorator(Hud,
				new LabelStringDecorator(Hud, () => SelectedFilter is object ? SelectedFilter.Name : Filters[0].Name) {Hint = plugin.CreateHint("Filter Plugins Displayed"), Font = TextFont, SpacingLeft = 10, SpacingRight = 10},
				new LabelTableDecorator(Hud, 
					new LabelRowDecorator(Hud, 
						new LabelStringDecorator(Hud) {Font = TextFont, Alignment = HorizontalAlign.Left, OnClick = SelectFilter, SpacingLeft = 10, SpacingRight = 10}
					)
				) {
					BackgroundBrush = plugin.BgBrush,
					Alignment = HorizontalAlign.Left,
					OnFillRow = (row, index) => {
						if (Filters == null || index >= Filters.Count)
							return false;
						
						((LabelStringDecorator)row.Labels[0]).StaticText = Filters[index].Name;
						
						return true;
					}
				}
			) {Alignment = HorizontalAlign.Left},*/
			SelectUI = new LabelRowDecorator(Hud) {BackgroundBrush = plugin.BgBrush, HoveredBrush = plugin.HighlightBrush};
			foreach (var filter in Filters)
			{
				filter.Decorator = new LabelRowDecorator(Hud,
					new LabelStringDecorator(Hud, filter.Name) {Font = TextFont, SpacingBottom = 3},
					new LabelStringDecorator(Hud, (filter.Plugins.Length > 0 ? " ("+filter.Plugins.Length+")" : string.Empty)) {Font = plugin.TitleFont}
				) {
					BorderBrush = SelectedFilter == filter ? SelectedBrush : null,
					SpacingLeft = 10, 
					SpacingRight = 10,
					OnClick = SelectFilter
				};
				SelectUI.Labels.Add(filter.Decorator);
			}

			
			Panel = new LabelColumnDecorator(Hud,
				new LabelDelayedDecorator(Hud,
					new LabelAlignedDecorator(Hud, 
						new LabelStringDecorator(Hud, () => "TOGGLE PLUGINS") {Font = plugin.TitleFont, SpacingLeft = 15, SpacingRight = 15},
						DeleteUI, //plugin.CreateDelete(ConfigFile), //
						plugin.CreateSave(Save)//, //SaveUI,
						//plugin.CreatePin(this)
					)
				) {BackgroundBrush = plugin.BgBrush},
				/*new LabelAlignedDecorator(Hud,
					
					//DeleteUI,
					plugin.CreateSave(Save)//, //SaveUI,
				) {BackgroundBrush = plugin.BgBrush, SpacingBottom = 4},*/
				SelectUI,
				Table
			);
			
			Plugin = plugin;
		}
		
		public void SelectFilter(ILabelDecorator label)
		{
			//string selected = ((LabelStringDecorator)((LabelRowDecorator)label).Labels[0]).StaticText;
			if (SelectedFilter.Decorator == label)
				return;

			SelectedFilter.Decorator.BorderBrush = null;
			
			SelectedFilter = Filters.FirstOrDefault(f => f.Decorator == label);
			if (SelectedFilter is object)
				SelectedFilter.Decorator.BorderBrush = SelectedBrush;
		
			//force DeleteUI update
			FileCheckUpdateTick = 0;
		}
		
		public void OnClick(ILabelDecorator label)
		{
			//SaveUI.Enabled = true;
			//DeleteUI.Enabled = File.Exists(ConfigFile);
			if (SelectedFilter == null)
				return;
			
			if (Table.HoveredRow > -1 && Table.HoveredRow < SelectedFilter.Plugins.Length) //Table.HoveredRow > -1 && Table.HoveredCol > -1) //&& 
			{
				//Hud.Sound.Speak("Row: " + Table.HoveredRow + ", Column: " + col + ", Index: " + index); //(int)Math.Floor((float)col / (float)PluginsPerColumn));
				IPlugin addon = SelectedFilter.Plugins[Table.HoveredRow];
				addon.Enabled = !addon.Enabled;
				
				//write config file
				//Hud.TextLog.Queue(this); //calls Log() at the appropriate time
				SaveFilter = SelectedFilter;
				/*SaveFilter = null;
				if (SelectedFilter.ConfigFile == null)
				{
					//find the matching filter...
					foreach (PluginFilter filter in Filters.Where(f => f.ConfigFile is object))
					{
						if (filter.Check(addon))
						{
							SaveFilter = filter;
							break;
						}
					}
				}
				
				if (SaveFilter is object)*/
				//	Save(null);
				Hud.TextLog.Queue(this, 3);
			}
		}
		
		public void Save(ILabelDecorator label)
		{
			//SaveUI.Enabled = false;
			//if (SaveFilter == null)
			//	return;
			
			//Hud.Sound.Speak("save");
			//Plugin.Save();
			Hud.TextLog.Queue(this, 3);
		}
		
		public void Log()
		{
			//Hud.Sound.Speak("log");
			TextBuilder.Clear();
			TextBuilder.Append("/*\n\tThis file was generated by Razor\\Menu\\MenuTogglePlugins.cs and contains custom plugin toggle states.\n*/\n\n");
			TextBuilder.Append("namespace Turbo.Plugins.User\n{\n");
			TextBuilder.Append("\tusing Turbo.Plugins.Default;\n");
			TextBuilder.Append("\tusing Turbo.Plugins.Razor.Util;\n\n");
			//TextBuilder.Append("\tusing Turbo.Plugins.Razor;\n\n");
			//TextBuilder.AppendFormat("\tpublic class {0} : BasePlugin, ICustomizer\n\t{\n", ConfigFileName); //can't use { or } in AppendFormat without escaping it
			TextBuilder.AppendFormat("\tpublic class {0} : BasePlugin, ICustomizer\n", SaveFilter.ConfigFile);
			TextBuilder.Append("\t{\n");
			TextBuilder.AppendFormat("\t\tpublic {0}() ", SaveFilter.ConfigFile);
			TextBuilder.Append("{ Enabled = true; Order = int.MaxValue; }\n\n"); //apply these defaults last
			//TextBuilder.Append("\t\tpublic override void Load(IController hud) { base.Load(hud); }\n\n");
			TextBuilder.Append("\t\tpublic void Customize()\n\t\t{\n");
			
			foreach (IPlugin plugin in SaveFilter.Plugins) //Addons)
			{
				//string nameSpace = plugin.GetType().Namespace;
				//nameSpace = nameSpace.Remove(0, 14);
				//string path = typeof(plugin).ToString();

				//TextBuilder.AppendFormat("\t\t\tHud.TogglePlugin<{0}.{1}>({2});\n", nameSpace, plugin.GetType().Name, plugin.Enabled.ToString().ToLower()); // "\t\t\t\tplugin.ConfigureDock(\"{0}\", \"{1}\");\n", dock.Id, string.Join("\", \"", dock.Addons.Select(a => a.Id)));
				TextBuilder.AppendFormat("\t\t\tHud.TryTogglePlugin(\"{0}\", {1});\n", plugin.GetType().ToString().Remove(0, 14), plugin.Enabled.ToString().ToLower()); // "\t\t\t\tplugin.ConfigureDock(\"{0}\", \"{1}\");\n", dock.Id, string.Join("\", \"", dock.Addons.Select(a => a.Id)));
			}
			
			TextBuilder.Append("\n\t\t}\n\t}\n}");

			//save backup to logs folder
			//Hud.TextLog.Log(ConfigFileName, TextBuilder.ToString(), false, false);
			
			//tier 3 only
			//File.WriteAllText(ConfigFile, TextBuilder.ToString());
			File.WriteAllText(string.Format("{0}\\{1}.cs", !string.IsNullOrEmpty(SaveFilter.ConfigFilePath) ? SaveFilter.ConfigFilePath : "plugins\\User", SaveFilter.ConfigFile), TextBuilder.ToString());
		}
	}
}