using UnityEngine;

namespace TeamSuneat
{
    public enum GameTags
    {
        Untagged = 0,
        LevelBounds,
        Music,
        Background,
        NoMask,
        Ground,
        Through,
        Ladder,
        Interaction,
        Character,
        Vital,
    }

    public static class GameTagComparer
    {
        public static bool CompareTag(this Component component, GameTags gameTag)
        {
            return component.CompareTag(gameTag.ToString());
        }

        public static bool CompareTag(this GameObject component, GameTags gameTag)
        {
            return component.CompareTag(gameTag.ToString());
        }
    }
}