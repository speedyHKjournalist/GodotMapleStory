using Godot;

namespace MapleStory
{
    public partial class MapleFrame : Control
    {
        private TextureRect? center;
        private TextureRect? east;
        private TextureRect? northEast;
        private TextureRect? north;
        private TextureRect? northWest;
        private TextureRect? west;
        private TextureRect? southWest;
        private TextureRect? south;
        private TextureRect? southEast;

        public override void _Ready()
        {
            center = GetNode<TextureRect>("center");
            east = GetNode<TextureRect>("east");
            northEast = GetNode<TextureRect>("northEast");
            north = GetNode<TextureRect>("north");
            northWest = GetNode<TextureRect>("northWest");
            west = GetNode<TextureRect>("west");
            southWest = GetNode<TextureRect>("southWest");
            south = GetNode<TextureRect>("south");
            southEast = GetNode<TextureRect>("southEast");
        }

        public void Init(string basePath)
        {
            string _frameTexturesBasePath = basePath.TrimEnd('/') + "/";

            center!.Texture = LoadTexture(_frameTexturesBasePath + "/c.png");
            east!.Texture = LoadTexture(_frameTexturesBasePath + "/e.png");
            northEast!.Texture = LoadTexture(_frameTexturesBasePath + "/ne.png");
            north!.Texture = LoadTexture(_frameTexturesBasePath + "/n.png");
            northWest!.Texture = LoadTexture(_frameTexturesBasePath + "/nw.png");
            west!.Texture = LoadTexture(_frameTexturesBasePath + "/w.png");
            southWest!.Texture = LoadTexture(_frameTexturesBasePath + "/sw.png");
            south!.Texture = LoadTexture(_frameTexturesBasePath + "/s.png");
            southEast!.Texture = LoadTexture(_frameTexturesBasePath + "/se.png");

            north!.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            north!.StretchMode = TextureRect.StretchModeEnum.Tile;
            south!.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            south!.StretchMode = TextureRect.StretchModeEnum.Tile;
            west!.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            west!.StretchMode = TextureRect.StretchModeEnum.Tile;
            east!.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            east!.StretchMode = TextureRect.StretchModeEnum.Tile;
            center!.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            center!.StretchMode = TextureRect.StretchModeEnum.Tile;

            northWest!.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
            northEast!.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
            southWest!.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
            southEast!.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        }

        public static Texture2D? LoadTexture(string path)
        {
            Texture2D? loadedTexture = GD.Load<Texture2D>(path);
            if (loadedTexture == null)
            {
                GD.PrintErr($"MapleFrame: Failed to load texture from '{path}'. Check if file exists and is imported.");
            }
            return loadedTexture;
        }

        public void ResetLayout(int frameWidth, int frameHeight)
        {
            float nwW = northWest!.Texture.GetWidth();
            float nwH = northWest!.Texture.GetHeight();
            float neW = northEast!.Texture.GetWidth();
            float neH = northEast!.Texture.GetHeight();
            float swW = southWest!.Texture.GetWidth();
            float swH = southWest!.Texture.GetHeight();
            float seW = southEast!.Texture.GetWidth();
            float seH = southEast!.Texture.GetHeight();

            float nH = north!.Texture.GetHeight();
            float wW = west!.Texture.GetWidth();

            // Calculate the stretchable inner dimensions
            float innerWidth = frameWidth - nwW - neW;
            float innerHeight = frameHeight - nwH - swH;

            if (innerWidth < 0) innerWidth = 0;
            if (innerHeight < 0) innerHeight = 0;

            // Position and size corner TextureRects
            northWest.Position = new Vector2(0, 0); northWest.Size = new Vector2(nwW, nwH);
            northEast!.Position = new Vector2(frameWidth - neW, 0); northEast.Size = new Vector2(neW, neH);
            southWest!.Position = new Vector2(0, frameHeight - swH); southWest.Size = new Vector2(swW, swH);
            southEast!.Position = new Vector2(frameWidth - seW, frameHeight - seH); southEast.Size = new Vector2(seW, seH);

            // Position and size horizontal (top/bottom) TextureRects
            north!.Position = new Vector2(nwW, 0); north.Size = new Vector2(innerWidth, nH);
            south!.Position = new Vector2(swW, frameHeight - nH); south.Size = new Vector2(innerWidth, nH);

            // Position and size vertical (left/right) TextureRects
            west!.Position = new Vector2(0, nwH); west.Size = new Vector2(wW, innerHeight);
            east!.Position = new Vector2(frameWidth - wW, neH); east.Size = new Vector2(wW, innerHeight);

            // Position and size center TextureRect
            center!.Position = new Vector2(wW, nH); center.Size = new Vector2(innerWidth, innerHeight);

            // Set the size of the parent Control node itself to match the layout
            CustomMinimumSize = new Vector2(frameWidth, frameHeight);
            Size = new Vector2(frameWidth, frameHeight);
        }
    }
}
