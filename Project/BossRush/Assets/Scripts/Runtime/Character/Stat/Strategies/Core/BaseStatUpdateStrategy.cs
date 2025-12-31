namespace TeamSuneat
{
    /// <summary>
    /// 능력치 업데이트 전략의 기본 구현 클래스
    /// </summary>
    public abstract class BaseStatUpdateStrategy : IStatUpdateStrategy
    {
        public StatSystem System { get; set; }

        #region 이벤트

        /// <summary>
        /// 능력치가 추가될 때 호출됩니다.
        /// 기본 구현은 아무것도 하지 않습니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public virtual void OnAdd(StatNames statName, float value)
        {
            // 기본 구현 (대부분의 경우 변경 없음)
        }

        /// <summary>
        /// 능력치가 제거될 때 호출됩니다.
        /// 기본 구현은 아무것도 하지 않습니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public virtual void OnRemove(StatNames statName, float value)
        {
            // 기본 구현 (대부분의 경우 변경 없음)
        }

        #endregion 이벤트

        #region Log

        protected string GetOwnerName(StatSystem statSystem)
        {
            string ownerName = statSystem?.Owner?.Name.ToLogString();
            if (string.IsNullOrEmpty(ownerName))
            {
                return "None";
            }

            return ownerName;
        }

        /// <summary>
        /// 능력치 업데이트를 로깅합니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">변경될 값</param>
        protected void LogStatUpdate(StatNames statName, float value)
        {
            if (Log.LevelInfo)
            {
                string ownerName = GetOwnerName(System);
                Log.Info(LogTags.Stat, "(System) {0}의 능력치({1})의 변경 값({2}) 적용에 따라 다른 능력치를 추가합니다.",
                    ownerName,
                    statName.ToLogString(),
                    ValueStringEx.GetValueString(value, true));
            }
        }

        /// <summary>
        /// 시스템 새로고침을 로깅합니다.
        /// </summary>

        /// <param name="systemName">시스템 이름</param>
        protected void LogRefresh(string systemName)
        {
            if (Log.LevelInfo)
            {
                string ownerName = GetOwnerName(System);
                Log.Info(LogTags.Stat, "(System) {0}, 능력치를 갱신합니다: {1}", ownerName, systemName);
            }
        }

        #endregion Log
    }
}