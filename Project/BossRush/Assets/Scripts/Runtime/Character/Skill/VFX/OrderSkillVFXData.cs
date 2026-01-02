using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class OrderSkillVFXData
    {
        [InfoBox("Order가 음수일 때, 공중 공격을 의미합니다.", "IsAirAttack")]
        public int Order;
        public WeaponSkillVFXData[] WeaponSkillVFXs;

        public void Initialize(Character ownerCharacter)
        {
            foreach (WeaponSkillVFXData item in WeaponSkillVFXs)
            {
                item.Initialize(ownerCharacter);
            }
        }

        public void SpawnVFX(Transform parent, Vector3 feedbackPosition)
        {
            foreach (WeaponSkillVFXData item in WeaponSkillVFXs)
            {
                item.SpawnVisualEffect(parent, feedbackPosition);
            }
        }
    }

    [System.Serializable]
    public class WeaponSkillVFXData
    {
        public VisualEffectSpawnData SpawnData;

        public void Initialize(Character ownerCharacter)
        {
            SpawnData.LoadToggleValues();
            SpawnData.SetOwner(ownerCharacter);
        }

        public void SpawnVisualEffect(Transform parent, Vector3 feedbackPosition)
        {
            if (SpawnData.TrySpawnVisualEffect())
            {
                if (SpawnData.UseParent)
                {
                    SpawnData.SpawnVisualEffect(parent, 0);
                }
                else
                {
                    SpawnData.SpawnVisualEffect(feedbackPosition, 0);
                }
            }
        }
    }
}