

namespace TeamSuneat.Passive
{
    public class PassiveTriggerAddBuffToMonster : PassiveTriggerReceiver
    {
        private BuffNames _buffName;
        private Character _targetCharacter;
        protected override PassiveTriggers Trigger => PassiveTriggers.AddBuffToMonster;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<BuffNames, int, SID>.Register(GlobalEventType.MONSTER_CHARACTER_ADD_BUFF, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<BuffNames, int, SID>.Unregister(GlobalEventType.MONSTER_CHARACTER_ADD_BUFF, OnGlobalEvent);
        }

        private void OnGlobalEvent(BuffNames buffName, int buffLevel, SID ownerSID)
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
            else if (!Checker.CheckTriggerBuffType(_buffName))
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
            TriggerInfo.BuffName = _buffName;
            if (_targetCharacter != null)
            {
                TriggerInfo.TargetCharacter = _targetCharacter;
                TriggerInfo.TargetVital = _targetCharacter.MyVital;
                TriggerInfo.Stack = _targetCharacter.Buff.GetStack(_buffName);

                base.Execute();
            }
            else if (Log.LevelWarning)
            {
                Log.Warning(LogTags.PassiveTrigger, "목표 캐릭터를 찾을 수 없어 패시브({0})를 발동할 수 없습니다.", Entity.Name.ToLogString());
            }
        }
    }
}