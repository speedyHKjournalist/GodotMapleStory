using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class BodyAction
    {
        // A frame of animation for a skill or similar 'meta-stance' 
        // This simply redirects to a different stance and frame to use
        private Stance.Id stance;
        private int frame;
        private int delay;
        private MaplePoint<int> move;
        bool attackFrame;

        public BodyAction(Wz_Node frameNode)
        {
            stance = Stance.StanceUtils.ByString(frameNode.FindNodeByPath("action").GetValue<string>());
            frame = frameNode.FindNodeByPath("frame").GetValue<int>();

            Wz_Node moveNode = frameNode.FindNodeByPath("move");
            if (moveNode == null)
                move = new MaplePoint<int>();
            else
                move = new MaplePoint<int>(moveNode.GetValue<Wz_Vector>().X, moveNode.GetValue<Wz_Vector>().Y);

            int signedDelay = frameNode.FindNodeByPath("delay").GetValueEx<int>(0);
            if (signedDelay == 0)
                signedDelay = 100;

            if (signedDelay > 0)
            {
                delay = signedDelay;
                attackFrame = true;
            }
            else if (signedDelay < 0)
            {
                delay = -signedDelay;
                attackFrame = false;
            }
        }

        public bool IsAttackFrame()
        {
            return attackFrame;
        }

        public int GetFrame()
        {
            return frame;
        }

        public int GetDelay()
        {
            return delay;
        }

        public MaplePoint<int> GetMove()
        {
            return move;
        }

        public Stance.Id GetStance()
        {
            return stance;
        }
    }

    public class BodyDrawInfo
    {
        private Dictionary<int, MaplePoint<int>>[] bodyPositionShift;
        private Dictionary<int, MaplePoint<int>>[] armPositionShift;
        private Dictionary<int, MaplePoint<int>>[] handPositionShift;
        private Dictionary<int, MaplePoint<int>>[] headPositionShift;
        private Dictionary<int, MaplePoint<int>>[] facePositionShift;
        private Dictionary<int, MaplePoint<int>>[] hairPositionShift;
        private Dictionary<int, int>[] stanceDelays;

        private Dictionary<string, Dictionary<int, BodyAction>> bodyAction;
        private Dictionary<string, List<int>> attackDelays;

        public BodyDrawInfo()
        {
            int length = (int)Stance.Id.LENGTH;

            bodyPositionShift = new Dictionary<int, MaplePoint<int>>[length];
            armPositionShift = new Dictionary<int, MaplePoint<int>>[length];
            handPositionShift = new Dictionary<int, MaplePoint<int>>[length];
            headPositionShift = new Dictionary<int, MaplePoint<int>>[length];
            facePositionShift = new Dictionary<int, MaplePoint<int>>[length];
            hairPositionShift = new Dictionary<int, MaplePoint<int>>[length];
            stanceDelays = new Dictionary<int, int>[length];
            bodyAction = new Dictionary<string, Dictionary<int, BodyAction>>();
            attackDelays = new Dictionary<string, List<int>>();

            for (int i = 0; i < (int)Stance.Id.LENGTH; i++)
            {
                bodyPositionShift[i] = new Dictionary<int, MaplePoint<int>>();
                armPositionShift[i] = new Dictionary<int, MaplePoint<int>>();
                handPositionShift[i] = new Dictionary<int, MaplePoint<int>>();
                headPositionShift[i] = new Dictionary<int, MaplePoint<int>>();
                facePositionShift[i] = new Dictionary<int, MaplePoint<int>>();
                hairPositionShift[i] = new Dictionary<int, MaplePoint<int>>();
                stanceDelays[i] = new Dictionary<int, int>();
            }

            Wz_Node bodyNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", $"00002000.img");
            Wz_Node headNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", $"00012000.img");

            foreach (Wz_Node bodyStanceNode in bodyNode.Nodes)
            {
                if (bodyStanceNode.Text == "info")
                {
                    continue;
                }
                bodyAction[bodyStanceNode.Text] = new Dictionary<int, BodyAction>();
                attackDelays[bodyStanceNode.Text] = new List<int>();

                LoadAnimationPositionShift(bodyStanceNode, headNode);
            }
        }
        public MaplePoint<int> GetBodyPostionShift(Stance.Id stance, int frame)
        {
            if (bodyPositionShift[(int)stance].TryGetValue(frame, out var positionShift))
                return positionShift;
            return new();
        }
        public MaplePoint<int> GetArmPostionShift(Stance.Id stance, int frame)
        {
            if (armPositionShift[(int)stance].TryGetValue(frame, out var positionShift))
                return positionShift;
            return new();
        }
        public MaplePoint<int> GetHandPostionShift(Stance.Id stance, int frame)
        {
            if (handPositionShift[(int)stance].TryGetValue(frame, out var positionShift))
                return positionShift;
            return new();
        }
        public MaplePoint<int> GetHeadPostionShift(Stance.Id stance, int frame)
        {
            if (headPositionShift[(int)stance].TryGetValue(frame, out var positionShift))
                return positionShift;
            return new();
        }
        public MaplePoint<int> GetFacePostionShift(Stance.Id stance, int frame)
        {
            if (facePositionShift[(int)stance].TryGetValue(frame, out var positionShift))
                return positionShift;
            return new();
        }
        public MaplePoint<int> GetHairPostionShift(Stance.Id stance, int frame)
        {
            if (hairPositionShift[(int)stance].TryGetValue(frame, out var positionShift))
                return positionShift;
            return new();
        }
        public int GetDelay(Stance.Id stance, int frame)
        {
            if (stanceDelays[(int)stance].TryGetValue(frame, out int delay))
                return delay;

            return 100;
        }
        public int GetAttackDelay(string action, int number)
        {
            if (attackDelays.TryGetValue(action, out var delays))
                if (number >= 0 && number < delays.Count)
                    return delays[number];

            return 0;
        }
        public int NextFrame(Stance.Id stance, int frame)
        {
            if (stanceDelays[(int)stance].ContainsKey(frame + 1))
                return frame + 1;
            else
                return 0;
        }
        public int NextActionFrame(string action, int frame)
        {
            if (bodyAction.TryGetValue(action, out var actionFrames))
            {
                if (actionFrames.ContainsKey(frame + 1))
                    return frame + 1;
            }

            return 0;
        }
        public BodyAction? GetAction(string action, int frame)
        {
            if (bodyAction.TryGetValue(action, out var actionFrames))
            {
                if (actionFrames.TryGetValue(frame, out var bodyAction))
                    return bodyAction;
            }

            return null;
        }
        private void LoadAnimationPositionShift(Wz_Node bodyStanceNode, Wz_Node headNode)
        {
            string stanceName = bodyStanceNode.Text;
            int attackDelay = 0;
            int frame = 0;

            foreach (Wz_Node frameNode in bodyStanceNode.Nodes)
            {
                if (frameNode.FindNodeByPath("action") != null)
                {
                    BodyAction action = new BodyAction(frameNode);
                    bodyAction[stanceName].Add(frame, action);

                    if (action.IsAttackFrame())
                    {
                        attackDelays[stanceName].Add(attackDelay);
                    }
                    attackDelay += action.GetDelay();
                }
                else
                {
                    Stance.Id stance = Stance.StanceUtils.ByString(stanceName);
                    if (stance == Stance.Id.NONE || stance == Stance.Id.LENGTH)
                    {
                        GD.Print("Unknown Stance name " + stanceName);
                        continue;
                    }

                    Wz_Node delayNode = frameNode.FindNodeByPath("delay");
                    int delay = (delayNode == null) ? 100 : delayNode.GetValue<int>();
                    stanceDelays[(int)stance].Add(frame, delay);

                    Dictionary<Body.Layer, Dictionary<string, MaplePoint<int>>> bodyShiftMap = new Dictionary<Body.Layer, Dictionary<string, MaplePoint<int>>>();
                    foreach (Wz_Node partNode in frameNode.Nodes)
                    {
                        string part = partNode.Text;
                        if (part != "delay" && part != "face")
                        {
                            string zstr;
                            Wz_Node mapNodes;
                            if (partNode.GetValue<Wz_Uol>() == null)
                            {
                                zstr = partNode.FindNodeByPath("z").GetValue<string>();
                                mapNodes = partNode.FindNodeByPath("map");
                            }
                            else
                            {
                                zstr = partNode.ResolveUol().FindNodeByPath("z").GetValue<string>();
                                mapNodes = partNode.ResolveUol().FindNodeByPath("map");
                            }
                            Body.Layer z = Body.LayerByName(zstr);

                            if (!bodyShiftMap.ContainsKey(z))
                            {
                                bodyShiftMap[z] = new Dictionary<string, MaplePoint<int>>();
                            }
                            foreach (Wz_Node mapNode in mapNodes.Nodes)
                            {
                                bodyShiftMap[z][mapNode.Text] = new MaplePoint<int>(mapNode.GetValue<Wz_Vector>().X, mapNode.GetValue<Wz_Vector>().Y);
                            }
                        }
                    }

                    Wz_Node headStanceNode = headNode.FindNodeByPath(stanceName);
                    if (headStanceNode != null)
                    {
                        Wz_Node headnode = headStanceNode.FindNodeByPath(frame.ToString()).FindNodeByPath("head").ResolveUol();
                        Wz_Node headMapNode = headnode.FindNodeByPath("map");
                        string zstr = headnode.FindNodeByPath("z").GetValue<string>();
                        Body.Layer z = Body.LayerByName(zstr);
                        bodyShiftMap[z] = new Dictionary<string, MaplePoint<int>>();
                        foreach (Wz_Node mapNode in headMapNode.Nodes)
                        {
                            bodyShiftMap[z][mapNode.Text] = new MaplePoint<int>(mapNode.GetValue<Wz_Vector>().X, mapNode.GetValue<Wz_Vector>().Y);
                        }
                    }

                    Dictionary<string, MaplePoint<int>> bodyShiftMapArm = bodyShiftMap.GetValueOrDefault(Body.Layer.ARM, new Dictionary<string, MaplePoint<int>>());
                    Dictionary<string, MaplePoint<int>> bodyShiftMapBody = bodyShiftMap.GetValueOrDefault(Body.Layer.BODY, new Dictionary<string, MaplePoint<int>>());
                    Dictionary<string, MaplePoint<int>> bodyShiftMapHead = bodyShiftMap.GetValueOrDefault(Body.Layer.HEAD, new Dictionary<string, MaplePoint<int>>());
                    Dictionary<string, MaplePoint<int>> bodyShiftMapArmOverHair = bodyShiftMap.GetValueOrDefault(Body.Layer.ARM_OVER_HAIR, new Dictionary<string, MaplePoint<int>>());
                    Dictionary<string, MaplePoint<int>> bodyShiftMapHandBelowWeapon = bodyShiftMap.GetValueOrDefault(Body.Layer.HAND_BELOW_WEAPON, new Dictionary<string, MaplePoint<int>>());

                    bodyPositionShift[(int)stance][frame] = bodyShiftMapBody.GetValueOrDefault("navel", new MaplePoint<int>(0, 0));
                    if (bodyShiftMapArm.Count != 0)
                    {
                        armPositionShift[(int)stance][frame] = bodyShiftMapArm.GetValueOrDefault("hand", new MaplePoint<int>(0, 0))
                            - bodyShiftMapArm.GetValueOrDefault("navel", new MaplePoint<int>(0, 0))
                            + bodyShiftMapBody.GetValueOrDefault("navel", new MaplePoint<int>(0, 0));
                    }
                    else
                    {
                        armPositionShift[(int)stance][frame] = bodyShiftMapArmOverHair.GetValueOrDefault("hand", new MaplePoint<int>(0, 0))
                            - bodyShiftMapArmOverHair.GetValueOrDefault("navel", new MaplePoint<int>(0, 0))
                            + bodyShiftMapBody.GetValueOrDefault("navel", new MaplePoint<int>(0, 0));
                    }
                    handPositionShift[(int)stance][frame] = bodyShiftMapHandBelowWeapon.GetValueOrDefault("handMove", new MaplePoint<int>(0, 0));
                    headPositionShift[(int)stance][frame] = bodyShiftMapBody.GetValueOrDefault("neck", new MaplePoint<int>(0, 0)) - bodyShiftMapHead.GetValueOrDefault("neck", new MaplePoint<int>(0, 0));
                    facePositionShift[(int)stance][frame] = bodyShiftMapBody.GetValueOrDefault("neck", new MaplePoint<int>(0, 0)) - bodyShiftMapHead.GetValueOrDefault("neck", new MaplePoint<int>(0, 0)) + bodyShiftMapHead.GetValueOrDefault("brow", new MaplePoint<int>(0, 0));
                    hairPositionShift[(int)stance][frame] = bodyShiftMapHead.GetValueOrDefault("brow", new MaplePoint<int>(0, 0)) - bodyShiftMapHead.GetValueOrDefault("neck", new MaplePoint<int>(0, 0)) + bodyShiftMapBody.GetValueOrDefault("neck", new MaplePoint<int>(0, 0));
                }
                frame++;
            }
        }
    }
}