using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 플리커 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Flicker Get Methods

        /// <summary>
        /// 플리커 에셋을 가져옵니다.
        /// </summary>
        public FlickerAsset GetFlickerAsset(RendererFlickerNames flickerName)
        {
            int key = BitConvert.Enum32ToInt(flickerName);
            return _flickerAssets.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion Flicker Get Methods

        #region Flicker Find Methods

        /// <summary>
        /// 플리커 에셋을 찾습니다.
        /// </summary>
        public FlickerAsset FindFlicker(RendererFlickerNames key)
        {
            return FindFlicker(BitConvert.Enum32ToInt(key));
        }

        private FlickerAsset FindFlicker(int TID)
        {
            if (_flickerAssets.ContainsKey(TID))
            {
                return _flickerAssets[TID];
            }

            return null;
        }

        #endregion Flicker Find Methods

        #region Flicker Load Methods

        /// <summary>
        /// 플리커 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadFlickerSync(string filePath)
        {
            if (!filePath.Contains("Flicker_"))
            {
                return false;
            }

            FlickerAsset asset = ResourcesManager.LoadResource<FlickerAsset>(filePath);

            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 플리커 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_flickerAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 Flicker가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _flickerAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _flickerAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Flicker Load Methods

        #region Flicker Refresh Methods

        /// <summary>
        /// 모든 플리커 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllFlicker()
        {
            foreach (KeyValuePair<int, FlickerAsset> item in _flickerAssets) { Refresh(item.Value); }
        }

        private void Refresh(FlickerAsset flickerAsset)
        {
            flickerAsset?.Refresh();
        }

        #endregion Flicker Refresh Methods
    }
}