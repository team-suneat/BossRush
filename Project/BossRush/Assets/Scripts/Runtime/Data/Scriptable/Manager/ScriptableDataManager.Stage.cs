using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 스테이지 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Stage Find Methods

        /// <summary>
        /// 스테이지 에셋을 찾습니다.
        /// </summary>
        public StageAsset FindStage(StageNames key)
        {
            return FindStage(BitConvert.Enum32ToInt(key));
        }

        private StageAsset FindStage(int tid)
        {
            if (_stageAssets.ContainsKey(tid))
            {
                return _stageAssets[tid];
            }

            return null;
        }

        #endregion Stage Find Methods

        #region Stage Load Methods

        /// <summary>
        /// 스테이지 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadStageSync(string filePath)
        {
            if (!filePath.Contains("Stage_"))
            {
                return false;
            }

            StageAsset asset = ResourcesManager.LoadResource<StageAsset>(filePath);
            if (asset != null)
            {
                int tid = BitConvert.Enum32ToInt(asset.Name);
                if (asset.Name == StageNames.None)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 스테이지 이름이 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_stageAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 이름으로 중복 Stage가 로드 되고 있습니다. Name: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.Name, _stageAssets[tid].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _stageAssets[tid] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Stage Load Methods

        #region Stage Refresh Methods

        /// <summary>
        /// 모든 스테이지 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllStage()
        {
            foreach (KeyValuePair<int, StageAsset> item in _stageAssets) { Refresh(item.Value); }
        }

        private void Refresh(StageAsset stageAsset)
        {
            stageAsset?.Refresh();
        }

        #endregion Stage Refresh Methods
    }
}
