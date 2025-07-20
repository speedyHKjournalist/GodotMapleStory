using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class Stage : Node2D
    {
        private Player player = GD.Load<PackedScene>("res://Scene/Player.tscn").Instantiate<Player>();

        private enum State
        {
            INACTIVE,
            TRANSITION,
            ACTIVE
        }

        private MapBackgrounds? backgrounds;
        private MapTilesObjs? tilesObjs;
        private MapNpcs? npcs;
        private MapMobs? mobs;
        private MapInfo? mapInfo;
        private Physics? physics;
        private Camera? camera;

        private State state = State.ACTIVE;

        public override void _Ready()
        {
            camera = GetNode<Camera>("Camera");
            backgrounds = GetNode<MapBackgrounds>("Backgrounds");
            tilesObjs = GetNode<MapTilesObjs>("MapTilesObjs");
            physics = GetNode<Physics>("Physics");
            npcs = GetNode<MapNpcs>("MapNpcs_2");
            mobs = GetNode<MapMobs>("MapMobs_3");

            LoadMap(60000);

            //Test NPC Spawn
            npcs.Spawn(new NpcSpawn(1, 2100, new MaplePoint<int>(833, 125), true, 9));
            npcs.Spawn(new NpcSpawn(2, 2101, new MaplePoint<int>(130, 293), true, 51));

            // Test Mob Spawn
            mobs.Spawn(new MobSpawn(3, 5130101, 1, 0, 83, true, 0, new MaplePoint<int>(300, 100)));
            mobs.Spawn(new MobSpawn(4, 5130101, 1, 0, 83, true, 0, new MaplePoint<int>(300, 100)));

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

        public void LoadMap(int mapId)
        {
            string strId = mapId.ToString("D9");
            string prefix = (mapId / 100000000).ToString();
            Wz_Node mapImgNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Map", "Map", $"Map{prefix}", $"{strId}.img");

            backgrounds?.Init(mapImgNode.FindNodeByPath("back"));
            tilesObjs?.Init(mapImgNode);
            physics?.Init(mapImgNode.FindNodeByPath("foothold"));

            mapInfo = new MapInfo(mapId, physics!.GetFootholdTree()!.GetWalls(), physics!.GetFootholdTree()!.GetBorders());
            camera?.Init();
            camera?.SetView(mapInfo.GetWalls(), mapInfo.GetBorders());
        }

        public void CheckLadders(bool up)
        {
            if (!player.CanClimb() || player.IsClimbing() || player.IsAttacking())
                return;

            Ladder? ladder = mapInfo!.FindLadder(player.GetPosition(), up);
            player.SetLadder(ladder);
        }

        public void SendKey(KeyType.Id type, KeyAction.Id action, bool down)
        {
            if (state != State.ACTIVE/* || !playable*/)
                return;

            switch (type)
            {
                case KeyType.Id.ACTION:
                    player.SendAction(action, down);
                    break;
/*                case KeyType::Id::SKILL:
                    combat.use_move(action);
                    break;
                case KeyType::Id::ITEM:
                    player.use_item(action);
                    break;
                case KeyType::Id::FACE:
                    player.set_expression(action);
                    break;*/
                default:
                    break;
            }
        }

        public override void _Process(double delta)
        {
        }

        public override void _PhysicsProcess(double delta)
        {
            if (state != State.ACTIVE)
                return;

            camera?.Update(player.GetPosition());

            if (!player.IsClimbing() && !player.IsSitting() && !player.IsAttacking())
            {
                if (player.IsKeyDown(KeyAction.Id.UP) && !player.IsKeyDown(KeyAction.Id.DOWN))
                    CheckLadders(true);

/*                if (player.IsKeyDown(KeyAction::Id::UP))
                    check_portals();*/

                if (player.IsKeyDown(KeyAction.Id.DOWN))
                    CheckLadders(false);

/*                if (player.IsKeyDown(KeyAction::Id::SIT))
                    check_seats();

                if (player.IsKeyDown(KeyAction::Id::ATTACK))
                    combat.use_move(0);

                if (player.IsKeyDown(KeyAction::Id::PICKUP))
                    check_drops();*/
            }

            if (player.IsInvincible())
                return;

            int objectId = mobs!.FindColliding(player.GetPlayerRect());
            if (objectId != 0)
            {
                MobAttack attack = mobs!.CreateAttack(objectId);
                if (attack.valid)
                {
                    MobAttackResult result = player.Damage(attack);
/*                    TakeDamagePacket(result, TakeDamagePacket::From::TOUCH).dispatch();*/
                }
            }
        }
    }
}
