using Godot;
using System;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class FootholdTree : Node2D
    {
        private Dictionary<int, FootHold> footholds = new();
        private Dictionary<int, List<int>> footholdsByX = new();
        private FootHold nullFoothold = new FootHold();
        private Range<int> walls = new(), borders = new();
        public void Init(Wz_Node source)
        {
            LoadFootholdTree(source);
        }
        public FootHold GetFoothold(int footholdId)
        {
            if (footholds.TryGetValue(footholdId, out FootHold? foothold))
            {
                return foothold;
            }
            else
            {
                return nullFoothold;
            }
        }
        public Range<int> GetWalls()
        {
            return walls;
        }
        public Range<int> GetBorders()
        {
            return borders;
        }
        public int GetFootHoldIdBelow(double footholdX, double footholdY)
        {
            int ret = 0;
            double comp = borders.Second;

            int x = (int)footholdX;

            if (footholdsByX.TryGetValue(x, out List<int>? footholdIds))
            {
                foreach (int id in footholdIds)
                {
                    // Get the Foothold by its id
                    if (footholds.TryGetValue(id, out FootHold? foothold))
                    {
                        double ycomp = foothold.GroundBelow(footholdX);

                        if (comp >= ycomp && ycomp >= footholdY)
                        {
                            comp = ycomp;
                            ret = foothold.id;
                        }
                    }
                }
            }

            return ret;
        }
        public double GetWall(int currentId, bool left, double footholdY)
        {
            int shortY = (int)footholdY;
            Range<int> vertical = new Range<int>(shortY - 50, shortY - 1);
            FootHold current = GetFoothold(currentId);

            if (left)
            {
                FootHold prev = GetFoothold(current.Prev());
                while (prev.IsWall())
                {
                    if (prev.IsBlocking(vertical))
                        return current.Left();

                    current = prev;
                    prev = GetFoothold(prev.Prev());
                }

                return walls.First;
            }
            else
            {
                FootHold next = GetFoothold(current.Next());
                while (next.IsWall())
                {
                    if (next.IsBlocking(vertical))
                        return current.Right();

                    current = next;
                    next = GetFoothold(next.Next());
                }

                return walls.Second;
            }
        }
        public double GetEdge(int currentId, bool left)
        {
            FootHold foothold = GetFoothold(currentId);

            if (left)
            {
                int prevId = foothold.Prev();

                if (prevId == 0)
                    return foothold.Left();

                FootHold prev = GetFoothold(prevId);
                int prevPrevId = prev.Prev();

                if (prevPrevId == 0)
                    return prev.Left();

                return walls.First;
            }
            else
            {
                int nextId = foothold.Next();

                if (nextId == 0)
                    return foothold.Right();

                FootHold next = GetFoothold(nextId);
                int nextNextId = next.Next();

                if (nextNextId == 0)
                    return next.Right();

                return walls.Second;
            }
        }
        public void UpdateFootHold(PhysicsObject physicsObject)
        {
            if (physicsObject.objectType == PhysicsObject.Type.Fixated && physicsObject.footHoldId > 0)
                return;

            FootHold currentFootHold = GetFoothold(physicsObject.footHoldId);
            bool checkSlope = false;

            double x = physicsObject.CurrentX();
            double y = physicsObject.CurrentY();

            if (physicsObject.onGround)
            {
                if (Math.Floor(x) > currentFootHold.Right())
                    physicsObject.footHoldId = currentFootHold.Next();
                else if (Math.Ceiling(x) < currentFootHold.Left())
                    physicsObject.footHoldId = currentFootHold.Prev();

                if (physicsObject.footHoldId == 0)
                    physicsObject.footHoldId = GetFootHoldIdBelow(x, y);
                else
                    checkSlope = true;
            }
            else
            {
                physicsObject.footHoldId = GetFootHoldIdBelow(x, y);

                if (physicsObject.footHoldId == 0)
                    return;
            }

            FootHold nextFootHold = GetFoothold(physicsObject.footHoldId);
            physicsObject.footHoldSlope = nextFootHold.Slope();

            double ground = nextFootHold.GroundBelow(x);

            if (physicsObject.vspeed == 0 && checkSlope)
            {
                double vdelta = Math.Abs(physicsObject.footHoldSlope);

                if (physicsObject.footHoldSlope < 0)
                    vdelta *= (ground - y);
                else if (physicsObject.footHoldSlope > 0)
                    vdelta *= (y - ground);

                if (currentFootHold.Slope() != 0 || nextFootHold.Slope() != 0)
                {
                    if (physicsObject.hspeed > 0 && vdelta <= physicsObject.hspeed)
                        physicsObject.y.UpdateValue(ground);
                    else if (physicsObject.hspeed < 0 && vdelta >= physicsObject.hspeed)
                        physicsObject.y.UpdateValue(ground);
                }
            }

            physicsObject.onGround = (physicsObject.y == ground);

            if (physicsObject.enableJumpDown || physicsObject.IsFlagSet(PhysicsObject.Flag.CheckBelow))
            {
                int belowid = GetFootHoldIdBelow(x, nextFootHold.GroundBelow(x) + 1.0);

                if (belowid > 0)
                {
                    double nextground = GetFoothold(belowid).GroundBelow(x);
                    physicsObject.enableJumpDown = (nextground - ground) < 600.0;
                    physicsObject.groundBelow = ground + 1.0;
                }
                else
                {
                    physicsObject.enableJumpDown = false;
                }

                physicsObject.ClearFlag(PhysicsObject.Flag.CheckBelow);
            }

            if (physicsObject.footHoldLayer == 0 || physicsObject.onGround)
                physicsObject.footHoldLayer = nextFootHold.layer;

            if (physicsObject.footHoldId == 0)
            {
                physicsObject.footHoldId = currentFootHold.id;
                physicsObject.LimitX(currentFootHold.x1());
            }
        }
        public void LimitMovement(PhysicsObject physicsObject)
        {
            if (physicsObject.HMobile())
            {
                double currentX = physicsObject.CurrentX();
                double nextX = physicsObject.NextX();

                bool left = physicsObject.hspeed < 0;
                double wall = GetWall(physicsObject.footHoldId, left, physicsObject.NextY());
                bool collision = left ? currentX >= wall && nextX <= wall : currentX <= wall && nextX >= wall;

                if (!collision && physicsObject.IsFlagSet(PhysicsObject.Flag.TurnAtEdges))
                {
                    wall = GetEdge(physicsObject.footHoldId, left);
                    collision = left ? currentX >= wall && nextX <= wall : currentX <= wall && nextX >= wall;
                }

                if (collision)
                {
                    physicsObject.LimitX(wall);
                    physicsObject.ClearFlag(PhysicsObject.Flag.TurnAtEdges);
                }
            }

            if (physicsObject.VMobile())
            {
                double currentY = physicsObject.CurrentY();
                double nextY = physicsObject.NextY();

                Range<double> ground = new Range<double>(
                    GetFoothold(physicsObject.footHoldId).GroundBelow(physicsObject.CurrentX()),
                    GetFoothold(physicsObject.footHoldId).GroundBelow(physicsObject.NextX())
                    );

                bool collision = currentY <= ground.First && nextY >= ground.Second;

                if (collision)
                {
                    physicsObject.LimitY(ground.Second);
                    LimitMovement(physicsObject);
                }
                else
                {
                    if (nextY < borders.First)
                        physicsObject.LimitY(borders.First);
                    else if (nextY > borders.Second)
                        physicsObject.LimitY(borders.Second);
                }
            }
        }
        public void LoadFootholdTree(Wz_Node source)
        {
            footholds = new Dictionary<int, FootHold>();
            footholdsByX = new Dictionary<int, List<int>>();

            int left = 30000;
            int right = -30000;
            int bottom = -30000;
            int top = 30000;

            Wz_Node fhListNode = source;
            if (fhListNode != null)
            {
                foreach (Wz_Node layerNode in fhListNode.Nodes)
                {
                    Int32.TryParse(layerNode.Text, out int _layer);
                    foreach (Wz_Node zNode in layerNode.Nodes)
                    {
                        Int32.TryParse(zNode.Text, out int _z);
                        foreach (Wz_Node fhNode in zNode.Nodes)
                        {
                            Int32.TryParse(fhNode.Text, out int _fh);

                            Wz_Node x1 = fhNode.FindNodeByPath("x1"),
                                x2 = fhNode.FindNodeByPath("x2"),
                                y1 = fhNode.FindNodeByPath("y1"),
                                y2 = fhNode.FindNodeByPath("y2"),
                                prev = fhNode.FindNodeByPath("prev"),
                                next = fhNode.FindNodeByPath("next"),
                                piece = fhNode.FindNodeByPath("piece");

                            FootHold footHold = new FootHold();
                            footHold.layer = _layer;
                            footHold.group = _z;
                            footHold.id = _fh;
                            footHold.horizontal = new Range<int>(x1.GetValueEx<int>(0), x2.GetValueEx<int>(0));
                            footHold.vertical = new Range<int>(y1.GetValueEx<int>(0), y2.GetValueEx<int>(0));
                            footHold.prev = prev.GetValueEx<int>(0);
                            footHold.next = next.GetValueEx<int>(0);
                            footHold.piece = piece.GetValueEx<int>(0);
                            footholds.Add(_fh, footHold);

                            if (footHold.Left() < left)
                            {
                                left = footHold.Left();
                            }

                            if (footHold.Right() > right)
                            {
                                right = footHold.Right();
                            }

                            if (footHold.Bottom() > bottom)
                            {
                                bottom = footHold.Bottom();
                            }

                            if (footHold.Top() < top)
                            {
                                top = footHold.Top();
                            }

                            if (footHold.IsWall())
                            {
                                continue;
                            }

                            int start = footHold.Left();
                            int end = footHold.Right();

                            for (int i = start; i <= end; i++)
                            {
                                if (!footholdsByX.ContainsKey(i))
                                {
                                    footholdsByX[i] = new List<int>();
                                }
                                footholdsByX[i].Add(_fh);
                            }
                        }
                    }
                }

                walls = new Range<int>(left + 25, right - 25);
                borders = new Range<int>(top - 300, bottom + 100);
            }
        }
        public int GetYBelow(MaplePoint<int> position)
        {
            int footholdId = GetFootHoldIdBelow(position.X, position.Y);
            if (footholdId != 0)
            {
                FootHold foothold = GetFoothold(footholdId);
                return (int)foothold.GroundBelow(position.X);
            }
            else
            {
                return borders.Second;
            }
        }
    }
}
