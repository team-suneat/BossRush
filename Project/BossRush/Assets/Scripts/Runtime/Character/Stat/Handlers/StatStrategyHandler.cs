using System.Collections.Generic;

namespace TeamSuneat
{
    public class StatStrategyHandler
    {
        #region Fields

        private readonly StatEventHandler _eventHandler;
        private readonly StatLogHandler _logHandler;
        private readonly Dictionary<StatNames, BaseStatUpdateStrategy> _strategies = new();

        #endregion Fields

        #region Constructor

        public StatStrategyHandler(StatEventHandler eventHandler, StatLogHandler logHandler)
        {
            _eventHandler = eventHandler;
            _logHandler = logHandler;
        }

        #endregion Constructor

        #region Strategy Management

        public void InitializeStrategies(StatSystem system)
        {
            RegisterSystemStrategies();
            RegisterCombatStrategies();
            RegisterSpecialStrategies();

            // 모든 전략에 System 참조 설정
            foreach (KeyValuePair<StatNames, BaseStatUpdateStrategy> strategy in _strategies)
            {
                strategy.Value.System = system;
            }
        }

        public bool HasStrategy(StatNames statName)
        {
            return _strategies.ContainsKey(statName);
        }

        public bool TryGetStrategy(StatNames statName, out BaseStatUpdateStrategy strategy)
        {
            return _strategies.TryGetValue(statName, out strategy);
        }

        #endregion Strategy Management

        #region Strategy Execution

        public void ProcessAdd(StatNames statName, float value)
        {
            _eventHandler.CallRefreshEvent(statName, value);

            if (_strategies.TryGetValue(statName, out BaseStatUpdateStrategy strategy))
            {
                strategy.OnAdd(statName, value);
            }

            _eventHandler.CallRefreshedEvent(statName, value);
        }

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

        private void RegisterSystemStrategies()
        {
            _strategies[StatNames.Life] = new LifeUpdateStrategy();
            _strategies[StatNames.Mana] = new ManaUpdateStrategy();
            _strategies[StatNames.Pulse] = new PulseUpdateStrategy();
            _strategies[StatNames.PulseRegen] = new PulseRegenUpdateStrategy();
        }

        private void RegisterCombatStrategies()
        {
            _strategies[StatNames.Attack] = new AttackUpdateStrategy();
            _strategies[StatNames.AttackSpeed] = new AttackSpeedUpdateStrategy();
            _strategies[StatNames.AttackRange] = new AttackRangeUpdateStrategy();

            _strategies[StatNames.MoveSpeed] = new MoveSpeedUpdateStrategy();
            _strategies[StatNames.MoveSpeedMulti] = new MoveSpeedUpdateStrategy();
        }

        private void RegisterSpecialStrategies()
        {
        }

        #endregion Strategy Registration
    }
}