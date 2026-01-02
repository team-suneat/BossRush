

namespace TeamSuneat.Passive
{
    public class PassiveTriggerMoveToStage : PassiveTriggerReceiver
    {
        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerMoveToStage;

        protected override void Register()
        {
            base.Register();

            GlobalEvent.Register(GlobalEventType.MOVE_TO_STAGE, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_STAGE, OnGlobalEvent);
        }

        private void OnGlobalEvent()
        {
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

            return CheckConditions();
        }
    }
}