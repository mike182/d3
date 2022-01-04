namespace Turbo.Plugins.Razor.Movable
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.Linq;
	using System.Text; //StringBuilder
	using System.Windows.Forms; //Keys
	using SharpDX.DirectInput; //Key
	using SharpDX.DirectWrite; //TextLayout
	using System.IO; //File.WriteAllText, tier 3
	//using Gma.System.MouseKeyHook;

	using Turbo.Plugins.Default;
	using Turbo.Plugins.Razor.Click;
	using Turbo.Plugins.Razor.Hotkey;
	using Turbo.Plugins.Razor.Log;
	using Turbo.Plugins.Razor.Label; //debug
	using Turbo.Plugins.Razor.Util;

    public class MovableController : BasePlugin, IInGameTopPainter, IHotkeyEventHandler, IAfterCollectHandler, ILeftClickHandler, IRightClickHandler, ITextLogger, ILeftBlockHandler //, ICustomizer, IInGameWorldPainter
    {
		//public bool AllowDragAndDrop { get; set; } = true; //only while outlines shown
		public bool AutoSaveConfigChanges { get; set; } = true; //whenever the floater position or enable/disable status changes, update the config file written to /logs folder
		public string ConfigFileName { get; set; } = "MovablePluginConfig";
		public string ConfigFilePath { get; set; } = @"plugins\User\"; //tier 3

		public IKeyEvent TogglePickup { get; set; } //optional
		public IKeyEvent ToggleEnable { get; set; } //optional
		public IKeyEvent ToggleEditMode { get; set; } //optional
		public IKeyEvent ToggleDragAndDrop { get; set; } //optional
		public IKeyEvent ToggleGrid { get; set; } //optional
		public IKeyEvent HotkeyCancel { get; set; } //optional
		public IKeyEvent HotkeySave { get; set; } //optional
		public IKeyEvent HotkeyUndo { get; set; } //optional
		public IKeyEvent HotkeyUndoAll { get; set; } //optional
		public IKeyEvent HotkeyPickupNext { get; set; } //optional
		public IKeyEvent HotkeyPickupPrev { get; set; } //optional
		
		public string ArrowBottomLeft { get; set; } = "↙";
		public string ArrowBottomRight { get; set; } = "↘"; //
		public string CornerBottomLeft { get; set; } = "◣";
		public string CornerBottomRight { get; set; } = "◢";
		public string ArrowHorizontal { get; set; } = "↔";
		public string ArrowVertical { get; set; } = "↕";
		
		//public string CornerTopLeft { get; set; } = "◤";
		//public string CornerTopRight { get; set; } = "◥";
		
		public bool EditMode { get; set; } = false; //starting state, can be toggled on and off with a hotkey
		public bool ShowGrid { get; set; } = true; //while in Edit Mode only
		public int GridSize { get; set; } = 10;
		public IBrush GridBrush { get; set; }
		public IBrush GridBrush2 { get; set; }
		public IFont EnabledFont { get; set; }
		public IFont EnabledSelectedFont { get; set; }
		public IFont EnabledUnselectedFont { get; set; }
		public IBrush EnabledBrush { get; set; }
		public IBrush EnabledSelectedBrush { get; set; }
		public IFont DisabledFont { get; set; }
		public IFont DisabledSelectedFont { get; set; }
		public IFont DisabledUnselectedFont { get; set; }
		public IBrush DisabledBrush { get; set; }
		public IBrush DisabledSelectedBrush { get; set; }
		public IBrush TemporaryBrush { get; set; }
		public IBrush TemporarySelectedBrush { get; set; }
		public IFont SelectedFont { get; set; }
		public IFont EnabledDragFont { get; set; }
		public IFont DisabledDragFont { get; set; }
		
		public string HoverHint { get; set; } = "{0}: Pick Up / Put Down\n{1}: Enable / Disable\n{2}: Cancel"; //set to null or empty string to disable
		public string DragAndDropHint { get; set; } = "Drag and Drop";
		
		public Func<bool> LeftClickBlockCondition { get; private set; }
		
		public Dictionary<IMovable, List<MovableArea>> MovablePlugins { get; private set; } //= new List<IMovable>();
		public List<Tuple<string, int, RectangleF, bool, string, string, string>> Config { get; set; } = new List<Tuple<string, int, RectangleF, bool, string, string, string>>(); //so that a config file can fail without exceptions if a floater-implemented plugin or addon is removed
		
		public MovableArea CursorPluginArea { get; set; }
		public MovableArea HoveredPluginArea { get; set; }
		public MovableArea ResizePluginArea { get; set; }
		public UIOverlapHelper Overlay { get; private set; }
		
		private IMovable[] Lookup;
		private List<MovableArea> DeletionQueue = new List<MovableArea>();
		private float PickedUpAtX = 0;
		private float PickedUpAtY = 0;
		private float ResizeHoverSize;
		private bool HoveredResizeRight = false;
		private bool HoveredResizeLeft = false;
		private bool LButtonPressed = false;
		private List<string> ConfigChanged = new List<string>();
		private bool Queued = false;
		//private bool ManualWrite = false;
		private bool AnyMovablePlugins = false;
		private StringBuilder TextBuilder;
		
		private bool clicked = false;
		
        public MovableController() : base()
        {
            Enabled = true;
			Order = -10001; //run this before the other floater stuff
        }
		
        public override void Load(IController hud)
        {
            base.Load(hud);
			
			TextBuilder = new StringBuilder();
			
			//TogglePickup = Hud.Input.CreateKeyEvent(true, Key.C, true, false, false); //Ctrl + C
			//TogglePickup = Hud.Input.CreateKeyEvent(true, Key.E, false, false, false); //Ctrl + C
			ToggleEnable = Hud.Input.CreateKeyEvent(true, Key.X, true, false, false); //Ctrl + X
			ToggleEditMode = Hud.Input.CreateKeyEvent(true, Key.F12, false, false, false); //F12
			ToggleGrid = Hud.Input.CreateKeyEvent(true, Key.G, true, false, false); //Ctrl + G
			HotkeyCancel = Hud.Input.CreateKeyEvent(true, Key.Escape, false, false, false); //Esc
			HotkeySave = Hud.Input.CreateKeyEvent(true, Key.S, true, false, false); //Ctrl + S
			HotkeyUndo = Hud.Input.CreateKeyEvent(true, Key.Z, true, false, false); //Ctrl + Z
			HotkeyUndoAll = Hud.Input.CreateKeyEvent(true, Key.D0, true, false, false); //Ctrl + Z
			HotkeyPickupNext = Hud.Input.CreateKeyEvent(true, Key.Right, false, false, false); //Left Arrow
			HotkeyPickupPrev = Hud.Input.CreateKeyEvent(true, Key.Left, false, false, false); //Right Arrow
			
			GridBrush = Hud.Render.CreateBrush(35, 175, 175, 175, 1);
			GridBrush2 = Hud.Render.CreateBrush(35, 10, 10, 10, 1);
			EnabledBrush = Hud.Render.CreateBrush(255, 200, 200, 255, 2, SharpDX.Direct2D1.DashStyle.Dash);
			EnabledSelectedBrush = Hud.Render.CreateBrush(255, 200, 200, 255, 2);
			EnabledFont = Hud.Render.CreateFont("tahoma", 7f, 255, 200, 200, 255, false, false, 175, 0, 0, 0, true); //, 255, 0, 0, 0, true
			EnabledUnselectedFont = Hud.Render.CreateFont("tahoma", 7f, 125, 200, 200, 255, false, false, false); //, 255, 0, 0, 0, true
			EnabledSelectedFont = Hud.Render.CreateFont("tahoma", 7f, 255, 255, 255, 255, false, false, 100, 122, 147, 255, true);
			DisabledBrush = Hud.Render.CreateBrush(255, 255, 0, 0, 2, SharpDX.Direct2D1.DashStyle.Dash);
			DisabledSelectedBrush = Hud.Render.CreateBrush(255, 255, 0, 0, 2);
			DisabledFont = Hud.Render.CreateFont("tahoma", 7f, 255, 255, 50, 50, false, false, 255, 0, 0, 0, true); //, 255, 0, 0, 0, true
			DisabledUnselectedFont = Hud.Render.CreateFont("tahoma", 7f, 150, 255, 50, 50, false, false, false); //, 255, 0, 0, 0, true
			DisabledSelectedFont = Hud.Render.CreateFont("tahoma", 7f, 255, 255, 255, 255, false, false, 175, 255, 50, 50, true);
			SelectedFont = Hud.Render.CreateFont("tahoma", 14f, 255, 255, 255, 255, true, false, 175, 0, 0, 0, true);
			EnabledDragFont = Hud.Render.CreateFont("tahoma", 10f, 255, 200, 200, 255, false, false, false); //, 175, 0, 0, 0, true);
			DisabledDragFont = Hud.Render.CreateFont("tahoma", 10f, 255, 255, 50, 50, false, false, false); //, 255, 0, 0, 0, true);
			//SelectedBrush = Hud.Render.CreateBrush(100, 200, 200, 200, 4, SharpDX.Direct2D1.DashStyle.Dash);
			TemporaryBrush = Hud.Render.CreateBrush(255, 69, 41, 255, 2, SharpDX.Direct2D1.DashStyle.Dash);
			TemporarySelectedBrush = Hud.Render.CreateBrush(255, 69, 41, 255, 2);
			
			ResizeHoverSize = SelectedFont.MaxHeight*0.12f;
			
			/*Hook.GlobalEvents().MouseDownExt += (sender, e) =>
            {
                //Console.Write(e.KeyChar);
                //if (e.KeyChar == 'q') quit();
				if (Hud.Window.IsForeground && EditMode)
				{
					Hud.Sound.Speak("click");
					clicked = true;
					
					if (HoveredPluginArea is object)
					{
						//suppress the left mouse button click only if hovering over a MovableArea in Edit Mode
						if (e.Button == System.Windows.Forms.MouseButtons.Left) 
							e.Handled = true;
					}
				}
            };*/
			
			//prevent left clicks when trying to drag or resize MovableAreas
			LeftClickBlockCondition = () => EditMode && HoveredPluginArea is object; // || CursorPluginArea is object)
        }
		
		public void OnHotkeyEvent(IKeyEvent keyEvent)
		{
			if (keyEvent.IsPressed) //Ctrl + C
			{
				//Hud.Sound.Speak("move");
				
				if (ToggleEditMode is object && ToggleEditMode.Matches(keyEvent))
				{
					EditMode = !EditMode;
					
					if (!EditMode) //CursorPluginArea is object
					{
						//CursorPluginArea = null;
						//ResizePluginArea = null;
						PutDown();
						HoveredPluginArea = null;
					}
				}
				else if (HotkeySave is object && HotkeySave.Matches(keyEvent))
				{
					//ConfigChanged = true;
					//Hud.Sound.Speak("Queue Config Save");
					//ManualWrite = true;
					QueueConfigSave();
				}
				else if (TogglePickup is object && TogglePickup.Matches(keyEvent)) //if (keyEvent.Key == Key.C && keyEvent.ControlPressed) //Ctrl + C
				{
					if (LButtonPressed) //if (CursorPluginArea is object)
						OnLeftMouseUp();
					else
						OnLeftMouseDown();
				}
				else
				{
					MovableArea selected = CursorPluginArea ?? HoveredPluginArea;
					
					//pass along the key event if the MovableArea is currently selected while in Modify Mode
					if (selected is object && selected.Owner is IMovableKeyEventHandler)
						((IMovableKeyEventHandler)selected.Owner).OnKeyEvent(this, keyEvent, selected); //, CursorPluginArea == selected

					if (!EditMode)
						return;

					//process default hotkeys
					if (ToggleEnable is object && ToggleEnable.Matches(keyEvent)) 
					{
						if (selected is object)
							ToggleArea(selected);
					}
					else if (ToggleGrid is object && ToggleGrid.Matches(keyEvent)) 
					{
						ShowGrid = !ShowGrid;
					}
					else if (HotkeyPickupNext is object && HotkeyPickupNext.Matches(keyEvent))
					{
						if (selected == null)
							return;
						
						if (CursorPluginArea == null) // && HoveredPluginArea is object
						{
							PickUp(HoveredPluginArea);
						}
						
						//only start searching after finding the currently selected plugin
						int index = Array.IndexOf(Lookup, selected.Owner);
						if (index < 0)
							return;
						
						//try to find it in the same plugin first
						List<MovableArea> areas = MovablePlugins[Lookup[index]];
						for (int i = areas.IndexOf(selected) + 1; i < areas.Count && CursorPluginArea == selected; ++i)
						{
							MovableArea area = areas[i];
							if (IsHovered(area))
							{
								PickUp(area);
								break;
							}
						}
						
						if (CursorPluginArea == selected)
						{
							for (int i = index+1; i < Lookup.Length && CursorPluginArea == selected; ++i)
							{
								areas = MovablePlugins[Lookup[i]];
								for (int j = 0; j < areas.Count; ++j) //j = areas.IndexOf(selected)
								{
									MovableArea area = areas[j];
									if (IsHovered(area))
									{
										PickUp(area);
										break;
									}
								}
							}
						}
						
						if (CursorPluginArea == selected)
						{
							for (int i = 0; i <= index && CursorPluginArea == selected; ++i)
							{
								areas = MovablePlugins[Lookup[i]];
								for (int j = 0; j < areas.Count; ++j)
								{
									MovableArea area = areas[j];
									
									if (area == selected)
										break;
									if (IsHovered(area))
									{
										PickUp(area);
										break;
									}
								}
							}
						}
					}
					else if (HotkeyPickupPrev is object && HotkeyPickupPrev.Matches(keyEvent))
					{
						if (selected == null)
							return;
						
						if (CursorPluginArea == null) // && HoveredPluginArea is object
						{
							PickUp(HoveredPluginArea);
						}
						
						//only start searching after finding the currently selected plugin
						int index = Array.IndexOf(Lookup, selected.Owner);
						if (index < 0)
							return;
						
						//try to find it in the same plugin first
						List<MovableArea> areas = MovablePlugins[Lookup[index]];
						for (int i = areas.IndexOf(selected) - 1; i > -1 && CursorPluginArea == selected; --i)
						{
							MovableArea area = areas[i];
							if (IsHovered(area))
							{
								PickUp(area);
								break;
							}
						}
						
						if (CursorPluginArea == selected)
						{
							for (int i = index-1; i > -1 && CursorPluginArea == selected; --i)
							{
								areas = MovablePlugins[Lookup[i]];
								for (int j = areas.Count-1; j > -1; --j) //j = areas.IndexOf(selected)
								{
									MovableArea area = areas[j];
									if (IsHovered(area))
									{
										PickUp(area);
										break;
									}
								}
							}
						}
						
						if (CursorPluginArea == selected)
						{
							for (int i = Lookup.Length-1; i >= index && CursorPluginArea == selected; --i)
							{
								areas = MovablePlugins[Lookup[i]];
								for (int j = areas.Count-1; j > -1; --j)
								{
									MovableArea area = areas[j];
									
									if (area == selected)
										break;
									if (IsHovered(area))
									{
										PickUp(area);
										break;
									}
								}
							}
						}
					}
					else if (HotkeyCancel is object && HotkeyCancel.Matches(keyEvent))
					{
						PutDown();
					}
					else if (HotkeyUndo is object && HotkeyUndo.Matches(keyEvent))
					{
						if (selected is object)
						{
							selected.Undo();
							PutDown();
						}
					}
					else if (HotkeyUndoAll is object && HotkeyUndoAll.Matches(keyEvent))
					{
						if (selected is object)
						{
							selected.Reset();
							PutDown();
						}
					}
				}
			}
		}
		
		public void AfterCollect()
		{
			if (!Hud.Game.IsInGame)
			{
				if (CursorPluginArea is object)
				{
					CursorPluginArea = null;
				}
				
				if (ResizePluginArea is object)
					ResizePluginArea = null;
				
				return;
			}

			//initialization
			//have to wait until you are in a game for the first time, otherwise OnRegister calls that reference existing UI element positions may return 0 because those haven't been initialized yet
			if (MovablePlugins == null)
			{
				/*if (BlockingFunc is object)
				{
					ClickEventHandler handler = Hud.GetPlugin<ClickEventHandler>();
					handler.BlockLeftClick.Add(() => EditMode && HoveredPluginArea is object); //BlockingFunc);
					//BlockingFunc = null;
				}*/
				
				MovablePlugins = new Dictionary<IMovable, List<MovableArea>>();
				Overlay = Hud.GetPlugin<UIOverlapHelper>();
				
				//find all plugins that this controller will manage
				foreach (IPlugin p in Hud.AllPlugins.Where(p => p is IMovable)) //p.Enabled && 
				{
					//add to the master list
					//List<MovableArea> areas = new List<MovableArea>();
					IMovable plugin = (IMovable)p;
					MovablePlugins.Add(plugin, new List<MovableArea>()); //areas
					
					//tell the plugin to define all the MovableAreas (with MovableController.CreateArea calls)
					plugin.OnRegister(this);
				}
				
				Lookup = MovablePlugins.Keys.ToArray();
				
				//loading any new config settings introduced after initialization
				if (Config.Count > 0)
				{
					foreach (Tuple<string, int, RectangleF, bool, string, string, string> settings in Config.ToArray())
					{
						IMovable m = MovablePlugins.Keys.FirstOrDefault(p => p.GetType().Name == settings.Item1); //Hud.AllPlugins.FirstOrDefault(p => p.GetType().Name == settings.Item1);
						if (m is object)
						{
							List<MovableArea> areas = MovablePlugins[m];
							MovableArea area = null;
							bool isNamed = !string.IsNullOrEmpty(settings.Item5);
							
							if (isNamed)
							{
								area = areas.FirstOrDefault(ma => ma.Name == settings.Item5);
							}
							else if (settings.Item2 < areas.Count)
							{
								area = areas[settings.Item2]; //.SetConfig(settings.Item3.X, settings.Item3.Y, settings.Item3.Width, settings.Item3.Height, settings.Item4, settings.Item5);
							}
							
							if (area is object)
							{
								area.SetConfig(settings.Item3.X, settings.Item3.Y, settings.Item3.Width, settings.Item3.Height, settings.Item4, settings.Item7, settings.Item6);
							}
							else if (isNamed)
							{
								//the area doesn't currently exist, create it
								//public MovableArea CreateArea(IMovable owner, string areaName, RectangleF rect, bool enabledAtStart, bool saveToConfig, string configName, ResizeMode resize = ResizeMode.Off, ClipState clipState = ClipState.BeforeClip)
								//public MovableArea CreateArea(IMovable owner, string areaName, RectangleF rect, bool enabledAtStart, bool saveToConfig, ResizeMode resize = ResizeMode.Off, ClipState clipState = ClipState.BeforeClip)
								area = CreateArea(
									m,
									settings.Item5, //area name
									settings.Item3,
									settings.Item4, //enabled at start?
									true, //save to config file?
									settings.Item7, //configFileName
									ResizeMode.Off //resizable?
								);
								
								if (area is object)
								{
									area.ConfigSettings = settings.Item6;
									//areas.Add(area);
								}
							}
						}
						
						//Config.Remove(settings);
					}
					
					Config.Clear();
				}
			}
			else
				AnyMovablePlugins = MovablePlugins.Any();
			
			//if a MovableArea removal was requested, do it now
			if (DeletionQueue.Count > 0)
			{
				bool save = false;
				foreach (MovableArea area in DeletionQueue)
				{
					if (MovablePlugins.ContainsKey(area.Owner))
					{
						if (area.Owner is IMovableDeleteAreaHandler)
							((IMovableDeleteAreaHandler)area.Owner).OnDeleteArea(area);

						MovablePlugins[area.Owner].Remove(area);
						
						if (area.SaveToConfig)
							save = true;
					}
				}
					
				DeletionQueue.Clear();
				
				if (save)
					QueueConfigSave();
			}
			
			//output a config file into logs folder so that user can preserve these position settings
			if (ConfigChanged.Count > 0 && !Queued)
			{
				//Queued = true;
				Hud.TextLog.Queue(this);
				//QueueConfigSave();
			}
		}
		
        public void PaintTopInGame(ClipState clipState)
        {
			//wait until initialization
			if (MovablePlugins == null)
				return;
			
			if (clipState == ClipState.BeforeClip)
			{
				//debug
				//LabelDecorator.DebugWrite("CursorPluginArea? " + (CursorPluginArea is object) + "\nHoveredPluginArea? " + (HoveredPluginArea is object), Hud.Window.CursorX, Hud.Window.CursorY);
				
				//draw grid
				if (ShowGrid && EditMode)
				{
					//GridBrush.Opacity = 0.1f;
					for (int j = GridSize; j < Hud.Window.Size.Width; j += GridSize)
					{
						GridBrush.DrawLine(j, 0, j, Hud.Window.Size.Height);
						GridBrush2.DrawLine(j+1, 1, j+1, Hud.Window.Size.Height);
						
						GridBrush.DrawLine(0, j, Hud.Window.Size.Width, j);
						GridBrush2.DrawLine(1, j+1, Hud.Window.Size.Width, j+1);
					}
				}

				if (!AnyMovablePlugins)
					return;
				
				//hover check
				if (ResizePluginArea == null && CursorPluginArea == null)
				{
					HoveredPluginArea = null;
					HoveredResizeRight = false;
					HoveredResizeLeft = false;
					
					foreach (IMovable m in MovablePlugins.Keys)
					{
						if (!m.Enabled)
							continue;

						foreach (MovableArea area in MovablePlugins[m]) //areas) //for (int i = 0; i < areas.Count; ++i)
						{
							var rect = area.Rectangle;

							if (Hud.Window.CursorInsideRect(rect.X, rect.Y, rect.Width, rect.Height) && (area.ClipState == ClipState.AfterClip || Overlay.GetUiObstructingCursor() == null))
							{
								if (!Overlay.IsUiObstructingArea(area.Rectangle, UIOverlapHelper.UIGroup.Clip)) //, UIOverlapHelper.UIGroup.Tooltip
								{
									HoveredPluginArea = area;
									
									if (area.ResizeMode != ResizeMode.Off)
									{
										if (Hud.Window.CursorInsideRect(rect.Right - ResizeHoverSize, rect.Bottom - ResizeHoverSize, ResizeHoverSize, ResizeHoverSize)) //check bottom right
											HoveredResizeRight = true;
										else if (Hud.Window.CursorInsideRect(rect.Left, rect.Bottom - ResizeHoverSize, ResizeHoverSize, ResizeHoverSize)) //check bottom left
											HoveredResizeLeft = true;
									}
								}
								
								break;
							}
						}
						
						//found
						if (HoveredPluginArea is object)
							break;
					}
				}
			}
			
			if (!AnyMovablePlugins)
				return;
				
			bool isAnyHighlighted = CursorPluginArea is object || ResizePluginArea is object || HoveredPluginArea is object;
			
			foreach (IMovable m in MovablePlugins.Keys)
			{
				if (!m.Enabled)
					continue;

				string pluginName = m.GetType().Name;
				List<MovableArea> areas = MovablePlugins[m];
				
				for (int i = 0; i < areas.Count; ++i)
				{
					MovableArea area = areas[i];
					
					if (!area.Enabled && area.DeleteOnDisable)
					{
						DeleteArea(area);
						continue;
					}
					
					//save changes if requested
					if (area.Changed)
						QueueConfigSave(area);
					
					bool isAreaOnCursor = CursorPluginArea == area;
					bool isAreaResizing = ResizePluginArea == area;
					
					//check if this element should be clipped
					if (!isAreaOnCursor && !isAreaResizing && Overlay.IsUiObstructingArea(area.Rectangle, UIOverlapHelper.UIGroup.Clip, UIOverlapHelper.UIGroup.Custom)) //, UIOverlapHelper.UIGroup.Tooltip //ForceClipUI(area.Rectangle) is object)
						continue;
					
					//draw on top if highlighted, otherwise skip
					bool highlighted = isAreaOnCursor || isAreaResizing || HoveredPluginArea == area;
					if (highlighted)
					{
						if (clipState != ClipState.AfterClip)
							continue;
					}
					else if (area.ClipState != clipState)
						continue;
					
					if (area.Enabled && Hud.Game.MapMode == MapMode.Minimap)
					{
						if (isAreaOnCursor)
							m.PaintArea(this, area, Hud.Window.CursorX - PickedUpAtX, Hud.Window.CursorY - PickedUpAtY);
						else if (!Overlay.IsUiObstructingArea(area.Rectangle, UIOverlapHelper.UIGroup.Prompt, UIOverlapHelper.UIGroup.Clip, UIOverlapHelper.UIGroup.Mail, UIOverlapHelper.UIGroup.Custom)) //ForceClipUIWithEditMode(area.Rectangle) == null)
							m.PaintArea(this, area, 0, 0);
					}	
					
					if (EditMode)
					{
						RectangleF rect = area.Rectangle;
						float x = rect.X; //(isAreaOnCursor ? rect.X + Hud.Window.CursorX - PickedUpAtX : rect.X);
						float y = rect.Y; //(isAreaOnCursor ? rect.Y + Hud.Window.CursorY - PickedUpAtY : rect.Y);
						float w = rect.Width;
						float h = rect.Height;
						
						if (isAreaOnCursor)
						{
							x += Hud.Window.CursorX - PickedUpAtX;
							y += Hud.Window.CursorY - PickedUpAtY;
							//SelectedBrush.DrawRectangle(x, y, rect.Width, rect.Height);
						}
						else if (ResizePluginArea == area)
						{
							float deltaX = Hud.Window.CursorX - PickedUpAtX;
							float deltaY = Hud.Window.CursorY - PickedUpAtY;
							
							//correct the deltas based on the resize mode
							switch(area.ResizeMode)
							{
								case ResizeMode.FixedRatio:
									deltaY = deltaX * h/w;
									break;
								case ResizeMode.Horizontal:
									deltaY = 0;
									break;
								case ResizeMode.Vertical:
									deltaX = 0;
									break;
							}
							
							if (HoveredResizeRight)
							{
								if (w + deltaX < ResizeHoverSize)
								{
									deltaX = ResizeHoverSize - w;
									deltaY = deltaX * h/w;
								}
								
								if (h + deltaY < ResizeHoverSize)
								{
									deltaY = ResizeHoverSize - h;
									deltaX = deltaY * w/h;
								}

								w += deltaX;
								h += deltaY;
							}	
							else if (HoveredResizeLeft)
							{
								if (w - deltaX < ResizeHoverSize)
								{
									deltaX = w - ResizeHoverSize;
									deltaY = deltaX * h/w;
								}
								if (h + deltaY < ResizeHoverSize)
								{
									deltaY = ResizeHoverSize - h;
									deltaX = deltaY * w/h;
								}
								
								x += deltaX;
								w -= deltaX;
								h -= deltaY;
							}
								
							//debug							
							//TextLayout test = DisabledFont.GetTextLayout(string.Format("Δ({0},{1})\n=({2},{3})", deltaX, deltaY, Hud.Window.CursorX - PickedUpAtX, Hud.Window.CursorY - PickedUpAtY));
							//DisabledFont.DrawText(test, Hud.Window.CursorX, Hud.Window.CursorY - test.Metrics.Height);
						}

						IBrush brush = area.DeleteOnDisable ?
							(highlighted ? TemporarySelectedBrush : TemporaryBrush) :
							(area.Enabled ? (highlighted ? EnabledSelectedBrush : EnabledBrush) : (highlighted ? DisabledSelectedBrush : DisabledBrush )); //area.Enabled ? EnabledBrush : DisabledBrush;
						IFont font = area.Enabled ? (isAreaOnCursor ? EnabledSelectedFont : (isAnyHighlighted && !highlighted ? EnabledUnselectedFont : EnabledFont)) : (isAreaOnCursor ? DisabledSelectedFont : (isAnyHighlighted && !highlighted ? DisabledUnselectedFont : DisabledFont));
						IFont drag = area.Enabled ? EnabledDragFont : DisabledDragFont;
						
						TextLayout layout = font.GetTextLayout(pluginName + "." + area.Name);
						float y2 = y - layout.Metrics.Height - 2;
						if (y2 < 0) y2 = (y > 0 ? y : 1) + 2;
						font.DrawText(layout, x, y2);
						brush.DrawRectangle(x, y, w, h);
						
						if (area.ResizeMode != ResizeMode.Off && highlighted)
						{
							//DisabledSelectedBrush.DrawRectangle(x + w - ResizeHoverSize, y + h - ResizeHoverSize, ResizeHoverSize, ResizeHoverSize);
							//DisabledSelectedBrush.DrawRectangle(x, y + h - ResizeHoverSize, ResizeHoverSize, ResizeHoverSize);
							float strokeCorrection = brush.StrokeWidth; // + 1;
							
							TextLayout arrow = drag.GetTextLayout(CornerBottomRight);
							drag.DrawText(arrow, x + w - arrow.Metrics.Width + strokeCorrection, y + h - arrow.Metrics.Height*0.85f + strokeCorrection); // + brush.StrokeWidth
							
							arrow = drag.GetTextLayout(CornerBottomLeft);
							drag.DrawText(arrow, x - strokeCorrection, y + h - arrow.Metrics.Height*0.85f + strokeCorrection); // + brush.StrokeWidth
							
							//draw resize arrows
							if (HoveredResizeRight) 
							{
								arrow = SelectedFont.GetTextLayout(area.ResizeMode == ResizeMode.Horizontal ? ArrowHorizontal : (area.ResizeMode == ResizeMode.Vertical ? ArrowVertical : ArrowBottomRight));
								SelectedFont.DrawText(arrow, Hud.Window.CursorX - arrow.Metrics.Width, Hud.Window.CursorY - arrow.Metrics.Height);
							}
							else if (HoveredResizeLeft)
							{
								arrow = SelectedFont.GetTextLayout(area.ResizeMode == ResizeMode.Horizontal ? ArrowHorizontal : (area.ResizeMode == ResizeMode.Vertical ? ArrowVertical : ArrowBottomLeft));
								SelectedFont.DrawText(arrow, Hud.Window.CursorX, Hud.Window.CursorY - arrow.Metrics.Height);
							}
						}
					}
				}
			}
			
        }
		
		public void OnLeftMouseDown()
		{
			//Hud.Sound.Speak("down");
			
			LButtonPressed = true; //CursorPluginArea == null; //

			//if (!AllowDragAndDrop) return;
			if (!EditMode)
			{
				//pass mouse events to Movables that want to know them in a way that is managed by MovableController (e.g. it is the MovableArea that is on top of the stack under the mouse cursor)
				if (CursorPluginArea == null && ResizePluginArea == null && HoveredPluginArea is object)
				{
					if (HoveredPluginArea.Owner is IMovableLeftClickHandler)
						((IMovableLeftClickHandler)HoveredPluginArea.Owner).OnLeftMouseDown(HoveredPluginArea);
				}
				
				return;
			}
			
			if (CursorPluginArea == null && ResizePluginArea == null && HoveredPluginArea is object) //HoveredPluginArea would always be null if UiObstructingCursor is object
			{
				PickedUpAtX = Hud.Window.CursorX;
				PickedUpAtY = Hud.Window.CursorY;

				if (HoveredResizeLeft || HoveredResizeRight)
				{
					//drag resize
					ResizePluginArea = HoveredPluginArea;
				}
				else
				{
					//CursorPlugin = HoveredPlugin;
					CursorPluginArea = HoveredPluginArea;
					//PickUp();
				}
			}
		}
		
		public void OnLeftMouseUp()
		{
			//Hud.Sound.Speak("up");
			
			LButtonPressed = false;

			//if something is already on the cursor, let it be placed even if not in Modify mode
			//if (!EditMode)
			//	return;
			
			if (CursorPluginArea is object)
			{
				CursorPluginArea.Move(Hud.Window.CursorX - PickedUpAtX, Hud.Window.CursorY - PickedUpAtY);
				CursorPluginArea = null;
			}
			else if (ResizePluginArea is object)
			{
				float deltaX = Hud.Window.CursorX - PickedUpAtX;
				float deltaY = Hud.Window.CursorY - PickedUpAtY;
				
				//correct the deltas based on the resize mode
				float w = ResizePluginArea.Rectangle.Width;
				float h = ResizePluginArea.Rectangle.Height;
				switch(ResizePluginArea.ResizeMode)
				{
					case ResizeMode.FixedRatio:
						deltaY = deltaX * h/w;
						break;
					case ResizeMode.Horizontal:
						deltaY = 0;
						break;
					case ResizeMode.Vertical:
						deltaX = 0;
						break;
				}
				
				//enforce minimum dimensions
				float x = ResizePluginArea.Rectangle.X;
				
				if (HoveredResizeRight)
				{
					if (w + deltaX < ResizeHoverSize)
					{
						deltaX = ResizeHoverSize - w;
						deltaY = deltaX * h/w;
					}
					
					if (h + deltaY < ResizeHoverSize)
					{
						deltaY = ResizeHoverSize - h;
						deltaX = deltaY * w/h;
					}

					w += deltaX;
					h += deltaY;
					
					ResizePluginArea.Rectangle = new RectangleF(x, ResizePluginArea.Rectangle.Y, w, h);
				}
				else if (HoveredResizeLeft)
				{								
					if (w - deltaX < ResizeHoverSize)
					{
						deltaX = w - ResizeHoverSize;
						deltaY = deltaX * h/w;
					}
					if (h + deltaY < ResizeHoverSize)
					{
						deltaY = ResizeHoverSize - h;
						deltaX = deltaY * w/h;
					}

					x += deltaX;
					w -= deltaX;
					h -= deltaY;
					
					ResizePluginArea.Rectangle = new RectangleF(x, ResizePluginArea.Rectangle.Y, w, h);
				}
				
				ResizePluginArea = null;
			}
			else if (!EditMode && HoveredPluginArea is object)
			{
				//pass mouse events to Movables that want to know them in a way that is managed by MovableController (e.g. it is the MovableArea that is on top of the stack under the mouse cursor)
				if (CursorPluginArea == null && ResizePluginArea == null && HoveredPluginArea is object)
				{
					if (HoveredPluginArea.Owner is IMovableLeftClickHandler)
						((IMovableLeftClickHandler)HoveredPluginArea.Owner).OnLeftMouseUp(HoveredPluginArea);
				}
			}
		}
		
		public void OnRightMouseDown()
		{
			if (!EditMode && HoveredPluginArea is object)
			{
				//pass mouse events to Movables that want to know them in a way that is managed by MovableController (e.g. it is the MovableArea that is on top of the stack under the mouse cursor)
				if (CursorPluginArea == null && ResizePluginArea == null && HoveredPluginArea is object)
				{
					if (HoveredPluginArea.Owner is IMovableRightClickHandler)
						((IMovableRightClickHandler)HoveredPluginArea.Owner).OnRightMouseDown(HoveredPluginArea);
				}
			}
		}
		
		public void OnRightMouseUp()
		{
			if (!EditMode && HoveredPluginArea is object)
			{
				//pass mouse events to Movables that want to know them in a way that is managed by MovableController (e.g. it is the MovableArea that is on top of the stack under the mouse cursor)
				if (CursorPluginArea == null && ResizePluginArea == null && HoveredPluginArea is object)
				{
					if (HoveredPluginArea.Owner is IMovableRightClickHandler)
						((IMovableRightClickHandler)HoveredPluginArea.Owner).OnRightMouseUp(HoveredPluginArea);
				}
			}
		}
		
		//cursor bookkeeping
		//requires area != null
		public void PickUp(MovableArea area = null)
		{
			if (area == null)
			{
				if (HoveredPluginArea == null)
					return;
				
				area = HoveredPluginArea;
			}
			
			CursorPluginArea = area;
			ResizePluginArea = null;
			PickedUpAtX = Hud.Window.CursorX;
			PickedUpAtY = Hud.Window.CursorY;
			LButtonPressed = true;
		}
		
		//cursor bookkeeping, does not apply changes like OnLeftMouseUp does
		public void PutDown()
		{
			CursorPluginArea = null;
			ResizePluginArea = null;
			//LButtonPressed = false;
		}
		
		public void SnapTo(float x, float y)
		{
			if (CursorPluginArea is object)
			{
				float deltaX = CursorPluginArea.Rectangle.X - PickedUpAtX;
				float deltaY = CursorPluginArea.Rectangle.Y - PickedUpAtY;
	
				PickedUpAtX = x + deltaX;
				PickedUpAtY = y + deltaY;
			}
		}
		
		//legacy config (for backwards config file format support)
		public void Configure(string pluginName, int index, float x, float y, float w, float h, bool enabled = true, string areaName = null, string areaSettings = null, string configFileName = null)
		{
			Config.Add(new Tuple<string, int, RectangleF, bool, string, string, string>(pluginName, index, new RectangleF(x, y, w, h), enabled, areaName, areaSettings, configFileName));
		}
		
		//current config (supports more fields)
		public void Configure(string pluginName, string areaName, float x, float y, float w, float h, bool enabled = true, string configFileName = null, string areaSettings = null)
		{
			Config.Add(new Tuple<string, int, RectangleF, bool, string, string, string>(pluginName, 0, new RectangleF(x, y, w, h), enabled, areaName, areaSettings, configFileName));
		}

		public void ToggleArea(MovableArea area)
		{
			area.Enabled = !area.Enabled;
			
			//if (AutoSaveConfigChanges && area.SaveToConfig)
			//	ConfigChanged = true;
			//QueueConfigSave(area);
		}
		
		public bool IsHovered(MovableArea area)
		{
			return Hud.Window.CursorInsideRect(area.Rectangle.X, area.Rectangle.Y, area.Rectangle.Width, area.Rectangle.Height);
		}
		
		public MovableArea CreateArea(IMovable owner, string areaName, RectangleF rect, bool enabledAtStart, bool saveToConfig, ResizeMode resize = ResizeMode.Off, ClipState clipState = ClipState.BeforeClip)
		{
			//Hud.Sound.Speak("one");
			if (MovablePlugins.ContainsKey(owner))
			{
				MovableArea area = MovablePlugins[owner].FirstOrDefault(ma => ma.Name == areaName);
				if (area is object)
				{
					area.Enabled = enabledAtStart;
					area.Rectangle = rect;
					area.SaveToConfig = saveToConfig;
					area.ConfigFile = ConfigFileName;
					area.ResizeMode = resize;
					area.ClipState = clipState;
				}
				else
				{
					area = new MovableArea(areaName) 
					{ 
						Owner = owner, 
						Enabled = enabledAtStart, 
						Rectangle = rect, 
						SaveToConfig = saveToConfig,
						ConfigFile = ConfigFileName,
						ResizeMode = resize, 
						ClipState = clipState
					};
					
					MovablePlugins[owner].Add(area);
				}
				
				//QueueConfigSave(area);
				
				return area;
			}
			
			return null;
			//return MovableAreas.Count - 1;
		}

		public MovableArea CreateArea(IMovable owner, string areaName, RectangleF rect, bool enabledAtStart, bool saveToConfig, string configName, ResizeMode resize = ResizeMode.Off, ClipState clipState = ClipState.BeforeClip)
		{
			//Hud.Sound.Speak("two");
			if (MovablePlugins.ContainsKey(owner))
			{
				MovableArea area = MovablePlugins[owner].FirstOrDefault(ma => ma.Name == areaName);
				if (area is object)
				{
					area.Enabled = enabledAtStart;
					area.Rectangle = rect;
					area.SaveToConfig = saveToConfig;
					area.ConfigFile = configName;
					area.ResizeMode = resize;
					area.ClipState = clipState;
				}
				else
				{
					area = new MovableArea(areaName) 
					{ 
						Owner = owner, 
						Enabled = enabledAtStart, 
						Rectangle = rect, 
						SaveToConfig = saveToConfig, 
						ConfigFile = configName,
						ResizeMode = resize, 
						ClipState = clipState
					};
					
					MovablePlugins[owner].Add(area);
				}
				
				//QueueConfigSave(area);
				
				return area;
			}
			
			return null;
			//return MovableAreas.Count - 1;
		}
		
		public void DeleteArea(MovableArea area)
		{
			if (!DeletionQueue.Contains(area))
				DeletionQueue.Add(area);
			//area.DeleteOnDisable = true;
			//area.Enabled = false;
		}
		
		public string ConfigToString(string configName = null)
		{
			string fileName = string.IsNullOrEmpty(configName) ? ConfigFileName : configName;
			
			TextBuilder.Clear();
			TextBuilder.Append("/*\n\tThis file contains \"movable\" plugin positions and enabled/disabled state settings.\n\tChange the file extension from .txt to .cs and move this file into the TurboHUD / plugins / User folder\n*/\n\n");
			TextBuilder.Append("namespace Turbo.Plugins.User\n{\n");
			TextBuilder.Append("\tusing Turbo.Plugins.Default;\n");
			TextBuilder.Append("\tusing Turbo.Plugins.Razor.Movable;\n\n");
			//TextBuilder.AppendFormat("\tpublic class {0} : BasePlugin, ICustomizer\n\t{\n", ConfigFileName);
			TextBuilder.AppendFormat("\tpublic class {0} : BasePlugin, ICustomizer\n", fileName); //ConfigFileName
			TextBuilder.Append("\t{\n");
			TextBuilder.AppendFormat("\t\tpublic {0}() ", fileName); //ConfigFileName
			TextBuilder.Append("{ Enabled = true; }\n\n");
			TextBuilder.Append("\t\tpublic override void Load(IController hud) { base.Load(hud); }\n\n");
			TextBuilder.Append("\t\tpublic void Customize()\n\t\t{\n");
			//TextBuilder.AppendFormat("\t\t\tHud.RunOnPlugin<{0}>(plugin =>\n\t\t\t{\n", this.GetType().Name);
			TextBuilder.AppendFormat("\t\t\tHud.RunOnPlugin<{0}>(plugin =>\n", this.GetType().Name);
			TextBuilder.Append("\t\t\t{\n");
			//TextBuilder.Append(string.Format("\t\t\tHud.RunOnPlugin<{0}>(plugin =>\n\t\t\t{\n", this.GetType().Name)); //"\t\t\tHud.RunOnPlugin<MovableController>(plugin =>\n\t\t\t{\n"
			TextBuilder.Append("\t\t\t\t//Configure(string pluginName, string areaName, float x, float y, float width, float height, bool enabled = true, string configFileName = null, string areaSettings = null)\n");
			
			foreach (IMovable f in MovablePlugins.Keys)
			{
				string pluginName = f.GetType().Name;
				List<MovableArea> areas = MovablePlugins[f];
				//for (int i = 0; i < areas.Count; ++i)
				foreach (var area in areas)
				{
					//MovableArea area = areas[i];
					
					if (area.SaveToConfig && ((configName == null && string.IsNullOrEmpty(area.ConfigFile)) || area.ConfigFile == configName))
					{
						//TextBuilder.AppendFormat("\t\t\t\t//{0}\n", area.Name);
						if (string.IsNullOrEmpty(area.ConfigSettings))
							//TextBuilder.AppendFormat("\t\t\t\tplugin.Configure(\"{0}\", {1}, {2}f, {3}f, {4}f, {5}f, {6}, \"{7}\");\n", pluginName, i, (int)area.Rectangle.X, (int)area.Rectangle.Y, (int)area.Rectangle.Width, (int)area.Rectangle.Height, area.Enabled.ToString().ToLower(), area.Name);
							TextBuilder.AppendFormat("\t\t\t\tplugin.Configure(\"{0}\", \"{1}\", {2}f, {3}f, {4}f, {5}f, {6}, \"{7}\");\n", pluginName, area.Name, (int)area.Rectangle.X, (int)area.Rectangle.Y, (int)area.Rectangle.Width, (int)area.Rectangle.Height, area.Enabled.ToString().ToLower(), fileName);
						else
							//TextBuilder.AppendFormat("\t\t\t\tplugin.Configure(\"{0}\", {1}, {2}f, {3}f, {4}f, {5}f, {6}, \"{7}\", \"{8}\");\n", pluginName, i, (int)area.Rectangle.X, (int)area.Rectangle.Y, (int)area.Rectangle.Width, (int)area.Rectangle.Height, area.Enabled.ToString().ToLower(), area.Name, area.ConfigSettings);
							TextBuilder.AppendFormat("\t\t\t\tplugin.Configure(\"{0}\", \"{1}\", {2}f, {3}f, {4}f, {5}f, {6}, \"{7}\", \"{8}\");\n", pluginName, area.Name, (int)area.Rectangle.X, (int)area.Rectangle.Y, (int)area.Rectangle.Width, (int)area.Rectangle.Height, area.Enabled.ToString().ToLower(), fileName, area.ConfigSettings);
					}
				}
			}

			TextBuilder.Append("\t\t\t});\n\t\t}\n\t}\n}");
			
			return TextBuilder.ToString();
		}
		
		public void QueueConfigSave(MovableArea area = null)
		{
			if (area is object)
			{
				area.Changed = false;
				
				if (!area.SaveToConfig)
					return;

				string config = string.IsNullOrEmpty(area.ConfigFile) ? ConfigFileName : area.ConfigFile;
				if (!ConfigChanged.Contains(config))
				{
					//Hud.Sound.Speak("queued " + config);
					ConfigChanged.Add(config);
				}
			}
			else
			{
				//Hud.Sound.Speak("queued all");
				foreach (List<MovableArea> areas in MovablePlugins.Values)
				{
					foreach (MovableArea a in areas)
					{
						if (!string.IsNullOrEmpty(a.ConfigFile) && !ConfigChanged.Contains(a.ConfigFile))
							ConfigChanged.Add(a.ConfigFile);
					}
				}
				
				if (!ConfigChanged.Contains(ConfigFileName))
					ConfigChanged.Add(ConfigFileName);
			}
		}
		
		public void Log()
		{
			//Hud.Sound.Speak("Log");
			Queued = false;

			foreach (string configName in ConfigChanged)
			{
				//Hud.Sound.Speak("logged "+configName);
				string filename = (string.IsNullOrEmpty(configName) ? ConfigFileName : configName);
				string data = ConfigToString(configName);
				
				//Hud.TextLog.Log((string.IsNullOrEmpty(configName) ? ConfigFileName : configName), ConfigToString(configName), false, false);				
				//Hud.TextLog.Log(filename, data, false, false);
				
				//if (ManualWrite)
					File.WriteAllText(string.Format("{0}\\{1}.cs", ConfigFilePath, filename), data);
			}

			//ManualWrite = false;
			ConfigChanged.Clear();
		}
		
		/*public IPlugin GetPlugin(string pluginName)
		{
			return Hud.AllPlugins.FirstOrDefault(p => p.GetType().Name == pluginName);
		}*/
    }
}