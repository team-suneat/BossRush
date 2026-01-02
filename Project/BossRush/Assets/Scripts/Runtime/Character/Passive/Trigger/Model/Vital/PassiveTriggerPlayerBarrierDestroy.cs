

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerBarrierDestroy : PassiveTriggerReceiver
    {
        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerBarrierDestroy;

        protected override void Register()
        {
            base.Register();

            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BARRIER_DESTROY, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BARRIER_DESTROY, OnGlobalEvent);
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