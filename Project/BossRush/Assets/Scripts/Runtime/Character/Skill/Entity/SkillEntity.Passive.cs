using Lean.Pool;


namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        #region 스킬 패시브 (Skill Passive)

        /// <summary>
        /// 패시브 설정을 확인합니다.
        /// </summary>
        /// <returns>패시브 설정 여부</returns>
        private bool CheckPassiveSetting()
        {
            for (int i = 0; i < _skillData.Passives.Length; i++)
            {
                if (_skillData.Passives[i] == PassiveNames.None)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 패시브를 추가합니다.
        /// </summary>
        private void AddPassives()
        {
            if (_skillData.Passives.IsValidArray())
            {
                for (int i = 0; i < _skillData.Passives.Length; i++)
                {
                    Owner.Passive.Add(_skillData.Passives[i], Level);
                }
            }
        }

        /// <summary>
        /// 패시브를 제거합니다.
        /// </summary>
        public void RemovePassives()
        {
            if (_skillData.Passives.IsValidArray())
            {
                for (int i = 0; i < _skillData.Passives.Length; i++)
                {
                    Owner.Passive.Remove(_skillData.Passives[i]);
                }
            }
        }

        /// <summary>
        /// 중복되지 않는 패시브를 제거합니다.
        /// </summary>
        private void RemoveOverlapPassives()
        {
            if (_skillData.OverlapPassives.IsValidArray())
            {
                for (int i = 0; i < _skillData.OverlapPassives.Length; i++)
                {
                    Owner.Passive.Remove(_skillData.OverlapPassives[i]);
                }
            }
        }

        #endregion 스킬 패시브 (Skill Passive)
    }
}