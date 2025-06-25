using Godot;

namespace MapleStory
{
    public partial class MapNpcs : Node2D
    {
        private Physics? physics;
        private MapObjects? npcs;

        public override void _Ready()
        {
            Name = "MapNpcs_2";
            physics = GetNode<Physics>("/root/Root/ViewportContainer/SubViewport/Stage/Physics");
            npcs = GetNode<MapObjects>("Npcs");
        }

        public void Spawn(NpcSpawn spawn)
        {
            int oid = spawn.GetObjectId();
            MapObject? npc = npcs?.Get(oid);

            if (npc != null)
                npc.MakeActive();
            else if (physics != null)
                npcs?.Add(spawn.Instantiate(physics));
        }

        public void Remove(int objectId)
        {
            MapObject? npc = npcs?.Get(objectId);
            if (npc != null)
                npc.Deactivate();
        }

        public void Clear()
        {
            npcs?.Clear();
        }

        public MapObjects? GetNpcs()
        {
            return npcs;
        }
    }
}