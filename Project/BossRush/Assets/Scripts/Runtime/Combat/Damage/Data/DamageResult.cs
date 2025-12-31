using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class DamageResult
    {
        /// <summary> 치명타 여부 </summary>
        public bool IsCritical;

        /// <summary> 회심의 일격 여부 </summary>
        public bool IsDevastatingStrike;

        /// <summary> 회피 여부 </summary>
        public bool IsEvasion;

        /// <summary> 명중 실패 여부 </summary>
        public bool IsMiss;

        //───────────────────────────────────────────────────────────────────────────────────────────────────=

        /// <summary> 피해량 </summary>
        public float DamageValue;

        /// <summary> 줄어든 피해량 </summary>
        public float ReducedDamageValue;

        public int DamageValueToInt
        {
            get
            {
                if (DamageValue is float.MaxValue)
                {
                    return int.MaxValue;
                }
                else if (DamageValue.IsZero())
                {
                    return 0;
                }

                return Mathf.RoundToInt(DamageValue);
            }
        }

        public int ReducedDamageValueToInt => Mathf.RoundToInt(ReducedDamageValue);

        //───────────────────────────────────────────────────────────────────────────────────────────────────=

        /// <summary> 공격자 </summary>
        public Character Attacker;

        /// <summary> 공격 독립체 </summary>
        public AttackEntity AttackEntity;

        /// <summary> 피격자 바이탈 </summary>
        public Vital TargetVital;

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary> 히트마크 레벨 </summary>
        public int HitmarkLevel;

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public Vector3 AttackPosition
        {
            get
            {
                if (AttackEntity != null)
                {
                    return AttackEntity.transform.position;
                }

                return Vector3.zero;
            }
        }

        public Vector3 DamagePosition
        {
            get
            {
                if (TargetVital != null)
                {
                    return TargetVital.transform.position;
                }

                return Vector3.zero;
            }
        }

        public Character TargetCharacter
        {
            get
            {
                if (TargetVital != null)
                {
                    return TargetVital.Owner;
                }

                return null;
            }
        }

        // 에셋 데이터 (AssetData)

        public HitmarkAssetData Asset;

        public HitmarkNames HitmarkName
        {
            get
            {
                if (Asset != null)
                {
                    return Asset.Name;
                }

                return HitmarkNames.None;
            }
        }

        public DamageTypes DamageType
        {
            get
            {
                if (Asset != null)
                {
                    return Asset.DamageType;
                }

                return DamageTypes.None;
            }
        }

        public VFXInstantDamageType InstantVFXDamageType { get; internal set; }
    }
}