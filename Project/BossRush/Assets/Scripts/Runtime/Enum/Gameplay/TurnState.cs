namespace TeamSuneat
{
    /// <summary>
    /// 턴 상태를 나타내는 열거형
    /// </summary>
    public enum TurnState
    {
        /// <summary>초기 상태 (턴 시작 전)</summary>
        None = 0,

        /// <summary>플레이어 턴 진행 중</summary>
        PlayerTurn = 1,

        /// <summary>보상 턴 진행 중</summary>
        RewardTurn = 2,

        /// <summary>몬스터 턴 진행 중</summary>
        MonsterTurn = 3,

        /// <summary>게임 종료 상태 (클리어 또는 오버)</summary>
        GameEnd = 4
    }
}

