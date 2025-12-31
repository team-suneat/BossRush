using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "StageAsset", menuName = "TeamSuneat/Scriptable/Stage")]
    public class StageAsset : XScriptableObject
    {
        [Title("기본 정보")]
        [LabelText("스테이지 ID (지역 내 1~20)")]
        public StageNames Name;

        [LabelText("소속 지역 ID")]
        public AreaNames AreaName;

        [LabelText("장비 등급 (일반, 고급, 레어, 희귀, 영웅, 전설)")]
        public GradeNames EquipmentGrade;

        [LabelText("장비 품질 (4등급 ~ 1등급)")]
        [Range(1, 4)]
        public int EquipmentQuality = 4;

        [LabelText("드랍 확률 (%)")]
        [Range(0, 1)]
        public float DropRate;

        [LabelText("아이템 종류 (무기, 악세서리)")]
        public ItemTypes ItemType;

        [Title("몬스터 설정")]
        [LabelText("일반 몬스터 후보 (지역에 설정된 일반 몬스터 종류 인덱스)")]
        public List<int> MonsterCandidates = new List<int>();

        [LabelText("보스 몬스터 후보 (지역에 설정된 보스 몬스터 종류 인덱스)")]
        public int BossMonsterIndex;

        [Title("웨이브 설정")]
        [LabelText("웨이브 수 (기본 20웨이브)")]
        public int WaveCount = 20;

        [LabelText("웨이브당 생성 몬스터 수")]
        public int MonsterCountPerWave = 5;

        //

        [FoldoutGroup("#String", 2)] private string AreaNameString;
        [FoldoutGroup("#String", 2)] private string EquipmentGradeString;
        [FoldoutGroup("#String", 2)] private string ItemTypeString;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (Name == StageNames.None)
            {
                Log.Error("스테이지 ID가 유효하지 않습니다 (1~20): {0}", name);
            }
            if (AreaName == AreaNames.None)
            {
                Log.Error("지역 ID가 유효하지 않습니다 (1~41): {0}", name);
            }
            if (WaveCount < 1)
            {
                Log.Error("웨이브 수가 1보다 작습니다: {0}", name);
            }
            if (MonsterCountPerWave < 1)
            {
                Log.Error("웨이브당 생성 몬스터 수가 1보다 작습니다: {0}", name);
            }
            if (MonsterCandidates == null || MonsterCandidates.Count == 0)
            {
                Log.Warning(LogTags.ScriptableData, "[Stage] 생성 몬스터 후보가 설정되지 않았습니다: {0}", name);
            }
#endif
        }

        public int GetStageMonsterCount()
        {
            return WaveCount * MonsterCountPerWave - 1;
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();
            EnumEx.ConvertTo(ref Name, NameString);
            EnumEx.ConvertTo(ref AreaName, AreaNameString);
            EnumEx.ConvertTo(ref EquipmentGrade, EquipmentGradeString);
            EnumEx.ConvertTo(ref ItemType, ItemTypeString);
        }

        public override void Refresh()
        {
            if (Name != StageNames.None)
            {
                NameString = Name.ToString();
            }
            if (AreaName != AreaNames.None)
            {
                AreaNameString = AreaName.ToString();
            }
            if (EquipmentGrade != GradeNames.None)
            {
                EquipmentGradeString = EquipmentGrade.ToString();
            }
            if (ItemType != ItemTypes.None)
            {
                ItemTypeString = ItemType.ToString();
            }
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("Stage");
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
            Debug.LogFormat("스테이지 에셋의 갱신을 시작합니다.");

            base.RefreshAll();
            OnRefreshAll();

            Debug.LogFormat("스테이지 에셋의 갱신을 종료합니다.");
        }

        protected override void CreateAll()
        {
            base.CreateAll();
            PathManager.UpdatePathMetaData();
        }

#endif
    }
}