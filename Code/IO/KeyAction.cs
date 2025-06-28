using System;

namespace MapleStory
{
    namespace KeyAction
    {
        // Maple-specific keycodes, sent via the Keymap Packet.
        public enum Id : int
        {
            EQUIPMENT,
            ITEMS,
            STATS,
            SKILLS,
            FRIENDS,
            WORLDMAP,
            MAPLECHAT,
            MINIMAP,
            QUESTLOG,
            KEYBINDINGS,
            SAY,
            WHISPER,
            PARTYCHAT,
            FRIENDSCHAT,
            MENU,
            QUICKSLOTS,
            TOGGLECHAT,
            GUILD,
            GUILDCHAT,
            PARTY,
            NOTIFIER,
            MAPLENEWS,          // TOSPOUSE (v83)
            CASHSHOP,           // MONSTERBOOK (v83)
            ALLIANCECHAT,       // CASHSHOP (v83)
                                //NONE = 24,		// TOALLIANCE (v83)
            MANAGELEGION = 25,  // PARTYSEARCH (v83)
            MEDALS,             // FAMILY (v83)
            BOSSPARTY,          // MEDAL (v83)
            PROFESSION = 29,
            ITEMPOT,
            EVENT,
            SILENTCRUSADE = 33,
            BITS,
            BATTLEANALYSIS,
            GUIDE = 39,
            VIEWERSCHAT,
            ENHANCEEQUIP,
            MONSTERCOLLECTION,
            SOULWEAPON,
            CHARINFO,
            CHANGECHANNEL,
            MAINMENU,
            SCREENSHOT,
            PICTUREMODE,
            MAPLEACHIEVEMENT,
            PICKUP,
            SIT,
            ATTACK,
            JUMP,
            INTERACT_HARVEST,
            FACE1 = 100,
            FACE2,
            FACE3,
            FACE4,
            FACE5,
            FACE6,
            FACE7,
            MAPLESTORAGE = 200,
            SAFEMODE,
            MUTE,
            EMOTICON,
            MAPLERELAY = 300,
            FAMILIAR = 1000,
            TOSPOUSE,
            // Static keys
            LEFT,
            RIGHT,
            UP,
            DOWN,
            BACK,
            TAB,
            RETURN,
            ESCAPE,
            SPACE,
            DELETE,
            HOME,
            END,
            COPY,
            PASTE,
            LENGTH
        };
    }
}