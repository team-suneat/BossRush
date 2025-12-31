using Sirenix.OdinInspector;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "EnhancementConfig", menuName = "TeamSuneat/Scriptable/EnhancementConfig")]
    public class EnhancementConfigAsset : XScriptableObject
    {
        [TableList(ShowIndexLabels = true)]
        [Tooltip("강화 시스템 능력치 데이터 목록")]
        public EnhancementConfigData[] DataArray;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (DataArray == null || DataArray.Length == 0)
            {
                Log.Error("강화 시스템 데이터가 설정되지 않았습니다: {0}", name);
                return;
            }

            for (int i = 0; i < DataArray.Length; i++)
            {
                EnhancementConfigData data = DataArray[i];
                if (data == null)
                {
                    Log.Error("강화 시스템 데이터[{0}]가 null입니다: {1}", i, name);
                    continue;
                }

                if (data.StatName == StatNames.None)
                {
                    Log.Error("강화 시스템 데이터[{0}]의 능력치 이름이 설정되지 않았습니다: {1}", i, name);
                }
                if (data.MaxLevel == 0)
                {
                    Log.Error("강화 시스템 데이터[{0}]의 최대 레벨이 설정되지 않았습니다: {1}", i, name);
                }

                // 요구 능력치 검증
                if (data.RequiredStatName != StatNames.None)
                {
                    if (data.RequiredStatLevel <= 0)
                    {
                        Log.Error("강화 시스템 데이터[{0}]의 요구 능력치 레벨이 0 이하입니다: {1}", i, name);
                    }

                    // 요구 능력치가 자기 자신을 참조하는지 확인
                    if (data.RequiredStatName == data.StatName)
                    {
                        Log.Error("강화 시스템 데이터[{0}]의 요구 능력치가 자기 자신을 참조하고 있습니다: {1}", i, name);
                    }

                    // 요구 능력치가 데이터 배열에 존재하는지 확인
                    EnhancementConfigData requiredData = FindEnhancementData(data.RequiredStatName);
                    if (requiredData == null)
                    {
                        Log.Warning("강화 시스템 데이터[{0}]의 요구 능력치({1})가 데이터 배열에 존재하지 않습니다: {2}",
                            i, data.RequiredStatName, name);
                    }
                }
            }

            // 중복 체크
            var duplicates = DataArray
                .Where(d => d != null && d.StatName != StatNames.None)
                .GroupBy(d => d.StatName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var duplicate in duplicates)
            {
                Log.Error("강화 시스템 데이터에 중복된 스탯이 있습니다: {0} in {1}", duplicate, name);
            }
#endif
        }

        /// <summary>
        /// 능력치 이름으로 강화 데이터를 찾습니다.
        /// </summary>
        public EnhancementConfigData FindEnhancementData(StatNames statName)
        {
            if (DataArray == null)
            {
                return null;
            }

            for (int i = 0; i < DataArray.Length; i++)
            {
                if (DataArray[i] != null && DataArray[i].StatName == statName)
                {
                    return DataArray[i];
                }
            }

            return null;
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (DataArray != null)
            {
                for (int i = 0; i < DataArray.Length; i++)
                {
                    if (DataArray[i] != null)
                    {
                        DataArray[i].Validate();
                    }
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            if (DataArray != null)
            {
                for (int i = 0; i < DataArray.Length; i++)
                {
                    if (DataArray[i] != null)
                    {
                        DataArray[i].Refresh();
                    }
                }
            }
        }

        public override bool RefreshWithoutSave()
        {
            bool hasChanged = base.RefreshWithoutSave();

            if (DataArray != null)
            {
                for (int i = 0; i < DataArray.Length; i++)
                {
                    if (DataArray[i] != null)
                    {
                        if (DataArray[i].RefreshWithoutSave())
                        {
                            hasChanged = true;
                        }
                    }
                }
            }

            return hasChanged;
        }

        public override void Rename()
        {
#if UNITY_EDITOR
            PerformRename("EnhancementConfig");
#endif
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Log.Warning(LogTags.ScriptableData, "여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif
            Log.Info(LogTags.ScriptableData, "강화 시스템 데이터 에셋의 갱신을 시작합니다.");

            base.RefreshAll();
            OnRefreshAll();

            Log.Info(LogTags.ScriptableData, "강화 시스템 데이터 에셋의 갱신을 종료합니다.");
        }

        protected override void CreateAll()
        {
            base.CreateAll();
            PathManager.UpdatePathMetaData();
        }

#endif
    }
}