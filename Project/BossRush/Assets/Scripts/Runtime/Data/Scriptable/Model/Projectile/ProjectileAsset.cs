using UnityEditor;
using UnityEngine;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "TeamSuneat/Scriptable/Projectile")]
    public class ProjectileAsset : XScriptableObject
    {
        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public ProjectileNames Name => Data.Name;

        public ProjectileAssetData Data;

        public override void OnLoadData()
        {
            base.OnLoadData();

            LogError();
            EnumLog();

            Data.OnLoadData();
        }

        private void LogError()
        {
#if UNITY_EDITOR

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }
            if (Data.Name == ProjectileNames.None)
            {
                Log.Warning(LogTags.ScriptableData, "[Projectile] Projectile Asset의 Name 변수가 설정되지 않았습니다. {0}", name);
            }

#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "ProjectileAsset".ToSelectString();
            // 필요시 EnumExplorer 로그 추가
#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                _ = EnumEx.ConvertTo(ref Data.Name, NameString);
            }

            Data.Validate();
        }

        public override void Refresh()
        {
            NameString = Data.Name.ToString();
            Data.Refresh();

            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Data.Name);
            if (Data.RefreshWithoutSave())
            {
                _hasChangedWhiteRefreshAll = true;
            }

            _ = base.RefreshWithoutSave();

            return _hasChangedWhiteRefreshAll;
        }

        public override void Rename()
        {
            Rename("Projectile");
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif
            ProjectileNames[] projectileNames = EnumEx.GetValues<ProjectileNames>();
            int projectileCount = 0;

            Log.Info("모든 발사체 에셋의 갱신을 시작합니다: {0}", projectileNames.Length);

            base.RefreshAll();

            for (int i = 1; i < projectileNames.Length; i++)
            {
                if (projectileNames[i] != ProjectileNames.None)
                {
                    ProjectileAsset asset = ScriptableDataManager.Instance.FindProjectile(projectileNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            projectileCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(projectileNames.Length);
                EditorUtility.DisplayProgressBar("모든 발사체 에셋의 갱신", projectileNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 발사체 에셋의 갱신을 종료합니다: {0}/{1}", projectileCount.ToSelectString(projectileNames.Length), projectileNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            ProjectileNames[] projectileNames = EnumEx.GetValues<ProjectileNames>();
            for (int i = 1; i < projectileNames.Length; i++)
            {
                if (projectileNames[i] is ProjectileNames.None)
                {
                    continue;
                }

                ProjectileAsset asset = ScriptableDataManager.Instance.FindProjectile(projectileNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<ProjectileAsset>("Projectile", projectileNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new ProjectileAssetData
                        {
                            Name = projectileNames[i]
                        };
                        asset.NameString = projectileNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif

        public ProjectileAssetData Clone()
        {
            return Data.Clone();
        }
    }
}

