using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 스킬 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Skill Find Methods

        /// <summary>
        /// 스킬 에셋을 찾습니다.
        /// </summary>
        public SkillAsset FindSkill(SkillNames key)
        {
            return FindSkill(BitConvert.Enum32ToInt(key));
        }

        private SkillAsset FindSkill(int tid)
        {
            return _skillAssets.ContainsKey(tid) ? _skillAssets[tid] : null;
        }

        public SkillAssetData FindSkillClone(SkillNames key)
        {
            return FindSkillClone(BitConvert.Enum32ToInt(key));
        }

        private SkillAssetData FindSkillClone(int tid)
        {
            SkillAsset asset = FindSkill(tid);
            if (asset != null)
            {
                return asset.Data;
            }

            return null;
        }

        #endregion Skill Find Methods

        #region SkillAnimation Find Methods

        /// <summary>
        /// 스킬 애니메이션 에셋 목록을 찾습니다.
        /// </summary>
        public List<SkillAnimationAsset> FindSkillAnimations(SkillNames skillName)
        {
            return FindSkillAnimations(BitConvert.Enum32ToInt(skillName));
        }

        private List<SkillAnimationAsset> FindSkillAnimations(int tid)
        {
            if (_skillAnimationAssets.ContainsKey(tid))
            {
                return _skillAnimationAssets[tid];
            }

            return new List<SkillAnimationAsset>();
        }

        #endregion SkillAnimation Find Methods

        #region Skill Load Methods

        /// <summary>
        /// 스킬 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadSkillSync(string filePath)
        {
            if (!filePath.Contains("Skill_"))
            {
                return false;
            }

            SkillAsset asset = ResourcesManager.LoadResource<SkillAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 스킬 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_skillAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 스킬이 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _skillAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _skillAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Skill Load Methods

        #region SkillAnimation Load Methods

        /// <summary>
        /// 스킬 애니메이션 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadSkillAnimationSync(string filePath)
        {
            if (!filePath.Contains("SkillAnimation_"))
            {
                return false;
            }

            SkillAnimationAsset asset = ResourcesManager.LoadResource<SkillAnimationAsset>(filePath);
            if (asset != null)
            {
                int tid = BitConvert.Enum32ToInt(asset.SkillName);
                if (tid == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 스킬 애니메이션의 스킬 이름이 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else
                {
                    if (!_skillAnimationAssets.ContainsKey(tid))
                    {
                        _skillAnimationAssets[tid] = new List<SkillAnimationAsset>();
                    }

                    _skillAnimationAssets[tid].Add(asset);
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion SkillAnimation Load Methods

        #region Skill Refresh Methods

        /// <summary>
        /// 모든 스킬 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllSkill()
        {
            foreach (KeyValuePair<int, SkillAsset> item in _skillAssets) { Refresh(item.Value); }
        }

        private void Refresh(SkillAsset skillAsset)
        {
            skillAsset?.Refresh();
        }

        #endregion Skill Refresh Methods

        #region SkillAnimation Refresh Methods

        /// <summary>
        /// 모든 스킬 애니메이션 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllSkillAnimation()
        {
            foreach (KeyValuePair<int, List<SkillAnimationAsset>> item in _skillAnimationAssets)
            {
                foreach (SkillAnimationAsset asset in item.Value)
                {
                    Refresh(asset);
                }
            }
        }

        private void Refresh(SkillAnimationAsset skillAnimationAsset)
        {
            skillAnimationAsset?.Refresh();
        }

        #endregion SkillAnimation Refresh Methods

        #region SkillCardUnlock Get Methods

        /// <summary>
        /// 스킬 카드 해금 에셋을 가져옵니다.
        /// </summary>
        public SkillCardUnlockAsset GetSkillCardUnlockAsset()
        {
            return _skillCardUnlockAsset;
        }

        #endregion SkillCardUnlock Get Methods

        #region SkillSlotUnlock Get Methods

        /// <summary>
        /// 스킬 슬롯 해금 에셋을 가져옵니다.
        /// </summary>
        public SkillSlotUnlockAsset GetSkillSlotUnlockAsset()
        {
            return _skillSlotUnlockAsset;
        }

        #endregion SkillSlotUnlock Get Methods
    }
}