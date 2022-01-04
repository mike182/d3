/*

IMovable plugins that want to receive IKeyEvents when they are hovered or picked up in Modify Mode

*/

//using System.Drawing; //RectangleF
using System.Collections.Generic;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.Razor.Movable
{
	public interface IMovableKeyEventHandler
	{
		void OnKeyEvent(MovableController mover, IKeyEvent keyEvent, MovableArea area);
	}
}