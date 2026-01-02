using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public class PassiveTriggerAddBuffStackToPlayer : PassiveTriggerReceiver
    {
        private BuffNames _buffName;
        private StateEffects _stateEffectName;
        private Character _targetCharacter;
        private int _stack;
        private int _maxStack;
        protected override PassiveTriggers Trigger => PassiveTriggers.AddBuffStackToPlayer;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<BuffNames, int, int>.Register(GlobalEventType.PLAYER_CHARACTER_ADD_BUFF_STACK, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<BuffNames, int, int>.Unregister(GlobalEventType.PLAYER_CHARACTER_ADD_BUFF_STACK, OnGlobalEvent);
        }

        private void OnGlobalEvent(BuffNames buffName, int buffStack, int buffMaxStack)
        {
            _buffName = buffName;
            _targetCharacter = CharacterManager.Instance.Player;
            _stack = buffStack;
            _maxStack = buffMaxStack;

            BuffAssetData assetData = ScriptableDataManager.Instance.FindBuffClone(buffName);
            if (assetData.IsValid())
            {
                _stateEffectName = assetData.StateEffect;
            }
            else
            {
                _stateEffectName = StateEffects.None;
            }

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
            else if (!Checker.CheckContainsTriggerStateEffect(_targetCharacter, _stateEffectName))
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
                TriggerInfo.TargetCharacter = _targetCharacter;
                TriggerInfo.TargetVital = _targetCharacter.MyVital;
            }

            base.Execute();
        }
    }
}