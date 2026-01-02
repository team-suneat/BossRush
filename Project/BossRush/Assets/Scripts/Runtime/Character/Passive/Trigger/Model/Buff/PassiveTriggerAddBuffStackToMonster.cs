

namespace TeamSuneat.Passive
{
    public class PassiveTriggerAddBuffStackToMonster : PassiveTriggerReceiver
    {
        private BuffNames _buffName;
        private Character _targetCharacter;
        private int _stack;
        private int _maxStack;
        protected override PassiveTriggers Trigger => PassiveTriggers.AddBuffStackToMonster;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<BuffNames, int, int, SID>.Register(GlobalEventType.MONSTER_CHARACTER_ADD_BUFF_STACK, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<BuffNames, int, int, SID>.Unregister(GlobalEventType.MONSTER_CHARACTER_ADD_BUFF_STACK, OnGlobalEvent);
        }

        private void OnGlobalEvent(BuffNames buffName, int buffStack, int buffMaxStack, SID monsterSID)
        {
            _buffName = buffName;
            _targetCharacter = CharacterManager.Instance.FindMonster(monsterSID);
            _stack = buffStack;
            _maxStack = buffMaxStack;

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

            if (!Checker.CheckTriggerBuff(_buffName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerBuffType(_buffName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerRestTime())
            {
                return false;
            }
            else if (!Checker.CheckTriggerMaxStack(_stack, _maxStack))
            {
                return false;
            }

            return CheckConditions(_targetCharacter);
        }

        public override void Execute()
        {
            if (_targetCharacter != null)
            {
                TriggerInfo.TargetVital = _targetCharacter.MyVital;
                TriggerInfo.TargetCharacter = _targetCharacter;
            }

            base.Execute();
        }
    }
}