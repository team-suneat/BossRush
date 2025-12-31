namespace TeamSuneat
{
    public enum PassiveTriggerChanceCalcType
    {
        None = 0,         // 100%
        Fixed,            // TriggerChance + TriggerChanceByLevel
        FromStat,         // TriggerChanceByStat
        FromOptionRange   // TriggerChanceOptionMin/MaxRange 기반
    }
}