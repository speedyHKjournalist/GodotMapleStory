using System.Collections.Generic;

namespace MapleStory
{
    namespace Stance
    {
        public enum Id : int
        {
            NONE = 0,
            ALERT,
            DEAD,
            FLY,
            HEAL,
            JUMP,
            LADDER,
            PRONE,
            PRONESTAB,
            ROPE,
            SHOT,
            SHOOT1,
            SHOOT2,
            SHOOTF,
            SIT,
            STABO1,
            STABO2,
            STABOF,
            STABT1,
            STABT2,
            STABTF,
            STAND1,
            STAND2,
            SWINGO1,
            SWINGO2,
            SWINGO3,
            SWINGOF,
            SWINGP1,
            SWINGP2,
            SWINGPF,
            SWINGT1,
            SWINGT2,
            SWINGT3,
            SWINGTF,
            WALK1,
            WALK2,
            LENGTH
        }

        public static class StanceUtils
        {
            public static readonly Dictionary<Id, string> Names = new Dictionary<Id, string>
        {
            { Id.NONE, "" },
            { Id.ALERT, "alert" },
            { Id.DEAD, "dead" },
            { Id.FLY, "fly" },
            { Id.HEAL, "heal" },
            { Id.JUMP, "jump" },
            { Id.LADDER, "ladder" },
            { Id.PRONE, "prone" },
            { Id.PRONESTAB, "proneStab" },
            { Id.ROPE, "rope" },
            { Id.SHOT, "shot" },
            { Id.SHOOT1, "shoot1" },
            { Id.SHOOT2, "shoot2" },
            { Id.SHOOTF, "shootF" },
            { Id.SIT, "sit" },
            { Id.STABO1, "stabO1" },
            { Id.STABO2, "stabO2" },
            { Id.STABOF, "stabOF" },
            { Id.STABT1, "stabT1" },
            { Id.STABT2, "stabT2" },
            { Id.STABTF, "stabTF" },
            { Id.STAND1, "stand1" },
            { Id.STAND2, "stand2" },
            { Id.SWINGO1, "swingO1" },
            { Id.SWINGO2, "swingO2" },
            { Id.SWINGO3, "swingO3" },
            { Id.SWINGOF, "swingOF" },
            { Id.SWINGP1, "swingP1" },
            { Id.SWINGP2, "swingP2" },
            { Id.SWINGPF, "swingPF" },
            { Id.SWINGT1, "swingT1" },
            { Id.SWINGT2, "swingT2" },
            { Id.SWINGT3, "swingT3" },
            { Id.SWINGTF, "swingTF" },
            { Id.WALK1, "walk1" },
            { Id.WALK2, "walk2" }
        };

            public static Id ByString(string name)
            {
                foreach (var pair in Names)
                {
                    if (pair.Value == name)
                        return pair.Key;
                }
                return Id.NONE;
            }

            public static Id ByState(int state)
            {
                int index = (state / 2) - 1;
                if (index < 0 || index > 9)
                {
                    return Id.WALK1;
                }
                Id[] stateValues =
                [
                    Id.WALK1,
                Id.STAND1,
                Id.JUMP,
                Id.ALERT,
                Id.PRONE,
                Id.FLY,
                Id.LADDER,
                Id.ROPE,
                Id.DEAD,
                Id.SIT
                ];
                return stateValues[index];
            }

            public static Id ById(int id)
            {
                if (id <= (int)Id.NONE || id >= (int)Id.LENGTH)
                {
                    return Id.NONE;
                }

                return (Id)id;
            }

            public static Id BaseOf(Id value)
            {
                switch (value)
                {
                    case Id.STAND2:
                        return Id.STAND1;
                    case Id.WALK2:
                        return Id.WALK1;
                    default:
                        return value;
                }
            }

            public static bool IsClimbing(Id value)
            {
                return value == Id.LADDER || value == Id.ROPE;
            }
        }
    }
}