using Godot;
using System;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    namespace Layer
    {
        public enum Id
        {
            ZERO = 0,
            ONE,
            TWO,
            THREE,
            FOUR,
            FIVE,
            SIX,
            SEVEN
        };
    }

    public partial class TilesObjs : Node2D
    {
        private SortedDictionary<int, List<Tile>> tiles;
        private SortedDictionary<int, List<Obj>> objs;

        private const int TILE_RENDERING_OFFSET = 1000;
        public TilesObjs(Layer.Id layer, Wz_Node source)
        {
            tiles = [];
            objs = [];

            Wz_Node tileSet = source.FindNodeByPath(@"info\tS");
            if (tileSet != null)
            {
                foreach (Wz_Node tileNode in source.FindNodeByPath("tile").Nodes)
                {
                    Tile tile = new(tileNode, tileSet.GetValue<string>() + ".img");
                    int zIndex = tile.GetZIndex();
                    tile.ZIndex = zIndex + TILE_RENDERING_OFFSET;

                    if (!tiles.ContainsKey(zIndex))
                    {
                        tiles[zIndex] = [];
                    }
                    tiles[zIndex].Add(tile);
                    AddChild(tile);
                }
            }

            Wz_Node objectNode = source.FindNodeByPath("obj");
            if (objectNode != null)
            {
                foreach (Wz_Node objNode in objectNode.Nodes)
                {
                    Obj obj = new(objNode);
                    int zIndex = obj.GetZIndex();
                    obj.ZIndex = zIndex;

                    if (!objs.ContainsKey(zIndex))
                    {
                        objs[zIndex] = [];
                    }
                    objs[zIndex].Add(obj);
                    AddChild(obj);
                }
            }
        }
    }

    public partial class MapTilesObjs : Node2D
    {
        private EnumMap<Layer.Id, TilesObjs> layers = new();
        public void Init(Wz_Node source)
        {
            Stage stage = GetNode<Stage>("/root/Root/ViewportContainer/SubViewport/Stage");

            foreach (Layer.Id layer in Enum.GetValues(typeof(Layer.Id)))
            {
                Wz_Node layerNode = source.FindNodeByPath(((int)layer).ToString());

                if (layerNode != null)
                {
                    TilesObjs tileobj = new(layer, layerNode);
                    layers[layer] = tileobj;

                    stage.GetNode<CanvasLayer>($"Layer{(int)layer}/MapTilesObjs_0").AddChild(tileobj);
                }
            }
        }
    }
}