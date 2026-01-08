namespace TeamSuneat
{
    /// <summary>
    /// Mana 관련 능력치 업데이트 전략
    /// Mana를 처리합니다.
    /// </summary>
    public class ManaUpdateStrategy : BaseStatUpdateStrategy
    {
        /// <summary>
        /// Mana 관련 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshMana(System);
        }

        /// <summary>
        /// Mana 관련 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public override void OnRemove(StatNames statName, float value)
        {
            RefreshMana(System);
        }

        /// <summary>
        /// Mana 시스템을 새로고침합니다.
        /// </summary>

        private void RefreshMana(StatSystem StatSystem)
        {
            if (StatSystem.Owner.MyVital.Mana != null)
            {
                LogRefresh("Mana");
                StatSystem.Owner.MyVital.Mana.RefreshMaxValue();
            }
        }
    }
}

