namespace TeamSuneat
{
    public enum ProjectileMotionTypes
    {
        None,

        ///<summary> 지정된 위치에 생성됩니다. 이동하지 않습니다. </summary>
        Point,

        ///<summary> 시전자가 바라보는 수평 방향으로 날아갑니다. </summary>
        Linear,

        ///<summary> 시전자가 바라보는 수평 방향으로 날아갑니다. 중력의 영향을 받습니다. </summary>
        LinearGravity,

        /// <summary> 수평 이동과 함께 일정한 주기로 위아래로 움직이는 발사체입니다. </summary>
        Oscillating,

        ///<summary> 매 프레임 목표의 위치를 추적하여 날아갑니다. </summary>
        Homing,

        ///<summary> 원뿔형으로 일정 간격의 각도로 여러 개 날아갑니다. </summary>
        Cone,

        ///<summary> 사격 시점에 캐릭터의 목표 위치로 날아갑니다. </summary>
        TargetDirection,

        ///<summary> 짝수의 발사체가 양방향으로 날아갑니다. </summary>
        Bidirectional,

        ///<summary> 유니티의 물리 법칙(Rigidbody-Dynimic)을 따릅니다. </summary>
        Physics,

        ///<summary> 목표 위치에 포물선을 그리며 날아갑니다. </summary>
        Parabola,

        ///<summary> 영역 내 무작위 위치에 생성됩니다. 이동하지 않습니다. </summary>
        AreaPoint,

        ///<summary> 목표의 위치를 향해 던지고, 돌아옵니다. </summary>
        TargetBoomerang,

        /// <summary> 시전자가 바라보는 수평 방향으로 던지고, 돌아옵니다. </summary>
        HorizontalBoomerang,

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

        /// <summary> 수평으로 순찰합니다. </summary>
        HorizontalPatrol,

        /// <summary> 궤도를 가지고 돕니다. </summary>
        Orbit,

        /// <summary> 특정 위치를 따라다닙니다. </summary>
        Follow,

        /// <summary> 탐지한 적에게 날아가거나, 직선 방향으로 날아갑니다. </summary>
        DetectTargetOrLinear,

        /// <summary> 지면을 탐지해서 발사체가 멈추고, </summary>
        GroundTrigger,
    }
}