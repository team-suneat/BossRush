

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerUseVitalResource : PassiveTriggerReceiver
    {
        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerUseVitalResource;

        private int _usedResource;

        public override void Activate()
        {
            base.Activate();

            _usedResource = 0;
        }

        protected override void Register()
        {
            base.Register();

            GlobalEvent<int>.Register(GlobalEventType.PLAYER_CHARACTER_USE_VITAL_RESOURCE, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<int>.Unregister(GlobalEventType.PLAYER_CHARACTER_USE_VITAL_RESOURCE, OnGlobalEvent);
        }

        private void OnGlobalEvent(int useResource)
        {
            _usedResource += useResource;

            if (TryExecute())
            {
                Execute();
            }
        }

        public override bool TryExecute()
        {
            if (!Checker.CheckTriggerVitalResource(CharacterManager.Instance.Player, _usedResource))
            {
                return false;
            }
            else if (!base.TryExecute())
            {
                return false;
            }

            return CheckConditions();
        }

        public override void Execute()
        {
            base.Execute();

            _usedResource = 0;
        }
    }
}