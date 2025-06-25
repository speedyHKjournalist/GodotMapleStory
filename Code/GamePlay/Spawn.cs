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
}