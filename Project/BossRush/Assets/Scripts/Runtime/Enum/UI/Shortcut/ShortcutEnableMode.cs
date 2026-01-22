using System;

namespace TeamSuneat.UserInterface
{
    [Flags]
    public enum ShortcutEnableMode
    {
        None = 0,
        Keyboard = 1 << 0,
        Mouse = 1 << 1,
        Joystick = 1 << 2,
        All = Keyboard | Mouse | Joystick
    }
}