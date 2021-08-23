using System;
using System.Linq;
using System.Collections.Generic;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.Custom
{
    public class MinimapCursorPlugin : BasePlugin, IInGameWorldPainter
	{
        public WorldDecoratorCollection MiniMapVisorDecorator { get; set; }
        public bool ShowInTown { get; set; }
		
		public MinimapCursorPlugin()
		{
            Enabled = true;
            ShowInTown = false;
		}

        public override void Load(IController hud)
        {
            base.Load(hud);
            
            MiniMapVisorDecorator = new WorldDecoratorCollection( 
			new MapShapeDecorator(Hud)
            {
                Brush = Hud.Render.CreateBrush(255, 255, 255, 255, 1f),
                ShapePainter = new PlusShapePainter(Hud),
                Radius = 4,
            },
			
			new MapShapeDecorator(Hud)
            {
                Brush = Hud.Render.CreateBrush(255, 255, 255, 255, 0f),
                ShapePainter = new CircleShapePainter(Hud),
                Radius = 2,
            }
			);
        }

        public void PaintWorld(WorldLayer layer)
        {
            if (!ShowInTown && Hud.Game.IsInTown) return;
            
			var cursorScreenCoord = Hud.Window.CreateScreenCoordinate(Hud.Window.CursorX, Hud.Window.CursorY);
			var visorWorldCoord = cursorScreenCoord.ToWorldCoordinate();
			
			MiniMapVisorDecorator.Paint(layer, null, visorWorldCoord, null);						
        }
    }
}