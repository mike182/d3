using System.Drawing;
using System.Collections.Generic;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.Razor.Movable
{
	public class MovableArea
	{
		public IMovable Owner { get; set; }

		public string Name
		{		
			get
			{
				return _name;
			}
			set
			{
				Changed = true;
				_name = value;
			}
		}
		private string _name;

		public RectangleF Rectangle
		{ 
			get
			{
				return _rectangle;
				/*if (RectangleHistory.Count < 1)
					return default(RectangleF);
				else
					return RectangleHistory[RectangleHistory.Count - 1];*/
			}
			set
			{
				if (!_rectangle.Equals(value))
				{
					Changed = true;
					RectangleHistory.Add(value);
					_rectangle = value;
				}
			}				
		}
		private RectangleF _rectangle = default(RectangleF);
		
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (_enabled != value)
				{
					Changed = true;
					_enabled = value;
				}
			}
		}
		private bool _enabled = true;
		
		public bool SaveToConfig { get; set; } = true;
		public string ConfigFile
		{ 
			get
			{
				return _filename;
			}
			set
			{
				if (_filename != value)
				{
					Changed = true;
					_filename = value;
				}
			}
		} //optional, leave empty to save to MovablePluginConfig
		private string _filename;
		
		public string ConfigSettings
		{ 
			get
			{
				return _settings;
			}
			set
			{
				if (_settings != value)
				{
					//System.Console.Beep(150, 50);
					Changed = true;
					_settings = value;
				}
			}
		} //optional, to be saved in config file
		private string _settings;
		
		public bool DeleteOnDisable { get; set; } = false;
		public ResizeMode ResizeMode { get; set; } = ResizeMode.Off;
		public ClipState ClipState { get; set; } = ClipState.BeforeClip;

		public bool Changed { get; set; } = false; //handled by get/set and MovableController to flag changes that should affect config file settings
		
		//private RectangleF OldRectangle;
		private List<RectangleF> RectangleHistory = new List<RectangleF>();
		
		public MovableArea(string s) {
			Name = s;
		}
		
		/*public bool IsHovered(IController Hud)
		{
			return Hud.Window.CursorInsideRect(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
		}*/
		
		public void Move(float deltaX, float deltaY)
		{
			if (deltaX != 0 || deltaY != 0)
				Rectangle = new RectangleF(Rectangle.X + deltaX, Rectangle.Y + deltaY, Rectangle.Width, Rectangle.Height);
				//SetRectangle(Rectangle.X + deltaX, Rectangle.Y + deltaY, Rectangle.Width, Rectangle.Height);
		}
		
		public void SetConfig(float x, float y, float w, float h, bool enabled = true, string configFile = null, string settings = null)
		{
			if (_rectangle.X != x || _rectangle.Y != y || _rectangle.Width != w || _rectangle.Height != h)
				Rectangle = new RectangleF(x, y, w, h); //SetRectangle(x, y, w, h);
			
			Enabled = enabled;
			ConfigSettings = settings;

			if (!string.IsNullOrEmpty(configFile))
				ConfigFile = configFile;
		}
		
		//avoid adding this to the history (doesn't trigger change flag)
		public void SetRectangle(float x, float y, float w, float h)
		{
			if (_rectangle.Width != w || _rectangle.Height != h || _rectangle.X != x || _rectangle.Y != y)
			{
				if (RectangleHistory.Count > 0)
					RectangleHistory.RemoveAt(RectangleHistory.Count - 1);
				
				_rectangle = new RectangleF(x, y, w, h);
				RectangleHistory.Add(_rectangle);
			}
		}
		
		public void Undo()
		{
			//Rectangle = OldRectangle;
			if (RectangleHistory.Count > 1)
			{
				RectangleHistory.RemoveAt(RectangleHistory.Count - 1);
				_rectangle = RectangleHistory[RectangleHistory.Count - 1];
				Changed = true;
			}
			//Rectangle = RectangleHistory[RectangleHistory.Count - 1];			
		}
		
		public void Reset()
		{
			if (RectangleHistory.Count > 1)
			{
				RectangleHistory.RemoveRange(1, RectangleHistory.Count - 1);
				_rectangle = RectangleHistory[0];
				Changed = true;
			}
		}
	}
}