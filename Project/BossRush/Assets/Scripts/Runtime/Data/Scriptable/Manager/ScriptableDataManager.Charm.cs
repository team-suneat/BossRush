using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 참 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Charm Find Methods

        /// <summary>
        /// 참 에셋을 찾습니다.
        /// </summary>
        public CharmAsset FindCharm(CharmName key)
        {
            return FindCharm(BitConvert.Enum32ToInt(key));
        }

        private CharmAsset FindCharm(int tid)
        {
            if (_charmAssets.ContainsKey(tid))
            {
                return _charmAssets[tid];
            }

            return null;
        }

        #endregion Charm Find Methods

        #region Charm FindClone Methods

        /// <summary>
        /// 참 데이터 클론을 찾습니다.
        /// </summary>
        public CharmAssetData FindCharmClone(CharmName charmName)
        {
            if (charmName != CharmName.None)
            {
                CharmAssetData assetData = FindCharmClone(BitConvert.Enum32ToInt(charmName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "참 데이터를 찾을 수 없습니다. {0}({1})", charmName, charmName.ToLogString());
                }
                return assetData;
            }

            return new CharmAssetData();
        }

        public CharmAssetData FindCharmClone(int charmTID)
        {
            if (_charmAssets.ContainsKey(charmTID))
            {
                return _charmAssets[charmTID].CreateDataClone();
            }

#if UNITY_EDITOR
            CharmName charmName = charmTID.ToEnum<CharmName>();
            Log.Warning(LogTags.ScriptableData, "참 데이터를 찾을 수 없습니다. {0}({1})", charmName, charmName.ToLogString());
#endif

            return new CharmAssetData();
        }

        #endregion Charm FindClone Methods

        #region Charm Refresh Methods

        /// <summary>
        /// 모든 참 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllCharm()
        {
            foreach (KeyValuePair<int, CharmAsset> item in _charmAssets) { Refresh(item.Value); }
        }

        private void Refresh(CharmAsset charmAsset)
        {
            charmAsset?.Refresh();
        }

        #endregion Charm Refresh Methods

        #region Charm Validation Methods

        /// <summary>
        /// 참 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidCharmsOnLoadAssets()
        {
#if UNITY_EDITOR
            CharmName[] keys = EnumEx.GetValues<CharmName>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_charmAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "참 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Charm Validation Methods
    }
}
