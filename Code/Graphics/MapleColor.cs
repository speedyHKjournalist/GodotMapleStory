// Simple color class which stores RGBA components
using System;
using System.Collections.Generic;

namespace MapleStory
{
    public class MapleColor
    {
        public const int LENGTH = 4;
        private float[] rgba = new float[LENGTH];

        // Codes of predefined colors
        public enum ColorCode : uint
        {
            CNONE = 0x00000000,
            CWHITE = 0xFFFFFFFF,
            CBLACK = 0x000000FF,
            CRED = 0xFF0000FF,
            CGREEN = 0x00FF00FF,
            CBLUE = 0x0000FFFF,
            CYELLOW = 0xFFFF00FF,
            CTURQUOISE = 0x00FFFFFF,
            CPURPLE = 0xFF00FFFF
        }

        // ColorName of predefined colors
        public enum ColorName
        {
            BLACK,
            WHITE,
            YELLOW,
            BLUE,
            RED,
            DARKRED,
            BROWN,
            JAMBALAYA,
            LIGHTGREY,
            DARKGREY,
            ORANGE,
            MEDIUMBLUE,
            VIOLET,
            TOBACCOBROWN,
            EAGLE,
            LEMONGRASS,
            TUNA,
            GALLERY,
            DUSTYGRAY,
            EMPEROR,
            MINESHAFT,
            HALFANDHALF,
            ENDEAVOUR,
            BROWNDERBY,
            PORCELAIN,
            IRISHCOFFEE,
            BOULDER,
            GREEN,
            LIGHTGREEN,
            JAPANESELAUREL,
            GRAYOLIVE,
            ELECTRICLIME,
            SUPERNOVA,
            CHARTREUSE,
            MALIBU,
            SILVERCHALICE,
            GRAY,
            TORCHRED,
            CREAM,
            PERSIANGREEN,
            SILVER,
            NUM_COLORS
        };

        public static readonly Dictionary<ColorName, float[]> predefinedColors = new()
        {
            [ColorName.BLACK] = new float[] { 0.00f, 0.00f, 0.00f },
            [ColorName.WHITE] = new float[] { 1.00f, 1.00f, 1.00f },
            [ColorName.YELLOW] = new float[] { 1.00f, 1.00f, 0.00f },
            [ColorName.BLUE] = new float[] { 0.00f, 0.00f, 1.00f },
            [ColorName.RED] = new float[] { 1.00f, 0.00f, 0.00f },
            [ColorName.DARKRED] = new float[] { 0.80f, 0.30f, 0.30f },
            [ColorName.BROWN] = new float[] { 0.50f, 0.25f, 0.00f },
            [ColorName.JAMBALAYA] = new float[] { 0.34f, 0.20f, 0.07f },
            [ColorName.LIGHTGREY] = new float[] { 0.50f, 0.50f, 0.50f },
            [ColorName.DARKGREY] = new float[] { 0.25f, 0.25f, 0.25f },
            [ColorName.ORANGE] = new float[] { 1.00f, 0.50f, 0.00f },
            [ColorName.MEDIUMBLUE] = new float[] { 0.00f, 0.75f, 1.00f },
            [ColorName.VIOLET] = new float[] { 0.50f, 0.00f, 0.50f },
            [ColorName.TOBACCOBROWN] = new float[] { 0.47f, 0.40f, 0.27f },
            [ColorName.EAGLE] = new float[] { 0.74f, 0.74f, 0.67f },
            [ColorName.LEMONGRASS] = new float[] { 0.60f, 0.60f, 0.54f },
            [ColorName.TUNA] = new float[] { 0.20f, 0.20f, 0.27f },
            [ColorName.GALLERY] = new float[] { 0.94f, 0.94f, 0.94f },
            [ColorName.DUSTYGRAY] = new float[] { 0.60f, 0.60f, 0.60f },
            [ColorName.EMPEROR] = new float[] { 0.34f, 0.34f, 0.34f },
            [ColorName.MINESHAFT] = new float[] { 0.20f, 0.20f, 0.20f },
            [ColorName.HALFANDHALF] = new float[] { 1.00f, 1.00f, 0.87f },
            [ColorName.ENDEAVOUR] = new float[] { 0.00f, 0.40f, 0.67f },
            [ColorName.BROWNDERBY] = new float[] { 0.30f, 0.20f, 0.10f },
            [ColorName.PORCELAIN] = new float[] { 0.94f, 0.95f, 0.95f },
            [ColorName.IRISHCOFFEE] = new float[] { 0.34f, 0.27f, 0.14f },
            [ColorName.BOULDER] = new float[] { 0.47f, 0.47f, 0.47f },
            [ColorName.GREEN] = new float[] { 0.00f, 0.75f, 0.00f },
            [ColorName.LIGHTGREEN] = new float[] { 0.00f, 1.00f, 0.00f },
            [ColorName.JAPANESELAUREL] = new float[] { 0.00f, 0.50f, 0.00f },
            [ColorName.GRAYOLIVE] = new float[] { 0.67f, 0.67f, 0.60f },
            [ColorName.ELECTRICLIME] = new float[] { 0.80f, 1.00f, 0.00f },
            [ColorName.SUPERNOVA] = new float[] { 1.00f, 0.80f, 0.00f },
            [ColorName.CHARTREUSE] = new float[] { 0.47f, 1.00f, 0.00f },
            [ColorName.MALIBU] = new float[] { 0.47f, 0.80f, 1.00f },
            [ColorName.SILVERCHALICE] = new float[] { 0.67f, 0.67f, 0.67f },
            [ColorName.GRAY] = new float[] { 0.54f, 0.54f, 0.54f },
            [ColorName.TORCHRED] = new float[] { 0.94f, 0.00f, 0.20f },
            [ColorName.CREAM] = new float[] { 1.00f, 1.00f, 0.80f },
            [ColorName.PERSIANGREEN] = new float[] { 0.00f, 0.67f, 0.67f },
            [ColorName.SILVER] = new float[] { 0.80f, 0.80f, 0.80f }
        };

        public MapleColor(float r, float g, float b, float a)
        {
            rgba[0] = r;
            rgba[1] = g;
            rgba[2] = b;
            rgba[3] = a;
        }

        public MapleColor(float[] comps)
        {
            Array.Copy(comps, rgba, LENGTH);
        }

        public MapleColor(int r, int g, int b, int a) :
            this(r / 255f, g / 255f, b / 255f, a / 255f)
        { }

        public MapleColor(uint code) :
            this((byte)(code >> 24), (byte)(code >> 16), (byte)(code >> 8), (byte)(code))
        { }

        public MapleColor(ColorCode code) : this((uint)code) { }

        public MapleColor(ColorName name) :
            this(predefinedColors[name][0], predefinedColors[name][1], predefinedColors[name][2], 1.0f)
        { }

        public MapleColor() : this(ColorCode.CNONE) { }

        public float R => rgba[0];
        public float G => rgba[1];
        public float B => rgba[2];
        public float A => rgba[3];
        public float[] Data => rgba;

        public bool Invisible => A <= 0.0f;

        public MapleColor Blend(MapleColor other, float alpha)
        {
            var blended = new float[LENGTH];
            for (int i = 0; i < LENGTH; ++i)
            {
                blended[i] = MathUtils.Lerp(rgba[i], other.rgba[i], alpha);
            }
            return new MapleColor(blended);
        }

        public static MapleColor operator *(MapleColor a, MapleColor b)
        {
            return new MapleColor(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
        }

        public static MapleColor operator /(MapleColor a, MapleColor b)
        {
            return new MapleColor(a.R / b.R, a.G / b.G, a.B / b.B, a.A / b.A);
        }
    }
}