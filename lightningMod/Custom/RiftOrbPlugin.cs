using System.Linq;
using Turbo.Plugins.Default;
using System.Collections.Generic;

namespace Turbo.Plugins.Custom
{
	public class RiftOrbPlugin : BasePlugin, IInGameWorldPainter
	{
		public WorldDecoratorCollection RiftOrbDecorator { get; set; }

		public RiftOrbPlugin()
		{
			Enabled = true;
		}

		public override void Load(IController hud)
		{
			base.Load(hud);
			
			RiftOrbDecorator = new WorldDecoratorCollection(
				new MapShapeDecorator(Hud)
				{/*
					Brush = Hud.Render.CreateBrush(255, 255, 0, 255, 0),
					ShadowBrush = Hud.Render.CreateBrush(96, 0, 0, 0, 1),
					Radius = 4.0f,
					ShapePainter = new CircleShapePainter(Hud),*/
				},
				new GroundCircleDecorator(Hud)
				{
					Brush = Hud.Render.CreateBrush(255, 255, 0, 255, 0f),
					Radius = 1f,
				}
			);
		}

		public void PaintWorld(WorldLayer layer)
		{
			var actors = Hud.Game.Actors.Where(x => x.SnoActor.Kind == ActorKind.RiftOrb);
			foreach (var actor in actors)
			{
				RiftOrbDecorator.ToggleDecorators<GroundLabelDecorator>(!actor.IsOnScreen); // do not display ground labels when the actor is on the screen
				RiftOrbDecorator.Paint(layer, actor, actor.FloorCoordinate, "rift globe");
			}
		}
	}
}
