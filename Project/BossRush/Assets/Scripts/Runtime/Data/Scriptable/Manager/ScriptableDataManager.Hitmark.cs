using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 히트마크 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Hitmark Find Methods

        /// <summary>
        /// 히트마크 에셋을 찾습니다.
        /// </summary>
        public HitmarkAsset FindHitmark(HitmarkNames key)
        {
            return FindHitmark(BitConvert.Enum32ToInt(key));
        }

        private HitmarkAsset FindHitmark(int tid)
        {
            if (_hitmarkAssets.ContainsKey(tid))
            {
                return _hitmarkAssets[tid];
            }

            return null;
        }

        #endregion Hitmark Find Methods

        #region Hitmark FindClone Methods

        /// <summary>
        /// 히트마크 데이터 클론을 찾습니다.
        /// </summary>
        public HitmarkAssetData FindHitmarkClone(HitmarkNames hitmarkName)
        {
            if (hitmarkName != HitmarkNames.None)
            {
                HitmarkAssetData assetData = FindHitmarkClone(BitConvert.Enum32ToInt(hitmarkName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "히트마크 데이터를 찾을 수 없습니다. {0}({1})", hitmarkName, hitmarkName.ToLogString());
                }
                return assetData;
            }

            return new HitmarkAssetData();
        }

        public HitmarkAssetData FindHitmarkClone(int hitmarkTID)
        {
            if (_hitmarkAssets.ContainsKey(hitmarkTID))
            {
                return _hitmarkAssets[hitmarkTID].CreateDataClone();
            }

#if UNITY_EDITOR
            HitmarkNames hitmarkName = hitmarkTID.ToEnum<HitmarkNames>();
            Log.Warning(LogTags.ScriptableData, "히트마크 데이터를 찾을 수 없습니다. {0}({1})", hitmarkName, hitmarkName.ToLogString());
#endif

            return new HitmarkAssetData();
        }

        #endregion Hitmark FindClone Methods

        #region Hitmark Load Methods

        /// <summary>
        /// 히트마크 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadHitmarkSync(string filePath)
        {
            if (!filePath.Contains("Hitmark_"))
            {
                return false;
            }

            HitmarkAsset asset = ResourcesManager.LoadResource<HitmarkAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 히트마크 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_hitmarkAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 Hitmark가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _hitmarkAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _hitmarkAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Hitmark Load Methods

        #region Hitmark Refresh Methods

        /// <summary>
        /// 모든 히트마크 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllHitmark()
        {
            foreach (KeyValuePair<int, HitmarkAsset> item in _hitmarkAssets) { Refresh(item.Value); }
        }

        private void Refresh(HitmarkAsset hitmarkAsset)
        {
            hitmarkAsset?.Refresh();
        }

        #endregion Hitmark Refresh Methods

        #region Hitmark Validation Methods

        /// <summary>
        /// 히트마크 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidHitmarksOnLoadAssets()
        {
#if UNITY_EDITOR
            HitmarkNames[] keys = EnumEx.GetValues<HitmarkNames>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_hitmarkAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "히트마크 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Hitmark Validation Methods
    }
}