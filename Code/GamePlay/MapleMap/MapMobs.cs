using Godot;
using System;
using System.Collections.Generic;

namespace MapleStory
{
    public partial class MapMobs : Node2D
    {
        private MapObjects? mobs;
        private Queue<MobSpawn> spawns = new();

        public override void _Ready()
        {
            mobs = GetNode<MapObjects>("mobs");
        }

        public void Spawn(MobSpawn spawn)
        {
            spawns.Enqueue(spawn);
        }

        public void Clear()
        {
            mobs?.Clear();
        }

        public void SetControl(int objectId, bool control)
        {
            int mode = control ? 1 : 0;
            Mob? mob = (Mob?)mobs?.Get(objectId);

            mob?.SetControl(mode);
        }

        public void SendMobHp(int objectId, int percent, ushort playerLevel)
        {
            Mob? mob = (Mob?)mobs?.Get(objectId);

            mob?.ShowHp(percent, playerLevel);
        }

        public void SendMovement(int objectId, MaplePoint<int> start, List<Movement> movements)
        {
            Mob? mob = (Mob?)mobs?.Get(objectId);
            mob?.SendMovement(start, movements);
        }

        public void SendAttack(AttackResult result, Attack attack, List<int> targets, int mobCount)
        {
            foreach (int target in targets)
            {
                Mob? mob = (Mob?)mobs?.Get(target);

                if (mob != null)
                {
                    result.damageLines[target] = mob.CalculateDamage(attack);
                    result.mobCount++;

                    if (result.mobCount == 1)
                        result.firstObjectId = target;

                    if (result.mobCount == mobCount)
                        result.lastObjectId = target;
                }
            }
        }

        public void ApplyDamage(int objectId, int damage, bool toLeft, AttackResult.AttackUser user, SpecialMove move)
        {
            Mob? mob = (Mob?)mobs?.Get(objectId);

            if (mob != null)
            {
                mob.ApplyDamage(damage, toLeft);
                move.ApplyHitEffects(user, mob);
            }
        }

        public bool Contains(int objectId)
        {
            return mobs!.Contains(objectId);
        }

        public MobAttack CreateAttack(int objectId)
        {
            Mob? mob = (Mob?)mobs?.Get(objectId);

            if (mob != null)
                return mob.CreateTouchAttack();
            
            return new();
        }

        public MaplePoint<int> GetMobPosition(int objectId)
        {
            Mob? mob = (Mob?)mobs?.Get(objectId);

            if (mob != null)
                return mob.GetPosition();
            else
                return new MaplePoint<int>();
        }

        public MaplePoint<int> GetMobHeadPosition(int objectId)
        {
            Mob? mob = (Mob?)mobs?.Get(objectId);

            if (mob != null)
                return mob.GetHeadPosition();
            else
                return new MaplePoint<int>();
        }

        public MapObjects? GetMobs()
        {
            return mobs;
        }

        public int FindColliding(MapleRectangle<int> playerRect)
        {
            foreach (var mobPair in mobs!.GetObjects())
            {
                Mob mob = (Mob)mobPair.Value;
                if (mob.IsAlive() && mob.IsInRange(playerRect))
                    return mob.GetObjectId();
            }

            return 0;
        }

        public override void _PhysicsProcess(double delta)
        {
            while (spawns.Count > 0)
            {
                MobSpawn spawn = spawns.Dequeue();
                Mob? mob = (Mob?)mobs?.Get(spawn.GetObjectId());

                if (mob != null)
                {
                    int mode = spawn.GetMode();
                    if (mode > 0)
                        mob.SetControl(mode);

                    mob.MakeActive();
                }
                else
                    mobs?.Add(spawn.Instantiate());
            }
        }
    }
}