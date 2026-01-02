

namespace TeamSuneat.Passive
{
    public class PassiveTriggerRemoveBuffToMonster : PassiveTriggerReceiver
    {
        private Character _targetCharacter;
        private BuffNames _buffName;

        protected override PassiveTriggers Trigger => PassiveTriggers.RemoveBuffToMonster;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<BuffNames, SID>.Register(GlobalEventType.MONSTER_CHARACTER_REMOVE_BUFF, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<BuffNames, SID>.Unregister(GlobalEventType.MONSTER_CHARACTER_REMOVE_BUFF, OnGlobalEvent);
        }

        private void OnGlobalEvent(BuffNames buffName, SID ownerSID)
        {
            _buffName = buffName;

            _targetCharacter = CharacterManager.Instance.FindMonster(ownerSID);

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
            else if (!Checker.CheckContainsTriggerBuffType(_targetCharacter, _buffName))
            {
                return false;
            }
            else if (!Checker.CheckContainsTriggerMonsterCountStateEffects())
            {
                return false;
            }

            return CheckConditions(_targetCharacter);
        }

        public override void Execute()
        {
            if (_targetCharacter != null)
            {
                TriggerInfo.TargetCharacter = _targetCharacter;
                TriggerInfo.TargetVital = _targetCharacter.MyVital;
            }

            base.Execute();
        }
    }
}