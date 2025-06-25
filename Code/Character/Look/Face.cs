using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class Expression
    {
        public enum Id : int
        {
            DEFAULT = 0,
            BLINK,
            HIT,
            SMILE,
            TROUBLED,
            CRY,
            ANGRY,
            BEWILDERED,
            STUNNED,
            BLAZE,
            BOWING,
            CHEERS,
            CHU,
            DAM,
            DESPAIR,
            GLITTER,
            HOT,
            HUM,
            LOVE,
            OOPS,
            PAIN,
            SHINE,
            VOMIT,
            WINK,
            LENGTH
        };

        public static readonly Dictionary<Id, string> names = new()
    {
        { Id.DEFAULT, "default" },
        { Id.BLINK, "blink" },
        { Id.HIT, "hit" },
        { Id.SMILE, "smile" },
        { Id.TROUBLED, "troubled" },
        { Id.CRY, "cry" },
        { Id.ANGRY, "angry" },
        { Id.BEWILDERED, "bewildered" },
        { Id.STUNNED, "stunned" },
        { Id.BLAZE, "blaze" },
        { Id.BOWING, "bowing" },
        { Id.CHEERS, "cheers" },
        { Id.CHU, "chu" },
        { Id.DAM, "dam" },
        { Id.DESPAIR, "despair" },
        { Id.GLITTER, "glitter" },
        { Id.HOT, "hot" },
        { Id.HUM, "hum" },
        { Id.LOVE, "love" },
        { Id.OOPS, "oops" },
        { Id.PAIN, "pain" },
        { Id.SHINE, "shine" },
        { Id.VOMIT, "vomit" },
        { Id.WINK, "wink" }
    };

        public static Id ByAction(int action)
        {
            action -= 98;
            if (action >= 0 && action < (int)Id.LENGTH)
                return (Id)action;
            GD.Print($"Unknown Expression.Id action: [{action}]");
            return Id.DEFAULT;
        }
    }

    public class Face
    {
        private struct Frame
        {
            public MapleTexture texture;
            public int delay;

            public Frame(Wz_Node source)
            {
                texture = new MapleTexture(source.FindNodeByPath("face"));
                Wz_Node browNode = source.FindNodeByPath(true, "face", "map", "brow");
                MaplePoint<int> shift = new MaplePoint<int>(browNode.GetValue<Wz_Vector>().X, browNode.GetValue<Wz_Vector>().Y);
                texture.Shift(-shift);

                Wz_Node delayNode = source.FindNodeByPath("delay");
                if (delayNode != null)
                    delay = delayNode.GetValue<int>();
                else
                    delay = 2500;
            }
        }

        private Dictionary<int, Frame>[] expressions = new Dictionary<int, Frame>[(int)Expression.Id.LENGTH];

        public Face(int faceId)
        {
            for (int i = 0; i < (int)Expression.Id.LENGTH; i++)
                expressions[i] = [];

            foreach (var keyValuePair in Expression.names)
            {
                Expression.Id expression = keyValuePair.Key;
                string expressionName = keyValuePair.Value;

                Wz_Node expressionNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", "Face", $"000{faceId}.img", $"{expressionName}");
                Frame perFrame;

                if (expressionNode.Text == "default")
                {
                    perFrame = new Frame(expressionNode);
                    expressions[(int)expression].Add(0, perFrame);
                }
                else
                {
                    Wz_Node frameNode;
                    for (int frame = 0; (frameNode = expressionNode.FindNodeByPath(frame.ToString())) != null; frame++)
                    {
                        perFrame = new Frame(expressionNode.FindNodeByPath(frame.ToString()));
                        expressions[(int)expression].Add(frame, perFrame);
                    }
                }
            }
        }
        public int GetDelay(Expression.Id expression, int frame)
        {
            if (expressions[(int)expression].TryGetValue(frame, out Frame perFrame))
            {
                return perFrame.delay;
            }
            return 100;
        }
        public int NextFrame(Expression.Id expression, int frame)
        {
            if (expressions[(int)expression].ContainsKey(frame + 1))
                return frame + 1;

            return 0;
        }

        public void Render(CanvasItem canvas, Expression.Id interExpression, int interExpressionFrame, DrawArgument args)
        {
            if (expressions[(int)interExpression].TryGetValue(interExpressionFrame, out Frame frame))
                frame.texture.Render(canvas, args);
        }
    }
}