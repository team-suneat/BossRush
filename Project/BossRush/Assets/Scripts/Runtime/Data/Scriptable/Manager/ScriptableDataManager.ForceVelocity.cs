using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 ForceVelocity 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region ForceVelocity Find Methods

        /// <summary>
        /// ForceVelocity 에셋을 찾습니다.
        /// </summary>
        public ForceVelocityAsset FindForceVelocity(FVNames key)
        {
            return FindForceVelocity(BitConvert.Enum32ToInt(key));
        }

        private ForceVelocityAsset FindForceVelocity(int tid)
        {
            return _forceVelocityAssets.ContainsKey(tid) ? _forceVelocityAssets[tid] : null;
        }

        #endregion ForceVelocity Find Methods

        #region ForceVelocity FindClone Methods

        /// <summary>
        /// ForceVelocity 데이터 클론을 찾습니다.
        /// </summary>
        public ForceVelocityAssetData FindForceVelocityClone(FVNames forceVelocityName)
        {
            if (forceVelocityName != FVNames.None)
            {
                ForceVelocityAssetData assetData = FindForceVelocityClone(BitConvert.Enum32ToInt(forceVelocityName));
                if (assetData == null)
                {
                    Log.Warning(LogTags.ScriptableData, "ForceVelocity 데이터를 찾을 수 없습니다. {0}", forceVelocityName.ToLogString());
                }

                return assetData;
            }

            return null;
        }

        public ForceVelocityAssetData FindForceVelocityClone(int forceVelocityTID)
        {
            if (_forceVelocityAssets.ContainsKey(forceVelocityTID))
            {
                return _forceVelocityAssets[forceVelocityTID].Clone();
            }
            else
            {
                return null;
            }
        }

        #endregion ForceVelocity FindClone Methods

        #region ForceVelocity Refresh Methods

        /// <summary>
        /// 모든 ForceVelocity 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllForceVelocity()
        {
            foreach (KeyValuePair<int, ForceVelocityAsset> item in _forceVelocityAssets) { Refresh(item.Value); }
        }

        private void Refresh(ForceVelocityAsset forceVelocityAsset)
        {
            if (forceVelocityAsset != null)
            {
                forceVelocityAsset.Refresh();
            }
        }

        #endregion ForceVelocity Refresh Methods
    }
}

