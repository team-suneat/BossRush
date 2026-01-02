

namespace TeamSuneat.Passive
{
    public class PassiveTriggerRestoreMonsterVitalResource : PassiveTriggerReceiver
    {
        private float _vitalResourceRate;

        protected override PassiveTriggers Trigger => PassiveTriggers.RestoreMonsterVitalResource;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<SID, float>.Register(GlobalEventType.MONSTER_CHARACTER_RESTORE_VITAL_RESOURCE, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<SID, float>.Unregister(GlobalEventType.MONSTER_CHARACTER_RESTORE_VITAL_RESOURCE, OnGlobalEvent);
        }

        private void OnGlobalEvent(SID SID, float vitalResourceRate)
        {
            if (Owner.SID != SID)
            {
                return;
            }

            _vitalResourceRate = vitalResourceRate;

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

            if (!Checker.CheckTriggerPercent(_triggerSettings.TriggerOperator, _triggerSettings.TriggerPercent, _vitalResourceRate))
            {
                return false;
            }

            return CheckConditions();
        }
    }
}