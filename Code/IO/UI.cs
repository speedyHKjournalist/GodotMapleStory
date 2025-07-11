using Godot;
using System.Collections.Generic;

namespace MapleStory
{
    public partial class UI : Node2D
    {
        private Dictionary<string, Mapping> actionMap = [];
        private Stage? stage;

        public override void _Ready()
        {
            stage = GetNode<Stage>("../Stage");
            //test key mapping, can be saved or loaded from a config file
            actionMap["jump"] = new Mapping(KeyType.Id.ACTION, KeyAction.Id.JUMP);
            actionMap["attack"] = new Mapping(KeyType.Id.ACTION, KeyAction.Id.ATTACK);
            actionMap["pickup"] = new Mapping(KeyType.Id.ACTION, KeyAction.Id.PICKUP);
            actionMap["move_left"] = new Mapping(KeyType.Id.ACTION, KeyAction.Id.LEFT);
            actionMap["move_right"] = new Mapping(KeyType.Id.ACTION, KeyAction.Id.RIGHT);
            actionMap["move_up"] = new Mapping(KeyType.Id.ACTION, KeyAction.Id.UP);
            actionMap["move_down"] = new Mapping(KeyType.Id.ACTION, KeyAction.Id.DOWN);
        }

        public override void _Input(InputEvent @event)
        {
            foreach (var entry in actionMap)
            {
                string godotActionName = entry.Key;
                Mapping mapping = entry.Value;

                if (@event.IsActionPressed(godotActionName))
                {
                    stage?.SendKey(mapping.type, mapping.action, true);
                    GetViewport().SetInputAsHandled();
                }
                else if (@event.IsActionReleased(godotActionName))
                {
                    stage?.SendKey(mapping.type, mapping.action, false);
                    GetViewport().SetInputAsHandled();
                }
            }
        }
    }
}
