using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 사운드 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Sound Find Methods

        /// <summary>
        /// 사운드 에셋을 찾습니다.
        /// </summary>
        public SoundAsset FindSound(SoundNames key)
        {
            return FindSound(BitConvert.Enum32ToInt(key));
        }

        private SoundAsset FindSound(int tid)
        {
            return _soundAssets.ContainsKey(tid) ? _soundAssets[tid] : null;
        }

        #endregion Sound Find Methods

        #region Sound Load Methods

        /// <summary>
        /// 사운드 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadSoundSync(string filePath)
        {
            if (!filePath.Contains("Sound_"))
            {
                return false;
            }

            SoundAsset asset = ResourcesManager.LoadResource<SoundAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 사운드 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_soundAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 Sound가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.Name, _soundAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _soundAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Sound Load Methods

        #region Sound Refresh Methods

        /// <summary>
        /// 모든 사운드 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllSounds()
        {
            foreach (KeyValuePair<int, SoundAsset> item in _soundAssets) { Refresh(item.Value); }
        }

        private void Refresh(SoundAsset soundAsset)
        {
            soundAsset?.Refresh();
        }

        #endregion Sound Refresh Methods
    }
}
