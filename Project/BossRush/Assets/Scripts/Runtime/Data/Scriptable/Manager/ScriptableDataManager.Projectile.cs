using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 발사체 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Projectile Get Methods

        /// <summary>
        /// 발사체 에셋을 가져옵니다.
        /// </summary>
        public ProjectileAsset GetProjectileAsset(ProjectileNames projectileName)
        {
            int key = BitConvert.Enum32ToInt(projectileName);
            return _projectileAssets.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion Projectile Get Methods

        #region Projectile Find Methods

        /// <summary>
        /// 발사체 에셋을 찾습니다.
        /// </summary>
        public ProjectileAsset FindProjectile(ProjectileNames key)
        {
            return FindProjectile(BitConvert.Enum32ToInt(key));
        }

        private ProjectileAsset FindProjectile(int tid)
        {
            if (_projectileAssets.ContainsKey(tid))
            {
                return _projectileAssets[tid];
            }

            return null;
        }

        #endregion Projectile Find Methods

        #region Projectile FindClone Methods

        /// <summary>
        /// 발사체 데이터 클론을 찾습니다.
        /// </summary>
        public ProjectileAssetData FindProjectileClone(ProjectileNames projectileName)
        {
            if (projectileName != ProjectileNames.None)
            {
                ProjectileAssetData assetData = FindProjectileClone(BitConvert.Enum32ToInt(projectileName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "발사체 데이터를 찾을 수 없습니다. {0}", projectileName.ToLogString());
                }

                return assetData;
            }

            return new ProjectileAssetData();
        }

        public ProjectileAssetData FindProjectileClone(int projectileTID)
        {
            if (_projectileAssets.ContainsKey(projectileTID))
            {
                return _projectileAssets[projectileTID].Clone();
            }
            else
            {
                return new ProjectileAssetData();
            }
        }

        #endregion Projectile FindClone Methods

        #region Projectile Load Methods

        /// <summary>
        /// 발사체 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadProjectileSync(string filePath)
        {
            if (!filePath.Contains("Projectile_"))
            {
                return false;
            }

            ProjectileAsset asset = ResourcesManager.LoadResource<ProjectileAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 발사체 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_projectileAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 발사체가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _projectileAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _projectileAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Projectile Load Methods

        #region Projectile Refresh Methods

        /// <summary>
        /// 모든 발사체 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllProjectile()
        {
            foreach (KeyValuePair<int, ProjectileAsset> item in _projectileAssets) { Refresh(item.Value); }
        }

        private void Refresh(ProjectileAsset projectileAsset)
        {
            projectileAsset?.Refresh();
        }

        #endregion Projectile Refresh Methods

        #region Projectile Validation Methods

        /// <summary>
        /// 발사체 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidProjectilesOnLoadAssets()
        {
#if UNITY_EDITOR
            ProjectileNames[] keys = EnumEx.GetValues<ProjectileNames>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_projectileAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "발사체 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Projectile Validation Methods
    }
}

