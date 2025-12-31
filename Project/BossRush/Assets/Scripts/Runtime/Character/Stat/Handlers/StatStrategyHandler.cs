using System.Collections.Generic;

namespace TeamSuneat
{
    /// <summary>
    /// 능력치 전략 관리 핸들러
    /// 전략 등록, 관리, 실행을 담당합니다.
    /// </summary>
    public class StatStrategyHandler
    {
        #region Fields

        private readonly StatEventHandler _eventHandler;
        private readonly StatLogHandler _logHandler;
        private readonly Dictionary<StatNames, BaseStatUpdateStrategy> _strategies = new();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// StatStrategyHandler 생성자
        /// </summary>
        /// <param name="eventHandler">이벤트 핸들러</param>
        /// <param name="logHandler">로깅 핸들러</param>
        public StatStrategyHandler(StatEventHandler eventHandler, StatLogHandler logHandler)
        {
            _eventHandler = eventHandler;
            _logHandler = logHandler;
        }

        #endregion Constructor

        #region Strategy Management

        /// <summary>
        /// 모든 전략을 초기화합니다.
        /// </summary>
        /// <param name="system">StatSystem 인스턴스</param>
        public void InitializeStrategies(StatSystem system)
        {
            RegisterSystemStrategies();
            RegisterCombatStrategies();
            RegisterPassiveStrategies();
            RegisterSpecialStrategies();

            // 모든 전략에 System 참조 설정
            foreach (KeyValuePair<StatNames, BaseStatUpdateStrategy> strategy in _strategies)
            {
                strategy.Value.System = system;
            }
        }

        /// <summary>
        /// 전략이 존재하는지 확인합니다.
        /// </summary>
        /// <param name="statName">능력치 이름</param>
        /// <returns>전략 존재 여부</returns>
        public bool HasStrategy(StatNames statName)
        {
            return _strategies.ContainsKey(statName);
        }

        /// <summary>
        /// 전략을 조회합니다.
        /// </summary>
        /// <param name="statName">능력치 이름</param>
        /// <returns>전략 인스턴스 (없으면 null)</returns>
        public BaseStatUpdateStrategy GetStrategy(StatNames statName)
        {
            return _strategies.TryGetValue(statName, out BaseStatUpdateStrategy strategy) ? strategy : null;
        }

        #endregion Strategy Management

        #region Strategy Execution

        /// <summary>
        /// 능력치 추가 처리를 실행합니다.
        /// </summary>
        /// <param name="system">StatSystem 인스턴스</param>
        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public void ProcessAdd(StatNames statName, float value)
        {
            _eventHandler.CallRefreshEvent(statName, value);

            if (_strategies.TryGetValue(statName, out BaseStatUpdateStrategy strategy))
            {
                strategy.OnAdd(statName, value);
            }

            _eventHandler.CallRefreshedEvent(statName, value);
        }

        /// <summary>
        /// 능력치 제거 처리를 실행합니다.
        /// </summary>
        /// <param name="system">StatSystem 인스턴스</param>
        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public void ProcessRemove(StatNames statName, float value)
        {
            _eventHandler.CallRefreshEvent(statName, value * -1);

            if (_strategies.TryGetValue(statName, out BaseStatUpdateStrategy strategy))
            {
                strategy.OnRemove(statName, value);
            }

            _eventHandler.CallRefreshedEvent(statName, value * -1);
        }

        #endregion Strategy Execution

        #region Strategy Registration

        /// <summary>
        /// 시스템 전략들을 등록합니다.
        /// </summary>
        private void RegisterSystemStrategies()
        {
            _strategies[StatNames.Health] = new HealthUpdateStrategy();
            _strategies[StatNames.Attack] = new AttackUpdateStrategy();
            _strategies[StatNames.AttackSpeed] = new AttackSpeedUpdateStrategy();
            _strategies[StatNames.CriticalChance] = new CriticalUpdateStrategy();
            _strategies[StatNames.CriticalDamage] = new CriticalUpdateStrategy();
        }

        /// <summary>
        /// 전투 전략들을 등록합니다.
        /// </summary>
        private void RegisterCombatStrategies()
        {
        }

        /// <summary>
        /// 패시브 전략들을 등록합니다.
        /// </summary>
        private void RegisterPassiveStrategies()
        {
        }

        /// <summary>
        /// 특수 전략들을 등록합니다.
        /// </summary>
        private void RegisterSpecialStrategies()
        {
        }

        #endregion Strategy Registration
    }
}