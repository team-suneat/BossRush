

namespace TeamSuneat.Passive
{
    public class PassiveTriggerLevelUpPlayer : PassiveTriggerReceiver
    {
        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerLevelUp;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<int>.Register(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<int>.Unregister(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, OnGlobalEvent);
        }

        private void OnGlobalEvent(int addLevel)
        {
            for (int i = 0; i < addLevel; i++)
            {
                if (TryExecute())
                {
                    Execute();
                }
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