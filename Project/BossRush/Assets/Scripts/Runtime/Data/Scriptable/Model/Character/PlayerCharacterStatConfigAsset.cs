using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "PlayerCharacterStatConfig", menuName = "TeamSuneat/Scriptable/PlayerCharacterStatConfig")]
    public class PlayerCharacterStatConfigAsset : XScriptableObject
    {
        public int BaseAttack = 1; // 기본 공격력 (1)
        public int BaseLife = 10; // 기본 체력 (10)
        public float BaseAttackSpeed = 1.0f; // 기본 공격 속도 (100%)
        public int BaseMana = 100; // 기본 마나 (100)

        public override void Rename()
        {
#if UNITY_EDITOR
            PerformRename("PlayerCharacterStatConfig");
#endif
        }

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR

            if (BaseLife <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 체력이 0 이하입니다.");
            }

            if (BaseAttack <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 공격력이 0 이하입니다.");
            }

            if (BaseMana <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 마나가 0 이하입니다.");
            }

#endif
        }
    }
}