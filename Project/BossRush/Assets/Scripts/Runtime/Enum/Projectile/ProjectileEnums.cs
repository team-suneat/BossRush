namespace TeamSuneat
{
    public enum ProjectileTypes
    {
        None,

        Attack,
        Another,
        Return,
    }

    public enum ProjectileLaunchTypes
    {
        None,

        Single,
        Continuous,
        BuckShot,
        Max,
    }

    public enum ProjectileSpawnOffsets
    {
        None,

        Parent, // 발사차의 오너 캐릭터 오브젝트의 자식으로 생성한다.
        Point, // 발사체를 생성하는 오브젝트의 위치에서 생성한다.
        Box, // 박스 내부 무작위 위치에서 생성한다.
        Reserved, // 지정된 위치에서 생성한다.
        ReservedRandom, // 지정된 위치 중 무작위로 선택하여 생성한다.
        ReservedCircular, // 지정된 위치를 순서대로 순환하며 생성한다.
        IntervalPoints, // 일정한 간격으로 생성
        Raycast, // 원하는 방향으로 일정 거리 레이캐스트를 쏜다. 충돌했다면 충돌 위치에, 충돌하지 않았다면 그 거리만큼 떨어져서 생성한다.
    }

    public enum ProjectileMovementTypes
    {
        None,

        ToDirection,

        ToPosition,

        ToCharacter,

        Max,
    }

    public enum ProjectileReloadTypes : byte
    {
        None,

        /// <summary>
        /// 한 번에 장전
        /// </summary>
        ReloadAtOnce,

        /// <summary>
        /// 한 발씩 장전
        /// </summary>
        ReloadOneByOne,

        /// <summary>
        /// 발사시 한 발 장전
        /// </summary>
        ReloadOneForEachShot,
    }

    public enum ProjectileResults
    {
        None,

        Destroy,

        DestroyWithAttack,

        DestroyWithAnother,

        DestroyWithReturn,

        DestroyWithAllAttack,

        StopMove,

        Attack,

        ActivateDetector,

        DeactivateDetector,

        Immunity,

        ActivateTrigger,

        DeactivateTrigger,
    }

    public enum ProjectileHomingDirections : byte
    {
        None,

        Up,

        Down,

        Random,

        PingPong,
    }
}