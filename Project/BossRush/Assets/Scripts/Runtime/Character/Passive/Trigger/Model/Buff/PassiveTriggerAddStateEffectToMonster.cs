

namespace TeamSuneat.Passive
{
    public class PassiveTriggerAddStateEffectToMonster : PassiveTriggerReceiver
    {
        private StateEffects _stateEffectName;

        protected override PassiveTriggers Trigger => PassiveTriggers.AddStateEffectToMonster;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<StateEffects>.Register(GlobalEventType.MONSTER_CHARACTER_ADD_STATE_EFFECT, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<StateEffects>.Unregister(GlobalEventType.MONSTER_CHARACTER_ADD_STATE_EFFECT, OnGlobalEvent);
        }

        private void OnGlobalEvent(StateEffects stateEffectName)
        {
            _stateEffectName = stateEffectName;

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

            if (!Checker.CheckContainsTriggerStateEffect(Owner, _stateEffectName))
            {
                return false;
            }
            else if (!Checker.CheckContainsTriggerMonsterCountStateEffects())
            {
                return false;
            }

            return CheckConditions();
        }

        public override void Execute()
        {
            if (Owner != null)
            {
                TriggerInfo.TargetCharacter = Owner;
                TriggerInfo.TargetVital = Owner.MyVital;
            }

            base.Execute();
        }
    }
}