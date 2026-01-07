namespace TeamSuneat
{
    public static class GameLayers
    {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int Ignore_Raycast = 2;
        public const int Water = 4;
        public const int UI = 5;
        
        public const int Background = 6;
        public const int Foreground = 7;
        public const int Collision = 8;
        
        public const int Player = 10;
        public const int Enemies = 11;
        public const int Friendlies = 12;
        public const int InteractionObject = 13;
        
        public const int Attackable = 20;
        public const int Detectable = 21;
        public const int Breakable = 22;
        

        public static class Mask
        {
            public const int Default = 1 << GameLayers.Default;
            public const int TransparentFX = 1 << GameLayers.TransparentFX;
            public const int Ignore_Raycast = 1 << GameLayers.Ignore_Raycast;
            public const int Water = 1 << GameLayers.Water;
            public const int UI = 1 << GameLayers.UI;
            public const int Background = 1 << GameLayers.Background;
            public const int Foreground = 1 << GameLayers.Foreground;
            public const int Collision = 1 << GameLayers.Collision;
            public const int Player = 1 << GameLayers.Player;
            public const int Enemies = 1 << GameLayers.Enemies;
            public const int Friendlies = 1 << GameLayers.Friendlies;
            public const int InteractionObject = 1 << GameLayers.InteractionObject;
            public const int Detectable = 1 << GameLayers.Detectable;
            public const int Breakable = 1 << GameLayers.Breakable;
        }
    }
}