namespace TeamSuneat
{
    /// <summary>
    /// Critical 관련 능력치 업데이트 전략
    /// CriticalChance, CriticalDamage를 처리합니다.
    /// </summary>
    public class CriticalUpdateStrategy : BaseStatUpdateStrategy
    {
        /// <summary>
        /// Critical 관련 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshCritical(System);
        }

        /// <summary>
        /// Critical 관련 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public override void OnRemove(StatNames statName, float value)
        {
            RefreshCritical(System);
        }

        private void RefreshCritical(StatSystem StatSystem)
        {
            float criticalChance = StatSystem.FindValueOrDefault(StatNames.CriticalChance);
            float criticalDamage = StatSystem.FindValueOrDefault(StatNames.CriticalDamage);
            
            LogStatUpdate(StatNames.CriticalChance, criticalChance);
            LogStatUpdate(StatNames.CriticalDamage, criticalDamage);

            // 치명타 관련 시스템 새로고침
            // 실제 구현은 치명타 계산 시스템에 따라 달라질 수 있습니다
        }
    }
}
