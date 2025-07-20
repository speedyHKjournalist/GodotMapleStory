using System.Collections.Generic;

namespace MapleStory
{
    public struct Attack
    {
        public enum Type
        {
            CLOSE,
            RANGED,
            MAGIC
        }

        public enum DamageType
        {
            DMG_WEAPON,
            DMG_MAGIC,
            DMG_FIXED
        }

        public Type attackType;
        public DamageType damageType;

        public double minDamage;
        public double maxDamage;
        public float critical;
        public float ignoreDef;
        public int matk;
        public int accuracy;
        public int fixDamage;
        public int playerLevel;

        public int hitCount;
        public int mobCount;
        public int speed;
        public int stance;
        public int skill;
        public int bullet;

        public MaplePoint<int> origin;
        public MapleRectangle<int> range;
        public float hRange;
        public bool toLeft;

        public Attack()
        {
            attackType = Type.CLOSE;
            damageType = DamageType.DMG_WEAPON;

            minDamage = 1.0d;
            maxDamage = 1.0d;
            critical = 0.0f;
            ignoreDef = 0.0f;
            matk = 0;
            accuracy = 0;
            fixDamage = 0;
            playerLevel = 1;

            hitCount = 0;
            mobCount = 0;
            speed = 0;
            stance = 0;
            skill = 0;
            bullet = 0;

            origin = new MaplePoint<int>();
            range = new MapleRectangle<int>();
            hRange = 1.0f;
            toLeft = false;
        }
    }

    public struct MobAttack
    {
        public Attack.Type attackType;
        public int watk;
        public int matk;
        public int mobId;
        public int objectId;
        public MaplePoint<int> origin;
        public bool valid;
        public MobAttack()
        {
            attackType = Attack.Type.CLOSE;
            watk = 0;
            matk = 0;
            mobId = 0;
            objectId = 0;
            origin = new MaplePoint<int>();
            valid = false;
        }
        public MobAttack(int watk, MaplePoint<int> origin, int mobId, int objectId)
        {
            attackType = Attack.Type.CLOSE;
            this.watk = watk;
            this.matk = 0;
            this.mobId = mobId;
            this.objectId = objectId;
            this.origin = origin;
            this.valid = true;
        }
        public static implicit operator bool(MobAttack attack) => attack.valid;
    }
    public struct MobAttackResult(MobAttack attack, int damage, int direction)
    {
        public int damage = damage;
        public int mobId = attack.mobId;
        public int objectId = attack.objectId;
        public int direction = direction;
    }
    public struct AttackResult
    {
        public Attack.Type attackType;
        public int attacker;
        public int mobCount;
        public int hitCount;
        public int skill;
        public int charge;
        public int bullet;
        public int level;
        public int display;
        public int stance;
        public int speed;
        public bool toLeft;
        public Dictionary<int, List<(int, bool)>> damageLines;
        public int firstObjectId;
        public int lastObjectId;
        public AttackResult()
        {
            attacker = 0;
            mobCount = 0;
            hitCount = 1;
            skill = 0;
            charge = 0;
            bullet = 0;
            level = 0;
            display = 0;
            stance = 0;
            speed = 0;
            toLeft = false;
            damageLines = [];
            firstObjectId = 0;
            lastObjectId = 0;
        }
        public AttackResult(Attack attack)
        {
            attackType = attack.attackType;
            hitCount = attack.hitCount;
            skill = attack.skill;
            speed = attack.speed;
            stance = attack.stance;
            bullet = attack.bullet;
            toLeft = attack.toLeft;

            damageLines = new Dictionary<int, List<(int, bool)>>();
        }
        public struct AttackUser
        {
            public int skillLevel;
            public int level;
            public bool secondWeapon;
            public bool flip;
        }
    }
}