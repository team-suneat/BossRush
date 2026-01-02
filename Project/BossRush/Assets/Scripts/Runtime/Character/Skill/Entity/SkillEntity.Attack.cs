using Lean.Pool;


namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        /// <summary>
        /// 공격 히트마크를 스폰합니다.
        /// </summary>
        public void SpawnAttackEntity()
        {
            if (!_skillData.ActivateHitmarkWithoutAnimation)
            {
                return;
            }

            if (!_skillData.Hitmarks.IsValidArray())
            {
                return;
            }

            for (int i = 0; i < _skillData.Hitmarks.Length; i++)
            {
                if (!Owner.Attack.ContainEntity(_skillData.Hitmarks[i]))
                {
                    _ = Owner.Attack.SpawnAndRegisterEntity(_skillData.Hitmarks[i]);
                }
            }
        }

        /// <summary>
        /// 공격 엔티티를 활성화합니다.
        /// </summary>
        private void ActivateAttackEntities()
        {
            if (_skillData == null) { return; }
            if (_skillData.Hitmarks == null) { return; }

            for (int i = 0; i < _skillData.Hitmarks.Length; i++)
            {
                LogInfoHitmarkActivated(_skillData.Hitmarks[i].ToLogString());
                Owner.Attack.Activate(_skillData.Hitmarks[i]);
            }
        }

        /// <summary>
        /// 공격 엔티티를 비활성화합니다.
        /// </summary>
        private void DeactivateAttackEntities()
        {
            if (_skillData == null) { return; }
            if (_skillData.Hitmarks == null) { return; }

            for (int i = 0; i < _skillData.Hitmarks.Length; i++)
            {
                LogInfoHitmarkDeactivated(_skillData.Hitmarks[i].ToLogString());
                Owner.Attack.Deactivate(_skillData.Hitmarks[i]);
            }
        }

        /// <summary>
        /// 공격 엔티티의 시각 효과 피드백을 강제로 제거합니다.
        /// </summary>
        public void SetForceDespawnVisualEffectFeedback()
        {
            if (_skillData == null) { return; }
            if (_skillData.Hitmarks == null) { return; }

            for (int i = 0; i < _skillData.Hitmarks.Length; i++)
            {
                LogInfoForceDespawnVFX(_skillData.Hitmarks[i].ToLogString());

                AttackEntity attackEntity = Owner.Attack.FindEntity(_skillData.Hitmarks[i]);
                if (attackEntity != null)
                {
                    attackEntity.AttackStartTSFeedback?.SetForceDespawnVFX();
                }
            }
        }

        private void LogInfoHitmarkActivated(string hitmark)
        {
            if (Log.LevelInfo)
            {
                LogInfo("공격 히트마크를 활성화합니다. {0}", hitmark);
            }
        }

        private void LogInfoHitmarkDeactivated(string hitmark)
        {
            if (Log.LevelInfo)
            {
                LogInfo("공격 히트마크를 비활성화합니다. {0}", hitmark);
            }
        }

        private void LogInfoForceDespawnVFX(string hitmark)
        {
            if (Log.LevelInfo)
            {
                LogInfo("공격 엔티티의 시각 효과 피드백을 강제로 제거합니다. {0}", hitmark);
            }
        }
    }
}