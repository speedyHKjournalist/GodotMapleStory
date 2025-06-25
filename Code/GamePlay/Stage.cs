using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class Stage : Node2D
    {
        private Player player = GD.Load<PackedScene>("res://Scene/Player.tscn").Instantiate<Player>();

        [Signal]
        public delegate void CameraPositionChangedEventHandler(int viewPositionX, int viewPositionY);
        [Signal]
        public delegate void CameraRealPositionChangedEventHandler(double viewRealPositionX, double viewRealPositionY);

        private enum State
        {
            INACTIVE,
            TRANSITION,
            ACTIVE
        }

        private MapBackgrounds? backgrounds;
        private MapTilesObjs? tilesObjs;
        private MapNpcs? npcs;
        private Physics? physics;

        private MaplePoint<int> lastCameraPosition;
        private MaplePoint<double> lastCameraRealPosition;

        private State state;

        public override void _Ready()
        {
            backgrounds = GetNode<MapBackgrounds>("Backgrounds");
            tilesObjs = GetNode<MapTilesObjs>("MapTilesObjs");
            physics = GetNode<Physics>("Physics");
            npcs = GetNode<MapNpcs>("MapNpcs_2");

            LoadMap(10000);

            //Test
            npcs.Spawn(new NpcSpawn(1, 2100, new MaplePoint<int>(833, 125), true, 9));
            npcs.Spawn(new NpcSpawn(2, 2101, new MaplePoint<int>(130, 293), true, 51));

            CharEntry entry = new ();
            entry.look.skin = 0;
            entry.look.faceId = 20000;
            entry.look.hairId = 30000;
            entry.look.equips = new Dictionary<int, int>();
            entry.look.equips[(int)EquipSlot.Id.HAT] = 1000000;
            entry.look.equips[(int)EquipSlot.Id.WEAPON] = 1372000;

            entry.stats.name = "test";
            entry.stats.stats = new EnumMap<MapleStat.Id, int>();
            entry.stats.stats[MapleStat.Id.JOB] = 0;
            entry.stats.stats[MapleStat.Id.SKIN] = 0;
            entry.stats.stats[MapleStat.Id.FACE] = 0;
            entry.stats.stats[MapleStat.Id.HAIR] = 0;
            entry.stats.stats[MapleStat.Id.LEVEL] = 50;
            entry.stats.stats[MapleStat.Id.JOB] = 0;
            entry.stats.stats[MapleStat.Id.STR] = 666;
            entry.stats.stats[MapleStat.Id.DEX] = 666;
            entry.stats.stats[MapleStat.Id.INT] = 666;
            entry.stats.stats[MapleStat.Id.LUK] = 666;
            entry.stats.stats[MapleStat.Id.HP] = 13456;
            entry.stats.stats[MapleStat.Id.MAXHP] = 23456;
            entry.stats.stats[MapleStat.Id.MP] = 17890;
            entry.stats.stats[MapleStat.Id.MAXMP] = 19875;
            entry.stats.stats[MapleStat.Id.AP] = 0;
            entry.stats.stats[MapleStat.Id.SP] = 0;
            entry.stats.stats[MapleStat.Id.EXP] = 123456;
            entry.stats.stats[MapleStat.Id.FAME] = 1234;
            entry.stats.stats[MapleStat.Id.MESO] = 0;
            entry.stats.stats[MapleStat.Id.PET] = 0;
            entry.stats.stats[MapleStat.Id.GACHAEXP] = 0;
            entry.stats.exp = 123456;

            AddChild(player);
            player.Init(entry);
        }

        public void Interpolate(double alpha)
        {
            MaplePoint<int> viewPosition = new(100, 100);
            MaplePoint<double> viewRealPosition = new(100d, 100d);
            /*camera.RealPosition(alpha);*/

            if (viewPosition != lastCameraPosition)
            {
                EmitSignal(SignalName.CameraPositionChanged, viewPosition.X, viewPosition.Y);
                lastCameraPosition = viewPosition;
            }

            if (viewRealPosition != lastCameraRealPosition)
            {
                EmitSignal(SignalName.CameraRealPositionChanged, viewRealPosition.X, viewRealPosition.Y);
                lastCameraRealPosition = viewRealPosition;
            }
        }

        public void LoadMap(int mapId)
        {
            string strId = mapId.ToString("D9");
            string prefix = (mapId / 100000000).ToString();
            Wz_Node mapImgNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Map", "Map", $"Map{prefix}", $"{strId}.img");

            backgrounds?.Init(mapImgNode.FindNodeByPath("back"));
            tilesObjs?.Init(mapImgNode);
            physics?.Init(mapImgNode.FindNodeByPath("foothold"));
        }

        public override void _Process(double delta)
        {
            double alpha = Engine.GetPhysicsInterpolationFraction();
            Interpolate(alpha);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (state != State.ACTIVE)
            {
                return;
            }
        }
    }
}
