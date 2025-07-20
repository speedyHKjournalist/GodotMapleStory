// Base class for attacks and buffs

namespace MapleStory
{
    public abstract class SpecialMove
    {
        public enum ForbidReason
        {
            None,
            WeaponType,
            HpCost,
            MpCost,
            BulletCost,
            Cooldown,
            Other
        }

        public abstract void ApplyUseEffects(Character user);
        public abstract void ApplyActions(Character user, Attack.Type type);
        public abstract void ApplyStats(Character user, Attack attack);
        public abstract void ApplyHitEffects(AttackResult.AttackUser user, Mob target);
        public abstract MapleAnimation GetBullet(Character user, int bulletId);

        public abstract bool IsAttack();
        public abstract bool IsSkill();
        public abstract int GetId();

        public abstract ForbidReason CanUse(int level, Weapon.Type weapon, Job job, int hp, int mp, int bullets);
    }
}