

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerBarrierCharge : PassiveTriggerReceiver
    {
        private float _barrierRate;

        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerBarrierCharge;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<int, int>.Register(GlobalEventType.PLAYER_CHARACTER_BARRIER_CHARGE, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<int, int>.Unregister(GlobalEventType.PLAYER_CHARACTER_BARRIER_CHARGE, OnGlobalEvent);
        }

        private void OnGlobalEvent(int currentBarrier, int maxBarrier)
        {
            _barrierRate = currentBarrier.SafeDivide(maxBarrier);

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

            if (!Checker.CheckTriggerPercent(_triggerSettings.TriggerOperator, _triggerSettings.TriggerPercent, _barrierRate))
            {
                return false;
            }

            return CheckConditions();
        }
    }
}