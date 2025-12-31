using System;

namespace TeamSuneat
{
    [Flags]
    public enum BattleStageSlotFlags
    {
        None = 0,
        First = 1 << 0, // 1
        Second = 1 << 1, // 2
        Third = 1 << 2, // 4
        Fourth = 1 << 3, // 8
    }
}
