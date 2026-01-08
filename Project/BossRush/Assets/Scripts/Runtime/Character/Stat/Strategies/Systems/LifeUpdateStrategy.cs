namespace TeamSuneat
{
    /// <summary>
    /// Life 관련 능력치 업데이트 전략
    /// Life, HPRegen을 처리합니다.
    /// </summary>
    public class LifeUpdateStrategy : BaseStatUpdateStrategy
    {
        /// <summary>
        /// Life 관련 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshLife(System);
        }

        /// <summary>
        /// Life 관련 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public override void OnRemove(StatNames statName, float value)
        {
            RefreshLife(System);
        }

        /// <summary>
        /// Life 시스템을 새로고침합니다.
        /// </summary>

        private void RefreshLife(StatSystem statSystem)
        {
            if (statSystem.Owner.MyVital.Life != null)
            {
                LogRefresh("Life");
                statSystem.Owner.MyVital.Life.RefreshMaxValue();
            }
        }
    }
}
