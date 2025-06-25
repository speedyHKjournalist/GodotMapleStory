using Godot;
using System;
using System.Collections.Generic;

namespace MapleStory
{
    public partial class MapObjects : Node2D
    {
        private Dictionary<int, MapObject> objects = new Dictionary<int, MapObject>();
        private HashSet<int>[] layers = new HashSet<int>[Enum.GetValues(typeof(Layer.Id)).Length];
        private Physics? physics;
        private Stage? stage;
        public override void _Ready()
        {
            stage = GetNode<Stage>($"/root/Root/ViewportContainer/SubViewport/Stage");
            physics = stage.GetNode<Physics>("Physics");

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = [];
            }
        }

        private void OnMapObjectLayerChanged(int objectId, int oldLayer, int newLayer)
        {
            if (!objects.TryGetValue(objectId, out MapObject? mapObject))
            {
                return;
            }

            if (newLayer == -1)
            {
                layers[oldLayer].Remove(objectId);
                CanvasLayer? oldCanvas = stage?.GetNodeOrNull<CanvasLayer>($"Layer{oldLayer}/{GetParent<Node2D>().Name}");
                oldCanvas?.RemoveChild(mapObject);
                objects.Remove(objectId);
                mapObject.Free();
            }
            else if (newLayer != oldLayer)
            {
                layers[oldLayer].Remove(objectId);
                layers[newLayer].Add(objectId);

                CanvasLayer? oldCanvas = stage?.GetNodeOrNull<CanvasLayer>($"Layer{oldLayer}/{GetParent<Node2D>().Name}");
                CanvasLayer? newCanvas = stage?.GetNodeOrNull<CanvasLayer>($"Layer{newLayer}/{GetParent<Node2D>().Name}");

                oldCanvas?.RemoveChild(mapObject);
                newCanvas?.AddChild(mapObject);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                CanvasLayer? canvas = stage?.GetNode<CanvasLayer>($"Layer{i}/{GetParent<Node2D>().Name}");
                if (canvas != null)
                    for (int j = canvas.GetChildCount() - 1; j >= 0; j--)
                    {
                        Node child = canvas.GetChild(i);
                        RemoveChild(child);
                        child.Free();
                    }
            }

            objects.Clear();
            foreach (var layer in layers)
                layer.Clear();
        }

        public bool Contains(int objectId)
        {
            return objects.ContainsKey(objectId);
        }

        public void Add(MapObject toAdd)
        {
            int objectId = toAdd.GetObjectId();
            int layer = toAdd.GetLayer();

            objects[objectId] = toAdd;
            layers[layer].Add(objectId);

            toAdd.LayerChanged += OnMapObjectLayerChanged;
            stage?.GetNode<CanvasLayer>($"Layer{layer}/{GetParent<Node2D>().Name}").AddChild(toAdd);
        }

        public void Remove(int objectId)
        {
            if (objects.TryGetValue(objectId, out var mapObject))
            {
                mapObject.LayerChanged -= OnMapObjectLayerChanged;

                int layer = mapObject.GetLayer();
                stage?.GetNode<CanvasLayer>($"Layer{layer}/{GetParent<Node2D>().Name}").RemoveChild(mapObject);
                mapObject.Free();

                objects.Remove(objectId);
                layers[layer].Remove(objectId);
            }
        }

        public MapObject? Get(int objectId)
        {
            return objects.TryGetValue(objectId, out var mapObject) ? mapObject : null;
        }

        public int Size()
        {
            return objects.Count;
        }

        public Dictionary<int, MapObject> GetObjects()
        {
            return objects;
        }
    }
}