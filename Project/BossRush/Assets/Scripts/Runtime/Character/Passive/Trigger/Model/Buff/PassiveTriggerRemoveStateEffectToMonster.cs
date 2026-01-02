using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public class PassiveTriggerRemoveStateEffectToMonster : PassiveTriggerReceiver
    {
        private Character _targetCharacter;
        private StateEffects _stateEffectName;

        protected override PassiveTriggers Trigger => PassiveTriggers.RemoveStateEffectToMonster;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<Character, StateEffects>.Register(GlobalEventType.MONSTER_CHARACTER_REMOVE_STATE_EFFECT, OnGlobalEvent);
            GlobalEvent<BuffNames, SID>.Register(GlobalEventType.MONSTER_CHARACTER_REMOVE_BUFF, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<Character, StateEffects>.Unregister(GlobalEventType.MONSTER_CHARACTER_REMOVE_STATE_EFFECT, OnGlobalEvent);
            GlobalEvent<BuffNames, SID>.Unregister(GlobalEventType.MONSTER_CHARACTER_REMOVE_BUFF, OnGlobalEvent);
        }

        private void OnGlobalEvent(BuffNames buffName, SID monsterSID)
        {
            Character monster = CharacterManager.Instance.FindMonster(monsterSID);
            if (monster == null) return;

            BuffAsset buffAsset = ScriptableDataManager.Instance.FindBuff(buffName);
            if (!buffAsset.IsValid()) return;
            if (buffAsset.Data.StateEffect == StateEffects.None) return;

            OnGlobalEvent(monster, buffAsset.Data.StateEffect);
        }

        private void OnGlobalEvent(Character monsterCharacter, StateEffects stateEffectName)
        {
            _stateEffectName = stateEffectName;
            _targetCharacter = monsterCharacter;

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
            if (_targetCharacter != null)
            {
                TriggerInfo.TargetCharacter = _targetCharacter;
                TriggerInfo.TargetVital = _targetCharacter.MyVital;
            }

            base.Execute();
        }
    }
}