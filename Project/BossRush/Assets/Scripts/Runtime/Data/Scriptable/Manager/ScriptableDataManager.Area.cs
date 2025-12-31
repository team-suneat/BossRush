using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 지역 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Area Find Methods

        /// <summary>
        /// 지역 에셋을 찾습니다.
        /// </summary>
        public AreaAsset FindArea(AreaNames key)
        {
            return FindArea(BitConvert.Enum32ToInt(key));
        }

        private AreaAsset FindArea(int tid)
        {
            if (_areaAssets.ContainsKey(tid))
            {
                return _areaAssets[tid];
            }

            return null;
        }

        #endregion Area Find Methods

        #region Area Load Methods

        /// <summary>
        /// 지역 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadAreaSync(string filePath)
        {
            if (!filePath.Contains("Area_"))
            {
                return false;
            }

            AreaAsset asset = ResourcesManager.LoadResource<AreaAsset>(filePath);
            if (asset != null)
            {
                int tid = BitConvert.Enum32ToInt(asset.AreaName);
                if (asset.AreaName == AreaNames.None)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 지역 이름이 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_areaAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 이름으로 중복 Area가 로드 되고 있습니다. Name: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.AreaName, _areaAssets[tid].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _areaAssets[tid] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Area Load Methods

        #region Area Refresh Methods

        /// <summary>
        /// 모든 지역 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllArea()
        {
            foreach (KeyValuePair<int, AreaAsset> item in _areaAssets) { Refresh(item.Value); }
        }

        private void Refresh(AreaAsset areaAsset)
        {
            areaAsset?.Refresh();
        }

        #endregion Area Refresh Methods
    }
}
