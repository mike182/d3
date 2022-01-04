using Turbo.Plugins.Default;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Turbo.Plugins.Custom
{
    public class HallOfAgonyShortcutsHints : BasePlugin, IAfterCollectHandler, IInGameWorldPainter, ICustomizer
    {
        public IBrush GreenBrush { get; set; }
        public IBrush RedBrush { get; set; }
        private IBrush Brush { get; set; }
        public List<IPlugin> ListOverlapPlugin { get; set; }
        public WorldDecoratorCollection IronMaidenDecorator { get; set; }
        private IWorldCoordinate EllipseCloseToMeHint, EllipseJumpHint;
        private float RadiusEllipseJumpHint = 3.0f;
        private bool IsInShortcutArea = false;
        private bool ShowEllipseHint = false;
        private bool ShowEllipseJumpHint = false;

        public HallOfAgonyShortcutsHints()
        {
            Enabled = true;
        }

        public override void Load(IController hud)
        {
            base.Load(hud);
            GreenBrush = Brush = Hud.Render.CreateBrush(240, 0, 255, 0, 4);
            RedBrush = Hud.Render.CreateBrush(240, 255, 0, 0, 4);
            ListOverlapPlugin = new List<IPlugin>();

            IronMaidenDecorator = new WorldDecoratorCollection(
                new MapShapeDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 255, 100, 0, 1.5f),
                    Radius = 6.0f,
                    ShapePainter = new CircleShapePainter(Hud),
                },
                new GroundCircleDecorator(Hud)
                {
                    Brush = Hud.Render.CreateBrush(255, 255, 100, 0, 2, SharpDX.Direct2D1.DashStyle.Dash),
                    Radius = 3,
                }
                );
        }

        public void Customize()
        {
            // disabled the plugin when we in the corner - avoiding overlap
            // ListOverlapPlugin.Add(Hud.GetPlugin<Default.ConventionOfElementsBuffListPlugin>());
        }

        public void AfterCollect()
        {
            if (!Hud.Game.IsInGame)
                return;
            SetShortcutsCoord();
        }

        public void PaintWorld(WorldLayer layer)
        {
            if (!Hud.Game.IsInGame || !IsInShortcutArea)
                return;

            DrawHints();

            foreach (var actor in Hud.Game.Actors.Where(a => a.SnoActor.Sno == ActorSnoEnum._a1dun_leor_iron_maiden)) // _a1dun_leor_iron_maiden = 97023
                IronMaidenDecorator.Paint(layer, actor, actor.FloorCoordinate, null);
        }

        void DrawHints()
        {
            TogglePlugin(ShowEllipseJumpHint ? false : true); // disabled some plugins when we in the corner - avoiding overlap
            if (ShowEllipseJumpHint)
            {
                var radius = TransformRadius(RadiusEllipseJumpHint, 400, 0.8f, 1f);
                Brush.DrawWorldEllipse(radius, -1, EllipseJumpHint);
                Brush.DrawWorldEllipse(radius / 4f, -1, EllipseJumpHint);
                Hud.Render.GetMinimapCoordinates(EllipseJumpHint.X, EllipseJumpHint.Y, out var mapX, out var mapY);
                radius = TransformRadius(6, 400, 0.8f, 1f);
                Brush.DrawEllipse(mapX, mapY, radius, radius, -2.5f);
                Brush.DrawEllipse(mapX, mapY, 2, 2, -2.5f);
            }
            else if (ShowEllipseHint)
            {
                var radius = TransformRadius(1, 400, 0.8f, 1f);
                Brush.DrawWorldEllipse(radius, -1, EllipseCloseToMeHint);
                Hud.Render.GetMinimapCoordinates(EllipseCloseToMeHint.X, EllipseCloseToMeHint.Y, out var mapX, out var mapY);
                radius = TransformRadius(6, 400, 0.8f, 1f);
                Brush.DrawEllipse(mapX, mapY, radius, radius, -1.5f);
            }
        }

        private void SetShortcutsCoord()
        {
            IsInShortcutArea = ShowEllipseJumpHint = ShowEllipseHint = false;
            Brush = GreenBrush;
            if (Hud.Game.Me.Scene?.SnoScene?.Code == "a1dun_leor_nw_01")
            {
                var doors = Hud.Game.Actors.Any(a => a.SnoActor.Sno == ActorSnoEnum._a1dun_leor_jail_door_breakable_a);
                Brush = doors ? RedBrush : GreenBrush;

                IsInShortcutArea = true;
                RadiusEllipseJumpHint = 3.0f;
                var leftJumpHint = SetWorldCoordinate(90.0f, 50.0f);
                var rightJumpHint = SetWorldCoordinate(50.0f, 90.0f);
                var leftHint = SetWorldCoordinate(83.0f, 53.0f);
                var rightHint = SetWorldCoordinate(53.0f, 83.0f);
                // var ironMaidenIsInLeftCorner = Hud.Game.Actors.Any(a => a.SnoActor.Sno == ActorSnoEnum._a1dun_leor_iron_maiden && a.FloorCoordinate.Equals(SetWorldCoordinate(92.5f, 40.0f, 0f))); // not used for now
                var ironMaidenIsInRightCorner = Hud.Game.Actors.Any(a => a.SnoActor.Sno == ActorSnoEnum._a1dun_leor_iron_maiden && a.FloorCoordinate.Equals(SetWorldCoordinate(51.0f, 85.0f, 0f)));
                if (ironMaidenIsInRightCorner)
                {
                    RadiusEllipseJumpHint = 1.2f;
                    rightHint = SetWorldCoordinate(53.0f, 90.0f);
                    leftJumpHint = SetWorldCoordinate(86.0f, 52.2f);
                    rightJumpHint = SetWorldCoordinate(51.8f, 92.1f);
                }

                var chestIsInLeftCorner = Hud.Game.Actors.Any(a => a.SnoActor.Sno == ActorSnoEnum._a1dun_leor_chest && a.FloorCoordinate.Equals(SetWorldCoordinate(84.5f, 51.0f, 0f))); // Left  _a1dun_leor_chest = 94708
                if (chestIsInLeftCorner)
                {
                    RadiusEllipseJumpHint = 1.2f;
                    leftHint = SetWorldCoordinate(87.0f, 54.0f);
                    leftJumpHint = SetWorldCoordinate(91.6f, 50.8f);
                    rightJumpHint = SetWorldCoordinate(51.6f, 87.0f);
                }

                var leftHintDistToMe = YardsDistToMe(leftHint);
                var rightHintDistToMe = YardsDistToMe(rightHint);
                EllipseCloseToMeHint = leftHintDistToMe > rightHintDistToMe ? rightHint : leftHint;
                var closeToMeHintDist = YardsDistToMe(EllipseCloseToMeHint);
                var yards = 2.0f;
                EllipseJumpHint = leftHintDistToMe < yards ? rightJumpHint : rightHintDistToMe < yards ? leftJumpHint : null;
                if (closeToMeHintDist < yards)
                    ShowEllipseJumpHint = true;
                else if (closeToMeHintDist < 105f /*yards*/)
                    ShowEllipseHint = true;
            }
        }

        // \plugins\Default\RadiusTransformators\StandardPingRadiusTransformator.cs
        float TransformRadius(float radius, int PingSpeed, float RadiusMinimumMultiplier = 0.5f, float RadiusMaximumMultiplier = 1.0f)
        {
            if (PingSpeed <= 0)
                return radius;

            var msec = Hud.Game.CurrentRealTimeMilliseconds;
            return Math.Floor((double)msec / PingSpeed) % 2 == 1
                ? radius * (RadiusMinimumMultiplier + ((RadiusMaximumMultiplier - RadiusMinimumMultiplier) * (msec % PingSpeed) / PingSpeed))
                : radius * (RadiusMaximumMultiplier - ((RadiusMaximumMultiplier - RadiusMinimumMultiplier) * (msec % PingSpeed) / PingSpeed));
        }

        void TogglePlugin(bool togglePlugin)
        {
            foreach (var p in ListOverlapPlugin)
                if (p.Enabled != togglePlugin)
                    p.Enabled = togglePlugin;
        }

        IWorldCoordinate SetWorldCoordinate(float relativeX, float relativeY, float z = 0.1f) { return Hud.Window.CreateWorldCoordinate(Hud.Game.Me.Scene.PosX + relativeX, Hud.Game.Me.Scene.PosY + relativeY, z); }

        float YardsDistToMe(IWorldCoordinate wc) { return Hud.Game.Me.FloorCoordinate.XYZDistanceTo(wc); }
    }
}
