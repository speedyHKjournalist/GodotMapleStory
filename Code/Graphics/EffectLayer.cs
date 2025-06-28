using Godot;
using System.Collections.Generic;

namespace MapleStory
{
    // A list of animations. Animations will be removed after all frames were displayed.
    public partial class EffectLayer : Node2D
    {
        private partial class Effect(MapleAnimation animation, DrawArgument args, float speed) : Node2D
        {
            private MapleSprite sprite = new(animation, args);
            private float speed = speed;

            public override void _Ready()
            {
                sprite.SetSpeed(speed);
                AddChild(sprite);
            }

            public bool IsAnimationEnd()
            {
                return sprite.IsAnimationEnd();
            }

            public void Interpolate(MaplePoint<int> position)
            {
                sprite.Interpolate(position);
            }
        }

        private SortedDictionary<int, List<Effect>> effects = [];

        public void Interpolate(MaplePoint<int> position)
        {
            foreach (var entry in effects)
            {
                foreach (Effect effect in entry.Value)
                {
                    effect.Interpolate(position);
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            foreach (var entry in effects)
            {
                entry.Value.RemoveAll(effect => {
                    bool finished = effect.IsAnimationEnd();
                    if (finished)
                        effect.QueueFree();
                    return finished;
                });
            }

            List<int> emptyZIndexes = [];
            foreach (var entry in effects)
            {
                if (entry.Value.Count == 0)
                    emptyZIndexes.Add(entry.Key);
            }

            foreach (int zIndex in emptyZIndexes)
                effects.Remove(zIndex);
        }

        public void AddEffect(MapleAnimation animationTemplate, DrawArgument args, int z, float speed)
        {
            if (!effects.ContainsKey(z))
                effects[z] = [];

            int finalZIndex = (z < 0) ? z : z + 5;

            Effect newEffect = new(animationTemplate, args, speed) { ZIndex = finalZIndex };
            effects[finalZIndex].Add(newEffect);
            AddChild(newEffect);
        }

        public void AddEffect(MapleAnimation animationTemplate, DrawArgument args, int z)
        {
            AddEffect(animationTemplate, args, z, 1.0f);
        }

        public void AddEffect(MapleAnimation animationTemplate, DrawArgument args)
        {
            AddEffect(animationTemplate, args, 0, 1.0f);
        }

        public void AddEffect(MapleAnimation animationTemplate)
        {
            AddEffect(animationTemplate, new DrawArgument(), 0, 1.0f);
        }
    }
}