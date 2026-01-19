using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 버프 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Buff Find Methods

        /// <summary>
        /// 버프 에셋을 찾습니다.
        /// </summary>
        public BuffAsset FindBuff(BuffName key)
        {
            return FindBuff(BitConvert.Enum32ToInt(key));
        }

        private BuffAsset FindBuff(int tid)
        {
            if (_buffAssets.ContainsKey(tid))
            {
                return _buffAssets[tid];
            }

            return null;
        }

        #endregion Buff Find Methods

        #region Buff FindClone Methods

        /// <summary>
        /// 버프 데이터 클론을 찾습니다.
        /// </summary>
        public BuffAssetData FindBuffClone(BuffName buffName)
        {
            if (buffName != BuffName.None)
            {
                BuffAssetData assetData = FindBuffClone(BitConvert.Enum32ToInt(buffName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "버프 데이터를 찾을 수 없습니다. {0}({1})", buffName, buffName.ToLogString());
                }
                return assetData;
            }

            return new BuffAssetData();
        }

        public BuffAssetData FindBuffClone(int buffTID)
        {
            if (_buffAssets.ContainsKey(buffTID))
            {
                return _buffAssets[buffTID].CreateDataClone();
            }

#if UNITY_EDITOR
            BuffName buffName = buffTID.ToEnum<BuffName>();
            Log.Warning(LogTags.ScriptableData, "버프 데이터를 찾을 수 없습니다. {0}({1})", buffName, buffName.ToLogString());
#endif

            return new BuffAssetData();
        }

        #endregion Buff FindClone Methods

        #region Buff Load Methods

        /// <summary>
        /// 버프 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadBuffSync(string filePath)
        {
            if (!filePath.Contains("Buff_"))
            {
                return false;
            }

            BuffAsset asset = ResourcesManager.LoadResource<BuffAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 버프 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_buffAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 Buff가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _buffAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _buffAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Buff Load Methods

        #region Buff Refresh Methods

        /// <summary>
        /// 모든 버프 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllBuff()
        {
            foreach (KeyValuePair<int, BuffAsset> item in _buffAssets) { Refresh(item.Value); }
        }

        private void Refresh(BuffAsset buffAsset)
        {
            buffAsset?.Refresh();
        }

        #endregion Buff Refresh Methods

        #region Buff Validation Methods

        /// <summary>
        /// 버프 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidBuffsOnLoadAssets()
        {
#if UNITY_EDITOR
            BuffName[] keys = EnumEx.GetValues<BuffName>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_buffAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "버프 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Buff Validation Methods
    }
}