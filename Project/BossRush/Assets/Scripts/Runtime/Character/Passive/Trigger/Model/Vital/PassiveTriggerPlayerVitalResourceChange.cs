

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerVitalResourceChange : PassiveTriggerReceiver
    {
        private float _resourceRate;

        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerChangeVitalResource;

        public override void Activate()
        {
            _resourceRate = Owner.MyVital.ResourceRate;

            base.Activate();
        }

        protected override void Register()
        {
            base.Register();

            GlobalEvent<int, int>.Register(GlobalEventType.PLAYER_CHARACTER_CHANGE_VITAL_RESOURCE, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<int, int>.Unregister(GlobalEventType.PLAYER_CHARACTER_CHANGE_VITAL_RESOURCE, OnGlobalEvent);
        }

        private void OnGlobalEvent(int currentResource, int maxResource)
        {
            _resourceRate = currentResource.SafeDivide(maxResource);

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

            if (!Checker.CheckTriggerPercent(_triggerSettings.TriggerOperator, _triggerSettings.TriggerPercent, _resourceRate))
            {
                return false;
            }

            return CheckConditions();
        }
    }
}