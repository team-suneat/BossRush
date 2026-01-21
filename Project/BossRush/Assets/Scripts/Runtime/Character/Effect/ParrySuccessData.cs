using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 패링 성공 시 필요한 데이터를 담는 구조체
    /// </summary>
    public struct ParrySuccessData
    {
        /// <summary> 공격자 캐릭터 </summary>
        public Character Attacker;

        /// <summary> 피격자 캐릭터 (패링을 수행한 캐릭터) </summary>
        public Character TargetCharacter;

        /// <summary> 공격 위치 </summary>
        public Vector3 AttackPosition;

        /// <summary> 패링 타입 </summary>
        public ParryTypes ParryType;

        /// <summary> 패링 넉백 타입 </summary>
        public KnockbackType KnockbackType;

        /// <summary> 패링 넉백 ForceVelocity 이름 </summary>
        public FVNames KnockbackForceVelocityName;


        public ParrySuccessData(Character attacker, Character targetCharacter, Vector3 attackPosition, ParryTypes parryType, KnockbackType knockbackType, FVNames knockbackForceVelocityName = FVNames.None)
        {
            Attacker = attacker;
            TargetCharacter = targetCharacter;
            AttackPosition = attackPosition;
            ParryType = parryType;
            KnockbackType = knockbackType;
            KnockbackForceVelocityName = knockbackForceVelocityName;
        }
    }
}
