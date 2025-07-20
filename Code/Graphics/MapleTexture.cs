using Godot;
using System;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class MapleTexture
    {
        private Texture2D texture;
        private MaplePoint<int> origin;
        private MaplePoint<int> dimensions;

        public MapleTexture()
        {
            texture = new Texture2D();
        }

        public MapleTexture(Wz_Node source)
        {
            Image image = new Image();
            Texture2D texture = new Texture2D();

            source = source.ResolveUol();
            // resolve link
            /*        Wz_Node linkedPngNode = GetLinkedSourceNode(source) ?? source;*/
            Wz_Png png = source.GetValue<Wz_Png>() ?? throw new Exception($"{source.FullPathToFile} is not a PNG node");

            byte[] buffer = png.ExtractPngBytes();
            var err = image.LoadPngFromBuffer(buffer);
            if (err != Error.Ok)
            {
                GD.PrintErr("Failed to load PNG from buffer: ", err);
            }

            Wz_Vector origin = source.FindNodeByPath("origin")?.GetValue<Wz_Vector>() ?? new Wz_Vector(0, 0);
            this.origin = new MaplePoint<int>(origin.X, origin.Y);
            dimensions = new MaplePoint<int>(png.Width, png.Height);

            this.texture = ImageTexture.CreateFromImage(image);
        }

        public MapleTexture Clone()
        {
            MapleTexture copy = new MapleTexture();

            if (texture.GetImage().Duplicate() is Image imageCopy)
            {
                copy.texture = ImageTexture.CreateFromImage(imageCopy);
            }
            else
            {
                GD.PrintErr("Failed to duplicate image for texture.");
            }

            // Deep copy of origin and dimensions
            copy.origin = new MaplePoint<int>(origin.X, origin.Y);
            copy.dimensions = new MaplePoint<int>(dimensions.X, dimensions.Y);

            return copy;
        }

        public void Render(CanvasItem targetNode, DrawArgument drawArg)
        {
            if (texture == null || targetNode == null)
                return;

            float xScale = drawArg.GetXScale();
            float yScale = drawArg.GetYScale();
            float angleRad = Mathf.DegToRad(drawArg.GetAngle());
            MapleRectangle<int> rect = drawArg.GetRectangle(origin, dimensions);

            Vector2 size = new(rect.Width(), rect.Height());
            Vector2 center = rect.GetLeftTop().ToVector2() + size / 2f;

            Rect2 drawRect = new Rect2(-size / 2f, size);

            if (xScale < 0)
            {
                center.X -= size.X;
            }
            if (yScale < 0)
            {
                center.Y -= size.Y;
            }

            // Apply scaling and flipping directly via negative values
            targetNode.DrawSetTransform(center, angleRad, new Vector2(xScale, yScale));
            MapleColor color = drawArg.GetColor();
            targetNode.DrawTextureRect(texture, drawRect, false, new Color(color.R, color.G, color.B, color.A));
            targetNode.DrawSetTransform(Vector2.Zero, 0, Vector2.One);
        }
        public void Shift(MaplePoint<int> amount)
        {
            origin -= amount;
        }
        public int Width()
        {
            return (int)dimensions.X;
        }
        public int Height()
        {
            return (int)dimensions.Y;
        }
        public MaplePoint<int> GetOrigin()
        {
            return origin;
        }
        public MaplePoint<int> GetDimension()
        {
            return dimensions;
        }
        public bool IsValid()
        {
            return texture != null;
        }
    }
}