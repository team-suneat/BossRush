using TeamSuneat.Data;

namespace TeamSuneat
{
    public class PlayerCharacter : Character
    {
        public override LogTags LogTag => LogTags.Player;

        public override void OnDespawn()
        {
            base.OnDespawn();
            GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_DESPAWNED);
        }

        public override void Initialize()
        {
            base.Initialize();
            SetupLevel();
        }

        public override void BattleReady()
        {
            base.BattleReady();

            CharacterManager.Instance.RegisterPlayer(this);
            SetupAnimatorLayerWeight();

            IsBattleReady = true;
        }

        //

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
        }

        //

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            if (!ActiveSelf)
            {
                return;
            }

            base.PhysicsUpdate();
        }

        private void SpawnLevelUpText(int addedLevel)
        {
            if (addedLevel == 0)
            {
                return;
            }

            string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_LEVEL_UP);
            string content = string.Format(format, addedLevel);

            ResourcesManager.SpawnFloatyText(content, true, transform);
        }

        public override void AddCharacterStats()
        {
            PlayerCharacterStatConfigAsset asset = ScriptableDataManager.Instance.GetPlayerCharacterStatAsset();
            if (asset != null)
            {
                // 기본 능력치 적용
                ApplyBaseStats(asset);

                LogInfo("캐릭터 스탯이 스크립터블 데이터에서 적용되었습니다. 캐릭터: {0}, 레벨: {1}", Name, Level);
            }
        }

        private void ApplyBaseStats(PlayerCharacterStatConfigAsset asset)
        {
            if (!asset.IsValid()) return;
            Stat.AddWithSourceInfo(StatNames.Life, asset.BaseLife, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Attack, asset.BaseAttack, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.AttackSpeed, asset.BaseAttackSpeed, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Mana, asset.BaseMana, this, NameString, "CharacterBase");
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.UnregisterPlayer(this);

            GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_DEATH);
        }
    }
}