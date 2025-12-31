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

            Buff?.OnBattleReady();
            CharacterManager.Instance.RegisterPlayer(this);
            SetupAnimatorLayerWeight();

            IsBattleReady = true;
        }

        //

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent<StatNames, int>.Register(GlobalEventType.GAME_DATA_CHARACTER_ENHANCEMENT_LEVEL_CHANGED, OnEnhancementLevelChanged);
            GlobalEvent<CharacterGrowthTypes, int>.Register(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED, OnGrowthLevelChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent<StatNames, int>.Unregister(GlobalEventType.GAME_DATA_CHARACTER_ENHANCEMENT_LEVEL_CHANGED, OnEnhancementLevelChanged);
            GlobalEvent<CharacterGrowthTypes, int>.Unregister(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED, OnGrowthLevelChanged);
        }

        private void OnEnhancementLevelChanged(StatNames statName, int level)
        {
            if (statName == StatNames.None)
            {
                return;
            }

            Stat.RemoveBySourceInfo(statName, this, NameString, "CharacterEnhancement");

            if (level <= 0)
            {
                return;
            }

            EnhancementConfigData data = ScriptableDataManager.Instance.GetEnhancementData(statName);
            if (data == null)
            {
                Log.Warning(LogTags.Player, "강화 데이터가 존재하지 않습니다: {0}", statName);
                return;
            }

            float statValue = data.CalculateStatValue(level);
            Stat.AddWithSourceInfo(statName, statValue, this, NameString, "CharacterEnhancement");
        }

        private void OnGrowthLevelChanged(CharacterGrowthTypes growthType, int level)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                Log.Warning(LogTags.Player, "성장 타입 변환에 실패했습니다: {0}", growthType);
                return;
            }

            GrowthConfigData data = ScriptableDataManager.Instance.GetGrowthData(growthType);
            if (data == null)
            {
                Log.Warning(LogTags.Player, "성장 데이터가 존재하지 않습니다: {0}", growthType);
                return;
            }

            Stat.RemoveBySourceInfo(data.StatName, this, NameString, "CharacterGrowth");

            if (level <= 0)
            {
                return;
            }

            float statValue = data.CalculateStatValue(level);
            Stat.AddWithSourceInfo(data.StatName, statValue, this, NameString, "CharacterGrowth");
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

        //

        public override void SetupLevel()
        {
            base.SetupLevel();

            Level = ProfileInfo.Level.Level;
        }

        public void OnLevelup(int addedLevel)
        {
            SetupLevel();

            if (OnLevelUpFeedbacks != null)
            {
                OnLevelUpFeedbacks.PlayFeedbacks(position, 0);
            }

            SpawnLevelUpText(addedLevel);
        }

        public void OnLevelDown()
        {
            SetupLevel();
            MyVital?.OnLevelDown();
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

                // 강화 능력치 적용
                ApplyEnhanceStats();

                // 성장 능력치 적용
                ApplyGrowthStats();

                LogInfo("캐릭터 스탯이 스크립터블 데이터에서 적용되었습니다. 캐릭터: {0}, 레벨: {1}", Name, Level);
            }
        }

        private void ApplyBaseStats(PlayerCharacterStatConfigAsset asset)
        {
            if (!asset.IsValid()) return;
            Stat.AddWithSourceInfo(StatNames.Health, asset.BaseHealth, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Attack, asset.BaseAttack, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.HealthRegen, asset.BaseHealthRegen, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.AttackSpeed, asset.BaseAttackSpeed, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.CriticalChance, asset.BaseCriticalChance, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.CriticalDamage, asset.BaseCriticalDamage, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Mana, asset.BaseMana, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.ManaRegen, asset.BaseManaRegen, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Accuracy, asset.BaseAccuracy, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Dodge, asset.BaseDodge, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.GoldGain, asset.BaseGoldGain, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.XPGain, asset.BaseXPGain, this, NameString, "CharacterBase");
        }

        private void ApplyEnhanceStats()
        {
            StatNames[] enhanceStats = new StatNames[]
            {
                StatNames.Attack,
                StatNames.Health,
                StatNames.HealthRegen,
                StatNames.CriticalChance,
                StatNames.CriticalDamage,
                StatNames.DevastatingStrikeChance,
                StatNames.DevastatingStrike,
            };

            for (int i = 0; i < enhanceStats.Length; i++)
            {
                StatNames statName = enhanceStats[i];
                int level = ProfileInfo.Enhancement.GetLevel(enhanceStats[i]);
                if (level == 0) continue;

                EnhancementConfigData data = ScriptableDataManager.Instance.GetEnhancementData(enhanceStats[i]);
                float statValue = data.CalculateStatValue(level);

                Stat.AddWithSourceInfo(statName, statValue, this, NameString, "CharacterEnhancement");
            }
        }

        private void ApplyGrowthStats()
        {
            CharacterGrowthTypes[] growthTypes = EnumEx.GetValues<CharacterGrowthTypes>(true);
            for (int i = 0; i < growthTypes.Length; i++)
            {
                CharacterGrowthTypes type = growthTypes[i];
                int level = ProfileInfo.Growth.GetLevel(type);
                if (level == 0) continue;

                GrowthConfigData data = ScriptableDataManager.Instance.GetGrowthData(type);
                float statValue = data.CalculateStatValue(level);
                Stat.AddWithSourceInfo(data.StatName, statValue, this, NameString, "CharacterGrowth");
            }
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