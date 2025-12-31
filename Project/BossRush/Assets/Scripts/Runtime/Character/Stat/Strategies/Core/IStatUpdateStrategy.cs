namespace TeamSuneat
{
    /// <summary>
    /// 능력치 업데이트 전략 인터페이스
    /// </summary>
    public interface IStatUpdateStrategy
    {
        StatSystem System { get; set; }

        /// <summary>
        /// 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        void OnAdd(StatNames statName, float value);

        /// <summary>
        /// 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        void OnRemove(StatNames statName, float value);
    }
}