using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 카메라 임펄스 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region CameraImpulse Get Methods

        /// <summary>
        /// 카메라 임펄스 에셋을 가져옵니다.
        /// </summary>
        public CameraImpulseAsset GetCameraImpulseAsset(GameImpulseType impulseName)
        {
            int key = BitConvert.Enum32ToInt(impulseName);
            return _cameraImpulseAssets.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion CameraImpulse Get Methods

        #region CameraImpulse Find Methods

        /// <summary>
        /// 카메라 임펄스 에셋을 찾습니다.
        /// </summary>
        public CameraImpulseAsset FindCameraImpulse(GameImpulseType key)
        {
            return FindCameraImpulse(BitConvert.Enum32ToInt(key));
        }

        private CameraImpulseAsset FindCameraImpulse(int TID)
        {
            if (_cameraImpulseAssets.ContainsKey(TID))
            {
                return _cameraImpulseAssets[TID];
            }

            return null;
        }

        #endregion CameraImpulse Find Methods

        #region CameraImpulse Load Methods

        /// <summary>
        /// 카메라 임펄스 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadCameraImpulseSync(string filePath)
        {
            if (!filePath.Contains("CameraImpulse_"))
            {
                return false;
            }

            CameraImpulseAsset asset = ResourcesManager.LoadResource<CameraImpulseAsset>(filePath);

            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 카메라 임펄스 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_cameraImpulseAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 CameraImpulse가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _cameraImpulseAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _cameraImpulseAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion CameraImpulse Load Methods

        #region CameraImpulse Refresh Methods

        /// <summary>
        /// 모든 카메라 임펄스 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllCameraImpulse()
        {
            foreach (KeyValuePair<int, CameraImpulseAsset> item in _cameraImpulseAssets) { Refresh(item.Value); }
        }

        private void Refresh(CameraImpulseAsset asset)
        {
            asset?.Refresh();
        }

        #endregion CameraImpulse Refresh Methods
    }
}
