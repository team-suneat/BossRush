

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerAddStat : PassiveTriggerReceiver
    {
        private StatNames _statName;

        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerAddStat;

        public override void Activate()
        {
            if (_triggerSettings.TriggerStats.IsValidArray())
            {
                _statName = _triggerSettings.TriggerStats[0];
            }

            base.Activate();
        }

        protected override void Register()
        {
            base.Register();

            GlobalEvent<StatNames>.Register(GlobalEventType.PLAYER_CHARACTER_ADD_STAT, OnGlobalEventAddStat);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<StatNames>.Unregister(GlobalEventType.PLAYER_CHARACTER_ADD_STAT, OnGlobalEventAddStat);
        }

        private void OnGlobalEventAddStat(StatNames statName)
        {
            _statName = statName;

            if (TryExecute())
            {
                Execute();
            }
        }

        public override bool TryExecute()
        {
            if (!base.TryExecute())
            {
                return false;
            }

            if (!Checker.CheckTriggerStat(_statName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerStatOperator())
            {
                return false;
            }

            return CheckConditions();
        }
    }
}