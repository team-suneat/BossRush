using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 플로티 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Floaty Get Methods

        /// <summary>
        /// 플로티 에셋을 가져옵니다.
        /// </summary>
        public FloatyAsset GetFloatyAsset(UIFloatyMoveNames floatyName)
        {
            int key = BitConvert.Enum32ToInt(floatyName);
            return _floatyAssets.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion Floaty Get Methods

        #region Floaty Find Methods

        /// <summary>
        /// 플로티 에셋을 찾습니다.
        /// </summary>
        public FloatyAsset FindFloaty(UIFloatyMoveNames key)
        {
            return FindFloaty(BitConvert.Enum32ToInt(key));
        }

        private FloatyAsset FindFloaty(int TID)
        {
            if (_floatyAssets.ContainsKey(TID))
            {
                return _floatyAssets[TID];
            }

            return null;
        }

        #endregion Floaty Find Methods

        #region Floaty Load Methods

        /// <summary>
        /// 플로티 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadFloatySync(string filePath)
        {
            if (!filePath.Contains("Floaty_"))
            {
                return false;
            }

            FloatyAsset asset = ResourcesManager.LoadResource<FloatyAsset>(filePath);

            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 플로티 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_floatyAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 Floaty가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _floatyAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _floatyAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Floaty Load Methods

        #region Floaty Refresh Methods

        /// <summary>
        /// 모든 플로티 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllFloaty()
        {
            foreach (KeyValuePair<int, FloatyAsset> item in _floatyAssets) { Refresh(item.Value); }
        }

        private void Refresh(FloatyAsset floatyAsset)
        {
            floatyAsset?.Refresh();
        }

        #endregion Floaty Refresh Methods
    }
}