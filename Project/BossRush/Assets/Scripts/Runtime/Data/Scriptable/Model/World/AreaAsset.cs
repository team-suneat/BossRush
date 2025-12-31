using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "AreaAsset", menuName = "TeamSuneat/Scriptable/Area")]
    public class AreaAsset : XScriptableObject
    {
        [Title("기본 정보")]
        [LabelText("지역 이름")]
        public AreaNames AreaName;

        [Title("몬스터 설정")]
        [LabelText("일반 몬스터 7종")]
        [ListDrawerSettings()]
        public CharacterNames[] NormalMonsters;

        [InfoBox("보스 몬스터 5종(5스테이지 단위로 보스가 설정됩니다.\n마지막 보스 몬스터는 최종 스테이지 지역 보스 몬스터로 지정됩니다.)", InfoMessageType.Info)]
        [LabelText("보스 몬스터")]
        [ListDrawerSettings()]
        public CharacterNames[] BossMonsters;

        [FoldoutGroup("#String")] public string[] NormalMonstersString;
        [FoldoutGroup("#String")] public string[] BossMonstersString;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (AreaName == AreaNames.None)
            {
                Log.Warning(LogTags.ScriptableData, "[Area] 지역 이름이 설정되지 않았습니다: {0}", name);
            }
            if (NormalMonsters == null || NormalMonsters.Length != 7)
            {
                Log.Warning(LogTags.ScriptableData, "[Area] 일반 몬스터가 7종이 아닙니다: {0}", name);
            }
            if (BossMonsters == null || BossMonsters.Length != 5)
            {
                Log.Warning(LogTags.ScriptableData, "[Area] 보스 몬스터가 5종이 아닙니다: {0}", name);
            }
#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            EnumEx.ConvertTo(ref AreaName, NameString);
            EnumEx.ConvertTo(ref NormalMonsters, NormalMonstersString);
            EnumEx.ConvertTo(ref BossMonsters, BossMonstersString);
        }

        public override void Refresh()
        {
            if (AreaName != AreaNames.None)
            {
                NameString = AreaName.ToString();
            }
            if (NormalMonsters.IsValid())
            {
                NormalMonstersString = NormalMonsters.ToStringArray();
            }
            if (BossMonsters.IsValid())
            {
                BossMonstersString = BossMonsters.ToStringArray();
            }

            base.Refresh();
        }

        public override void Rename()
        {
            Rename("Area");
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
            Debug.LogFormat("지역 에셋의 갱신을 시작합니다.");

            base.RefreshAll();
            OnRefreshAll();

            Debug.LogFormat("지역 에셋의 갱신을 종료합니다.");
        }

        protected override void CreateAll()
        {
            base.CreateAll();
            PathManager.UpdatePathMetaData();
        }

#endif
    }
}