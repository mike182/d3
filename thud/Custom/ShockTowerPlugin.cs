using System.Linq;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.Custom
{
    public class ShockTowerPlugin : BasePlugin, IInGameWorldPainter
	{
        public WorldDecoratorCollection ShockTowerDecorator { get; set; }
        public ShockTowerPlugin()
		{
            Enabled = true;
		}
        public override void Load(IController hud)
        {
            base.Load(hud);          
            ShockTowerDecorator = new WorldDecoratorCollection(
                new MapShapeDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 255, 255, 220, 0),
                    Radius = 6.0f,
                    ShapePainter = new CircleShapePainter(Hud),
                    RadiusTransformator = new StandardPingRadiusTransformator(Hud, 333),
                },
				new MapLabelDecorator(Hud)
                {
                    LabelFont = Hud.Render.CreateFont("tahoma", 6, 255, 0, 0, 255, true, false, false),
                },
                new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 255, 255, 220, 5, SharpDX.Direct2D1.DashStyle.Dash),
                    Radius = 30,
                },
                new GroundLabelDecorator(Hud)
                {
                    BackgroundBrush = Hud.Render.CreateBrush(160, 255, 255, 220, 0),
                    TextFont = Hud.Render.CreateFont("tahoma", 9, 255, 0, 0, 255, true, false, false),                    
                }
                );
        }

		public void PaintWorld(WorldLayer layer)
		{
            var shocktower = Hud.Game.Actors.Where(x => (uint)x.SnoActor.Sno == 322194);
            foreach (var actor in shocktower)
            {
                ShockTowerDecorator.Paint(layer, actor, actor.FloorCoordinate, "!!! " + actor.SnoActor.NameLocalized + " !!!");
            }
        }
    }
}