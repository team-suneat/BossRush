

namespace TeamSuneat.Passive
{
    public class PassiveTriggerAddBuffToPlayer : PassiveTriggerReceiver
    {
        private BuffNames _buffName;

        protected override PassiveTriggers Trigger => PassiveTriggers.AddBuffToPlayer;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<BuffNames, int>.Register(GlobalEventType.PLAYER_CHARACTER_ADD_BUFF, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<BuffNames, int>.Unregister(GlobalEventType.PLAYER_CHARACTER_ADD_BUFF, OnGlobalEvent);
        }

        private void OnGlobalEvent(BuffNames buffName, int buffLevel)
        {
            _buffName = buffName;

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
            else if (!Checker.CheckContainsTriggerBuffType(CharacterManager.Instance.Player, _buffName))
            {
                return false;
            }

            return CheckConditions(CharacterManager.Instance.Player);
        }

        public override void Execute()
        {
            TriggerInfo.BuffName = _buffName;

            if (CharacterManager.Instance.Player != null)
            {
                TriggerInfo.TargetCharacter = CharacterManager.Instance.Player;
                TriggerInfo.TargetVital = CharacterManager.Instance.Player.MyVital;
                TriggerInfo.Stack = CharacterManager.Instance.Player.Buff.GetStack(_buffName);

                base.Execute();
            }
            else
            {
                Log.Error("플레이어 캐릭터를 찾을 수 없어, 플레이어 캐릭터 버프 추가시 발동하는 패시브를 발동할 수 없습니다. {0}", TriggerInfo.Name.ToLogString());
            }
        }
    }
}