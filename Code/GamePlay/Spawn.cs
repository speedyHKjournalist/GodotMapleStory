namespace MapleStory
{
    public class NpcSpawn
    {
        private int objectId;
        private int npcId;
        private MaplePoint<int> position;
        private bool flip;
        private int foothold;

        public NpcSpawn(int objectId, int npcId, MaplePoint<int> position, bool mirrored, int footHold)
        {
            this.objectId = objectId;
            this.npcId = npcId;
            this.position = position;
            this.flip = mirrored;
            this.foothold = footHold;
        }

        public int GetObjectId()
        {
            return objectId;
        }

        public MapObject Instantiate(Physics physics)
        {
            MaplePoint<int> spawnPosition = physics.GetYBelow(position);
            return new Npc(npcId, objectId, flip, foothold, false, spawnPosition);
        }
    }

    public partial class MobSpawn
    {
        private int objectId;
        private int mobId;
        private int mode;
        private int stance;
        private int foothold;
        private bool newSpawn;
        private int team;
        private MaplePoint<int> position;
        public MobSpawn(int objectId, int mobId, int mode, int stance, int foothold, bool newSpawn, int team, MaplePoint<int> position)
        {
            this.objectId = objectId;
            this.mobId = mobId;
            this.mode = mode;
            this.stance = stance;
            this.foothold = foothold;
            this.newSpawn = newSpawn;
            this.team = team;
            this.position = position;
        }
        public int GetMode()
        {
            return mode;
        }
        public int GetObjectId()
        {
            return objectId;
        }
        public MapObject Instantiate()
        {
            return new Mob(objectId, mobId, mode, stance, foothold, newSpawn, team, position);
        }
    }
}