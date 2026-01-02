namespace TeamSuneat
{
    public enum ProjectileSpawnTypes
    {
        None,

        /// <summary>
        /// 부모 설정: 발사체를 생성하는 게임 오브젝트의 위치를 따른다.
        /// </summary>
        Parent,

        /// <summary>
        /// 부모 설정: 발사체를 생성하는 캐릭터의 위치를 따른다.
        /// </summary>
        ParentCharacter,

        /// <summary>
        /// 특정 지점 : 발사체를 생성하는 게임 오브젝트의 위치에서 생성한다.
        /// </summary>
        Point,

        /// <summary>
        /// 타겟 : 타겟으로 삼은 캐릭터의 위치에서 생성한다.
        /// </summary>
        Target,

        /// <summary>
        /// 시전자의 지면 위에서 생성합니다.
        /// </summary>
        OwnerGround,

        /// <summary>
        /// 타겟 캐릭터의 지면 위에서 생성한다.
        /// </summary>
        TargetGround,

        /// <summary>
        /// 무작위 땅 위
        /// </summary>
        RandomGround,

        /// <summary>
        /// 몬스터 공격 위치
        /// </summary>
        AttackMonster,

        /// <summary>
        /// 특정 지점 : 발사체를 생성하는 게임 오브젝트의 위치에서 가장 가까운 땅에 생성한다.
        /// </summary>
        PointGround,
    }
}