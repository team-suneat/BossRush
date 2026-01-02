using UnityEngine;

namespace TeamSuneat
{
    public enum ProjectileFacingDirectionTypes
    {
        /// <summary> 발사체가 공격자(Owner)의 방향을 따라갑니다. </summary>
        FollowOwner,

        /// <summary> 발사체가 부모 발사체의 방향을 따라갑니다. </summary>
        FollowParent,

        /// <summary> 발사체의 방향이 무작위로 설정됩니다. </summary>
        Random,

        /// <summary> 발사체 내부에서 설정된 기본 방향을 사용합니다. </summary>
        UseProjectileDefault
    }
} 