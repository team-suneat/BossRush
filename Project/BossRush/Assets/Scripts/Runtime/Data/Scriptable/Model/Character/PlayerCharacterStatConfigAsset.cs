using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "PlayerCharacterStatConfig", menuName = "TeamSuneat/Scriptable/PlayerCharacterStatConfig")]
    public class PlayerCharacterStatConfigAsset : XScriptableObject
    {
        public int BaseAttack = 1; // 기본 공격력 (1)
        public int BaseHealth = 10; // 기본 체력 (10)
        public int BaseHealthRegen = 1; // 기본 체력 회복량 (1초에 1만큼 회복)
        public float BaseAttackSpeed = 1.0f; // 기본 공격 속도 (100%)
        public float BaseCriticalChance = 0.001f; // 기본 치명타 확률 (0.1%)
        public float BaseCriticalDamage = 0.01f; // 기본 치명타 피해 (1%)
        public int BaseMana = 100; // 기본 마나 (100)
        public int BaseManaRegen = 1; // 기본 마나 회복량 (1초에 1만큼 회복)
        public int BaseAccuracy = 30; // 기본 명중
        public int BaseDodge = 10; // 기본 회피
        public float BaseGoldGain = 1.0f; // 기본 골드 획득량 배율 (100%)
        public float BaseXPGain = 1.0f; // 기본 경험치 획득량 배율 (100%)

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

            if (BaseHealth <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 체력이 0 이하입니다.");
            }

            if (BaseAttack <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 공격력이 0 이하입니다.");
            }

            if (BaseHealthRegen < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 체력 회복량이 음수입니다.");
            }

            if (BaseMana <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 마나가 0 이하입니다.");
            }

            if (BaseManaRegen < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 마나 회복량이 음수입니다.");
            }

            if (BaseCriticalChance < 0 || BaseCriticalChance > 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 치명타 확률이 0~1 범위를 벗어났습니다.");
            }

            if (BaseCriticalDamage < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 치명타 피해가 음수입니다.");
            }

            if (BaseAccuracy < 0 || BaseAccuracy > 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 명중률이 0~1 범위를 벗어났습니다.");
            }

            if (BaseGoldGain < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 골드 획득량 배율이 음수입니다.");
            }

            if (BaseXPGain < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 경험치 획득량 배율이 음수입니다.");
            }

#endif
        }
    }
}