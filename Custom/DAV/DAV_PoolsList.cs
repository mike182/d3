// version 2

using System;
using System.Linq;
using System.Collections.Generic;
using Turbo.Plugins.Default;

namespace Turbo.Plugins.DAV
{
	public class DAV_PoolsList : BasePlugin, IInGameTopPainter, IInGameWorldPainter, IAfterCollectHandler, IKeyEventHandler, INewAreaHandler {
		public bool resetWayPoint { get; set; }

		public SharpDX.DirectInput.Key Key { get; set; }
		public bool showAlways { get; set; }
		public bool showALLPool { get; set; }
		public float xPos { get; set; }
		public float yPos { get; set; }
		public float iconSize { get; set; }
		public string PoolSumHeader { get; set; }
		public string nearWayPointName { get; set; }
		public IFont Font_Pool_Summary { get; set; }
		public Func<int, int, int, string> PoolSumMessage { get; set; }

		public bool showMapName { get; set; }
		public float nullOffsetRatioX { get; set; }
		public float mapOffsetX { get; set; }
		public float mapOffsetY { get; set; }
		public IFont[] Font_Pool { get; set; } = new IFont[2];
		public Func<int, string>[] mapPoolMessage { get; set; } = new Func<int, string>[2];

		public bool skipPoolDecorator { get; set; }
		public float[] showInDistance { get; set; } = new float[2];
		public WorldDecoratorCollection[] PoolDecorator { get; set; } = new WorldDecoratorCollection[2];

		private bool showSummary { get; set; } = false;
		private int lastAct { get; set; } = 0;
		private uint lastWayPointSno { get; set; } = 0;
		private string summaryMessage { get; set; }
		private ITexture[] poolTexture { get; set; } = new ITexture[2];
		private int poolTotal { get; set; } = 0;
		private int[] poolFind { get; set; } = new int[] { 0, 0, 0, 0, 0, 0 };
		private int[] poolUsed { get; set; } = new int[] { 0, 0, 0, 0, 0, 0 };
		private List<DAV_WayPointSno> wayPointList { get; set; } = new List<DAV_WayPointSno>();
		private Dictionary<string, DAV_PoolInfo> PoolMarkers { get; set; } = new Dictionary<string, DAV_PoolInfo>();

		public DAV_PoolsList() {
			Enabled = true;
		}
		
		public override void Load(IController hud) {
			base.Load(hud);

			resetWayPoint = true; // reset waypoint data, WorldId vary in different game? (W.T.F.)

			Key = SharpDX.DirectInput.Key.E;
			xPos = 670; // Hud.Window.Size.Width * 0.4f;
			yPos = 10; // Hud.Window.Size.Height * 0.01f;
			iconSize = 80;
			showAlways = true;
			showALLPool = true; // summary show qty even if all pool used in that Act
			PoolSumHeader = "Pool Summary";
			nearWayPointName = "WayPoint"; // map name if in the waypoint map (cant use the game data as the marker may be "far far away")
			PoolSumMessage = (act, qtyExist, qtyTotal) => "A" + act.ToString() + " : " + qtyExist.ToString() + " Pool out of " + qtyTotal.ToString();
			Font_Pool_Summary = Hud.Render.CreateFont("Arial", 8f, 250, 255, 250, 0, false, false, false);

			// Array : 0 for Available Pool, 1 for Used Pool
			showMapName = true;
			nullOffsetRatioX = 0.25f;
			mapOffsetX = 6f;
			mapOffsetY = 0f;
			mapPoolMessage[0] = (qty) => "x " + qty.ToString();
			mapPoolMessage[1] = (qty) => "x " + qty.ToString();
			Font_Pool[0] = Hud.Render.CreateFont("Arial", 7f, 255, 255, 255, 51, true, false, 160, 51, 51, 51, true);
			Font_Pool[1] = Hud.Render.CreateFont("Arial", 7f, 255, 255, 51, 51, true, false, 160, 51, 51, 51, true);

			skipPoolDecorator = false; // show pool Decorator in Map or not
			showInDistance[0] = 5000f; // show Decorator if distance is less than this value, 0 notUsed 1 used
			showInDistance[1] = 150f;
			PoolDecorator[0] = new WorldDecoratorCollection(
				new MapShapeDecorator(Hud) {
					Brush = Hud.Render.CreateBrush(255, 255, 255, 51, -1),
					ShapePainter = new LineFromMeShapePainter(Hud)
				},
				new MapTextureDecorator(Hud) {
					Texture = Hud.Texture.GetTexture(376779, 0),
					Radius = 1f,
				},
				new MapShapeDecorator(Hud) {
					Brush = Hud.Render.CreateBrush(255, 255, 255, 51, -1),
					Radius = 15f,
					ShapePainter = new RectangleShapePainter(Hud),
				}
			);

			PoolDecorator[1] = new WorldDecoratorCollection(
				new MapShapeDecorator(Hud) {
					Brush = Hud.Render.CreateBrush(255, 255, 255, 51, -1),
					Radius = 15f,
					ShapePainter = new CircleShapePainter(Hud),
				}
			);

			poolTexture[0] = Hud.Texture.GetTexture(376779, 0);
			poolTexture[1] = Hud.Texture.GetTexture(384218, 0);
			if (!resetWayPoint) {
				wayPointList.Add(new DAV_WayPointSno(1, 91133, 1999831045, Hud.Window.CreateWorldCoordinate(1976.710f, 2788.146f, 41.2f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19780, 1999962119, Hud.Window.CreateWorldCoordinate(820.000f, 749.307f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19783, 2000027656, Hud.Window.CreateWorldCoordinate(820.000f, 701.500f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19785, 2000093193, Hud.Window.CreateWorldCoordinate(820.000f, 1229.307f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19787, 2000158730, Hud.Window.CreateWorldCoordinate(1059.357f, 578.648f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19954, 1999831045, Hud.Window.CreateWorldCoordinate(2895.395f, 2371.276f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 72712, 1999831045, Hud.Window.CreateWorldCoordinate(2161.802f, 1826.882f, 1.9f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19952, 1999831045, Hud.Window.CreateWorldCoordinate(2037.839f, 910.656f, 1.9f)));
				wayPointList.Add(new DAV_WayPointSno(1, 61573, 1999831045, Hud.Window.CreateWorldCoordinate(1263.054f, 827.765f, 63.1f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19953, 1999831045, Hud.Window.CreateWorldCoordinate(423.106f, 781.156f, 21.2f)));
				wayPointList.Add(new DAV_WayPointSno(1, 93632, 1999831045, Hud.Window.CreateWorldCoordinate(2310.500f, 4770.000f, 1.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 78572, 2000617489, Hud.Window.CreateWorldCoordinate(1339.771f, 1159.466f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19941, 1999831045, Hud.Window.CreateWorldCoordinate(1688.828f, 3890.089f, 41.9f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19943, 1999831045, Hud.Window.CreateWorldCoordinate(1080.022f, 3444.448f, 62.4f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19774, 2000814100, Hud.Window.CreateWorldCoordinate(820.000f, 736.500f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 119870, 2001207322, Hud.Window.CreateWorldCoordinate(56.772f, 146.284f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19775, 2000879637, Hud.Window.CreateWorldCoordinate(452.500f, 820.000f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(1, 19776, 2000945174, Hud.Window.CreateWorldCoordinate(979.000f, 1060.000f, 0.0f)));

				wayPointList.Add(new DAV_WayPointSno(2, 19836, 2001272859, Hud.Window.CreateWorldCoordinate(2760.302f, 1631.820f, 187.5f)));
				wayPointList.Add(new DAV_WayPointSno(2, 63666, 2001272859, Hud.Window.CreateWorldCoordinate(1419.449f, 321.863f, 176.3f)));
				wayPointList.Add(new DAV_WayPointSno(2, 19835, 2001272859, Hud.Window.CreateWorldCoordinate(1427.769f, 1185.675f, 186.2f)));
				wayPointList.Add(new DAV_WayPointSno(2, 460671, 2001403933, Hud.Window.CreateWorldCoordinate(1747.454f, 549.045f, 0.5f)));
				wayPointList.Add(new DAV_WayPointSno(2, 456638, 2001469470, Hud.Window.CreateWorldCoordinate(400.119f, 889.819f, 0.2f)));
				wayPointList.Add(new DAV_WayPointSno(2, 210451, 2001600544, Hud.Window.CreateWorldCoordinate(612.485f, 936.556f, -29.8f)));
				wayPointList.Add(new DAV_WayPointSno(2, 57425, 2001272859, Hud.Window.CreateWorldCoordinate(3548.838f, 4269.278f, 101.9f)));
				wayPointList.Add(new DAV_WayPointSno(2, 62752, 2001797155, Hud.Window.CreateWorldCoordinate(90.000f, 59.500f, 2.7f)));
				wayPointList.Add(new DAV_WayPointSno(2, 53834, 2001272859, Hud.Window.CreateWorldCoordinate(1257.886f, 3968.300f, 111.8f)));
				wayPointList.Add(new DAV_WayPointSno(2, 19800, 2002124840, Hud.Window.CreateWorldCoordinate(799.995f, 680.024f, -0.1f)));

				wayPointList.Add(new DAV_WayPointSno(3, 75436, 1999568897, Hud.Window.CreateWorldCoordinate(1065.148f, 472.449f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 93103, 2002321451, Hud.Window.CreateWorldCoordinate(825.148f, 1192.449f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 136448, 2002386988, Hud.Window.CreateWorldCoordinate(825.148f, 472.449f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 93173, 2002452525, Hud.Window.CreateWorldCoordinate(4272.158f, 4252.097f, -24.8f)));
				wayPointList.Add(new DAV_WayPointSno(3, 154644, 1999699971, Hud.Window.CreateWorldCoordinate(4356.238f, 408.241f, -2.9f)));
				wayPointList.Add(new DAV_WayPointSno(3, 155048, 1999699971, Hud.Window.CreateWorldCoordinate(3452.838f, 609.227f, 0.2f)));
				wayPointList.Add(new DAV_WayPointSno(3, 69504, 1999699971, Hud.Window.CreateWorldCoordinate(2708.557f, 586.267f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 86080, 2002583599, Hud.Window.CreateWorldCoordinate(1679.054f, 744.707f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 80791, 2002714673, Hud.Window.CreateWorldCoordinate(1041.245f, 983.039f, -10.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 119305, 2002845747, Hud.Window.CreateWorldCoordinate(2573.769f, 1206.666f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 119653, 2002976821, Hud.Window.CreateWorldCoordinate(959.404f, 1097.913f, -10.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 119306, 2003107895, Hud.Window.CreateWorldCoordinate(1162.255f, 686.218f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(3, 428494, 2003238969, Hud.Window.CreateWorldCoordinate(606.110f, 1065.332f, 0.0f)));

				wayPointList.Add(new DAV_WayPointSno(4, 109526, 2003435580, Hud.Window.CreateWorldCoordinate(1073.872f, 786.390f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(4, 464857, 2003501117, Hud.Window.CreateWorldCoordinate(1340.086f, 110.277f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(4, 409001, 2003566654, Hud.Window.CreateWorldCoordinate(1339.833f, 359.946f, -1.3f)));
				wayPointList.Add(new DAV_WayPointSno(4, 464065, 2003697728, Hud.Window.CreateWorldCoordinate(1030.028f, 870.214f, 15.0f)));
				wayPointList.Add(new DAV_WayPointSno(4, 409512, 2003632191, Hud.Window.CreateWorldCoordinate(2072.500f, 2747.500f, -30.0f)));
				wayPointList.Add(new DAV_WayPointSno(4, 109514, 2003763265, Hud.Window.CreateWorldCoordinate(873.785f, 1114.032f, -14.9f)));
				wayPointList.Add(new DAV_WayPointSno(4, 409517, 2003828802, Hud.Window.CreateWorldCoordinate(2520.398f, 1979.950f, 15.0f)));
				wayPointList.Add(new DAV_WayPointSno(4, 109538, 2003894339, Hud.Window.CreateWorldCoordinate(1079.837f, 856.173f, -1.3f)));
				wayPointList.Add(new DAV_WayPointSno(4, 109540, 2004090950, Hud.Window.CreateWorldCoordinate(345.398f, 359.990f, -1.3f)));
				wayPointList.Add(new DAV_WayPointSno(4, 464066, 2004222024, Hud.Window.CreateWorldCoordinate(2500.083f, 1390.092f, 31.0f)));
				wayPointList.Add(new DAV_WayPointSno(4, 475856, 2004222024, Hud.Window.CreateWorldCoordinate(348.659f, 341.491f, 50.8f)));
				wayPointList.Add(new DAV_WayPointSno(4, 475854, 2004287561, Hud.Window.CreateWorldCoordinate(1559.163f, 3636.186f, -23.3f)));
				wayPointList.Add(new DAV_WayPointSno(4, 464063, 2004287561, Hud.Window.CreateWorldCoordinate(350.023f, 350.276f, 0.0f)));

				wayPointList.Add(new DAV_WayPointSno(5, 263493, 2004353098, Hud.Window.CreateWorldCoordinate(1026.961f, 418.969f, 10.8f)));
				wayPointList.Add(new DAV_WayPointSno(5, 338946, 2004484172, Hud.Window.CreateWorldCoordinate(1260.250f, 540.750f, 3.3f)));
				wayPointList.Add(new DAV_WayPointSno(5, 261758, 2004549709, Hud.Window.CreateWorldCoordinate(402.102f, 419.735f, 10.3f)));
				wayPointList.Add(new DAV_WayPointSno(5, 283553, 2004680783, Hud.Window.CreateWorldCoordinate(608.812f, 417.468f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(5, 258142, 2004746320, Hud.Window.CreateWorldCoordinate(1140.714f, 540.347f, 12.4f)));
				wayPointList.Add(new DAV_WayPointSno(5, 283567, 2004877394, Hud.Window.CreateWorldCoordinate(877.314f, 399.194f, 0.0f)));
				wayPointList.Add(new DAV_WayPointSno(5, 338602, 2004942931, Hud.Window.CreateWorldCoordinate(1433.629f, 220.720f, 0.3f)));
				wayPointList.Add(new DAV_WayPointSno(5, 271234, 2005139542, Hud.Window.CreateWorldCoordinate(1069.580f, 679.856f, 20.2f)));
				wayPointList.Add(new DAV_WayPointSno(5, 459863, 2005205079, Hud.Window.CreateWorldCoordinate(1000.500f, 1160.500f, 0.5f)));
				wayPointList.Add(new DAV_WayPointSno(5, 427763, 2005270616, Hud.Window.CreateWorldCoordinate(661.430f, 423.897f, 2.9f)));
			}
		}

		public void OnNewArea(bool newGame, ISnoArea area) {
			if (newGame) {
				if (resetWayPoint)
					wayPointList.Clear();

				showSummary = false;
				PoolMarkers.Clear();
				lastWayPointSno = 0;
				poolTotal = 0;
				for (var i = 0; i < 6; i++) {
					poolFind[i] = 0;
					poolUsed[i] = 0;
				}
				return;
			}

			if (area.IsTown) {
				lastAct = 0;
				lastWayPointSno = 0;
			}
		}

		public void AfterCollect() {
			// Scene data update with a higher priority
			if (Hud.Game.Me.Scene == null || Hud.Game.Me.Scene.SnoArea == null) return;
			if (Hud.Game.Me.Scene.SnoArea.IsTown) return;

			var act = Hud.Game.Me.Scene.SnoArea.ActFixed();
			var newWayPoint = Hud.Game.Actors.FirstOrDefault(x => x.SnoActor.Sno == ActorSnoEnum._waypoint || x.SnoActor.Sno == ActorSnoEnum._a4_heaven_waypoint || x.SnoActor.Sno == ActorSnoEnum._waypoint_oldtristram);
			if (newWayPoint != null && newWayPoint.Scene != null) {
				lastAct = act; //newWayPoint.Scene.SnoArea.ActFixed();
				lastWayPointSno = newWayPoint.Scene.SnoArea.Sno;
				switch(lastWayPointSno) { // Adjust for SceneSnoArea, W.T.F. not match with the waypoint TargetSnoArea
					case 101351 : lastWayPointSno = 91133; break;
					case 19952 :
						if (newWayPoint.FloorCoordinate.XYDistanceTo(1263.054f, 827.765f) < 30)
							lastWayPointSno = 61573;
						break;
					case 19839 : lastWayPointSno = 19835; break;
					case 112565 : lastWayPointSno = 69504; break;
				}

				// save Way Point (works only if resetWayPoint = true)
				if (resetWayPoint && lastWayPointSno > 0 && !wayPointList.Any(x => x.AreaSno == lastWayPointSno)) {
					wayPointList.Add(new DAV_WayPointSno(act, lastWayPointSno, newWayPoint.WorldId, newWayPoint.FloorCoordinate));
					foreach (var key in PoolMarkers.Keys.ToList()) {
						var pool = PoolMarkers[key];
						if (pool.WorldId != newWayPoint.WorldId) continue;

						var revDist = newWayPoint.FloorCoordinate.XYDistanceTo(pool.FloorCoordinate);
						if (revDist < pool.distance) {
							pool.distance = revDist;
							pool.AreaName = nearWayPointName;
							pool.nearWayPointSno = lastWayPointSno;
						}
					}
				}
			}

			foreach (var marker in Hud.Game.Markers.Where(x => x.IsPoolOfReflection)) {
				if (PoolMarkers.ContainsKey(marker.Id)) {
					if (marker.IsUsed != PoolMarkers[marker.Id].IsUsed) {
						PoolMarkers[marker.Id].IsUsed = marker.IsUsed;
						poolUsed[act]++;
						poolTotal--;
						UpdateSummary();
					}
					continue;
				}

				var dist = float.MaxValue;
				var useAreaSno = act == lastAct ? lastWayPointSno : 0; // adjust if teleport to others in different act
				var mapName = Hud.Game.Me.Scene.SnoArea.NameLocalized;
				if (string.IsNullOrEmpty(mapName)) return;

				foreach(var waypoint in wayPointList.Where(x => x.WorldId == marker.WorldId)) {
					var thisDist = waypoint.FloorCoordinate.XYDistanceTo(marker.FloorCoordinate);
					if (thisDist < dist) {
						dist = thisDist;
						mapName = nearWayPointName;
						useAreaSno = waypoint.AreaSno;
					}
				}

				PoolMarkers.Add(marker.Id, new DAV_PoolInfo(marker, act, useAreaSno, mapName, dist));
				poolFind[act]++;
				poolTotal++;
				if (marker.IsUsed) {
					poolUsed[act]++;
					poolTotal--;
				}
				UpdateSummary();
			}
		}

		public void OnKeyEvent(IKeyEvent keyEvent) {
			if (keyEvent.IsPressed && keyEvent.Key == Key)
				showSummary = !showSummary;
		}

		public void PaintWorld(WorldLayer layer) {
			if (skipPoolDecorator) return;
			if (Hud.Game.IsInTown) return;
			if (Hud.Game.SpecialArea == SpecialArea.GreaterRift) return;

			foreach (var pool in Hud.Game.Markers.Where(x => x.IsPoolOfReflection)) {
				var i = pool.IsUsed ? 1 : 0;
				var range = Hud.Game.Me.FloorCoordinate.XYDistanceTo(pool.FloorCoordinate);

				if (range < showInDistance[i])
					PoolDecorator[i].Paint(layer, null, pool.FloorCoordinate, null);
			}
		}

		public void PaintTopInGame(ClipState clipState) {
			if (PoolMarkers.Count == 0) return;
			if (clipState != ClipState.AfterClip) return;
			if (Hud.Game.SpecialArea != SpecialArea.None) return;

			poolTexture[poolTotal > 0 ? 0 : 1].Draw(xPos - 0.76f * iconSize, yPos - 0.25f * iconSize, iconSize, iconSize);
			if ((showAlways || showSummary) && !string.IsNullOrEmpty(summaryMessage))
				Font_Pool_Summary.DrawText(summaryMessage, xPos, yPos);

			if (!Hud.Render.WorldMapUiElement.Visible || Hud.Render.ActMapUiElement.Visible) return;

			var mapCurrentAct = Hud.Game.ActMapCurrentAct;
			var rect = Hud.Render.WorldMapUiElement.Rectangle;
			var poolList = PoolMarkers.Values.ToList();
			foreach (var waypoint in Hud.Game.ActMapWaypoints.Where(z => z.BountyAct == mapCurrentAct && !z.TargetSnoArea.IsTown)) {
				var thisList = poolList.Where(z => z.nearWayPointSno == waypoint.TargetSnoArea.Sno);
				if (thisList.Count() == 0) continue;

				var xref = rect.X + Hud.Window.HeightUiRatio * (waypoint.CoordinateOnMapUiElement.X + 110);
				var yref = rect.Y + Hud.Window.HeightUiRatio * waypoint.CoordinateOnMapUiElement.Y + mapOffsetY;
				PaintPoolNumber(thisList, xref, yref, showMapName);
			}

			var currentAct = ((int)mapCurrentAct) / 100 + 1;
			var nullList = poolList.Where(z => z.Act == 0 || (z.Act == currentAct && z.nearWayPointSno == 0));
			if (nullList.Count() > 0)
				PaintPoolNumber(nullList, rect.X + rect.Width * nullOffsetRatioX, rect.Y + 110 * Hud.Window.HeightUiRatio);
		}

		private void PaintPoolNumber(IEnumerable<DAV_PoolInfo> poolList, float xref, float yref, bool showName = true) {
			int[] qty = { 0, 0 };
			string[] location = { " : ", " : " };
			foreach (var pool in poolList) {
				var i = pool.IsUsed ? 1 : 0;
				qty[i]++;
				if (showName && !location[i].Contains(pool.AreaName))
					location[i] += "<" + pool.AreaName + ">";
			}

			if (qty[0] > 0)
				Font_Pool[0].DrawText(mapPoolMessage[0](qty[0]) + (showName ? location[0] : ""), xref + mapOffsetX, yref);
			if (qty[1] > 0) {
				var layout = Font_Pool[1].GetTextLayout(mapPoolMessage[1](qty[1]) + (showName ? location[1] : ""));
				Font_Pool[1].DrawText(layout, xref - layout.Metrics.Width - mapOffsetX, yref);
			}
		}

		private void UpdateSummary() {
			summaryMessage = "";
			for (var i = 0; i < 6; i++) {
				var curPool = poolFind[i] - poolUsed[i];
				if (curPool > 0 || (showALLPool && poolFind[i] > 0))
					summaryMessage += PoolSumMessage(i, curPool, poolFind[i]) + "\n";
			}

			if (!string.IsNullOrEmpty(summaryMessage))
				summaryMessage = PoolSumHeader + "\n" + summaryMessage;
		}
	}

	public class DAV_WayPointSno {
		public int Act { get; set; }
		public uint AreaSno { get; set; }
		public uint WorldId { get; set; }
		public IWorldCoordinate FloorCoordinate { get; set; }

		public DAV_WayPointSno(int act, uint areaSno, uint worldId, IWorldCoordinate mapCoord) {
			Act = act;
			AreaSno = areaSno;
			WorldId = worldId;
			FloorCoordinate = mapCoord;
		}
	}

	public class DAV_PoolInfo {
		public int Act { get; set; }
		public bool IsUsed { get; set; }
		public uint WorldId { get; set; }
		public string uniqueID { get; set; }
		public IWorldCoordinate FloorCoordinate { get; set; }

		public string AreaName { get; set; }
		public uint nearWayPointSno { get; set; }
		public float distance { get; set; }

		public DAV_PoolInfo(IMarker pMarker, int act, uint areaSno, string name, float dist) {
			Act = act;
			IsUsed = pMarker.IsUsed;
			WorldId = pMarker.WorldId;
			uniqueID = pMarker.Id;
			FloorCoordinate = pMarker.FloorCoordinate;

			AreaName = name;
			distance = dist;
			nearWayPointSno = areaSno;
		}
	}
}