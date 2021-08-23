/*

This is an event handler to consolidate left and right mouse click input checks across all plugins that implement the ILeftClickHandler and/or IRightClickHandler interface.

Changelog
- July 14, 2021 - ILeftBlockHandler now inherits IPlugin so that doesn't need to be cast to check the Enabled flag
- June 26, 2021 - ClickEventHandler doesn't create a new worker thread if no plugins subscribe to mouse events
- June 13, 2021 - rewrote ClickEventHandler using a global mouse hook library to add the ability to intercept left clicks
- April 30, 2021 - disabled the Simulate keys by default
- April 19, 2021 - fixed SimulateLeftMouseClick, SimulateRightMouseClick checks...IKeyEvent.Matches is limited to key down and cannot trigger key up events, so simulate has to be individual Key configurations
- April 7, 2021 - added mouse click simulation hotkey handling as an optional alternative to clicking
- November 23, 2020 - rewrite (attempt to eliminate desync again)
- July 28, 2020 - optimization: cached the mouse event listeners so that this plugin doesn't have to iterate through the entire list of hud plugins for every click notification
- July 25, 2020 - rewrote code in a more self correcting way
- July 14, 2020 - rewrote the code logic to fix an issue with click detection desync when alt-tabbing
- July 3, 2020 - Initial release

*/

using SharpDX.DirectInput; //Key
using System.Collections.Generic; //List
using System.Linq;
using System.Windows.Forms; //Keys

using Turbo.Plugins.Default;
using Turbo.Plugins.Razor.Hotkey;

namespace Turbo.Plugins.Razor.Click
{
    public class ClickEventHandler : BasePlugin, IAfterCollectHandler, IInGameTopPainter//, IHotkeyEventHandler
    {
		//optional hotkeys to simulate clicks, which may be less annoying because we don't trap clicks from going through to the d3 client
		//public IKeyEvent MousePress { get; set; } 
		//public Key SimulateLeftMouseClick { get; set; }
		//public Key SimulateRightMouseClick { get; set; }
		
		public bool? LButtonPressed { get; private set; } //tracks mouse clicks (non-simulated)
		public bool? RButtonPressed { get; private set; } //tracks mouse clicks (non-simulated)
		
		public GlobalHookThread Thread { get; private set; }
		public List<System.Func<bool>> BlockLeftClick { get; private set; } = new List<System.Func<bool>>();
		
		private IPlugin[] LeftMouseHandlers;
		private ILeftBlockHandler[] LeftBlockHandlers;
		private IPlugin[] RightMouseHandlers;
		
		private bool IsForeground; //tracks changes in this state
		private bool IsInGame; //tracks changes in this state
		private bool LeftClickDown;
		private bool LeftClickUp;
		private bool RightClickDown;
		private bool RightClickUp;
		
        public ClickEventHandler()// : base()
        {
            Enabled = true;
			Order = -10001; //run this before some other stuff
        }
		
        public override void Load(IController hud)
        {
            base.Load(hud);
			
			IsForeground = Hud.Window.IsForeground;
			IsInGame = false;
			
			//SimulateLeftMouseClick = Key.E; //Hud.Input.CreateKeyEvent(true, SharpDX.DirectInput.Key.E, false, false, false);
			//SimulateRightMouseClick = Key.R; //Hud.Input.CreateKeyEvent(true, SharpDX.DirectInput.Key.R, false, false, false);
			
			
        }
		
		public void PaintTopInGame(ClipState clipState)
		{
			if (Thread == null || clipState != ClipState.BeforeClip)
				return;
			
			if (LeftClickDown)
				LeftMouseDown();
			else if (LeftClickUp)
				LeftMouseUp();
			
			if (RightClickDown)
				RightMouseDown();
			else if (RightClickUp)
				RightMouseUp();
			
			LeftClickDown = false;
			LeftClickUp = false;
			RightClickDown = false;
			RightClickUp = false;
			
			
	 		/*bool IsInGame = true;
			
			if (LeftMouseHandlers is object)
			{
				bool pressed = Hud.Input.IsKeyDown(Keys.LButton);
				
				if (LButtonPressed.HasValue)
				{
					if (LButtonPressed.Value != pressed)
					{
						//update state
						LButtonPressed = pressed;

						//notify
						if (pressed)
							LeftMouseDown();
						else
							LeftMouseUp();
					}
				}
				else
					LButtonPressed = pressed;
			}
			
			if (RightMouseHandlers is object)
			{
				bool pressed = Hud.Input.IsKeyDown(Keys.RButton);
				
				if (RButtonPressed.HasValue)
				{
					if (RButtonPressed.Value != pressed)
					{
						//update state
						RButtonPressed = pressed;

						//notify
						if (pressed)
							RightMouseDown();
						else
							RightMouseUp();
					}
				}
				else
					RButtonPressed = pressed;
			}*/
		}
		
		public void AfterCollect()
		{
			if (LeftMouseHandlers == null)
			{
				//find all plugins that want to be notified
				LeftMouseHandlers = Hud.AllPlugins.Where(p => p is ILeftClickHandler).ToArray();
			}
			if (RightMouseHandlers == null)
			{
				//find all plugins that want to be notified
				RightMouseHandlers = Hud.AllPlugins.Where(p => p is IRightClickHandler).ToArray();
			}
			if (LeftBlockHandlers == null)
			{
				//find all plugins that want to block left clicks
				LeftBlockHandlers = Hud.AllPlugins.Where(p => p is ILeftBlockHandler).Cast<ILeftBlockHandler>().ToArray();
			}
			if (Thread == null && (LeftMouseHandlers.Length > 0 || LeftBlockHandlers.Length > 0 || RightMouseHandlers.Length > 0))
			{
				Thread = Hud.Input.GetThread();
				Thread.Invoke((MethodInvoker)(() => {
					Thread.GlobalHook.MouseDownExt += (sender, e) =>
					{
						//Console.Write(e.KeyChar);
						//if (e.KeyChar == 'q') quit();
						if (Hud.Window.IsForeground)
						{
							//Hud.Sound.Speak("click");
							//suppress the left mouse button click only if there is a click handler bound to the hovered label
							if (e.Button == System.Windows.Forms.MouseButtons.Left)
							{
								LeftClickDown = true;
								LeftClickUp = false;
								
								if (LeftBlockHandlers.Any(b => b.Enabled && b.LeftClickBlockCondition())) //BlockLeftClick.Any(f => f())
								{
									e.Handled = true;
									//Hud.Sound.Speak("supress");
								}
							}
							else if (e.Button == System.Windows.Forms.MouseButtons.Right)
							{
								RightClickDown = true;
								RightClickUp = false;
							}
						}
					};
					
					Thread.GlobalHook.MouseUpExt += (sender, e) => 
					{
						if (e.Button == System.Windows.Forms.MouseButtons.Left)
						{
							//LeftClickDown = false;
							LeftClickUp = true;
						}
						else if (e.Button == System.Windows.Forms.MouseButtons.Right)
						{
							//RightClickDown = false;
							RightClickUp = true;
						}
					};
				}));
				
				/*Thread = new GlobalHookThread((thread) => {//(hook) => {
					thread.GlobalHook.MouseDownExt += (sender, e) =>
					{
						//Console.Write(e.KeyChar);
						//if (e.KeyChar == 'q') quit();
						if (Hud.Window.IsForeground)
						{
							//Hud.Sound.Speak("click");
							//suppress the left mouse button click only if there is a click handler bound to the hovered label
							if (e.Button == System.Windows.Forms.MouseButtons.Left)
							{
								LeftClickDown = true;
								LeftClickUp = false;
								
								if (LeftBlockHandlers.Any(b => b.Enabled && b.LeftClickBlockCondition())) //BlockLeftClick.Any(f => f())
								{
									e.Handled = true;
									//Hud.Sound.Speak("supress");
								}
							}
							else if (e.Button == System.Windows.Forms.MouseButtons.Right)
							{
								RightClickDown = true;
								RightClickUp = false;
							}
						}
					};
					
					thread.GlobalHook.MouseUpExt += (sender, e) => 
					{
						if (e.Button == System.Windows.Forms.MouseButtons.Left)
						{
							//LeftClickDown = false;
							LeftClickUp = true;
						}
						else if (e.Button == System.Windows.Forms.MouseButtons.Right)
						{
							//RightClickDown = false;
							RightClickUp = true;
						}
					};
				});*/
			}
		}
		
		public void LeftMouseDown()
		{
			foreach (IPlugin plugin in LeftMouseHandlers)
			{
				if (plugin.Enabled)
					((ILeftClickHandler)plugin).OnLeftMouseDown();
			}
		}
		
		public void RightMouseDown()
		{
			foreach (IPlugin plugin in RightMouseHandlers)
			{
				if (plugin.Enabled)
					((IRightClickHandler)plugin).OnRightMouseDown();
			}
		}
		
		public void LeftMouseUp()
		{
			foreach (IPlugin plugin in LeftMouseHandlers)
			{
				if (plugin.Enabled)
					((ILeftClickHandler)plugin).OnLeftMouseUp();
			}
		}
		
		public void RightMouseUp()
		{
			foreach (IPlugin plugin in RightMouseHandlers)
			{
				if (plugin.Enabled)
					((IRightClickHandler)plugin).OnRightMouseUp();
			}
		}
		
		/*public delegate void Register()
		{
			Thread.GlobalHook.MouseDownExt += (sender, e) =>
			{
				//Console.Write(e.KeyChar);
				//if (e.KeyChar == 'q') quit();
				if (Hud.Window.IsForeground)
				{
					//Hud.Sound.Speak("click");
					//suppress the left mouse button click only if there is a click handler bound to the hovered label
					if (e.Button == System.Windows.Forms.MouseButtons.Left)
					{
						LeftClickDown = true;
						LeftClickUp = false;
						
						if (LeftBlockHandlers.Any(b => b.Enabled && b.LeftClickBlockCondition())) //BlockLeftClick.Any(f => f())
						{
							e.Handled = true;
							//Hud.Sound.Speak("supress");
						}
					}
					else if (e.Button == System.Windows.Forms.MouseButtons.Right)
					{
						RightClickDown = true;
						RightClickUp = false;
					}
				}
			};
			
			Thread.GlobalHook.MouseUpExt += (sender, e) => 
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					//LeftClickDown = false;
					LeftClickUp = true;
				}
				else if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					//RightClickDown = false;
					RightClickUp = true;
				}
			};
		}*/

		/*public void OnHotkeyEvent(IKeyEvent keyEvent)
		{
			//check for the defined hotkey
			if (SimulateLeftMouseClick is object && SimulateLeftMouseClick == keyEvent.Key) //SimulateLeftMouseClick.Matches(keyEvent))
			{
				//Hud.Sound.Speak(keyEvent.Key.ToString() + (keyEvent.IsPressed ? " down" : " up"));
				//notify
				if (keyEvent.IsPressed)
					LeftMouseDown();
				else
					LeftMouseUp();
			}
			else if (SimulateRightMouseClick is object && SimulateRightMouseClick == keyEvent.Key) //.Matches(keyEvent))
			{
				//notify
				if (keyEvent.IsPressed)
					RightMouseDown();
				else
					RightMouseUp();
			}
		}*/
    }
}