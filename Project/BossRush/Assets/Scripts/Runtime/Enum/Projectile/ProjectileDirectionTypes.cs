namespace TeamSuneat
{
    public enum ProjectileDirectionTypes
    {
        None,

        Facing,

        FacingReverse,

        ///<summary> 원뿔형으로 일정 간격의 각도로 여러 개 날아갑니다. </summary>
        Cone,

        ///<summary> 사격 시점에 캐릭터의 목표 위치로 날아갑니다. </summary>
        TargetDirection,

        ///<summary> 사격 시점에 발사체의 목표 위치로 날아갑니다. </summary>
        TargetPoisitonOfProjectile,

        ///<summary> 짝수의 발사체가 양방향으로 날아갑니다. </summary>
        Bidirectional,

        ///<summary> 목표의 위치를 향해 던지고, 돌아옵니다. </summary>
        TargetBoomerang,

        ///<summary> 모든 발사체 스폰 후 타겟 방향으로 이동 </summary>
        AllSpawnToActivateTarget,

        ///<summary> 지정된 위치를 향해 날아갑니다. </summary>
        PointDirection,

        ///<summary> 지정된 방향으로 날아갑니다. </summary>
        CustomDirection,

        ///<summary> 지정된 원뿔형 범위 내 발사합니다. </summary>
        RandomCone,

        /// <summary> 수평 방향으로 목표를 추격합니다. </summary>
        HorizontalTrack,

        ///<summary> 미리 지정된 타겟 방향으로 날아갑니다. </summary>
        CustomTargetDirection,

        ///<summary> 십자형으로 발사합니다. </summary>
        Crisscross,

        /// <summary> 특정 위치를 따라다닙니다. </summary>
        Follow,

        /// <summary> 탐지한 적에게 날아가거나, 직선 방향으로 날아갑니다. </summary>
        DetectTargetOrLinear,
    }
}