using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class DamageNumber : Node2D
    {
        public enum Type : int
        {
            NORMAL,
            CRITICAL,
            TOPLAYER
        }

        private const int NUM_TYPES = 3;
        private const float FADE_TIME = 1.5f;

        private Type type;
        private bool miss;
        private bool multiple;
        private bool faded = false;
        private char firstNum;
        private string restNum = string.Empty;
        private int shift;
        private Linear<float> opacity = new();
        private BoolPair<Charset>[] charsets = new BoolPair<Charset>[NUM_TYPES];
        private MaplePoint<float> basePosition = new();
        private Dictionary<bool, Dictionary<int, List<DrawArgument>>> charPositions = [];

        public DamageNumber()
        {
            for (int i = 0; i < NUM_TYPES; i++)
                charsets[i] = new BoolPair<Charset>();

            opacity.SetAllValue(FADE_TIME);
            Init();
        }

        public DamageNumber(Type type, int damage)
        {
            this.type = type;

            for (int i = 0; i < NUM_TYPES; i++)
                charsets[i] = new BoolPair<Charset>();

            opacity.SetAllValue(FADE_TIME);

            Init();

            if (damage > 0)
            {
                miss = false;
                string number = damage.ToString();
                firstNum = number[0];

                if (number.Length > 1)
                {
                    restNum = number.Substring(1);
                    multiple = true;
                }
                else
                {
                    restNum = string.Empty;
                    multiple = false;
                }

                int total = GetAdvance(firstNum, true);

                for (int i = 0; i < restNum.Length; i++)
                {
                    char c = restNum[i];
                    int advance = GetAdvance(c, false);
                    total += advance;
                }
                shift = total / 2;
            }
            else
            {
                shift = charsets[(int)type][true]!.GetWidth('M') / 2;
                miss = true;
            }
        }

        public void Init()
        {
            Wz_Node BasicEffect = WzLib.wzs.WzNode.FindNodeByPath(true, "Effect", $"BasicEff.img");

            charsets[(int)Type.NORMAL].Set(false, new Charset(BasicEffect.FindNodeByPath("NoRed1"), Charset.Alignment.LEFT));
            charsets[(int)Type.NORMAL].Set(true, new Charset(BasicEffect.FindNodeByPath("NoRed0"), Charset.Alignment.LEFT));
            charsets[(int)Type.CRITICAL].Set(false, new Charset(BasicEffect.FindNodeByPath("NoCri1"), Charset.Alignment.LEFT));
            charsets[(int)Type.CRITICAL].Set(true, new Charset(BasicEffect.FindNodeByPath("NoCri0"), Charset.Alignment.LEFT));
            charsets[(int)Type.TOPLAYER].Set(false, new Charset(BasicEffect.FindNodeByPath("NoViolet1"), Charset.Alignment.LEFT));
            charsets[(int)Type.TOPLAYER].Set(true, new Charset(BasicEffect.FindNodeByPath("NoViolet0"), Charset.Alignment.LEFT));
        }

        public int GetAdvance(char c, bool first)
        {
            const int LENGTH = 10;

            int[] advances = { 24, 20, 22, 22, 24, 23, 24, 22, 24, 24 };

            int index = c - '0'; // Convert char to int index (same as subtracting 48)

            if (index >= 0 && index < LENGTH)
            {
                int advance = advances[index];

                switch (type)
                {
                    case Type.CRITICAL:
                        advance += first ? 8 : 4;
                        break;
                    default:
                        if (first)
                            advance += 2;
                        break;
                }

                return advance;
            }
            return 0;
        }

        public bool Faded => faded;

        public override void _Draw()
        {
            if (charPositions.Count != 0)
                if (miss)
                {
                    foreach (var pair in charPositions[true])
                    {
                        int c = pair.Key;
                        charsets[(int)type][true]?.Render(this, c, pair.Value);
                    }
                }
                else
                {
                    foreach (var pair in charPositions[false])
                    {
                        int c = pair.Key;
                        charsets[(int)type][true]?.Render(this, c, pair.Value);
                    }

                    foreach (var pair in charPositions[true])
                    {
                        int c = pair.Key;
                        charsets[(int)type][true]?.Render(this, c, pair.Value);
                    }
                }
        }

        public override void _Process(double delta)
        {
            float alpha = (float)Engine.GetPhysicsInterpolationFraction();
            MaplePoint<int> position =  new((int)basePosition.X, (int)(basePosition.Y - shift));
            float interOpacity = opacity.Get(alpha);

            charPositions.Clear();
            charPositions[true] = [];
            charPositions[false] = [];

            if (miss)
            {
                charPositions[true]['M'] = [];
                charPositions[true]['M'].Add(new DrawArgument(position, interOpacity));
            }
            else
            {
                charPositions[false][firstNum] = [];
                charPositions[false][firstNum].Add(new DrawArgument(position, interOpacity));

                if (multiple)
                {
                    int firstAdvance = GetAdvance(firstNum, true);
                    position = position.ShiftX(firstAdvance);

                    for (int i = 0; i < restNum.Length; i++)
                    {
                        char c = restNum[i];
                        MaplePoint<int> yShift = new(0, (i % 2 == 0) ? 2 : -2);

                        if (!charPositions[true].TryGetValue(c, out var value))
                            charPositions[true][c] = [];

                        charPositions[true][c].Add(new DrawArgument(position + yShift, interOpacity));

                        int advance;
                        if (i < restNum.Length - 1)
                        {
                            char n = restNum[i + 1];
                            int cAdvance = GetAdvance(c, false);
                            int nAdvance = GetAdvance(n, false);
                            advance = (cAdvance + nAdvance) / 2;
                        }
                        else
                        {
                            advance = GetAdvance(c, false);
                        }

                        position = position.ShiftX(advance);
                    }
                }
            }
            QueueRedraw();
        }

        public override void _PhysicsProcess(double delta)
        {
            basePosition = basePosition.ShiftY(-0.25f);
            opacity.Subtract((float)delta);

            if (opacity.Last() <= 0.0f)
                faded = true;
        }
    }
}