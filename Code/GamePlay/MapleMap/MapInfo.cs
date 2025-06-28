using MapleStory;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class Seat(Wz_Node source)
    {
        private MaplePoint<int> position = new MaplePoint<int>(source.GetValue<Wz_Vector>().X, source.GetValue<Wz_Vector>().Y);

        public bool InRange(MaplePoint<int> position)
        {
            Range<int> horizon = Range<int>.Symmetric((int)position.X, 10);
            Range<int> vertical = Range<int>.Symmetric((int)position.Y, 10);

            return horizon.Contains((int)position.X) && vertical.Contains((int)position.Y);
        }

        public MaplePoint<int> GetPosition()
        {
            return position;
        }
    }

    public class Ladder(Wz_Node source)
    {
        private int x = source.FindNodeByPath("x").GetValue<int>();
        private int y1 = source.FindNodeByPath("y1").GetValue<int>();
        private int y2 = source.FindNodeByPath("y2").GetValue<int>();
        private bool ladder = (source.FindNodeByPath("l")?.GetValue<int>()) != 0;
        private bool upperFloor = (source.FindNodeByPath("uf")?.GetValue<int>()) != 0;

        public bool IsLadder()
        {
            return ladder;
        }

        public bool HaveUpperFloor()
        {
            return upperFloor;
        }

        public bool InRange(MaplePoint<int> position, bool upwards)
        {
            Range<int> horizon = Range<int>.Symmetric((int)position.X, 10);
            Range<int> vertical = new Range<int>(y1, y2);
            int y = upwards ? (int)position.Y - 5 : (int)position.Y + 5;
            return horizon.Contains(x) && vertical.Contains(y);
        }

        public bool FellOff(int y, bool downwards)
        {
            int dy = downwards ? y + 5 : y - 5;
            return dy > y2 || y + 5 < y1;
        }

        public int GetX()
        {
            return x;
        }
    }

    public class MapInfo
    {
        private int fieldLimit;
        private bool cloud;
        private string bgm = string.Empty;
        private string mapDesc = string.Empty;
        private string mapName = string.Empty;
        private string streetName = string.Empty;
        private string mapMark = string.Empty;
        private bool swim;
        private bool town;
        private bool hideMiniMap;
        private Range<int> mapWalls;
        private Range<int> mapBorders;
        private List<Seat> seats = [];
        private List<Ladder> ladders = [];
        public MapInfo(int mapId, Range<int> walls, Range<int> borders)
        {
            string strId = mapId.ToString("D9");
            string prefix = (mapId / 100000000).ToString();
            Wz_Node mapNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Map", "Map", $"Map{prefix}", $"{strId}.img");
            Wz_Node infoNode = mapNode.FindNodeByPath("info");

            if (infoNode != null && infoNode.FindNodeByPath("VRLeft") != null)
            {
                mapWalls = new Range<int>(infoNode.FindNodeByPath("VRLeft").GetValue<int>(), infoNode.FindNodeByPath("VRRight").GetValue<int>());
                mapBorders = new Range<int>(infoNode.FindNodeByPath("VRTop").GetValue<int>(), infoNode.FindNodeByPath("VRBottom").GetValue<int>());
            }
            else
            {
                mapWalls = new Range<int>(walls.First, walls.Second);
                mapBorders = new Range<int>(borders.First, borders.Second);
            }

            string bgmPath = infoNode!.FindNodeByPath("bgm").GetValueEx<string>(string.Empty);
            int split = bgmPath.IndexOf('/');
            bgm = bgmPath.Substring(0, split) + ".img/" + bgmPath.Substring(split + 1);

            cloud = infoNode.FindNodeByPath("cloud")?.GetValue<int>() != 0;
            fieldLimit = infoNode.FindNodeByPath("fieldLimit")?.GetValue<int>() ?? 0;
            hideMiniMap = (infoNode.FindNodeByPath("hideMinimap")?.GetValue<int>()) != 0;
            mapMark = infoNode!.FindNodeByPath("mapMark").GetValueEx<string>(string.Empty);
            swim = (infoNode.FindNodeByPath("swim")?.GetValue<int>()) != 0;
            town = (infoNode.FindNodeByPath("town")?.GetValue<int>()) != 0;

            Wz_Node seatNode = mapNode.FindNodeByPath("seat");
            if (seatNode != null)
                foreach (Wz_Node partNode in seatNode.Nodes)
                    seats.Add(new Seat(partNode));

            Wz_Node ladderNode = mapNode.FindNodeByPath("ladderRope");
            if (ladderNode != null)
                foreach (Wz_Node partNode in ladderNode.Nodes)
                    ladders.Add(new Ladder(partNode));
        }

        public bool IsUnderWater()
        {
            return swim;
        }

        public string GetBGM()
        {
            return bgm;
        }

        public Range<int> GetWalls()
        {
            return mapWalls;
        }

        public Range<int> GetBorders()
        {
            return mapBorders;
        }

        // Find a seat the player's position
        public Seat? FindSeat(MaplePoint<int> position)
        {
            foreach (var seat in seats)
                if (seat.InRange(position))
                    return seat;
            return null;
        }

        // Find a ladder at the player's position
        // !upwards - implies downwards
        public Ladder? FindLadder(MaplePoint<int> position, bool upwards)
        {
            foreach (var ladder in ladders)
                if (ladder.InRange(position, upwards))
                    return ladder;
            return null;
        }
    }
}