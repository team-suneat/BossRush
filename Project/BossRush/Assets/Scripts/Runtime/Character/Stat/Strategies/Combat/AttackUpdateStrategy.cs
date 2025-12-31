namespace TeamSuneat
{
    /// <summary>
    /// Attack 관련 능력치 업데이트 전략
    /// Attack를 처리합니다.
    /// </summary>
    public class AttackUpdateStrategy : BaseStatUpdateStrategy
    {
        /// <summary>
        /// Attack 관련 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshAttack(System);
        }

        /// <summary>
        /// Attack 관련 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public override void OnRemove(StatNames statName, float value)
        {
            RefreshAttack(System);
        }

        private void RefreshAttack(StatSystem StatSystem)
        {
            float attack = StatSystem.FindValueOrDefault(StatNames.Attack);
            LogStatUpdate(StatNames.Attack, attack);
        }
    }
}
