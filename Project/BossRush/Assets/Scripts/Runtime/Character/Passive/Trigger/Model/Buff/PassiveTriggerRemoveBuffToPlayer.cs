

namespace TeamSuneat.Passive
{
    public class PassiveTriggerRemoveBuffToPlayer : PassiveTriggerReceiver
    {
        private BuffNames _buffName;
        protected override PassiveTriggers Trigger => PassiveTriggers.RemoveBuffToPlayer;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<BuffNames>.Register(GlobalEventType.PLAYER_CHARACTER_REMOVE_BUFF, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<BuffNames>.Unregister(GlobalEventType.PLAYER_CHARACTER_REMOVE_BUFF, OnGlobalEvent);
        }

        private void OnGlobalEvent(BuffNames buffName)
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

            if (!Checker.CheckContainsTriggerBuffType(CharacterManager.Instance.Player, _buffName))
            {
                return false;
            }

            return CheckConditions(CharacterManager.Instance.Player);
        }

        public override void Execute()
        {
            if (CharacterManager.Instance.Player != null)
            {
                TriggerInfo.TargetCharacter = CharacterManager.Instance.Player;
                TriggerInfo.TargetVital = CharacterManager.Instance.Player.MyVital;

                base.Execute();
            }
            else
            {
                Log.Error("플레이어 캐릭터를 찾을 수 없어, 플레이어 캐릭터 버프 삭제시 발동하는 패시브를 발동할 수 없습니다. {0}", TriggerInfo.Name.ToLogString());
            }
        }
    }
}