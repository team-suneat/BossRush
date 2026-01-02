namespace TeamSuneat.Passive
{
    public class PassiveTriggerActivate : PassiveTriggerReceiver
    {
        protected override PassiveTriggers Trigger => PassiveTriggers.Activate;

        public override void Initialize()
        {
            base.Initialize();

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