

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerChangeLife : PassiveTriggerReceiver
    {
        private float _lifeRate;

        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerChangeLife;

        public override void Activate()
        {
            _lifeRate = Owner.MyVital.LifeRate;

            base.Activate();
        }

        protected override void Register()
        {
            base.Register();

            GlobalEvent<int, int>.Register(GlobalEventType.PLAYER_CHARACTER_REFRESH_LIFE, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<int, int>.Unregister(GlobalEventType.PLAYER_CHARACTER_REFRESH_LIFE, OnGlobalEvent);
        }

        private void OnGlobalEvent(int currentLife, int maxLife)
        {
            _lifeRate = currentLife.SafeDivide(maxLife);

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

            if (!Checker.CheckTriggerPercent(_triggerSettings.TriggerOperator, _triggerSettings.TriggerPercent, _lifeRate))
            {
                return false;
            }

            return CheckConditions();
        }
    }
}